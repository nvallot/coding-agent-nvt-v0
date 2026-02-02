using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FAP_65.RetrivePOVendor.Services;

public class LastExecutionService : ILastExecutionService
{
    private readonly ILogger<LastExecutionService> _logger;
    private readonly TableClient _tableClient;
    private const string TableName = "LastExecutionDate";
    private const string PartitionKey = "NADIA_SPA";
    private readonly string _environment;

    public LastExecutionService(
        ILogger<LastExecutionService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _environment = configuration["AZURE_FUNCTIONS_ENVIRONMENT"] ?? "DEV";

        var storageConnectionString = configuration["StorageAccount"]
            ?? throw new InvalidOperationException("StorageAccount not configured");

        // Si Managed Identity est disponible, utiliser le endpoint
        if (storageConnectionString.Contains("AccountName"))
        {
            var accountName = ExtractAccountName(storageConnectionString);
            var serviceUri = new Uri($"https://{accountName}.table.core.windows.net");
            _tableClient = new TableClient(serviceUri, TableName, new DefaultAzureCredential());
        }
        else
        {
            _tableClient = new TableClient(storageConnectionString, TableName);
        }

        // Créer la table si elle n'existe pas
        _tableClient.CreateIfNotExists();
    }

    public async Task<(DateTime? lastExecDate, TimeSpan lastExecTime)> GetLastExecutionAsync()
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<LastExecutionEntity>(
                PartitionKey, 
                _environment);

            var entity = response.Value;

            _logger.LogInformation(
                "Retrieved LastExecutionDate: {LastExecDate} {LastExecTime} for environment {Environment}",
                entity.LastExecutionDate, entity.LastExecutionTime, _environment);

            return (
                DateTime.Parse(entity.LastExecutionDate),
                TimeSpan.Parse(entity.LastExecutionTime)
            );
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogWarning(
                "No LastExecutionDate found for environment {Environment}. First run.",
                _environment);
            return (null, TimeSpan.Zero);
        }
    }

    public async Task UpdateLastExecutionAsync(DateTime date, TimeSpan time)
    {
        var entity = new LastExecutionEntity
        {
            PartitionKey = PartitionKey,
            RowKey = _environment,
            LastExecutionDate = date.ToString("yyyy-MM-dd"),
            LastExecutionTime = time.ToString(@"hh\:mm\:ss"),
            Timestamp = DateTimeOffset.UtcNow
        };

        await _tableClient.UpsertEntityAsync(entity);

        _logger.LogInformation(
            "Updated LastExecutionDate: {LastExecDate} {LastExecTime} for environment {Environment}",
            entity.LastExecutionDate, entity.LastExecutionTime, _environment);
    }

    private string ExtractAccountName(string connectionString)
    {
        var accountNamePart = connectionString.Split(';')
            .FirstOrDefault(part => part.StartsWith("AccountName="));
        
        return accountNamePart?.Split('=')[1] ?? throw new InvalidOperationException("AccountName not found in connection string");
    }
}

public class LastExecutionEntity : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string LastExecutionDate { get; set; } = string.Empty;
    public string LastExecutionTime { get; set; } = string.Empty;
}
