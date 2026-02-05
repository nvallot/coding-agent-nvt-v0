---
name: azure-functions
description: >
  Skill pour développer des Azure Functions en C# (Isolated Worker Model).
  Triggers: azure function, function app, serverless, trigger, binding, durable functions
version: 1.0.0
---

# Azure Functions Skill

## Purpose

Guider le développement d'Azure Functions en C# avec le modèle Isolated Worker (.NET 8+).

## Prerequisites

- .NET 8 SDK ou supérieur
- Azure Functions Core Tools v4
- Extension VS Code Azure Functions

## Triggers & Bindings

| Trigger | Use Case | Exemple |
|---------|----------|---------|
| **HttpTrigger** | API REST, Webhooks | `[HttpTrigger(AuthorizationLevel.Function, "get", "post")]` |
| **TimerTrigger** | Jobs planifiés | `[TimerTrigger("0 */5 * * * *")]` (every 5 min) |
| **BlobTrigger** | Réaction upload fichier | `[BlobTrigger("container/{name}")]` |
| **QueueTrigger** | Message queue processing | `[QueueTrigger("myqueue")]` |
| **ServiceBusTrigger** | Enterprise messaging | `[ServiceBusTrigger("mytopic", "mysubscription")]` |
| **CosmosDBTrigger** | Change feed processing | `[CosmosDBTrigger("database", "collection")]` |

## Templates

### HTTP Function (REST API)

```csharp
public class OrderApi(IOrderService orderService, ILogger<OrderApi> logger)
{
    [Function("GetOrder")]
    public async Task<HttpResponseData> GetOrder(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id}")] HttpRequestData req,
        string id,
        CancellationToken ct)
    {
        logger.LogInformation("Getting order {OrderId}", id);
        
        var order = await orderService.GetByIdAsync(id, ct);
        
        if (order is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(order, ct);
        return response;
    }

    [Function("CreateOrder")]
    public async Task<HttpResponseData> CreateOrder(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req,
        CancellationToken ct)
    {
        var dto = await req.ReadFromJsonAsync<CreateOrderDto>(ct);
        
        if (dto is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        var order = await orderService.CreateAsync(dto, ct);
        
        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Location", $"/api/orders/{order.Id}");
        await response.WriteAsJsonAsync(order, ct);
        return response;
    }
}
```

### Timer Function (Scheduled Job)

```csharp
public class ScheduledJobs(IDataSyncService syncService, ILogger<ScheduledJobs> logger)
{
    [Function("DailySync")]
    public async Task DailySync(
        [TimerTrigger("0 0 2 * * *")] TimerInfo timer, // 2:00 AM daily
        CancellationToken ct)
    {
        logger.LogInformation("Daily sync started at {Time}", DateTime.UtcNow);
        
        await syncService.SyncAllAsync(ct);
        
        logger.LogInformation("Daily sync completed. Next run: {NextRun}", timer.ScheduleStatus?.Next);
    }
}
```

### Queue Function (Message Processing)

```csharp
public class QueueProcessor(IOrderProcessor processor, ILogger<QueueProcessor> logger)
{
    [Function("ProcessOrderQueue")]
    public async Task ProcessOrder(
        [QueueTrigger("orders-to-process")] OrderMessage message,
        CancellationToken ct)
    {
        logger.LogInformation("Processing order {OrderId}", message.OrderId);
        
        await processor.ProcessAsync(message, ct);
    }
}
```

## Durable Functions

### Orchestration Pattern

```csharp
public class OrderOrchestration
{
    [Function(nameof(ProcessOrderOrchestrator))]
    public async Task<OrderResult> ProcessOrderOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var orderId = context.GetInput<string>();
        
        // Step 1: Validate
        var isValid = await context.CallActivityAsync<bool>(nameof(ValidateOrder), orderId);
        if (!isValid) return new OrderResult(false, "Validation failed");
        
        // Step 2: Process payment
        var paymentResult = await context.CallActivityAsync<PaymentResult>(nameof(ProcessPayment), orderId);
        
        // Step 3: Send notification
        await context.CallActivityAsync(nameof(SendNotification), orderId);
        
        return new OrderResult(true, paymentResult.TransactionId);
    }

    [Function(nameof(ValidateOrder))]
    public async Task<bool> ValidateOrder([ActivityTrigger] string orderId)
    {
        // Validation logic
        return true;
    }

    [Function(nameof(ProcessPayment))]
    public async Task<PaymentResult> ProcessPayment([ActivityTrigger] string orderId)
    {
        // Payment logic
        return new PaymentResult(true, Guid.NewGuid().ToString());
    }

    [Function(nameof(SendNotification))]
    public async Task SendNotification([ActivityTrigger] string orderId)
    {
        // Notification logic
    }
}
```

## Configuration

### host.json
```json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true,
        "excludedTypes": "Request"
      }
    },
    "logLevel": {
      "default": "Information",
      "Host.Results": "Error",
      "Function": "Information"
    }
  },
  "extensions": {
    "queues": {
      "batchSize": 16,
      "maxDequeueCount": 5
    }
  }
}
```

### local.settings.json (DEV only)
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

## Best Practices

1. **Toujours** utiliser `CancellationToken`
2. **Toujours** logger avec des propriétés structurées
3. **Jamais** de secrets dans le code → Key Vault
4. **Utiliser** Managed Identity pour Azure services
5. **Configurer** retry policies pour résilience
