# Skill: Code Review

## 🎯 Objectif

Effectuer des revues de code approfondies pour améliorer la qualité, la sécurité et la maintenabilité du code.

## 📋 Checklist de Revue

### 1. Architecture & Design

#### Séparation des Responsabilités
```typescript
// ❌ MAUVAIS: Classe God Object
class UserService {
  createUser() { }
  deleteUser() { }
  sendEmail() { }  // ❌ Pas la responsabilité de UserService
  validateCreditCard() { }  // ❌ Pas la responsabilité de UserService
  generatePDF() { }  // ❌ Pas la responsabilité de UserService
}

// ✅ BON: Single Responsibility
class UserService {
  constructor(
    private emailService: EmailService,
    private notificationService: NotificationService
  ) {}
  
  async createUser(data: CreateUserDto): Promise<User> {
    const user = await this.repository.save(new User(data));
    await this.emailService.sendWelcomeEmail(user.email);
    return user;
  }
}
```

#### Dependency Injection
```typescript
// ❌ MAUVAIS: Couplage fort
class OrderService {
  private repository = new OrderRepository();  // ❌ Instantiation directe
  
  async getOrder(id: string) {
    return this.repository.findById(id);
  }
}

// ✅ BON: Injection de dépendance
class OrderService {
  constructor(
    private readonly repository: IOrderRepository  // ✅ Interface injectée
  ) {}
  
  async getOrder(id: string): Promise<Order | null> {
    return this.repository.findById(id);
  }
}
```

#### Abstraction
```typescript
// ❌ MAUVAIS: Détails d'implémentation exposés
class UserService {
  async getUser(id: string) {
    const result = await pool.query('SELECT * FROM users WHERE id = $1', [id]);
    return result.rows[0];  // ❌ Exposition de la structure DB
  }
}

// ✅ BON: Abstraction via Repository
class UserService {
  constructor(private repository: IUserRepository) {}
  
  async getUser(id: string): Promise<User | null> {
    return this.repository.findById(id);  // ✅ Abstraction
  }
}
```

### 2. Qualité du Code

#### Nommage
```typescript
// ❌ MAUVAIS
const d = new Date();  // ❌ Nom non descriptif
const temp = calculate();  // ❌ Nom générique
function proc(x, y) { }  // ❌ Noms cryptiques

// ✅ BON
const orderCreatedAt = new Date();  // ✅ Descriptif
const totalOrderAmount = calculateOrderTotal();  // ✅ Clair
function processPayment(amount: Money, paymentMethod: PaymentMethod) { }  // ✅ Explicite
```

#### Complexité Cyclomatique
```typescript
// ❌ MAUVAIS: Trop complexe (complexité = 12)
function calculatePrice(product, quantity, customer, coupon, season, dayOfWeek) {
  let price = product.basePrice;
  
  if (quantity > 10) {
    if (quantity > 50) {
      if (quantity > 100) {
        price *= 0.7;
      } else {
        price *= 0.8;
      }
    } else {
      price *= 0.9;
    }
  }
  
  if (customer.isPremium) {
    if (customer.yearsActive > 5) {
      price *= 0.85;
    }
  }
  
  if (coupon && coupon.isValid) {
    if (coupon.type === 'PERCENTAGE') {
      price *= (1 - coupon.value);
    } else if (coupon.type === 'FIXED') {
      price -= coupon.value;
    }
  }
  
  if (season === 'WINTER' || season === 'SUMMER') {
    price *= 0.95;
  }
  
  if (dayOfWeek === 'MONDAY' || dayOfWeek === 'TUESDAY') {
    price *= 0.98;
  }
  
  return price;
}

// ✅ BON: Refactoré (complexité = 3)
class PriceCalculator {
  calculate(context: PricingContext): Money {
    let price = context.product.basePrice;
    
    price = this.applyVolumeDiscount(price, context.quantity);
    price = this.applyCustomerDiscount(price, context.customer);
    price = this.applyCouponDiscount(price, context.coupon);
    price = this.applySeasonalDiscount(price, context.season);
    price = this.applyDailyDiscount(price, context.dayOfWeek);
    
    return price;
  }
  
  private applyVolumeDiscount(price: Money, quantity: number): Money {
    const discounts = [
      { threshold: 100, rate: 0.7 },
      { threshold: 50, rate: 0.8 },
      { threshold: 10, rate: 0.9 }
    ];
    
    const discount = discounts.find(d => quantity > d.threshold);
    return discount ? price.multiply(discount.rate) : price;
  }
  
  // ... autres méthodes privées
}
```

