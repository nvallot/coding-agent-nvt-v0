# Skill: Code Review

## 🎯 Objectif

Effectuer des revues de code approfondies pour améliorer la qualité, la sécurité et la maintenabilité du code.

## 📋 Checklist de Revue

### 1. Architecture & Design

#### Séparation des Responsabilités (C#)
```csharp
// ❌ MAUVAIS: Classe God Object
public class UserService
{
    public void CreateUser() { }
    public void DeleteUser() { }
    public void SendEmail() { }           // ❌ Pas la responsabilité de UserService
    public void ValidateCreditCard() { }  // ❌ Pas la responsabilité de UserService
    public void GeneratePDF() { }         // ❌ Pas la responsabilité de UserService
}

// ✅ BON: Single Responsibility
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, IEmailService emailService, ILogger<UserService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<User> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var user = await _repository.AddAsync(new User(dto), ct);
        await _emailService.SendWelcomeEmailAsync(user.Email, ct);
        return user;
    }
}
```

#### Séparation des Responsabilités (Python)
```python
# ❌ MAUVAIS: Classe God Object
class UserService:
    def create_user(self): pass
    def delete_user(self): pass
    def send_email(self): pass           # ❌ Pas la responsabilité de UserService
    def validate_credit_card(self): pass  # ❌ Pas la responsabilité de UserService

# ✅ BON: Single Responsibility
class UserService:
    def __init__(self, repository: IUserRepository, email_service: IEmailService):
        self._repository = repository
        self._email_service = email_service

    async def create_user(self, dto: CreateUserDto) -> User:
        user = await self._repository.add(User.from_dto(dto))
        await self._email_service.send_welcome_email(user.email)
        return user
```

#### Dependency Injection (C#)
```csharp
// ❌ MAUVAIS: Couplage fort
public class OrderService
{
    private readonly OrderRepository _repository = new OrderRepository();  // ❌ Instantiation directe

    public async Task<Order?> GetOrderAsync(Guid id)
    {
        return await _repository.FindByIdAsync(id);
    }
}

// ✅ BON: Injection de dépendance
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)  // ✅ Interface injectée
    {
        _repository = repository;
    }

    public async Task<Order?> GetOrderAsync(Guid id, CancellationToken ct = default)
    {
        return await _repository.FindByIdAsync(id, ct);
    }
}
```

### 2. Qualité du Code

#### Nommage (C#)
```csharp
// ❌ MAUVAIS
var d = DateTime.Now;        // ❌ Nom non descriptif
var temp = Calculate();      // ❌ Nom générique
void Proc(int x, int y) { }  // ❌ Noms cryptiques

// ✅ BON
var orderCreatedAt = DateTime.UtcNow;                          // ✅ Descriptif
var totalOrderAmount = CalculateOrderTotal();                   // ✅ Clair
void ProcessPayment(decimal amount, PaymentMethod method) { }   // ✅ Explicite
```

#### Nommage (Python)
```python
# ❌ MAUVAIS
d = datetime.now()           # ❌ Nom non descriptif
temp = calculate()           # ❌ Nom générique
def proc(x, y): pass         # ❌ Noms cryptiques

# ✅ BON
order_created_at = datetime.utcnow()                    # ✅ Descriptif (snake_case)
total_order_amount = calculate_order_total()            # ✅ Clair
def process_payment(amount: Decimal, method: PaymentMethod): pass  # ✅ Explicite
```

