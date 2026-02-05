#nullable enable

namespace CsvProcessor.Functions.Models;

/// <summary>
/// Result of CSV parsing operation.
/// </summary>
public sealed class CsvParseResult
{
    /// <summary>
    /// Successfully parsed and validated lines.
    /// </summary>
    public IReadOnlyList<CsvOrderLine> ValidLines { get; init; } = [];

    /// <summary>
    /// Lines that failed validation with their errors.
    /// </summary>
    public IReadOnlyList<ValidationError> InvalidLines { get; init; } = [];

    /// <summary>
    /// Total lines attempted to parse.
    /// </summary>
    public int TotalLines => ValidLines.Count + InvalidLines.Count;

    /// <summary>
    /// Whether parsing was successful (at least some valid lines).
    /// </summary>
    public bool HasValidLines => ValidLines.Count > 0;
}

/// <summary>
/// Represents a validation error for a specific line.
/// </summary>
public sealed class ValidationError
{
    /// <summary>
    /// Line number in the CSV file (1-based, excluding header).
    /// </summary>
    public int LineNumber { get; init; }

    /// <summary>
    /// Raw line content (if available).
    /// </summary>
    public string? RawContent { get; init; }

    /// <summary>
    /// Validation error messages.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = [];
}

/// <summary>
/// Result of file processing operation.
/// </summary>
public sealed class ProcessingResult
{
    public required string FileName { get; init; }
    public required string FileHash { get; init; }
    public required int LinesTotal { get; init; }
    public required int LinesValid { get; init; }
    public required int LinesInvalid { get; init; }
    public required int MessagesPublished { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool Success { get; init; } = true;
    public string? ErrorMessage { get; init; }
}
