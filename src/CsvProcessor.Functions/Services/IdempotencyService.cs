#nullable enable

using System.Security.Cryptography;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CsvProcessor.Functions.Configuration;
using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Manages file processing tracking in Azure Table Storage for idempotency.
/// Implements FR-006 (Idempotence) and ADR-002 (Table Storage).
/// </summary>
public sealed class IdempotencyService : IIdempotencyService
{
    private readonly TableClient _tableClient;
    private readonly ILogger<IdempotencyService> _logger;
    private readonly TimeProvider _timeProvider;

    public IdempotencyService(
        TableServiceClient tableServiceClient,
        IOptions<AppSettings> settings,
        ILogger<IdempotencyService> logger,
        TimeProvider? timeProvider = null)
    {
        _tableClient = tableServiceClient.GetTableClient(settings.Value.IdempotencyTableName);
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;

        // Ensure table exists
        _tableClient.CreateIfNotExists();
    }

    /// <inheritdoc />
    public async Task<bool> IsAlreadyProcessedAsync(string fileHash, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileHash);

        var partitionKey = GetPartitionKey();

        try
        {
            var response = await _tableClient.GetEntityIfExistsAsync<ProcessedFileEntity>(
                partitionKey, fileHash, cancellationToken: ct);

            if (response.HasValue && response.Value is not null)
            {
                var entity = response.Value;
                
                // Consider "processing" status as already processed to prevent parallel processing
                if (entity.Status is ProcessingStatus.Completed or ProcessingStatus.Processing)
                {
                    _logger.LogInformation(
                        "File already processed: {FileHash}, Status: {Status}", 
                        fileHash, entity.Status);
                    return true;
                }

                // Failed status allows reprocessing
                _logger.LogInformation(
                    "File previously failed, allowing reprocessing: {FileHash}", 
                    fileHash);
                return false;
            }

            return false;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task MarkAsProcessingAsync(string fileHash, string fileName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileHash);

        var entity = new ProcessedFileEntity
        {
            PartitionKey = GetPartitionKey(),
            RowKey = fileHash,
            Status = ProcessingStatus.Processing,
            FileName = fileName,
            ProcessedAt = null
        };

        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
        
        _logger.LogDebug("Marked file as processing: {FileHash}", fileHash);
    }

    /// <inheritdoc />
    public async Task MarkAsCompletedAsync(string fileHash, ProcessingResult result, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileHash);
        ArgumentNullException.ThrowIfNull(result);

        var entity = new ProcessedFileEntity
        {
            PartitionKey = GetPartitionKey(),
            RowKey = fileHash,
            Status = ProcessingStatus.Completed,
            FileName = result.FileName,
            ProcessedAt = _timeProvider.GetUtcNow(),
            LinesTotal = result.LinesTotal,
            LinesValid = result.LinesValid,
            LinesInvalid = result.LinesInvalid
        };

        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
        
        _logger.LogInformation(
            "Marked file as completed: {FileHash}, Lines: {LinesValid}/{LinesTotal}", 
            fileHash, result.LinesValid, result.LinesTotal);
    }

    /// <inheritdoc />
    public async Task MarkAsFailedAsync(string fileHash, string errorMessage, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileHash);

        var partitionKey = GetPartitionKey();

        try
        {
            // Try to get existing entity to preserve metadata
            var response = await _tableClient.GetEntityIfExistsAsync<ProcessedFileEntity>(
                partitionKey, fileHash, cancellationToken: ct);

            var entity = response.HasValue && response.Value is not null
                ? response.Value
                : new ProcessedFileEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = fileHash
                };

            entity.Status = ProcessingStatus.Failed;
            entity.ErrorMessage = errorMessage;
            entity.ProcessedAt = _timeProvider.GetUtcNow();

            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark file as failed: {FileHash}", fileHash);
            throw;
        }

        _logger.LogWarning("Marked file as failed: {FileHash}, Error: {Error}", fileHash, errorMessage);
    }

    /// <inheritdoc />
    public string ComputeHash(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        stream.Position = 0; // Reset stream for subsequent reads
        
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Gets the partition key based on current year-month.
    /// This allows efficient cleanup of old records and good partition distribution.
    /// </summary>
    private string GetPartitionKey()
    {
        return _timeProvider.GetUtcNow().ToString("yyyy-MM");
    }
}
