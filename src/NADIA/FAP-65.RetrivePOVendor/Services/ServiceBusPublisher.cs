using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models;
using System.Text.Json;

namespace FAP_65.RetrivePOVendor.Services;

public class ServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
{
    private readonly ILogger<ServiceBusPublisher> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private const string TopicName = "purchase-orders";

    public ServiceBusPublisher(
        ILogger<ServiceBusPublisher> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        
        var fullyQualifiedNamespace = configuration["ServiceBusConnection__fullyQualifiedNamespace"]
            ?? throw new InvalidOperationException("ServiceBusConnection__fullyQualifiedNamespace not configured");

        // Utiliser Managed Identity pour l'authentification
        _client = new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
        _sender = _client.CreateSender(TopicName);
    }

    public async Task PublishAsync(PurchaseOrderMessage message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var serviceBusMessage = new ServiceBusMessage(json)
            {
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = message.CorrelationId,
                ContentType = "application/json",
                Subject = "purchase-order",
                ApplicationProperties =
                {
                    { "poNumber", message.PoNumber },
                    { "mdmNumber", message.MdmNumber },
                    { "amount", message.Amount },
                    { "source", "NADIA" }
                }
            };

            await _sender.SendMessageAsync(serviceBusMessage);

            _logger.LogDebug(
                "[{CorrelationId}] Published PO {PoNumber} to Service Bus Topic {TopicName}",
                message.CorrelationId, message.PoNumber, TopicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[{CorrelationId}] Failed to publish PO {PoNumber} to Service Bus",
                message.CorrelationId, message.PoNumber);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
