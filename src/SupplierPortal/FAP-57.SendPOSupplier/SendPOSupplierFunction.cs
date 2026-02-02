using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Shared.Models;
using FAP_57.SendPOSupplier.Services;

namespace FAP_57.SendPOSupplier;

public class SendPOSupplierFunction
{
    private readonly ILogger<SendPOSupplierFunction> _logger;
    private readonly ILucyApiService _lucyService;
    private readonly IDataverseService _dataverseService;

    public SendPOSupplierFunction(
        ILogger<SendPOSupplierFunction> logger,
        ILucyApiService lucyService,
        IDataverseService dataverseService)
    {
        _logger = logger;
        _lucyService = lucyService;
        _dataverseService = dataverseService;
    }

    /// <summary>
    /// Function déclenchée par les messages du Service Bus Topic "purchase-orders"
    /// Enrichit les données avec Lucy API et envoie vers Dataverse
    /// </summary>
    [Function("SendPOSupplier")]
    public async Task Run(
        [ServiceBusTrigger("purchase-orders", "spa-processor", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var correlationId = message.CorrelationId ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "[{CorrelationId}] SPA_Message_Received: MessageId={MessageId}, DeliveryCount={DeliveryCount}",
            correlationId, message.MessageId, message.DeliveryCount);

        try
        {
            // 1. Désérialiser le message
            var poMessage = JsonSerializer.Deserialize<PurchaseOrderMessage>(message.Body.ToString());
            
            if (poMessage == null)
            {
                _logger.LogError(
                    "[{CorrelationId}] Failed to deserialize message. MessageId={MessageId}",
                    correlationId, message.MessageId);
                
                await messageActions.DeadLetterMessageAsync(
                    message,
                    deadLetterReason: "InvalidMessageFormat",
                    deadLetterErrorDescription: "Failed to deserialize PurchaseOrderMessage");
                return;
            }

            _logger.LogInformation(
                "[{CorrelationId}] Processing PO {PoNumber} from NADIA",
                correlationId, poMessage.PoNumber);

            // 2. Enrichir avec les informations PKM depuis Lucy API
            LucyUserResponse? pkmInfo = null;
            
            if (!string.IsNullOrWhiteSpace(poMessage.PkmGuid))
            {
                pkmInfo = await _lucyService.GetUserByIdAsync(poMessage.PkmGuid, correlationId);

                if (pkmInfo == null)
                {
                    _logger.LogWarning(
                        "[{CorrelationId}] SPA_PKM_NotFound: PKM {PkmGuid} not found in Lucy API for PO {PoNumber}",
                        correlationId, poMessage.PkmGuid, poMessage.PoNumber);

                    await messageActions.DeadLetterMessageAsync(
                        message,
                        deadLetterReason: "PKMNotFound",
                        deadLetterErrorDescription: $"PKM {poMessage.PkmGuid} not found in Lucy API");
                    return;
                }

                _logger.LogInformation(
                    "[{CorrelationId}] SPA_PKM_Enriched: {GivenName} {SurName} ({Email})",
                    correlationId, pkmInfo.GivenName, pkmInfo.SurName, pkmInfo.Email);
            }

            // 3. Mapper vers le modèle Dataverse
            var dataverseEntity = MapToDataverseEntity(poMessage, pkmInfo);

            // 4. Upsert dans Dataverse
            await _dataverseService.UpsertStagedPurchaseOrderAsync(dataverseEntity, correlationId);

            _logger.LogInformation(
                "[{CorrelationId}] SPA_Dataverse_Sent: PO {PoNumber} successfully upserted to Dataverse",
                correlationId, poMessage.PoNumber);

            // 5. Complete le message Service Bus
            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[{CorrelationId}] SPA_Dataverse_Error: Failed to process message {MessageId}. Error: {Error}",
                correlationId, message.MessageId, ex.Message);

            // Dead letter si trop de retries
            if (message.DeliveryCount >= 5)
            {
                await messageActions.DeadLetterMessageAsync(
                    message,
                    deadLetterReason: "MaxDeliveryAttemptsExceeded",
                    deadLetterErrorDescription: ex.Message);
            }
            else
            {
                // Abandon pour retry automatique
                await messageActions.AbandonMessageAsync(message);
            }
        }
    }

    private DataverseStagingPurchaseOrder MapToDataverseEntity(
        PurchaseOrderMessage source,
        LucyUserResponse? pkmInfo)
    {
        return new DataverseStagingPurchaseOrder
        {
            PoNumber = source.PoNumber,
            MdmNumber = source.MdmNumber,
            PkmPersonId = source.PkmGuid,
            PkmEmail = pkmInfo?.Email ?? source.PkmEmail,
            PkmFirstName = pkmInfo?.GivenName ?? string.Empty,
            PkmLastName = pkmInfo?.SurName ?? string.Empty,
            ProductCode = source.ProductCode,
            Amount = source.Amount,
            FirstDelivery = source.FirstDelivery?.ToString("yyyy-MM-dd"),
            LastDelivery = source.LastDelivery?.ToString("yyyy-MM-dd"),
            CloseOut = source.CloseOut,
            ErpLastUpdate = source.DateModified,
            ProjectNumber = source.ProjectNumber,
            Description = source.Description,
            StatusCode = 918860002 // Ready to be Processed
        };
    }
}
