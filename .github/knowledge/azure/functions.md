---
applyTo: "**/*.cs,**/Functions/**"
type: knowledge
---

# Knowledge: Azure Functions

## 📋 Vue d'ensemble

**Azure Functions** est la plateforme serverless d'Azure pour exécuter du code à la demande sans gérer l'infrastructure.

## 🎯 Use Cases

- APIs HTTP légères
- Traitement de messages (Service Bus, Event Hubs)
- Orchestration de workflows (Durable Functions)
- Scheduled jobs (Timer triggers)
- Réaction aux événements (Blob, Cosmos DB)

## 🏗️ Modèles d'Exécution

### In-Process (.NET)

- Même process que le host
- Accès direct aux APIs Functions
- Dépendance aux versions .NET du host
- **Déprécié** pour nouveaux projets

### Isolated Worker (.NET) ✅ Recommandé

- Process séparé
- Contrôle total des dépendances
- Support .NET 8/9/10
- Middleware personnalisé

```
┌─────────────────┐     ┌─────────────────┐
│  Functions Host │────→│  Worker Process │
│    (Runtime)    │←────│   (Your Code)   │
└─────────────────┘     └─────────────────┘
```

## 💻 Exemples

### Program.cs (Isolated Worker)

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        // Middleware custom
        builder.UseMiddleware<CorrelationIdMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        // Configuration
        services.Configure<LucyApiOptions>(
            context.Configuration.GetSection("LucyApi"));
        
        // Services
        services.AddSingleton<ILucyService, LucyService>();
        services.AddScoped<IOrderRepository, CosmosOrderRepository>();
        
        // HttpClient avec Polly
        services.AddHttpClient<IExternalApi, ExternalApiClient>()
            .AddPolicyHandler(GetRetryPolicy());
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddAzureKeyVault(
            new Uri(Environment.GetEnvironmentVariable("KeyVaultUri")!),
            new DefaultAzureCredential());
    })
    .Build();

