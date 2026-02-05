---
applyTo: "**/tests/**,**/*test*"
type: knowledge
---

# Knowledge: Testing Fixtures & Mocks

## 📋 Vue d'ensemble

Exemples de fixtures, mocks et patterns de test pour Python et C#.

## 🐍 Python (pytest)

### Fixtures de Base

```python
import pytest
from datetime import datetime
from pyspark.sql import SparkSession

# Fixture scope: function (default), class, module, session
@pytest.fixture(scope="session")
def spark():
    """SparkSession partagée pour tous les tests."""
    return SparkSession.builder \
        .master("local[2]") \
        .appName("tests") \
        .getOrCreate()

@pytest.fixture
def sample_orders(spark):
    """DataFrame d'ordres de test."""
    return spark.createDataFrame([
        ("ORD-001", "2026-01-15", 100.00, "PENDING"),
        ("ORD-002", "2026-01-16", 250.50, "APPROVED"),
        ("ORD-003", "2026-01-17", 75.00, "REJECTED"),
    ], ["order_id", "order_date", "amount", "status"])

@pytest.fixture
def empty_orders(spark):
    """DataFrame vide avec le bon schéma."""
    schema = "order_id STRING, order_date STRING, amount DOUBLE, status STRING"
    return spark.createDataFrame([], schema)
```

### Fixtures avec Paramètres

```python
@pytest.fixture(params=[
    {"env": "dev", "timeout": 30},
    {"env": "prd", "timeout": 60},
])
def config(request):
    """Configuration paramétrisée."""
    return request.param

def test_with_config(config):
    assert config["timeout"] > 0
```

### Mocking avec unittest.mock

```python
from unittest import mock
from unittest.mock import MagicMock, AsyncMock, patch

@pytest.fixture
def mock_api_client():
    """Mock du client API externe."""
    client = MagicMock()
    client.get_orders.return_value = [
        {"id": "1", "amount": 100},
        {"id": "2", "amount": 200},
    ]
    return client

def test_fetch_orders(mock_api_client):
    # Arrange
    service = OrderService(api_client=mock_api_client)
    
    # Act
    orders = service.fetch_orders()
    
    # Assert
    assert len(orders) == 2
    mock_api_client.get_orders.assert_called_once()

# Patch decorator
@patch("mymodule.external_api.fetch_data")
def test_with_patch(mock_fetch):
    mock_fetch.return_value = {"status": "ok"}
    result = process_data()
    assert result["status"] == "ok"

# Context manager
def test_with_context_manager():
    with patch("mymodule.datetime") as mock_dt:
        mock_dt.now.return_value = datetime(2026, 1, 1)
        result = get_current_date()
        assert result.year == 2026
```

### Fixtures Async

```python
import pytest_asyncio
from httpx import AsyncClient

@pytest_asyncio.fixture
async def async_client():
    """Client HTTP async pour tests API."""
    async with AsyncClient(base_url="http://test") as client:
        yield client

@pytest.mark.asyncio
async def test_async_endpoint(async_client):
    response = await async_client.get("/api/orders")
    assert response.status_code == 200
```

### Fixtures pour Bases de Données

```python
@pytest.fixture(scope="function")
def db_session():
    """Session de base de données avec rollback automatique."""
    engine = create_engine("sqlite:///:memory:")
    Base.metadata.create_all(engine)
    
    Session = sessionmaker(bind=engine)
    session = Session()
    
    yield session
    
    session.rollback()
    session.close()

@pytest.fixture
def populated_db(db_session):
    """Base de données avec données de test."""
    orders = [
        Order(id="1", amount=100),
        Order(id="2", amount=200),
    ]
    db_session.add_all(orders)
    db_session.commit()
    return db_session
```

## 🔷 C# (xUnit)

### Fixtures de Classe

```csharp
// Fixture partagée entre tests d'une classe
public class OrderServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    
    public OrderServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GetOrder_ReturnsOrder()
    {
        var order = await _fixture.DbContext.Orders.FindAsync("ORD-001");
        Assert.NotNull(order);
    }
}

// La fixture
public class DatabaseFixture : IAsyncLifetime
{
    public AppDbContext DbContext { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        DbContext = new AppDbContext(options);
        await SeedDataAsync();
    }
    
    public Task DisposeAsync()
    {
        DbContext.Dispose();
        return Task.CompletedTask;
    }
    
    private async Task SeedDataAsync()
    {
        DbContext.Orders.AddRange(
            new Order { Id = "ORD-001", Amount = 100 },
            new Order { Id = "ORD-002", Amount = 200 }
        );
        await DbContext.SaveChangesAsync();
    }
}
```

