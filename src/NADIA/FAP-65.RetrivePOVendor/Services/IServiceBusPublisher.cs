using Shared.Models;

namespace FAP_65.RetrivePOVendor.Services;

/// <summary>
/// Service pour publier les messages dans Service Bus Topic
/// </summary>
public interface IServiceBusPublisher
{
    Task PublishAsync(PurchaseOrderMessage message);
}
