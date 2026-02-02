using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Polly;
using Shared.Models;
using System.Net;

namespace FAP_57.SendPOSupplier.Services;

public class DataverseService : IDataverseService
{
    private readonly ILogger<DataverseService> _logger;
    private readonly ServiceClient _serviceClient;
    private const string EntityLogicalName = "sbm_stagedpurchaseorder";

    public DataverseService(
        ILogger<DataverseService> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        var baseUrl = configuration["DataverseApiBaseUrl"]
            ?? throw new InvalidOperationException("DataverseApiBaseUrl not configured");
        var clientId = configuration["DataverseClientId"]
            ?? throw new InvalidOperationException("DataverseClientId not configured");
        var clientSecret = configuration["DataverseClientSecret"]
            ?? throw new InvalidOperationException("DataverseClientSecret not configured");
        var tenantId = configuration["DataverseTenantId"]
            ?? throw new InvalidOperationException("DataverseTenantId not configured");

        // Connection string pour OAuth 2.0 Client Credentials
        var connectionString = $@"
            AuthType=ClientSecret;
            Url={baseUrl};
            ClientId={clientId};
            ClientSecret={clientSecret};
            TenantId={tenantId};
            RequireNewInstance=false";

        _serviceClient = new ServiceClient(connectionString);

        if (!_serviceClient.IsReady)
        {
            throw new InvalidOperationException(
                $"Failed to connect to Dataverse: {_serviceClient.LastError}");
        }

        _logger.LogInformation("Successfully connected to Dataverse at {BaseUrl}", baseUrl);
    }

    public async Task UpsertStagedPurchaseOrderAsync(
        DataverseStagingPurchaseOrder entity,
        string correlationId)
    {
        var retryPolicy = Policy
            .Handle<Exception>(ex => IsTransientError(ex))
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "[{CorrelationId}] Dataverse API retry {RetryCount} after {Delay}ms. Error: {Error}",
                        correlationId, retryCount, timeSpan.TotalMilliseconds, exception.Message);
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            // 1. Rechercher l'entité existante par ponumber (clé unique)
            var query = new Microsoft.Xrm.Sdk.Query.QueryExpression(EntityLogicalName)
            {
                ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("sbm_stagedpurchaseorderid"),
                Criteria = new Microsoft.Xrm.Sdk.Query.FilterExpression
                {
                    Conditions =
                    {
                        new Microsoft.Xrm.Sdk.Query.ConditionExpression(
                            "sbm_ponumber",
                            Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal,
                            entity.PoNumber)
                    }
                }
            };

            var results = await Task.Run(() => _serviceClient.RetrieveMultiple(query));

            Entity dataverseEntity;
            bool isUpdate = false;

            if (results.Entities.Count > 0)
            {
                // Update existing record
                dataverseEntity = results.Entities[0];
                isUpdate = true;
                
                _logger.LogInformation(
                    "[{CorrelationId}] Updating existing PO {PoNumber} in Dataverse",
                    correlationId, entity.PoNumber);
            }
            else
            {
                // Create new record
                dataverseEntity = new Entity(EntityLogicalName);
                
                _logger.LogInformation(
                    "[{CorrelationId}] Creating new PO {PoNumber} in Dataverse",
                    correlationId, entity.PoNumber);
            }

            // 2. Mapper les attributs
            dataverseEntity["sbm_ponumber"] = entity.PoNumber;
            dataverseEntity["sbm_mdmnumber"] = entity.MdmNumber;
            dataverseEntity["sbm_pkmpersonid"] = entity.PkmPersonId;
            dataverseEntity["sbm_pkmemail"] = entity.PkmEmail;
            dataverseEntity["sbm_pkmfirstname"] = entity.PkmFirstName;
            dataverseEntity["sbm_pkmlastname"] = entity.PkmLastName;
            dataverseEntity["sbm_productcode"] = entity.ProductCode;
            dataverseEntity["sbm_amount"] = new Money(entity.Amount);
            dataverseEntity["sbm_closeout"] = entity.CloseOut;
            dataverseEntity["sbm_erplastupdate"] = entity.ErpLastUpdate;
            dataverseEntity["sbm_projectnumber"] = entity.ProjectNumber;
            dataverseEntity["sbm_description"] = entity.Description;
            dataverseEntity["statuscode"] = new OptionSetValue(entity.StatusCode);

            // Dates optionnelles
            if (!string.IsNullOrEmpty(entity.FirstDelivery))
                dataverseEntity["sbm_firstdelivery"] = DateTime.Parse(entity.FirstDelivery);
            
            if (!string.IsNullOrEmpty(entity.LastDelivery))
                dataverseEntity["sbm_lastdelivery"] = DateTime.Parse(entity.LastDelivery);

            // 3. Exécuter Update ou Create
            if (isUpdate)
            {
                await Task.Run(() => _serviceClient.Update(dataverseEntity));
                
                _logger.LogInformation(
                    "[{CorrelationId}] Successfully updated PO {PoNumber} in Dataverse",
                    correlationId, entity.PoNumber);
            }
            else
            {
                var recordId = await Task.Run(() => _serviceClient.Create(dataverseEntity));
                
                _logger.LogInformation(
                    "[{CorrelationId}] Successfully created PO {PoNumber} in Dataverse with ID {RecordId}",
                    correlationId, entity.PoNumber, recordId);
            }
        });
    }

    private bool IsTransientError(Exception ex)
    {
        // Throttling (429) ou Service Unavailable (503)
        var message = ex.Message.ToLower();
        return message.Contains("throttling") ||
               message.Contains("429") ||
               message.Contains("service unavailable") ||
               message.Contains("503") ||
               message.Contains("timeout");
    }
}
