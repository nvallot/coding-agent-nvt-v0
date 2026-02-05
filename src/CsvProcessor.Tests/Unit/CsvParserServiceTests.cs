#nullable enable

using System.Text;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using CsvProcessor.Functions.Configuration;
using CsvProcessor.Functions.Models;
using CsvProcessor.Functions.Services;
using CsvProcessor.Functions.Validators;

namespace CsvProcessor.Tests.Unit;

public class CsvParserServiceTests
{
    private readonly Mock<ILogger<CsvParserService>> _loggerMock = new();
    private readonly IValidator<CsvOrderLine> _validator = new CsvOrderLineValidator();
    private readonly IOptions<AppSettings> _settings = Options.Create(new AppSettings());
    private readonly CsvParserService _sut;

    public CsvParserServiceTests()
    {
        _sut = new CsvParserService(_validator, _settings, _loggerMock.Object);
    }

    [Fact]
    public async Task ParseAsync_ValidCsv_ReturnsAllValidLines()
    {
        // Arrange
        var csv = """
            OrderId;CustomerEmail;ProductCode;Quantity;UnitPrice;OrderDate
            ORD-001;customer1@example.com;PROD-A;5;29.99;2026-02-04
            ORD-002;customer2@example.com;PROD-B;3;49.99;2026-02-04
            ORD-003;customer3@example.com;PROD-C;1;99.99;2026-02-04
            """;
        using var stream = CreateStream(csv);

        // Act
        var result = await _sut.ParseAsync(stream);

        // Assert
        result.ValidLines.Should().HaveCount(3);
        result.InvalidLines.Should().BeEmpty();
        result.TotalLines.Should().Be(3);
        result.HasValidLines.Should().BeTrue();
    }

    [Fact]
    public async Task ParseAsync_MixedValidAndInvalid_SeparatesCorrectly()
    {
        // Arrange
        var csv = """
            OrderId;CustomerEmail;ProductCode;Quantity;UnitPrice;OrderDate
            ORD-001;customer1@example.com;PROD-A;5;29.99;2026-02-04
            INVALID;not-an-email;PROD-B;0;-10;2026-02-04
            ORD-003;customer3@example.com;PROD-C;1;99.99;2026-02-04
            """;
        using var stream = CreateStream(csv);

        // Act
        var result = await _sut.ParseAsync(stream);

        // Assert
        result.ValidLines.Should().HaveCount(2);
        result.InvalidLines.Should().HaveCount(1);
        result.InvalidLines[0].LineNumber.Should().Be(2);
    }

    [Fact]
    public async Task ParseAsync_EmptyCsv_ReturnsEmptyResults()
    {
        // Arrange
        var csv = "OrderId;CustomerEmail;ProductCode;Quantity;UnitPrice;OrderDate";
        using var stream = CreateStream(csv);

        // Act
        var result = await _sut.ParseAsync(stream);

        // Assert
        result.ValidLines.Should().BeEmpty();
        result.InvalidLines.Should().BeEmpty();
        result.HasValidLines.Should().BeFalse();
    }

    [Fact]
    public async Task ParseAsync_CommaSeparated_AutoDetectsAndParses()
    {
        // Arrange
        var csv = """
            OrderId,CustomerEmail,ProductCode,Quantity,UnitPrice,OrderDate
            ORD-001,customer1@example.com,PROD-A,5,29.99,2026-02-04
            """;
        using var stream = CreateStream(csv);

        // Act
        var result = await _sut.ParseAsync(stream);

        // Assert
        result.ValidLines.Should().HaveCount(1);
        result.ValidLines[0].OrderId.Should().Be("ORD-001");
    }

    [Fact]
    public async Task ParseAsync_QuotedValues_ParsesCorrectly()
    {
        // Arrange
        var csv = """
            OrderId;CustomerEmail;ProductCode;Quantity;UnitPrice;OrderDate
            "ORD-001";"customer1@example.com";"PROD-A";5;29.99;2026-02-04
            """;
        using var stream = CreateStream(csv);

        // Act
        var result = await _sut.ParseAsync(stream);

        // Assert
        result.ValidLines.Should().HaveCount(1);
        result.ValidLines[0].OrderId.Should().Be("ORD-001");
    }

    [Fact]
    public async Task ParseAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var csv = """
            OrderId;CustomerEmail;ProductCode;Quantity;UnitPrice;OrderDate
            ORD-001;customer1@example.com;PROD-A;5;29.99;2026-02-04
            """;
        using var stream = CreateStream(csv);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = () => _sut.ParseAsync(stream, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    private static MemoryStream CreateStream(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }
}
