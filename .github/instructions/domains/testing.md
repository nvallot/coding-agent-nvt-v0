---
applyTo: "**/tests/**,**/*test*,**/*spec*"
excludeAgent: "code-review"
---

# 🧪 Testing Standards

## Test Pyramid
```
         /\
        /  \   E2E (few, slow)
       /    \
      /------\
     /   IT   \  Integration (moderate)
    /          \
   /----------  \
  /    Unit      \ Unit tests (many, fast)
 /________________\
```

**Target Coverage**: 80% overall, 95% critical paths

## Unit Tests

**Framework**: pytest (Python), xUnit (C#)

**Structure**:
```python
def test_transform_orders_returns_valid_schema():
    # Arrange
    input_df = spark.createDataFrame(
        [("1", "2024-01-01", 100)],
        ["order_id", "order_date", "amount"]
    )
    
    # Act
    result = transform_orders(input_df)
    
    # Assert
    assert result.count() == 1
    assert "order_id" in result.columns
    assert result.filter(F.col("amount") < 0).count() == 0
```

**Naming**: `test_<function>_<scenario>_<expected>`

**Principles**:
- One assertion per test (or related group)
- No external dependencies (mock if needed)
- Deterministic (no randomness)
- Fast (<100ms each)

## Integration Tests

**Scope**: Multiple components working together

**Example** (Databricks notebook):
```python
def test_bronze_to_silver_pipeline():
    # Setup: Write test data to Bronze
    spark.sql("""
        CREATE OR REPLACE TABLE bronze_orders AS
        SELECT '1' as order_id, '2024-01-01' as order_date, 100 as amount
    """)
    
    # Execute: Run transformation notebook
    dbutils.notebook.run("./transform_orders", timeout_seconds=300)
    
    # Verify: Check Silver has expected data
    silver_df = spark.sql("SELECT * FROM silver_orders")
    assert silver_df.count() == 1
    assert silver_df.filter("order_date < '2024-01-01'").count() == 0
```

**Coverage**: Critical workflows, error scenarios

## Data Quality Tests

```python
def test_orders_no_duplicates():
    df = spark.sql("SELECT * FROM silver_orders")
    dedup_count = df.select("order_id").distinct().count()
    assert dedup_count == df.count(), "Duplicates found"

def test_orders_dates_valid():
    df = spark.sql("SELECT * FROM silver_orders")
    invalid = df.filter("order_date > CURRENT_DATE() OR order_date < '2020-01-01'")
    assert invalid.count() == 0, "Invalid dates found"

def test_orders_amount_positive():
    df = spark.sql("SELECT * FROM silver_orders")
    negative = df.filter("amount < 0")
    assert negative.count() == 0, "Negative amounts found"
```

## E2E Tests

**Scope**: Full workflow from source to visualization

**When**: Before production releases

**Tools**: Selenium (UI), API clients, SQL queries

## Continuous Testing

**Pre-commit**: Unit tests (fast)
```bash
pytest tests/unit/ -v --cov=src --cov-fail-under=80
```

**Pre-push**: Integration tests
```bash
pytest tests/integration/ -v
```

**CI/CD Pipeline**: All tests + security scanning
```yaml
- name: Run tests
  run: pytest tests/ -v --junit-xml=results.xml
  
- name: Check coverage
  run: coverage report --fail-under=80
```

## Mocking & Fixtures

**Python**:
```python
@pytest.fixture
def mock_api():
    with mock.patch('requests.get') as m:
        m.return_value.json.return_value = {"id": 1}
        yield m

def test_api_call(mock_api):
    result = fetch_data()
    mock_api.assert_called_once()
    assert result == {"id": 1}
```

**C#**:
```csharp
var mockService = new Mock<IDataService>();
mockService.Setup(s => s.GetOrders())
    .Returns(Task.FromResult(new[] { order1 }));

var result = await service.ProcessOrders(mockService.Object);
Assert.Single(result);
```

## Test Data Management

**Scenarios to cover**:
- Happy path (normal data)
- Null values
- Empty datasets
- Duplicates
- Out-of-order data
- Invalid types
- Boundary values (max/min)
- Performance (large volumes)

**Fixtures** (reusable):
```python
@pytest.fixture
def sample_orders():
    return spark.createDataFrame([
        ("1", "2024-01-01", 100.00),
        ("2", "2024-01-02", 200.00),
    ], ["order_id", "order_date", "amount"])
```
