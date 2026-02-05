#nullable enable

using Azure;
using Azure.Data.Tables;

namespace CsvProcessor.Functions.Models;

/// <summary>
/// Entity stored in Azure Table Storage for idempotency tracking.
/// PartitionKey: YYYY-MM (for efficient querying and cleanup)
/// RowKey: MD5 hash of file content
/// </summary>
public sealed class ProcessedFileEntity : ITableEntity
{
    /// <summary>
    /// Year-Month partition (e.g., "2026-02").
    /// </summary>
    public string PartitionKey { get; set; } = string.Empty;

    /// <summary>
    /// MD5 hash of the file content.
    /// </summary>
    public string RowKey { get; set; } = string.Empty;

    /// <summary>
    /// Processing status: "processing", "completed", "failed".
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Original filename.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when processing completed.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>
    /// Total number of lines in the file.
    /// </summary>
    public int LinesTotal { get; set; }

    /// <summary>
    /// Number of successfully processed lines.
    /// </summary>
    public int LinesValid { get; set; }

    /// <summary>
    /// Number of lines with validation errors.
    /// </summary>
    public int LinesInvalid { get; set; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Azure Table Storage timestamp.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Azure Table Storage ETag for optimistic concurrency.
    /// </summary>
    public ETag ETag { get; set; }
}

/// <summary>
/// Processing status constants.
/// </summary>
public static class ProcessingStatus
{
    public const string Processing = "processing";
    public const string Completed = "completed";
    public const string Failed = "failed";
}
