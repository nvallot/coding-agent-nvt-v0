using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared.Models;
using FAP_65.RetrivePOVendor.Services;

namespace FAP_65.RetrivePOVendor;

public class RetrivePOVendorFunction
{
    private readonly ILogger<RetrivePOVendorFunction> _logger;
    private readonly INadiaDataService _nadiaService;
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly ILastExecutionService _lastExecutionService;

    public RetrivePOVendorFunction(
        ILogger<RetrivePOVendorFunction> logger,
        INadiaDataService nadiaService,
        IServiceBusPublisher serviceBusPublisher,
        ILastExecutionService lastExecutionService)
    {
        _logger = logger;
        _nadiaService = nadiaService;
        _serviceBusPublisher = serviceBusPublisher;
        _lastExecutionService = lastExecutionService;
    }

    /// <summary>
    /// Function trigger par timer quotidien (04:00 CET)
    /// Récupère les PO depuis NADIA et les publie dans Service Bus
    /// </summary>
    [Function("RetrivePOVendor")]
    public async Task Run([TimerTrigger("%TimerSchedule%")] TimerInfo timer)
    {
        var correlationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;

        _logger.LogInformation(
            "[{CorrelationId}] NADIA_Execution_Started at {StartTime}",
            correlationId, startTime);

        try
        {
            // 1. Récupérer la dernière date d'exécution
            var (lastExecDate, lastExecTime) = await _lastExecutionService.GetLastExecutionAsync();
            
            if (!lastExecDate.HasValue)
            {
                // Premier run : chercher les 30 derniers jours
                lastExecDate = DateTime.Today.AddDays(-30);
                lastExecTime = TimeSpan.Zero;
                
                _logger.LogInformation(
                    "[{CorrelationId}] First run detected. Using default lookback: {LookbackDate}",
                    correlationId, lastExecDate.Value);
            }

            _logger.LogInformation(
                "[{CorrelationId}] Retrieving PO since {LastExecDate} {LastExecTime}",
                correlationId, lastExecDate.Value, lastExecTime);

            // 2. Exécuter la stored procedure NADIA
            var purchaseOrders = await _nadiaService.GetPurchaseOrdersAsync(
                lastExecDate.Value,
                lastExecTime,
                correlationId);

            _logger.LogInformation(
                "[{CorrelationId}] NADIA_PO_Retrieved: {Count} PO",
                correlationId, purchaseOrders.Count);

            if (purchaseOrders.Count == 0)
            {
                _logger.LogInformation(
                    "[{CorrelationId}] No PO to process. Execution completed.",
                    correlationId);
                return;
            }

            // 3. Publier chaque PO dans Service Bus
            var successCount = 0;
            var errorCount = 0;

            foreach (var po in purchaseOrders)
            {
                try
                {
                    po.CorrelationId = correlationId;
                    await _serviceBusPublisher.PublishAsync(po);
                    successCount++;

                    _logger.LogDebug(
                        "[{CorrelationId}] NADIA_PO_Published: {PoNumber}",
                        correlationId, po.PoNumber);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError(ex,
                        "[{CorrelationId}] Failed to publish PO {PoNumber}",
                        correlationId, po.PoNumber);
                }
            }

            // 4. Mettre à jour la dernière date d'exécution
            await _lastExecutionService.UpdateLastExecutionAsync(
                DateTime.Today,
                startTime.TimeOfDay);

            // 5. Log final avec métriques
            var duration = DateTime.UtcNow - startTime;

            _logger.LogInformation(
                "[{CorrelationId}] NADIA_Execution_Completed: Total={Total}, Success={Success}, Error={Error}, Duration={Duration}s",
                correlationId, purchaseOrders.Count, successCount, errorCount, duration.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[{CorrelationId}] NADIA_Execution_Failed: {Error}",
                correlationId, ex.Message);
            
            throw; // Let Azure Functions handle retry logic
        }
    }
}
