---
applyTo: "**/*.cs,**/Functions/**"
type: knowledge
---

# Knowledge: C# Code Examples

## 📋 Vue d'ensemble

Exemples de code C# et patterns pour Azure Functions et applications .NET.

## 💻 Patterns .NET 10

### Records pour DTOs

```csharp
// Immutable DTO avec validation
public record PurchaseOrderDto(
    string OrderNumber,
    string VendorId,
    decimal Amount,
    DateTime OrderDate)
{
    // Computed property
    public string DisplayName => $"{OrderNumber} - {VendorId}";
    
    // Validation
    public bool IsValid => Amount > 0 && !string.IsNullOrEmpty(OrderNumber);
}

// Record avec init setters pour flexibilité
public record CreateOrderRequest
{
    public required string VendorId { get; init; }
    public required decimal Amount { get; init; }
    public string? Description { get; init; }
    public List<OrderLineDto> Lines { get; init; } = [];
}

// Record struct pour performance (value type)
public readonly record struct MoneyAmount(decimal Value, string Currency);
```

### Primary Constructors

```csharp
// Service avec primary constructor
public class OrderService(
    ILogger<OrderService> logger,
    IOrderRepository repository,
    IOptions<OrderServiceOptions> options)
{
    private readonly OrderServiceOptions _options = options.Value;
    
    public async Task<Order?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        logger.LogInformation("Getting order {OrderId}", id);
        return await repository.GetByIdAsync(id, ct);
    }
}

// Controller/Function avec primary constructor
public class OrderFunctions(
    ILogger<OrderFunctions> logger,
    IOrderService orderService)
{
    [Function(nameof(GetOrder))]
    public async Task<HttpResponseData> GetOrder(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id}")] 
        HttpRequestData req,
        string id)
    {
        var order = await orderService.GetByIdAsync(id);
        // ...
    }
}
```

### Pattern Matching

```csharp
// Switch expression avec patterns
public string GetOrderStatus(Order order) => order switch
{
    { Status: OrderStatus.Draft } => "En cours de création",
    { Status: OrderStatus.Submitted, Amount: > 10000 } => "En attente approbation (montant élevé)",
    { Status: OrderStatus.Submitted } => "En attente approbation",
    { Status: OrderStatus.Approved, ShippedDate: null } => "Approuvé, en attente expédition",
    { Status: OrderStatus.Approved } => "Approuvé et expédié",
    { Status: OrderStatus.Rejected, RejectionReason: var reason } => $"Rejeté: {reason}",
    _ => "Statut inconnu"
};

// Property pattern avec déconstruction
public decimal CalculateDiscount(Customer customer, Order order) => (customer, order) switch
{
    ({ IsPremium: true }, { Amount: > 1000 }) => order.Amount * 0.15m,
    ({ IsPremium: true }, _) => order.Amount * 0.10m,
    (_, { Amount: > 5000 }) => order.Amount * 0.05m,
    _ => 0
};

// List patterns (.NET 8+)
public string AnalyzeOrders(List<Order> orders) => orders switch
{
    [] => "Aucune commande",
    [var single] => $"Une commande: {single.OrderNumber}",
    [var first, .., var last] => $"De {first.OrderNumber} à {last.OrderNumber}",
};
```

### Options Pattern

```csharp
// Configuration class
public class LucyApiOptions
{
    public const string SectionName = "LucyApi";
    
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
    public int MaxRetries { get; init; } = 3;
}

// Registration in Program.cs
services.Configure<LucyApiOptions>(configuration.GetSection(LucyApiOptions.SectionName));

// Usage avec validation
services.AddOptions<LucyApiOptions>()
    .Bind(configuration.GetSection(LucyApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Injection
public class LucyService(IOptions<LucyApiOptions> options, HttpClient httpClient)
{
    private readonly LucyApiOptions _options = options.Value;
    
    public async Task<Person?> GetPersonAsync(string id)
    {
        httpClient.BaseAddress = new Uri(_options.BaseUrl);
        // ...
    }
}
```

