#nullable enable

using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Transforms CSV order lines into JSON messages.
/// Implements FR-004 (Transformation JSON).
/// </summary>
public interface IJsonTransformerService
{
    /// <summary>
    /// Transforms a CSV order line into a JSON message.
    /// </summary>
    /// <param name="line">The CSV order line.</param>
    /// <param name="sourceFile">The source file name.</param>
    /// <param name="lineNumber">The line number in the source file.</param>
    /// <param name="correlationId">The correlation ID for tracing.</param>
    /// <returns>The transformed JSON message.</returns>
    OrderMessage Transform(CsvOrderLine line, string sourceFile, int lineNumber, string correlationId);

    /// <summary>
    /// Transforms multiple CSV order lines into JSON messages.
    /// </summary>
    /// <param name="lines">The CSV order lines.</param>
    /// <param name="sourceFile">The source file name.</param>
    /// <param name="correlationId">The correlation ID for tracing.</param>
    /// <returns>The transformed JSON messages.</returns>
    IEnumerable<OrderMessage> TransformMany(
        IEnumerable<CsvOrderLine> lines, 
        string sourceFile, 
        string correlationId);
}
