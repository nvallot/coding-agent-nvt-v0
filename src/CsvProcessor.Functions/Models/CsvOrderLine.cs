#nullable enable

using CsvHelper.Configuration.Attributes;

namespace CsvProcessor.Functions.Models;

/// <summary>
/// Represents a single order line from the CSV file.
/// Maps CSV columns to strongly-typed properties.
/// </summary>
public sealed class CsvOrderLine
{
    [Name("OrderId")]
    public string OrderId { get; set; } = string.Empty;

    [Name("CustomerEmail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Name("ProductCode")]
    public string ProductCode { get; set; } = string.Empty;

    [Name("Quantity")]
    public int Quantity { get; set; }

    [Name("UnitPrice")]
    public decimal UnitPrice { get; set; }

    [Name("OrderDate")]
    public DateTime OrderDate { get; set; }
}