### Dependency Injection Patterns

```csharp
// Interface segregation
public interface IOrderReader
{
    Task<Order?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByVendorAsync(string vendorId, CancellationToken ct = default);
}

public interface IOrderWriter
{
    Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}

public interface IOrderRepository : IOrderReader, IOrderWriter { }

// Keyed services (.NET 8+)
services.AddKeyedSingleton<INotificationService, EmailNotificationService>("email");
services.AddKeyedSingleton<INotificationService, SmsNotificationService>("sms");

// Usage
public class OrderService([FromKeyedServices("email")] INotificationService notifier)
{
    // ...
}

// Factory pattern
services.AddSingleton<IOrderProcessorFactory, OrderProcessorFactory>();

public interface IOrderProcessorFactory
{
    IOrderProcessor Create(OrderType type);
}
```

### Async Patterns

```csharp
// Proper async/await
public async Task<Result<Order>> ProcessOrderAsync(
    string orderId, 
    CancellationToken cancellationToken = default)
{
    try
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<Order>.NotFound($"Order {orderId} not found");
        
        // Parallel async operations
        var (vendor, inventory) = await (
            _vendorService.GetByIdAsync(order.VendorId, cancellationToken),
            _inventoryService.CheckAvailabilityAsync(order.Lines, cancellationToken)
        );
        
        // Process...
        
        return Result<Order>.Success(order);
    }
    catch (OperationCanceledException)
    {
        _logger.LogWarning("Operation cancelled for order {OrderId}", orderId);
        throw;
    }
}

// IAsyncEnumerable for streaming
public async IAsyncEnumerable<Order> GetOrdersStreamAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var order in _repository.StreamAllAsync(cancellationToken))
    {
        yield return order;
    }
}

// ConfigureAwait in libraries
public async Task<string> FetchDataAsync()
{
    var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
}
```

### Error Handling

```csharp
// Result pattern
public readonly record struct Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess { get; }
    
    private Result(T value) => (Value, IsSuccess) = (value, true);
    private Result(string error) => (Error, IsSuccess) = (error, false);
    
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);
    public static Result<T> NotFound(string message) => new($"NotFound: {message}");
    
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error!);
}

// Usage
var result = await orderService.ProcessOrderAsync(orderId);

return result.Match(
    onSuccess: order => req.CreateResponse(HttpStatusCode.OK),
    onFailure: error => error.StartsWith("NotFound")
        ? req.CreateResponse(HttpStatusCode.NotFound)
        : req.CreateResponse(HttpStatusCode.BadRequest)
);
```

## 🧪 Testing Patterns

```csharp
// Arrange-Act-Assert avec xUnit
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly Mock<ILogger<OrderService>> _loggerMock = new();
    private readonly OrderService _sut;
    
    public OrderServiceTests()
    {
        _sut = new OrderService(_loggerMock.Object, _repositoryMock.Object, Options.Create(new OrderServiceOptions()));
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        // Arrange
        var orderId = "ORD-001";
        var expected = new Order { Id = orderId, Amount = 100 };
        _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        
        // Act
        var result = await _sut.GetByIdAsync(orderId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        _repositoryMock.Verify(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Theory]
    [InlineData(100, 0)]      // No discount
    [InlineData(1000, 50)]    // 5% discount
    [InlineData(5000, 500)]   // 10% discount
    public void CalculateDiscount_ReturnsExpectedAmount(decimal amount, decimal expectedDiscount)
    {
        // Arrange
        var order = new Order { Amount = amount };
        
        // Act
        var discount = _sut.CalculateDiscount(order);
        
        // Assert
        Assert.Equal(expectedDiscount, discount);
    }
}
```

## 📚 Références

- [C# Language Reference](https://learn.microsoft.com/dotnet/csharp/language-reference/)
- [.NET API Browser](https://learn.microsoft.com/dotnet/api/)
