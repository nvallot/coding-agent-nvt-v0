#nullable enable

using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Service for parsing CSV files into order lines.
/// Implements FR-002 (Parsing CSV).
/// </summary>
public interface ICsvParserService
{
    /// <summary>
    /// Parses a CSV stream and returns valid and invalid lines.
    /// </summary>
    /// <param name="stream">The CSV file stream.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Parse result containing valid lines and validation errors.</returns>
    Task<CsvParseResult> ParseAsync(Stream stream, CancellationToken ct = default);
}