#### Complexité Cyclomatique (C#)
```csharp
// ❌ MAUVAIS: Trop complexe (complexité = 12)
public decimal CalculatePrice(Product product, int quantity, Customer customer, Coupon? coupon)
{
    var price = product.BasePrice;
    
    if (quantity > 10)
    {
        if (quantity > 50)
        {
            if (quantity > 100)
                price *= 0.7m;
            else
                price *= 0.8m;
        }
        else
            price *= 0.9m;
    }
    // ... encore plus de conditions imbriquées
    return price;
}

// ✅ BON: Refactoré avec Strategy Pattern (complexité = 3)
public class PriceCalculator
{
    private readonly IEnumerable<IDiscountStrategy> _discountStrategies;

    public PriceCalculator(IEnumerable<IDiscountStrategy> discountStrategies)
    {
        _discountStrategies = discountStrategies;
    }

    public decimal Calculate(PricingContext context)
    {
        var price = context.Product.BasePrice;
        
        foreach (var strategy in _discountStrategies)
        {
            price = strategy.ApplyDiscount(price, context);
        }
        
        return price;
    }
}

public class VolumeDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal price, PricingContext context)
    {
        return context.Quantity switch
        {
            > 100 => price * 0.7m,
            > 50 => price * 0.8m,
            > 10 => price * 0.9m,
            _ => price
        };
    }
}
```

#### Duplication de Code (DRY) - C#
```csharp
// ❌ MAUVAIS: Code dupliqué
public async Task ProcessOrderAsync(Order order)
{
    _logger.LogInformation("Processing order {OrderId}", order.Id);
    _logger.LogInformation("Customer: {CustomerId}", order.CustomerId);
    _logger.LogInformation("Total: {Total}", order.Total);
    // Processing logic
}

public async Task CancelOrderAsync(Order order)
{
    _logger.LogInformation("Processing order {OrderId}", order.Id);  // Dupliqué!
    _logger.LogInformation("Customer: {CustomerId}", order.CustomerId);
    _logger.LogInformation("Total: {Total}", order.Total);
    // Cancellation logic
}

// ✅ BON: Factorisation
private void LogOrderDetails(Order order, string action)
{
    _logger.LogInformation("{Action} order {OrderId}, Customer: {CustomerId}, Total: {Total}",
        action, order.Id, order.CustomerId, order.Total);
}

public async Task ProcessOrderAsync(Order order)
{
    LogOrderDetails(order, "Processing");
    // Processing logic
}

public async Task CancelOrderAsync(Order order)
{
    LogOrderDetails(order, "Cancelling");
    // Cancellation logic
}
```

### 3. Sécurité

#### Injection SQL (C#)
```csharp
// ❌ MAUVAIS: Vulnérable à l'injection SQL
public async Task<User?> GetUserAsync(string username)
{
    var query = $"SELECT * FROM users WHERE username = '{username}'";
    return await _context.Users.FromSqlRaw(query).FirstOrDefaultAsync();  // ❌ DANGEREUX
}

// ✅ BON: Paramètres préparés avec Entity Framework
public async Task<User?> GetUserAsync(string username, CancellationToken ct = default)
{
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Username == username, ct);  // ✅ EF Core paramétrise automatiquement
}

// ✅ BON: Si SQL brut nécessaire
public async Task<User?> GetUserRawAsync(string username, CancellationToken ct = default)
{
    return await _context.Users
        .FromSqlInterpolated($"SELECT * FROM users WHERE username = {username}")
        .FirstOrDefaultAsync(ct);  // ✅ Paramétré
}
```

#### Injection SQL (Python)
```python
# ❌ MAUVAIS: Vulnérable à l'injection SQL
async def get_user(username: str):
    query = f"SELECT * FROM users WHERE username = '{username}'"
    return await db.fetch_one(query)  # ❌ DANGEREUX

# ✅ BON: Paramètres préparés
async def get_user(username: str):
    query = "SELECT * FROM users WHERE username = :username"
    return await db.fetch_one(query, {"username": username})  # ✅ Sécurisé
```
}
```

#### Gestion des Secrets (C#)
```csharp
// ❌ MAUVAIS: Secrets en dur
const string ApiKey = "sk_live_1234567890abcdef";  // ❌ DANGEREUX
const string ConnectionString = "Server=...;Password=P@ssw0rd;";  // ❌ DANGEREUX

// ✅ BON: Configuration + Azure Key Vault
public class OrderService
{
    private readonly SecretClient _secretClient;
    
