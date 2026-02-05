#nullable enable

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using CsvProcessor.Functions.Models;
using CsvProcessor.Functions.Services;

namespace CsvProcessor.Tests.Unit;

public class JsonTransformerServiceTests
{
    private readonly Mock<ILogger<JsonTransformerService>> _loggerMock = new();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly JsonTransformerService _sut;

    public JsonTransformerServiceTests()
    {
        _sut = new JsonTransformerService(_loggerMock.Object, _timeProvider);
    }

    [Fact]
    public void Transform_ValidLine_ReturnsCorrectMessage()
    {
        // Arrange
        var line = CreateValidLine();
        var sourceFile = "orders-2026-02-05.csv";
        var lineNumber = 1;
        var correlationId = "test-correlation-id";

        // Act
        var result = _sut.Transform(line, sourceFile, lineNumber, correlationId);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be("ORD-001");
        result.CustomerEmail.Should().Be("customer@example.com");
        result.ProductCode.Should().Be("PROD-A");
        result.Quantity.Should().Be(5);
        result.UnitPrice.Should().Be(29.99m);
        result.OrderDate.Should().Be("2026-02-04");
    }

    [Fact]
    public void Transform_ValidLine_IncludesCorrectMetadata()
    {
        // Arrange
        var line = CreateValidLine();
        var sourceFile = "orders-2026-02-05.csv";
        var lineNumber = 42;
        var correlationId = "test-correlation-id";

        // Act
        var result = _sut.Transform(line, sourceFile, lineNumber, correlationId);

        // Assert
        result.Metadata.Should().NotBeNull();
        result.Metadata.CorrelationId.Should().Be(correlationId);
        result.Metadata.SourceFile.Should().Be(sourceFile);
        result.Metadata.LineNumber.Should().Be(lineNumber);
        result.Metadata.ProcessedAt.Should().NotBeEmpty();
    }

    [Fact]
    public void Transform_NullLine_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Transform(null!, "file.csv", 1, "correlation-id");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TransformMany_MultipleLines_ReturnsCorrectCount()
    {
        // Arrange
        var lines = new[]
        {
            CreateValidLine(),
            CreateValidLine(),
            CreateValidLine()
        };

        // Act
        var results = _sut.TransformMany(lines, "file.csv", "correlation-id").ToList();

        // Assert
        results.Should().HaveCount(3);
    }

    [Fact]
    public void TransformMany_EmptyList_ReturnsEmptyEnumerable()
    {
        // Arrange
        var lines = Array.Empty<CsvOrderLine>();

        // Act
        var results = _sut.TransformMany(lines, "file.csv", "correlation-id").ToList();

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void TransformMany_AssignsIncrementalLineNumbers()
    {
        // Arrange
        var lines = new[]
        {
            CreateValidLine(),
            CreateValidLine(),
            CreateValidLine()
        };

        // Act
        var results = _sut.TransformMany(lines, "file.csv", "correlation-id").ToList();

        // Assert
        results[0].Metadata.LineNumber.Should().Be(1);
        results[1].Metadata.LineNumber.Should().Be(2);
        results[2].Metadata.LineNumber.Should().Be(3);
    }

    private static CsvOrderLine CreateValidLine() => new()
    {
        OrderId = "ORD-001",
        CustomerEmail = "customer@example.com",
        ProductCode = "PROD-A",
        Quantity = 5,
        UnitPrice = 29.99m,
        OrderDate = new DateTime(2026, 2, 4)
    };
}

/// <summary>
/// Fake TimeProvider for testing.
/// </summary>
public sealed class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow = new(2026, 2, 5, 10, 30, 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void SetUtcNow(DateTimeOffset value) => _utcNow = value;
}