### Collection Fixtures (partage entre classes)

```csharp
// Définition de la collection
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

// Usage dans plusieurs classes
[Collection("Database")]
public class OrderServiceTests
{
    private readonly DatabaseFixture _fixture;
    
    public OrderServiceTests(DatabaseFixture fixture) => _fixture = fixture;
    
    // Tests...
}

[Collection("Database")]
public class OrderValidationTests
{
    private readonly DatabaseFixture _fixture;
    
    public OrderValidationTests(DatabaseFixture fixture) => _fixture = fixture;
    
    // Tests...
}
```

### Mocking avec Moq

```csharp
using Moq;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly Mock<INotificationService> _notificationMock;
    private readonly OrderService _sut;
    
    public OrderServiceTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _notificationMock = new Mock<INotificationService>();
        
        _sut = new OrderService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _notificationMock.Object);
    }
    
    [Fact]
    public async Task CreateOrder_SendsNotification()
    {
        // Arrange
        var request = new CreateOrderRequest { VendorId = "V001", Amount = 100 };
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Order { Id = "ORD-001" });
        
        // Act
        await _sut.CreateOrderAsync(request);
        
        // Assert
        _notificationMock.Verify(
            n => n.SendAsync(It.Is<Notification>(x => x.Type == "OrderCreated")),
            Times.Once);
    }
    
    [Fact]
    public async Task GetOrder_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync("NOT_EXISTS", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);
        
        // Act
        var result = await _sut.GetByIdAsync("NOT_EXISTS");
        
        // Assert
        Assert.Null(result);
    }
}
```

### AutoFixture pour génération de données

```csharp
using AutoFixture;
using AutoFixture.Xunit2;

public class OrderTests
{
    private readonly IFixture _fixture = new Fixture();
    
    [Fact]
    public void Order_WithRandomData_IsValid()
    {
        // Génère un Order avec des valeurs aléatoires
        var order = _fixture.Create<Order>();
        
        Assert.NotNull(order.Id);
        Assert.True(order.Amount >= 0);
    }
    
    [Theory, AutoData]
    public void ProcessOrder_WithAnyOrder_Succeeds(Order order)
    {
        // AutoData génère automatiquement l'order
        var result = _sut.Process(order);
        Assert.True(result.IsSuccess);
    }
    
    // Customization
    [Fact]
    public void Order_CustomizedFixture()
    {
        _fixture.Customize<Order>(c => c
            .With(o => o.Status, OrderStatus.Pending)
            .Without(o => o.ShippedDate));
        
        var order = _fixture.Create<Order>();
        
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.ShippedDate);
    }
}
```

### Test Data Builders

```csharp
// Builder pattern pour données de test
public class OrderBuilder
{
    private string _id = "ORD-001";
    private decimal _amount = 100m;
    private OrderStatus _status = OrderStatus.Pending;
    private string _vendorId = "V001";
    private List<OrderLine> _lines = new();
    
    public OrderBuilder WithId(string id)
    {
        _id = id;
        return this;
    }
    
    public OrderBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }
    
    public OrderBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }
    
    public OrderBuilder WithLine(string productId, int quantity)
    {
        _lines.Add(new OrderLine { ProductId = productId, Quantity = quantity });
        return this;
    }
    
    public Order Build() => new()
    {
        Id = _id,
        Amount = _amount,
        Status = _status,
        VendorId = _vendorId,
        Lines = _lines
    };
    
    // Presets
    public static Order PendingOrder() => new OrderBuilder()
        .WithStatus(OrderStatus.Pending)
        .Build();
    
    public static Order ApprovedOrder() => new OrderBuilder()
        .WithStatus(OrderStatus.Approved)
        .WithAmount(1000m)
        .Build();
}

// Usage
[Fact]
public void ApproveOrder_WhenPending_ChangesStatus()
{
    var order = new OrderBuilder()
        .WithStatus(OrderStatus.Pending)
        .WithAmount(500m)
        .WithLine("PROD-1", 5)
        .Build();
    
    _sut.Approve(order);
    
    Assert.Equal(OrderStatus.Approved, order.Status);
}
```

## 📚 Références

- [pytest Documentation](https://docs.pytest.org/)
- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [AutoFixture](https://github.com/AutoFixture/AutoFixture)
