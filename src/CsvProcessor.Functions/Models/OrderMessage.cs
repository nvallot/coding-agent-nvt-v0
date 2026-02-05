#nullable enable

using System.Text.Json.Serialization;

namespace CsvProcessor.Functions.Models;

/// <summary>
/// JSON message published to Service Bus.
/// Conforms to the schema defined in TAD section 4.2.
/// </summary>
public sealed class OrderMessage
{
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("orderDate")]
    public string OrderDate { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public OrderMessageMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Metadata attached to each message for traceability.
/// </summary>
public sealed class OrderMessageMetadata
{
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;

    [JsonPropertyName("sourceFile")]
    public string SourceFile { get; set; } = string.Empty;

    [JsonPropertyName("processedAt")]
    public string ProcessedAt { get; set; } = string.Empty;

    [JsonPropertyName("lineNumber")]
    public int LineNumber { get; set; }
}
