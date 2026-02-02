using Shared.Models;

namespace FAP_65.RetrivePOVendor.Services;

/// <summary>
/// Service pour récupérer les données Purchase Order depuis NADIA SQL Server
/// </summary>
public interface INadiaDataService
{
    /// <summary>
    /// Exécute la stored procedure NADIA_SPA_SUPHEADERMETADATA_AZURE
    /// </summary>
    Task<List<PurchaseOrderMessage>> GetPurchaseOrdersAsync(
        DateTime lastExecDate,
        TimeSpan lastExecTime,
        string correlationId);
}
