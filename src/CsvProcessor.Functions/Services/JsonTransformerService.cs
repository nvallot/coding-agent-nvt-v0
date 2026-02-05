#nullable enable

using Microsoft.Extensions.Logging;
using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Transforms CSV order lines into JSON messages with metadata.
/// Implements FR-004 (Transformation JSON).
/// </summary>
public sealed class JsonTransformerService : IJsonTransformerService
{
    private readonly ILogger<JsonTransformerService> _logger;
    private readonly TimeProvider _timeProvider;

    public JsonTransformerService(
        ILogger<JsonTransformerService> logger,
        TimeProvider? timeProvider = null)
    {
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc />
    public OrderMessage Transform(
        CsvOrderLine line, 
        string sourceFile, 
        int lineNumber, 
        string correlationId)
    {
        ArgumentNullException.ThrowIfNull(line);

        return new OrderMessage
        {
            OrderId = line.OrderId,
            CustomerEmail = line.CustomerEmail,
            ProductCode = line.ProductCode,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            OrderDate = line.OrderDate.ToString("yyyy-MM-dd"),
            Metadata = new OrderMessageMetadata
            {
                CorrelationId = correlationId,
                SourceFile = sourceFile,
                ProcessedAt = _timeProvider.GetUtcNow().ToString("O"),
                LineNumber = lineNumber
            }
        };
    }

    /// <inheritdoc />
    public IEnumerable<OrderMessage> TransformMany(
        IEnumerable<CsvOrderLine> lines, 
        string sourceFile, 
        string correlationId)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var lineNumber = 0;
        foreach (var line in lines)
        {
            lineNumber++;
            yield return Transform(line, sourceFile, lineNumber, correlationId);
        }

        _logger.LogDebug("Transformed {Count} lines from {SourceFile}", lineNumber, sourceFile);
    }
}
