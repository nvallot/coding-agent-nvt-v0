#nullable enable

using System.Diagnostics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CsvProcessor.Functions.Services;
using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Functions;

/// <summary>
/// Azure Function triggered by blob uploads to process CSV files.
/// Implements FR-001 (Déclenchement Automatique) and orchestrates the full processing pipeline.
/// </summary>
public sealed class CsvBlobTriggerFunction
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ICsvParserService _csvParserService;
    private readonly IJsonTransformerService _transformerService;
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly ILogger<CsvBlobTriggerFunction> _logger;

    public CsvBlobTriggerFunction(
        IIdempotencyService idempotencyService,
        ICsvParserService csvParserService,
        IJsonTransformerService transformerService,
        IServiceBusPublisher serviceBusPublisher,
        ILogger<CsvBlobTriggerFunction> logger)
    {
        _idempotencyService = idempotencyService;
        _csvParserService = csvParserService;
        _transformerService = transformerService;
        _serviceBusPublisher = serviceBusPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Processes CSV files uploaded to the configured blob container.
    /// Only processes .csv files; other formats are ignored.
    /// </summary>
    /// <param name="stream">The blob content stream.</param>
    /// <param name="name">The blob name (file path in container).</param>
    /// <param name="ct">Cancellation token.</param>
    [Function("CsvBlobTrigger")]
    public async Task RunAsync(
        [BlobTrigger("%SourceContainerName%/{name}", Connection = "SourceBlobConnection")] 
        Stream stream,
        string name,
        CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["FileName"] = name
        });

        _logger.LogInformation(
            "Processing blob: {BlobName}, Size: {Size} bytes",
            name, stream.Length);

        // FR-001: Only process CSV files
        if (!name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Ignoring non-CSV file: {BlobName}", name);
            return;
        }

        string fileHash;
        try
        {
            // ADR-004: Compute MD5 hash for idempotency
            fileHash = _idempotencyService.ComputeHash(stream);
            _logger.LogDebug("Computed file hash: {FileHash}", fileHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to compute hash for {BlobName}", name);
            throw;
        }

        // FR-006: Check idempotency
        if (await _idempotencyService.IsAlreadyProcessedAsync(fileHash, ct))
        {
            _logger.LogInformation(
                "File already processed (idempotency check): {BlobName}, Hash: {FileHash}",
                name, fileHash);
            return;
        }

        // Mark as processing to prevent parallel processing
        await _idempotencyService.MarkAsProcessingAsync(fileHash, name, ct);

        try
        {
            // FR-002 & FR-003: Parse and validate CSV
            var parseResult = await _csvParserService.ParseAsync(stream, ct);

            if (!parseResult.HasValidLines)
            {
                var errorMsg = $"No valid lines found in file. Total errors: {parseResult.InvalidLines.Count}";
                _logger.LogWarning(errorMsg);
                
                await _idempotencyService.MarkAsFailedAsync(fileHash, errorMsg, ct);
                return;
            }

            // FR-004: Transform to JSON
            var messages = _transformerService
                .TransformMany(parseResult.ValidLines, name, correlationId)
                .ToList();

            // FR-005: Publish to Service Bus in batches
            var publishedCount = await _serviceBusPublisher.PublishBatchAsync(messages, ct);

            stopwatch.Stop();

            // Mark as completed with statistics
            var result = new ProcessingResult
            {
                FileName = name,
                FileHash = fileHash,
                LinesTotal = parseResult.TotalLines,
                LinesValid = parseResult.ValidLines.Count,
                LinesInvalid = parseResult.InvalidLines.Count,
                MessagesPublished = publishedCount,
                Duration = stopwatch.Elapsed,
                Success = true
            };

            await _idempotencyService.MarkAsCompletedAsync(fileHash, result, ct);

            // FR-007: Log final metrics
            _logger.LogInformation(
                "Successfully processed {BlobName}: " +
                "Lines={LinesTotal} (Valid={LinesValid}, Invalid={LinesInvalid}), " +
                "MessagesPublished={MessagesPublished}, Duration={DurationMs}ms",
                name,
                result.LinesTotal,
                result.LinesValid,
                result.LinesInvalid,
                result.MessagesPublished,
                result.Duration.TotalMilliseconds);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Processing cancelled for {BlobName}", name);
            await _idempotencyService.MarkAsFailedAsync(fileHash, "Processing cancelled", ct);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process {BlobName}", name);
            await _idempotencyService.MarkAsFailedAsync(fileHash, ex.Message, CancellationToken.None);
            throw;
        }
    }
}
