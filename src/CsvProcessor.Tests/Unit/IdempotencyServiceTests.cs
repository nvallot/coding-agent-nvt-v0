#nullable enable

using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using CsvProcessor.Functions.Configuration;
using CsvProcessor.Functions.Models;
using CsvProcessor.Functions.Services;

namespace CsvProcessor.Tests.Unit;

public class IdempotencyServiceTests
{
    private readonly Mock<TableServiceClient> _tableServiceClientMock = new();
    private readonly Mock<TableClient> _tableClientMock = new();
    private readonly Mock<ILogger<IdempotencyService>> _loggerMock = new();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly IOptions<AppSettings> _settings = Options.Create(new AppSettings
    {
        IdempotencyTableName = "ProcessedFiles"
    });

    public IdempotencyServiceTests()
    {
        _tableServiceClientMock
            .Setup(x => x.GetTableClient(It.IsAny<string>()))
            .Returns(_tableClientMock.Object);
    }

    [Fact]
    public void ComputeHash_SameContent_ReturnsSameHash()
    {
        // Arrange
        var sut = CreateService();
        var content = "test content for hashing";
        using var stream1 = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var hash1 = sut.ComputeHash(stream1);
        var hash2 = sut.ComputeHash(stream2);

        // Assert
        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(32); // MD5 produces 128-bit = 32 hex chars
    }

    [Fact]
    public void ComputeHash_DifferentContent_ReturnsDifferentHash()
    {
        // Arrange
        var sut = CreateService();
        using var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("content 1"));
        using var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("content 2"));

        // Act
        var hash1 = sut.ComputeHash(stream1);
        var hash2 = sut.ComputeHash(stream2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeHash_ResetsStreamPosition()
    {
        // Arrange
        var sut = CreateService();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));

        // Act
        sut.ComputeHash(stream);

        // Assert
        stream.Position.Should().Be(0);
    }

    [Fact]
    public async Task IsAlreadyProcessedAsync_CompletedFile_ReturnsTrue()
    {
        // Arrange
        var sut = CreateService();
        var fileHash = "abc123";

        var entity = new ProcessedFileEntity
        {
            PartitionKey = "2026-02",
            RowKey = fileHash,
            Status = ProcessingStatus.Completed
        };

        _tableClientMock
            .Setup(x => x.GetEntityIfExistsAsync<ProcessedFileEntity>(
                It.IsAny<string>(), fileHash, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(entity, Mock.Of<Response>()));

        // Act
        var result = await sut.IsAlreadyProcessedAsync(fileHash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAlreadyProcessedAsync_ProcessingFile_ReturnsTrue()
    {
        // Arrange
        var sut = CreateService();
        var fileHash = "abc123";

        var entity = new ProcessedFileEntity
        {
            PartitionKey = "2026-02",
            RowKey = fileHash,
            Status = ProcessingStatus.Processing
        };

        _tableClientMock
            .Setup(x => x.GetEntityIfExistsAsync<ProcessedFileEntity>(
                It.IsAny<string>(), fileHash, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(entity, Mock.Of<Response>()));

        // Act
        var result = await sut.IsAlreadyProcessedAsync(fileHash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAlreadyProcessedAsync_FailedFile_ReturnsFalse()
    {
        // Arrange
        var sut = CreateService();
        var fileHash = "abc123";

        var entity = new ProcessedFileEntity
        {
            PartitionKey = "2026-02",
            RowKey = fileHash,
            Status = ProcessingStatus.Failed
        };

        _tableClientMock
            .Setup(x => x.GetEntityIfExistsAsync<ProcessedFileEntity>(
                It.IsAny<string>(), fileHash, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(entity, Mock.Of<Response>()));

        // Act
        var result = await sut.IsAlreadyProcessedAsync(fileHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAlreadyProcessedAsync_NotFound_ReturnsFalse()
    {
        // Arrange
        var sut = CreateService();
        var fileHash = "abc123";

        _tableClientMock
            .Setup(x => x.GetEntityIfExistsAsync<ProcessedFileEntity>(
                It.IsAny<string>(), fileHash, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue<ProcessedFileEntity>(null!, Mock.Of<Response>()));

        // Act
        var result = await sut.IsAlreadyProcessedAsync(fileHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkAsProcessingAsync_CallsUpsertWithCorrectEntity()
    {
        // Arrange
        var sut = CreateService();
        var fileHash = "abc123";
        var fileName = "orders.csv";

        // Act
        await sut.MarkAsProcessingAsync(fileHash, fileName);

        // Assert
        _tableClientMock.Verify(x => x.UpsertEntityAsync(
            It.Is<ProcessedFileEntity>(e => 
                e.RowKey == fileHash &&
                e.FileName == fileName &&
                e.Status == ProcessingStatus.Processing),
            TableUpdateMode.Replace,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MarkAsCompletedAsync_CallsUpsertWithCorrectEntity()
    {
        // Arrange
        var sut = CreateService();
        var fileHash = "abc123";
        var result = new ProcessingResult
        {
            FileName = "orders.csv",
            FileHash = fileHash,
            LinesTotal = 100,
            LinesValid = 95,
            LinesInvalid = 5,
            MessagesPublished = 95,
            Duration = TimeSpan.FromSeconds(2)
        };

        // Act
        await sut.MarkAsCompletedAsync(fileHash, result);

        // Assert
        _tableClientMock.Verify(x => x.UpsertEntityAsync(
            It.Is<ProcessedFileEntity>(e => 
                e.RowKey == fileHash &&
                e.Status == ProcessingStatus.Completed &&
                e.LinesTotal == 100 &&
                e.LinesValid == 95 &&
                e.LinesInvalid == 5),
            TableUpdateMode.Replace,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private IdempotencyService CreateService()
    {
        return new IdempotencyService(
            _tableServiceClientMock.Object,
            _settings,
            _loggerMock.Object,
            _timeProvider);
    }
}