await host.RunAsync();
```

### HTTP Trigger

```csharp
public class OrderFunctions(
    ILogger<OrderFunctions> logger,
    IOrderService orderService)
{
    [Function(nameof(GetOrder))]
    public async Task<HttpResponseData> GetOrder(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id}")] 
        HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting order {OrderId}", id);
        
        var order = await orderService.GetByIdAsync(id, cancellationToken);
        
        if (order is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(order, cancellationToken);
        return response;
    }
    
    [Function(nameof(CreateOrder))]
    public async Task<HttpResponseData> CreateOrder(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] 
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var order = await req.ReadFromJsonAsync<CreateOrderDto>(cancellationToken);
        
        if (order is null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid order data");
            return badRequest;
        }
        
        var created = await orderService.CreateAsync(order, cancellationToken);
        
        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Location", $"/api/orders/{created.Id}");
        await response.WriteAsJsonAsync(created, cancellationToken);
        return response;
    }
}
```

### Service Bus Trigger

```csharp
[Function(nameof(ProcessPurchaseOrder))]
public async Task ProcessPurchaseOrder(
    [ServiceBusTrigger("purchase-orders", Connection = "ServiceBusConnection")] 
    ServiceBusReceivedMessage message,
    ServiceBusMessageActions messageActions,
    CancellationToken cancellationToken)
{
    var correlationId = message.CorrelationId ?? Guid.NewGuid().ToString();
    using var scope = logger.BeginScope(new Dictionary<string, object>
    {
        ["CorrelationId"] = correlationId,
        ["MessageId"] = message.MessageId
    });
    
    try
    {
        var order = message.Body.ToObjectFromJson<PurchaseOrderMessage>();
        logger.LogInformation("Processing order {OrderNumber}", order.OrderNumber);
        
        await orderProcessor.ProcessAsync(order, cancellationToken);
        
        await messageActions.CompleteMessageAsync(message, cancellationToken);
        logger.LogInformation("Order {OrderNumber} processed successfully", order.OrderNumber);
    }
    catch (ValidationException ex)
    {
        logger.LogWarning(ex, "Validation failed for order");
        await messageActions.DeadLetterMessageAsync(
            message, 
            deadLetterReason: "ValidationFailed",
            deadLetterErrorDescription: ex.Message,
            cancellationToken: cancellationToken);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to process order");
        throw; // Retry automatique
    }
}
```

### Timer Trigger

```csharp
[Function(nameof(DailyCleanup))]
public async Task DailyCleanup(
    [TimerTrigger("0 0 2 * * *")] TimerInfo timer, // 2h00 chaque jour
    CancellationToken cancellationToken)
{
    logger.LogInformation("Starting daily cleanup at {Time}", DateTime.UtcNow);
    
    if (timer.IsPastDue)
    {
        logger.LogWarning("Timer is running late!");
    }
    
    await cleanupService.CleanupOldRecordsAsync(cancellationToken);
    
    logger.LogInformation("Next cleanup scheduled at {NextRun}", timer.ScheduleStatus?.Next);
}
```

### Blob Trigger

```csharp
[Function(nameof(ProcessUploadedFile))]
public async Task ProcessUploadedFile(
    [BlobTrigger("uploads/{name}", Connection = "StorageConnection")] 
    Stream blobStream,
    string name,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Processing uploaded file: {FileName}", name);
    
    using var reader = new StreamReader(blobStream);
    var content = await reader.ReadToEndAsync(cancellationToken);
    
    await fileProcessor.ProcessAsync(name, content, cancellationToken);
}
```

## 🔧 Triggers & Bindings

### Triggers (déclencheurs)

| Trigger | Usage |
|---------|-------|
| HTTP | APIs REST |
| Timer | Jobs schedulés (CRON) |
| Service Bus | Messages queue/topic |
| Event Hubs | Streaming events |
| Blob Storage | Fichiers uploadés |
| Cosmos DB | Change feed |
| Event Grid | Events Azure |

### Bindings (entrées/sorties)

| Binding | Direction | Usage |
|---------|-----------|-------|
| Blob | In/Out | Lire/écrire fichiers |
| Table | In/Out | Azure Table Storage |
| Cosmos DB | In/Out | Documents Cosmos |
| Service Bus | Out | Envoyer messages |
| Event Hubs | Out | Publier events |

## ✅ Bonnes Pratiques

### Structure Projet

```
Functions/
├── Program.cs              # Host configuration
├── host.json               # Runtime configuration
├── local.settings.json     # Local dev settings (gitignore!)
├── OrderFunctions.cs       # Functions par domaine
├── Contracts/              # Interfaces
├── Services/               # Implémentations
├── Models/                 # DTOs, Entities
└── Helpers/                # Utilities
```

### Configuration

```json
// host.json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true,
        "maxTelemetryItemsPerSecond": 20
      }
    }
  },
  "extensions": {
    "serviceBus": {
      "prefetchCount": 100,
      "messageHandlerOptions": {
        "maxConcurrentCalls": 16,
        "autoComplete": false
      }
    }
  }
}
```

### Error Handling

- Utiliser try/catch explicites
- Logger avec correlation ID
- Dead Letter pour messages non traitables
- Ne pas catch Exception générique sans rethrow

### Performance

- Réutiliser HttpClient (singleton)
- Configurer prefetch pour Service Bus
- Utiliser async/await partout
- Éviter sync over async (.Result, .Wait())

## 💰 Coûts

| Plan | Modèle | Use Case |
|------|--------|----------|
| Consumption | Pay per execution | Workloads sporadiques |
| Premium | Pre-warmed instances | Production, cold start critique |
| Dedicated | App Service Plan | Workloads constants |

Facteurs: Nombre d'exécutions, durée (GB-s), réseau.

## 📚 Références

- [Azure Functions Documentation](https://learn.microsoft.com/azure/azure-functions/)
- [Isolated Worker Guide](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide)
- [Triggers & Bindings](https://learn.microsoft.com/azure/azure-functions/functions-triggers-bindings)