    public OrderService(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }
    
    public async Task<string> GetApiKeyAsync(CancellationToken ct = default)
    {
        var secret = await _secretClient.GetSecretAsync("api-key", cancellationToken: ct);
        return secret.Value.Value;
    }
}

// ✅ BON: Configuration via IConfiguration (injecté)
public class AppSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}
// Configuré dans appsettings.json ou variables d'environnement, jamais en dur
```

#### Gestion des Secrets (Python)
```python
# ❌ MAUVAIS: Secrets en dur
API_KEY = "sk_live_1234567890abcdef"  # ❌ DANGEREUX

# ✅ BON: Variables d'environnement + Azure Key Vault
import os
from azure.identity import DefaultAzureCredential
from azure.keyvault.secrets import SecretClient

credential = DefaultAzureCredential()
client = SecretClient(vault_url=os.environ["KEY_VAULT_URL"], credential=credential)

api_key = client.get_secret("api-key").value  # ✅ Sécurisé
```

#### Validation des Entrées (C#)
```csharp
// ❌ MAUVAIS: Pas de validation
public async Task<User> CreateUserAsync(object data)
{
    return await _repository.SaveAsync((User)data);  // ❌ Données non validées, cast dangereux
}

// ✅ BON: Validation avec Data Annotations ou FluentValidation
public class CreateUserDto
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(12)]
    [MaxLength(128)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z\s]+$")]
    public string Name { get; set; } = string.Empty;
}

