using System.Text.Json.Serialization;

namespace Shared.Models;

/// <summary>
/// Modèle pour l'entité Dataverse sbm_stagedpurchaseorder
/// Staging table pour les Purchase Orders destinés au Supplier Performance Assessment
/// </summary>
public class DataverseStagingPurchaseOrder
{
    [JsonPropertyName("sbm_ponumber")]
    public string PoNumber { get; set; } = string.Empty;

    [JsonPropertyName("sbm_mdmnumber")]
    public string MdmNumber { get; set; } = string.Empty;

    [JsonPropertyName("sbm_pkmpersonid")]
    public string PkmPersonId { get; set; } = string.Empty;

    [JsonPropertyName("sbm_pkmemail")]
    public string PkmEmail { get; set; } = string.Empty;

    [JsonPropertyName("sbm_pkmfirstname")]
    public string PkmFirstName { get; set; } = string.Empty;

    [JsonPropertyName("sbm_pkmlastname")]
    public string PkmLastName { get; set; } = string.Empty;

    [JsonPropertyName("sbm_productcode")]
    public string ProductCode { get; set; } = string.Empty;

    [JsonPropertyName("sbm_amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("sbm_firstdelivery")]
    public string? FirstDelivery { get; set; } // Format: YYYY-MM-DD

    [JsonPropertyName("sbm_lastdelivery")]
    public string? LastDelivery { get; set; } // Format: YYYY-MM-DD

    [JsonPropertyName("sbm_closeout")]
    public bool CloseOut { get; set; }

    [JsonPropertyName("sbm_erplastupdate")]
    public DateTime ErpLastUpdate { get; set; }

    [JsonPropertyName("sbm_projectnumber")]
    public string ProjectNumber { get; set; } = string.Empty;

    [JsonPropertyName("sbm_description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("statuscode")]
    public int StatusCode { get; set; } = 918860002; // Ready to be Processed
}
