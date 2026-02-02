using Shared.Models;

namespace FAP_57.SendPOSupplier.Services;

/// <summary>
/// Service pour interagir avec Dataverse (Power Platform)
/// </summary>
public interface IDataverseService
{
    Task UpsertStagedPurchaseOrderAsync(DataverseStagingPurchaseOrder entity, string correlationId);
}
