---
applyTo: "**/*.cs,**/Functions/**"
excludeAgent: ["code-review"]
---

# C# .NET 10 - Standards & Patterns

## MUST (Bloquant)

### Async/Await
- Toujours `async Task` ou `async ValueTask` pour I/O
- Jamais `.Result` ou `.Wait()` (deadlock risk)
- Toujours `ConfigureAwait(false)` dans les libraries
- Utiliser `CancellationToken` pour opérations longues

### Dependency Injection
- Constructeur injection uniquement (pas de Service Locator)
- Interfaces pour toutes les dépendances externes
- Scoped pour DbContext, Singleton pour HttpClient
- Jamais `new` pour les services injectables

### Sécurité
- Jamais de secrets en dur → Azure Key Vault
- Utiliser `IOptions<T>` pour la configuration
- Valider toutes les entrées utilisateur
- Paramétrer les requêtes SQL (pas de concaténation)

## SHOULD (Fortement recommandé)

### Naming Conventions
```csharp
public class PurchaseOrderService { }     // PascalCase classes
private readonly ILogger _logger;          // _camelCase private fields
public string OrderId { get; set; }        // PascalCase properties
public async Task ProcessAsync() { }       // Suffix Async pour async
```

### Patterns .NET 10
```csharp
// Records pour DTOs (immutables)
public record PurchaseOrderDto(string Id, string Vendor, decimal Amount);

// Primary constructors
public class OrderService(ILogger<OrderService> logger, IOrderRepository repo)
{
    public async Task<Order> GetAsync(string id) => await repo.GetByIdAsync(id);
}

// Pattern matching
return order switch
{
    { Status: OrderStatus.Pending } => ProcessPending(order),
    { Status: OrderStatus.Approved, Amount: > 10000 } => ProcessLargeOrder(order),
    _ => ProcessDefault(order)
};
```

### Options Pattern
```csharp
// Configuration classe
public class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";
    public required string ConnectionString { get; init; }
    public required string ContainerName { get; init; }
}

// Registration
services.Configure<AzureStorageOptions>(config.GetSection(AzureStorageOptions.SectionName));

// Usage
public class BlobService(IOptions<AzureStorageOptions> options)
{
    private readonly AzureStorageOptions _options = options.Value;
}
```

## Azure Functions (Isolated Worker)

### Structure Standard
```csharp
[Function(nameof(ProcessPurchaseOrder))]
public async Task<HttpResponseData> ProcessPurchaseOrder(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req,
    FunctionContext context,
    CancellationToken cancellationToken)
{
    var logger = context.GetLogger<OrderFunction>();
    
    try
    {
        var order = await req.ReadFromJsonAsync<PurchaseOrderDto>(cancellationToken);
        // Process...
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result, cancellationToken);
        return response;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to process order");
        return req.CreateResponse(HttpStatusCode.InternalServerError);
    }
}
```

### Program.cs Pattern
```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.Configure<AzureStorageOptions>(context.Configuration.GetSection("AzureStorage"));
        services.AddSingleton<IOrderRepository, CosmosOrderRepository>();
        services.AddHttpClient<IExternalApi, ExternalApiClient>();
    })
    .Build();

await host.RunAsync();
```

## MAY (Optionnel)

### Performance
- `ValueTask` pour hot paths fréquemment synchrones
- `IAsyncEnumerable<T>` pour streaming de données
- `Span<T>` et `Memory<T>` pour buffers
- Object pooling pour allocations fréquentes

### Testing avec xUnit
```csharp
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repoMock = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(Mock.Of<ILogger<OrderService>>(), _repoMock.Object);
    }

    [Fact]
    public async Task GetAsync_ExistingOrder_ReturnsOrder()
    {
        // Arrange
        var expected = new Order { Id = "123" };
        _repoMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(expected);

        // Act
        var result = await _sut.GetAsync("123");

        // Assert
        Assert.Equal(expected.Id, result.Id);
    }
}
```
