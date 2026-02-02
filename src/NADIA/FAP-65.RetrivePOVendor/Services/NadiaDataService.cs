using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Shared.Models;

namespace FAP_65.RetrivePOVendor.Services;

public class NadiaDataService : INadiaDataService
{
    private readonly ILogger<NadiaDataService> _logger;
    private readonly string _connectionString;
    private readonly AsyncRetryPolicy _retryPolicy;

    public NadiaDataService(
        ILogger<NadiaDataService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["NadiaConnectionString"] 
            ?? throw new InvalidOperationException("NadiaConnectionString not configured");

        // Retry policy: 3 tentatives avec backoff exponentiel
        _retryPolicy = Policy
            .Handle<SqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "NADIA connection retry {RetryCount} after {Delay}ms. Error: {Error}",
                        retryCount, timeSpan.TotalMilliseconds, exception.Message);
                });
    }

    public async Task<List<PurchaseOrderMessage>> GetPurchaseOrdersAsync(
        DateTime lastExecDate,
        TimeSpan lastExecTime,
        string correlationId)
    {
        var purchaseOrders = new List<PurchaseOrderMessage>();

        await _retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("NADIA_SPA_SUPHEADERMETADATA_AZURE", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = 300 // 5 minutes
            };

            // Paramètres stored procedure
            command.Parameters.AddWithValue("@LastExecDate", lastExecDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@LastExecTime", lastExecTime.ToString(@"hh\:mm\:ss"));

            _logger.LogInformation(
                "[{CorrelationId}] Executing NADIA SP with @LastExecDate={LastExecDate}, @LastExecTime={LastExecTime}",
                correlationId, lastExecDate.ToString("yyyy-MM-dd"), lastExecTime.ToString(@"hh\:mm\:ss"));

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var po = MapToPurchaseOrder(reader);
                
                // Appliquer les filtres métier
                if (ShouldIncludePurchaseOrder(po))
                {
                    purchaseOrders.Add(po);
                }
            }
        });

        return purchaseOrders;
    }

    private PurchaseOrderMessage MapToPurchaseOrder(SqlDataReader reader)
    {
        return new PurchaseOrderMessage
        {
            PoNumber = GetStringValue(reader, "PoNumber"),
            MdmNumber = GetStringValue(reader, "MDMNumber"),
            PkmGuid = GetStringValue(reader, "PKMGuid"),
            PkmEmail = GetStringValue(reader, "PKMEmail"),
            ProductCode = GetStringValue(reader, "ProductCode"),
            Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
            FirstDelivery = GetNullableDateTime(reader, "FirstDelivery"),
            LastDelivery = GetNullableDateTime(reader, "LastDelivery"),
            CloseOut = CalculateCloseOut(reader),
            DateModified = reader.GetDateTime(reader.GetOrdinal("DateModified")),
            ProjectNumber = GetStringValue(reader, "ProjectNumber"),
            Description = GetStringValue(reader, "Description")
        };
    }

    private bool ShouldIncludePurchaseOrder(PurchaseOrderMessage po)
    {
        // Filtre: Amount > 100,000
        if (po.Amount <= 100000)
        {
            _logger.LogDebug("PO {PoNumber} filtered: Amount {Amount} <= 100K", 
                po.PoNumber, po.Amount);
            return false;
        }

        // Filtre: ProductCode dans la liste autorisée
        var allowedProductCodes = new[] { "PKG", "EQT", "BLK", "SER", "LOG" };
        var productCodePrefix = po.ProductCode.Split('.').FirstOrDefault() ?? "";
        
        if (!allowedProductCodes.Contains(productCodePrefix))
        {
            _logger.LogDebug("PO {PoNumber} filtered: ProductCode {ProductCode} not in allowed list",
                po.PoNumber, po.ProductCode);
            return false;
        }

        return true;
    }

    private bool CalculateCloseOut(SqlDataReader reader)
    {
        // Logique Close Out selon les règles métier NADIA
        var firstDelivery = GetNullableDateTime(reader, "FirstDelivery");
        var lastDelivery = GetNullableDateTime(reader, "LastDelivery");
        var today = DateTime.Today;

        if (!firstDelivery.HasValue || !lastDelivery.HasValue)
            return false;

        // Close Out si dernière livraison passée et > 90 jours
        return lastDelivery.Value < today && (today - lastDelivery.Value).TotalDays > 90;
    }

    private string GetStringValue(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }

    private DateTime? GetNullableDateTime(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }
}