// Dans le controller avec [ApiController], la validation est automatique
[HttpPost]
public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
{
    // ModelState est déjà validé automatiquement
    var user = await _userService.CreateUserAsync(dto);
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

#### Validation des Entrées (Python)
```python
# ❌ MAUVAIS: Pas de validation
async def create_user(data: dict):
    return await repository.save(data)  # ❌ Données non validées

# ✅ BON: Validation avec Pydantic
from pydantic import BaseModel, EmailStr, Field, field_validator
import re

class CreateUserDto(BaseModel):
    email: EmailStr
    password: str = Field(min_length=12, max_length=128)
    name: str = Field(min_length=1, max_length=100)

    @field_validator('name')
    @classmethod
    def validate_name(cls, v: str) -> str:
        if not re.match(r'^[a-zA-Z\s]+$', v):
            raise ValueError('Name must contain only letters and spaces')
        return v

# FastAPI valide automatiquement
@app.post("/users")
async def create_user(dto: CreateUserDto):
    # dto est déjà validé
    user = await user_service.create_user(dto)
    return user
```

### 4. Performance

#### Requêtes N+1 (C#)
```csharp
// ❌ MAUVAIS: Problème N+1
public async Task<List<OrderDto>> GetOrdersWithCustomersAsync()
{
    var orders = await _context.Orders.ToListAsync();
    
    foreach (var order in orders)
    {
        // ❌ Une requête par order (N+1)
        order.Customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == order.CustomerId);
    }
    
    return orders.Select(o => o.ToDto()).ToList();
}

// ✅ BON: Eager Loading avec Include
public async Task<List<OrderDto>> GetOrdersWithCustomersAsync(CancellationToken ct = default)
{
    var orders = await _context.Orders
        .Include(o => o.Customer)  // ✅ Une seule requête avec JOIN
        .AsNoTracking()
        .ToListAsync(ct);
    
    return orders.Select(o => o.ToDto()).ToList();
}
```

#### Requêtes N+1 (Python)
```python
# ❌ MAUVAIS: Problème N+1
async def get_orders_with_customers():
    orders = await Order.all()
    
    for order in orders:
        # ❌ Une requête par order (N+1)
        order.customer = await Customer.get(id=order.customer_id)
    
    return orders

# ✅ BON: Eager Loading avec joinedload (SQLAlchemy)
async def get_orders_with_customers():
    async with async_session() as session:
        result = await session.execute(
            select(Order).options(joinedload(Order.customer))  # ✅ Une seule requête
        )
        return result.scalars().all()
```

// ✅ BON: Join ou eager loading
async function getOrdersWithCustomers() {
  return await db.query(`
    SELECT 
      o.*,
      c.id as customer_id,
      c.name as customer_name,
      c.email as customer_email
    FROM orders o
    LEFT JOIN customers c ON o.customer_id = c.id
  `);  // ✅ Une seule requête
}
```

#### Caching
```typescript
// ❌ MAUVAIS: Pas de cache
async function getPopularProducts() {
  return await db.query(`
    SELECT * FROM products 
    ORDER BY views DESC 
    LIMIT 10
  `);  // ❌ Requête à chaque appel
}

// ✅ BON: Avec cache
async function getPopularProducts() {
  const cacheKey = 'popular-products';
  
  // Check cache first
  const cached = await cache.get(cacheKey);
  if (cached) return JSON.parse(cached);
  
  // Query database
  const products = await db.query(`
    SELECT * FROM products 
    ORDER BY views DESC 
    LIMIT 10
  `);
  
  // Store in cache (5 minutes TTL)
  await cache.setEx(cacheKey, 300, JSON.stringify(products));
  
  return products;
}
```

#### Algorithmes Inefficaces
```typescript
// ❌ MAUVAIS: O(n²)
function findDuplicates(arr: number[]): number[] {
  const duplicates = [];
  for (let i = 0; i < arr.length; i++) {
    for (let j = i + 1; j < arr.length; j++) {
      if (arr[i] === arr[j] && !duplicates.includes(arr[i])) {
        duplicates.push(arr[i]);
      }
    }
  }
  return duplicates;
}

// ✅ BON: O(n)
function findDuplicates(arr: number[]): number[] {
  const seen = new Set<number>();
  const duplicates = new Set<number>();
  
  for (const num of arr) {
    if (seen.has(num)) {
      duplicates.add(num);
    } else {
      seen.add(num);
    }
  }
  
  return Array.from(duplicates);
}
```

### 5. Tests

#### Couverture de Tests
```typescript
// ❌ MAUVAIS: Pas de tests
export function divide(a: number, b: number): number {
  return a / b;  // ❌ Pas de validation, pas de tests
}

// ✅ BON: Tests complets
export function divide(a: number, b: number): number {
  if (b === 0) {
    throw new Error('Division by zero');
  }
  return a / b;
}

// Tests
describe('divide', () => {
  it('should divide two positive numbers', () => {
    expect(divide(10, 2)).toBe(5);
  });
  
  it('should divide two negative numbers', () => {
    expect(divide(-10, -2)).toBe(5);
  });
  
  it('should handle division by negative number', () => {
    expect(divide(10, -2)).toBe(-5);
  });
  
  it('should throw error when dividing by zero', () => {
    expect(() => divide(10, 0)).toThrow('Division by zero');
  });
  
  it('should handle decimal results', () => {
    expect(divide(7, 2)).toBe(3.5);
  });
});
```

#### Qualité des Tests
```typescript
// ❌ MAUVAIS: Tests fragiles
it('should create user', async () => {
  const user = await userService.createUser({
    name: 'John',
    email: 'john@example.com'
  });
  
  expect(user.id).toBe('550e8400-e29b-41d4-a716-446655440000');  // ❌ ID hardcodé
  expect(user.createdAt).toBe(new Date('2024-01-15T12:00:00Z'));  // ❌ Date hardcodée
});

// ✅ BON: Tests robustes
it('should create user with valid data', async () => {
  const userData = {
    name: 'John Doe',
    email: 'john@example.com'
  };
  
  const user = await userService.createUser(userData);
  
  expect(user.id).toMatch(/^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i);  // ✅ UUID v4
  expect(user.name).toBe(userData.name);
  expect(user.email).toBe(userData.email);
  expect(user.createdAt).toBeInstanceOf(Date);
  expect(user.createdAt.getTime()).toBeLessThanOrEqual(Date.now());
  expect(user.createdAt.getTime()).toBeGreaterThan(Date.now() - 1000);  // ✅ Created within last second
});
```

## 📝 Format de Commentaires

### Types de Commentaires

#### 🔴 Blocker (Doit être corrigé)
```markdown
**🔴 Blocker: SQL Injection Vulnerability**

Cette méthode est vulnérable à l'injection SQL. Le paramètre `username` 
est directement interpolé dans la requête.

**Solution**:
\`\`\`typescript
// Au lieu de
const query = `SELECT * FROM users WHERE username = '${username}'`;

// Utiliser
const query = 'SELECT * FROM users WHERE username = $1';
const result = await db.query(query, [username]);
\`\`\`

**Référence**: [OWASP SQL Injection](https://owasp.org/www-community/attacks/SQL_Injection)
```

#### 🟠 Important (Devrait être corrigé)
```markdown
**🟠 Important: Performance Issue**

Cette boucle effectue N+1 requêtes. Pour 1000 orders, cela génère 
1001 requêtes database.

**Impact**: 
- Temps de réponse: ~10s au lieu de ~100ms
- Charge database: 10x plus élevée

**Solution**: Utiliser un JOIN ou eager loading (voir suggestion ci-dessus)
```

#### 🟡 Mineur (Nice to have)
```markdown
**🟡 Mineur: Nommage**

Le nom `temp` n'est pas descriptif. Suggère `totalOrderAmount` 
pour améliorer la lisibilité.
```

#### 💡 Suggestion
```markdown
**💡 Suggestion: Refactoring Opportunity**

Cette logique pourrait être extraite dans une fonction utilitaire réutilisable:

\`\`\`typescript
function calculateDiscountedPrice(basePrice: number, discountPercent: number): number {
  return basePrice * (1 - discountPercent / 100);
}
\`\`\`

Cela améliorerait la testabilité et la réutilisabilité.
```

#### ❓ Question
```markdown
**❓ Question**

Pourquoi utiliser un timeout de 30 secondes ici? 
La plupart de nos APIs ont un timeout de 10 secondes.

Est-ce intentionnel ou devrions-nous standardiser?
```

#### 🎉 Félicitations
```markdown
**🎉 Great Job!**

Excellente utilisation du pattern Repository. 
Le code est propre et bien testé. 👍
```

## 📊 Matrice de Priorité

| Catégorie | Blocker | Important | Mineur |
|-----------|---------|-----------|--------|
| **Sécurité** | SQL Injection, XSS, Secrets exposés | Validation manquante, Logs sensibles | Headers manquants |
| **Bugs** | Crash application, Perte de données | Comportement incorrect, Edge cases | Typos, Formatting |
| **Performance** | Requêtes N+1 majeures, Memory leaks | Cache manquant, Algorithmes O(n²) | Optimisations mineures |
| **Architecture** | Couplage fort critique | SRP violations, God objects | Nommage, Comments |
| **Tests** | Aucun test sur logique critique | Coverage < 70% | Tests manquants sur utils |

## ✅ Checklist de Validation

Avant d'approuver un PR:

- [ ] **Architecture**: Respect des patterns et principes SOLID
- [ ] **Qualité**: Code lisible, noms explicites, pas de duplication
- [ ] **Sécurité**: Pas de vulnérabilités OWASP Top 10
- [ ] **Performance**: Pas de problèmes évidents (N+1, memory leaks)
- [ ] **Tests**: Coverage adéquate (> 80% pour nouvelle logique)
- [ ] **Documentation**: Commentaires pour logique complexe
- [ ] **Standards**: Respect des conventions d'équipe
- [ ] **Breaking Changes**: Documentés et communiqués
- [ ] **Migration**: Scripts de migration si changements DB
- [ ] **Rollback**: Plan de rollback documenté si changement risqué
