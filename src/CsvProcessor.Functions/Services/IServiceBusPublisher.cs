#nullable enable

using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Publishes messages to Azure Service Bus.
/// Implements FR-005 (Publication Service Bus).
/// </summary>
public interface IServiceBusPublisher
{
    /// <summary>
    /// Publishes messages to the Service Bus topic in batches.
    /// </summary>
    /// <param name="messages">The messages to publish.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of messages successfully published.</returns>
    Task<int> PublishBatchAsync(IEnumerable<OrderMessage> messages, CancellationToken ct = default);
}
