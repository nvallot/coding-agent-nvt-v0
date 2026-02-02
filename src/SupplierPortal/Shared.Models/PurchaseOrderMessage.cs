using System.Text.Json.Serialization;

namespace Shared.Models;

/// <summary>
/// Message publié dans Service Bus Topic "purchase-orders"
/// Représente les métadonnées d'un Purchase Order provenant de NADIA
/// </summary>
public class PurchaseOrderMessage
{
    [JsonPropertyName("poNumber")]
    public string PoNumber { get; set; } = string.Empty;

    [JsonPropertyName("mdmNumber")]
    public string MdmNumber { get; set; } = string.Empty;

    [JsonPropertyName("pkmGuid")]
    public string PkmGuid { get; set; } = string.Empty;

    [JsonPropertyName("pkmEmail")]
    public string PkmEmail { get; set; } = string.Empty;

    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("firstDelivery")]
    public DateTime? FirstDelivery { get; set; }

    [JsonPropertyName("lastDelivery")]
    public DateTime? LastDelivery { get; set; }

    [JsonPropertyName("closeOut")]
    public bool CloseOut { get; set; }

    [JsonPropertyName("dateModified")]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("projectNumber")]
    public string ProjectNumber { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}
