#nullable enable

using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CsvProcessor.Functions.Configuration;
using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Publishes messages to Azure Service Bus in optimized batches.
/// Implements FR-005 (Publication Service Bus) and ADR-003 (Batch publication).
/// </summary>
public sealed class ServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
{
    private readonly ServiceBusSender _sender;
    private readonly AppSettings _settings;
    private readonly ILogger<ServiceBusPublisher> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ServiceBusPublisher(
        ServiceBusClient client,
        IOptions<AppSettings> settings,
        ILogger<ServiceBusPublisher> logger)
    {
        _settings = settings.Value;
        _sender = client.CreateSender(_settings.ServiceBusTopicName);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> PublishBatchAsync(IEnumerable<OrderMessage> messages, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        var messageList = messages.ToList();
        if (messageList.Count == 0)
        {
            _logger.LogDebug("No messages to publish");
            return 0;
        }

        var totalPublished = 0;
        var batches = ChunkMessages(messageList, _settings.MaxBatchSize);

        foreach (var batch in batches)
        {
            ct.ThrowIfCancellationRequested();

            using var messageBatch = await _sender.CreateMessageBatchAsync(ct);
            var batchCount = 0;

            foreach (var message in batch)
            {
                var serviceBusMessage = CreateServiceBusMessage(message);
                
                if (!messageBatch.TryAddMessage(serviceBusMessage))
                {
                    // Batch is full (size limit reached), send what we have
                    if (messageBatch.Count > 0)
                    {
                        await SendBatchWithRetryAsync(messageBatch, ct);
                        totalPublished += messageBatch.Count;
                    }

                    // Start a new batch with the current message
                    using var newBatch = await _sender.CreateMessageBatchAsync(ct);
                    if (!newBatch.TryAddMessage(serviceBusMessage))
                    {
                        _logger.LogError(
                            "Message too large for Service Bus: {OrderId}", 
                            message.OrderId);
                        continue;
                    }

                    await SendBatchWithRetryAsync(newBatch, ct);
                    totalPublished++;
                }
                else
                {
                    batchCount++;
                }
            }

            // Send remaining messages in batch
            if (messageBatch.Count > 0)
            {
                await SendBatchWithRetryAsync(messageBatch, ct);
                totalPublished += messageBatch.Count;
            }
        }

        _logger.LogInformation(
            "Published {TotalPublished} messages to topic {TopicName}",
            totalPublished, _settings.ServiceBusTopicName);

        return totalPublished;
    }

    private ServiceBusMessage CreateServiceBusMessage(OrderMessage message)
    {
        var json = JsonSerializer.Serialize(message, JsonOptions);
        var sbMessage = new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            MessageId = $"{message.OrderId}-{message.Metadata.LineNumber}",
            CorrelationId = message.Metadata.CorrelationId,
            Subject = message.OrderId
        };

        // Add custom properties for filtering
        sbMessage.ApplicationProperties["orderId"] = message.OrderId;
        sbMessage.ApplicationProperties["sourceFile"] = message.Metadata.SourceFile;

        return sbMessage;
    }

    private async Task SendBatchWithRetryAsync(
        ServiceBusMessageBatch batch, 
        CancellationToken ct)
    {
        const int maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                await _sender.SendMessagesAsync(batch, ct);
                _logger.LogDebug("Sent batch of {Count} messages", batch.Count);
                return;
            }
            catch (ServiceBusException ex) when (ex.IsTransient && retryCount < maxRetries - 1)
            {
                retryCount++;
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                
                _logger.LogWarning(ex,
                    "Transient error sending batch, retry {RetryCount}/{MaxRetries} after {Delay}ms",
                    retryCount, maxRetries, delay.TotalMilliseconds);

                await Task.Delay(delay, ct);
            }
        }
    }

    private static IEnumerable<IEnumerable<T>> ChunkMessages<T>(IEnumerable<T> source, int chunkSize)
    {
        return source
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / chunkSize)
            .Select(g => g.Select(x => x.item));
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
    }
}
