#nullable enable

namespace CsvProcessor.Functions.Configuration;

/// <summary>
/// Application settings bound from configuration.
/// </summary>
public sealed class AppSettings
{
    public const string SectionName = "AppSettings";

    /// <summary>
    /// Name of the source container for CSV files.
    /// </summary>
    public string SourceContainerName { get; set; } = "csv-input";

    /// <summary>
    /// Name of the Service Bus topic to publish messages.
    /// </summary>
    public string ServiceBusTopicName { get; set; } = "orders-topic";

    /// <summary>
    /// Name of the Table Storage table for idempotency tracking.
    /// </summary>
    public string IdempotencyTableName { get; set; } = "ProcessedFiles";

    /// <summary>
    /// Maximum number of messages per Service Bus batch.
    /// Service Bus limit is 100 messages or 1MB per batch.
    /// </summary>
    public int MaxBatchSize { get; set; } = 100;

    /// <summary>
    /// CSV separator character. Default is semicolon for European format.
    /// </summary>
    public string CsvSeparator { get; set; } = ";";

    /// <summary>
    /// Whether to allow comma as fallback separator.
    /// </summary>
    public bool AllowCommaSeparator { get; set; } = true;
}