#### Duplication de Code (DRY)
```typescript
// ❌ MAUVAIS: Code dupliqué
function processOrder(order: Order) {
  console.log(`Processing order ${order.id}`);
  console.log(`Customer: ${order.customerId}`);
  console.log(`Total: ${order.total}`);
  // Processing logic
}

function cancelOrder(order: Order) {
  console.log(`Processing order ${order.id}`);
  console.log(`Customer: ${order.customerId}`);
  console.log(`Total: ${order.total}`);
  // Cancellation logic
}

// ✅ BON: Factorisation
function logOrderDetails(order: Order, action: string) {
  console.log(`${action} order ${order.id}`);
  console.log(`Customer: ${order.customerId}`);
  console.log(`Total: ${order.total}`);
}

function processOrder(order: Order) {
  logOrderDetails(order, 'Processing');
  // Processing logic
}

function cancelOrder(order: Order) {
  logOrderDetails(order, 'Cancelling');
  // Cancellation logic
}
```

### 3. Sécurité

#### Injection SQL
```typescript
// ❌ MAUVAIS: Vulnérable à l'injection SQL
async function getUser(username: string) {
  const query = `SELECT * FROM users WHERE username = '${username}'`;
  return await db.query(query);  // ❌ DANGEREUX
}

// ✅ BON: Paramètres préparés
async function getUser(username: string) {
  const query = 'SELECT * FROM users WHERE username = $1';
  return await db.query(query, [username]);  // ✅ Sécurisé
}
```

#### XSS (Cross-Site Scripting)
```typescript
// ❌ MAUVAIS: Vulnérable XSS
function displayComment(comment: string) {
  document.getElementById('comments').innerHTML = comment;  // ❌ DANGEREUX
}

// ✅ BON: Échappement
function displayComment(comment: string) {
  const escaped = he.escape(comment);  // ✅ Échappement
  document.getElementById('comments').textContent = escaped;  // ✅ textContent au lieu de innerHTML
}
```

#### Gestion des Secrets
```typescript
// ❌ MAUVAIS: Secrets en dur
const API_KEY = "sk_live_1234567890abcdef";  // ❌ DANGEREUX
const connectionString = "Server=...;Password=P@ssw0rd;";  // ❌ DANGEREUX

// ✅ BON: Variables d'environnement
const API_KEY = process.env.API_KEY;
if (!API_KEY) {
  throw new Error('API_KEY environment variable is required');
}

// ✅ MIEUX: Key Vault
const secretClient = new SecretClient(vaultUrl, credential);
const secret = await secretClient.getSecret('api-key');
const API_KEY = secret.value;
```

#### Validation des Entrées
```typescript
// ❌ MAUVAIS: Pas de validation
function createUser(data: any) {
  return repository.save(data);  // ❌ Données non validées
}

// ✅ BON: Validation stricte
import { z } from 'zod';

const CreateUserSchema = z.object({
  email: z.string().email().max(255),
  password: z.string().min(12).max(128),
  name: z.string().min(1).max(100).regex(/^[a-zA-Z\s]+$/),
  age: z.number().int().min(18).max(120).optional()
});

function createUser(data: unknown) {
  const validated = CreateUserSchema.parse(data);  // ✅ Validation + TypeScript
  const hashedPassword = await bcrypt.hash(validated.password, 10);
  
  return repository.save({
    ...validated,
    password: hashedPassword
  });
}
```

### 4. Performance

#### Requêtes N+1
```typescript
// ❌ MAUVAIS: Problème N+1
async function getOrdersWithCustomers() {
  const orders = await db.query('SELECT * FROM orders');
  
  for (const order of orders) {
    // ❌ Une requête par order (N+1)
    order.customer = await db.query(
      'SELECT * FROM customers WHERE id = $1', 
      [order.customer_id]
    );
  }
  
  return orders;
}

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
