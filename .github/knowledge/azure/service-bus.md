---
applyTo: "**/src/**,**/Functions/**,**/Development/**"
type: knowledge
---

# Knowledge: Azure Service Bus

## 📋 Vue d'ensemble

**Azure Service Bus** est un service de messaging enterprise managé pour découpler les applications et services.

## 🎯 Use Cases

- Communication asynchrone entre services
- Load leveling (absorber pics de charge)
- Patterns pub/sub avec Topics
- Transactions distribuées
- Intégration enterprise (SAP, ERP)

## 🏗️ Composants

### Namespace

Container logique pour queues et topics.

```
Namespace: sbns-{project}-{env}
├── Queue: orders-processing
├── Queue: notifications
├── Topic: purchase-orders
│   ├── Subscription: erp-sync
│   ├── Subscription: analytics
│   └── Subscription: audit
└── Topic: events
```

### Queues

Point-to-point messaging (1 sender → 1 receiver).

| Propriété | Description |
|-----------|-------------|
| FIFO | Ordre garanti (avec sessions) |
| At-least-once | Message livré au moins 1 fois |
| Dead Letter | Messages en échec isolés |
| TTL | Expiration configurable |

### Topics & Subscriptions

Pub/Sub messaging (1 sender → N receivers).

```
Publisher ──→ Topic ──→ Subscription A ──→ Consumer A
                   └──→ Subscription B ──→ Consumer B
                   └──→ Subscription C ──→ Consumer C
```

| Propriété | Description |
|-----------|-------------|
| Filters | SQL-like filters par subscription |
| Actions | Modifier headers en transit |
| Forward | Chaîner vers autre queue/topic |

## 💻 Exemples

### Connection String vs Managed Identity

```csharp
// ❌ Connection String (éviter en production)
var client = new ServiceBusClient(connectionString);

// ✅ Managed Identity (recommandé)
var client = new ServiceBusClient(
    "sbns-myproject-prd.servicebus.windows.net",
    new DefaultAzureCredential()
);
```

### Envoyer un message

```csharp
await using var sender = client.CreateSender("orders-processing");

var message = new ServiceBusMessage(JsonSerializer.SerializeToUtf8Bytes(order))
{
    ContentType = "application/json",
    Subject = "PurchaseOrder",
    CorrelationId = correlationId,
    MessageId = Guid.NewGuid().ToString(),
    ApplicationProperties =
    {
        ["OrderType"] = "Standard",
        ["Priority"] = "High"
    }
};

await sender.SendMessageAsync(message);
```

### Recevoir des messages

```csharp
await using var processor = client.CreateProcessor("orders-processing", new ServiceBusProcessorOptions
{
    AutoCompleteMessages = false,
    MaxConcurrentCalls = 10,
    PrefetchCount = 20
});

processor.ProcessMessageAsync += async args =>
{
    var order = JsonSerializer.Deserialize<Order>(args.Message.Body);
    
    // Process...
    
    await args.CompleteMessageAsync(args.Message);
};

processor.ProcessErrorAsync += args =>
{
    _logger.LogError(args.Exception, "Error processing message");
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();
```

### Azure Function Trigger

```csharp
[Function(nameof(ProcessOrder))]
public async Task ProcessOrder(
    [ServiceBusTrigger("orders-processing", Connection = "ServiceBusConnection")] 
    ServiceBusReceivedMessage message,
    ServiceBusMessageActions messageActions)
{
    try
    {
        var order = JsonSerializer.Deserialize<Order>(message.Body);
        await _orderService.ProcessAsync(order);
        await messageActions.CompleteMessageAsync(message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process order {MessageId}", message.MessageId);
        await messageActions.DeadLetterMessageAsync(message, "ProcessingFailed", ex.Message);
    }
}
```

### Topic avec filtres SQL

```csharp
// Subscription avec filtre SQL
// Filtre: "OrderType = 'Premium' AND Amount > 1000"

var message = new ServiceBusMessage(data)
{
    ApplicationProperties =
    {
        ["OrderType"] = "Premium",
        ["Amount"] = 5000
    }
};

await sender.SendMessageAsync(message);
```

## ✅ Bonnes Pratiques

### Messages

- Taille max: 256 KB (Standard), 100 MB (Premium)
- Utiliser `MessageId` unique pour déduplication
- Inclure `CorrelationId` pour tracing
- Structurer `ApplicationProperties` pour filtrage

### Sessions

- Utiliser pour garantir FIFO par entité
- `SessionId` = clé de groupement (ex: OrderId)
- Un seul consumer par session active

### Dead Letter

- Toujours monitorer la DLQ
- Configurer alertes si count > 0
- Implémenter replay automatique ou manuel

### Performance

- `PrefetchCount` pour réduire latence
- Batch send pour volumes importants
- Premium tier pour isolation et performance

## 💰 Coûts

| Tier | Messaging Units | Prix/mois | Use Case |
|------|-----------------|-----------|----------|
| Basic | N/A | ~$0.05/M ops | Dev/Test |
| Standard | N/A | ~$10/M ops | Production standard |
| Premium | 1-8 MU | ~$668/MU | High performance |

Facteurs: Nombre d'opérations, taille messages, rétention.

## 📚 Références

- [Service Bus Documentation](https://learn.microsoft.com/azure/service-bus-messaging/)
- [Service Bus SDK .NET](https://learn.microsoft.com/dotnet/api/overview/azure/messaging.servicebus-readme)
- [Pricing](https://azure.microsoft.com/pricing/details/service-bus/)
