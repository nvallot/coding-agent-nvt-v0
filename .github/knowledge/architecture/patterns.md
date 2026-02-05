# Architecture Patterns

## 🏗️ Patterns Fondamentaux

### 1. Layered Architecture (Architecture en Couches)

**Description**: Organisation du code en couches horizontales avec des responsabilités distinctes.

**Structure**:
```
┌─────────────────────────────┐
│   Presentation Layer        │  ← UI, API Controllers
├─────────────────────────────┤
│   Application Layer         │  ← Use Cases, Orchestration
├─────────────────────────────┤
│   Domain Layer              │  ← Business Logic, Entities
├─────────────────────────────┤
│   Infrastructure Layer      │  ← Database, External Services
└─────────────────────────────┘
```

**Cas d'usage**:
- Applications CRUD traditionnelles
- Monolithes bien structurés
- Projets avec équipes débutantes

**Avantages**:
- Séparation claire des responsabilités
- Facile à comprendre
- Testabilité

**Inconvénients**:
- Peut devenir rigide
- Couplage entre couches
- Difficulté à scaler

**Exemple**:
```typescript
// Domain Layer
export class Order {
  constructor(
    public readonly id: string,
    public items: OrderItem[],
    public status: OrderStatus
  ) {}
  
  addItem(item: OrderItem): void {
    if (this.status !== OrderStatus.Draft) {
      throw new Error('Cannot add items to non-draft order');
    }
    this.items.push(item);
  }
  
  calculateTotal(): number {
    return this.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
  }
}

// Application Layer
export class OrderService {
  constructor(
    private readonly repository: IOrderRepository,
    private readonly eventBus: IEventBus
  ) {}
  
  async createOrder(dto: CreateOrderDto): Promise<Order> {
    const order = new Order(uuidv4(), dto.items, OrderStatus.Draft);
    await this.repository.save(order);
    await this.eventBus.publish(new OrderCreatedEvent(order.id));
    return order;
  }
}

// Infrastructure Layer
export class OrderRepository implements IOrderRepository {
  constructor(private readonly db: Database) {}
  
  async save(order: Order): Promise<void> {
    await this.db.query(
      'INSERT INTO orders (id, items, status) VALUES ($1, $2, $3)',
      [order.id, JSON.stringify(order.items), order.status]
    );
  }
}

// Presentation Layer
@Controller('/orders')
export class OrderController {
  constructor(private readonly service: OrderService) {}
  
  @Post()
  async create(@Body() dto: CreateOrderDto): Promise<OrderResponse> {
    const order = await this.service.createOrder(dto);
    return OrderMapper.toResponse(order);
  }
}
```

### 2. Microservices Architecture

**Description**: Application décomposée en services indépendants, déployables séparément.

**Caractéristiques**:
- Services autonomes
- Communication via API (REST, gRPC, messaging)
- Base de données par service
- Déploiement indépendant

**Pattern de Communication**:

```
┌──────────────┐      HTTP/REST     ┌──────────────┐
│   API        │◄──────────────────►│   User       │
│   Gateway    │                    │   Service    │
└──────────────┘                    └──────────────┘
       │                                    │
       │ Events                             │ Events
       ▼                                    ▼
┌──────────────┐                    ┌──────────────┐
│   Message    │                    │   Database   │
│   Bus        │                    │   (Users)    │
└──────────────┘                    └──────────────┘
       │
       │ Events
       ▼
┌──────────────┐      gRPC          ┌──────────────┐
│   Order      │◄──────────────────►│   Inventory  │
│   Service    │                    │   Service    │
└──────────────┘                    └──────────────┘
       │                                    │
       ▼                                    ▼
┌──────────────┐                    ┌──────────────┐
│   Database   │                    │   Database   │
│   (Orders)   │                    │   (Inventory)│
└──────────────┘                    └──────────────┘
```

**Avantages**:
- Scalabilité indépendante
- Technologie par service
- Isolation des pannes
- Déploiements indépendants

**Inconvénients**:
- Complexité opérationnelle
- Transactions distribuées
- Debugging difficile
- Overhead réseau

**Patterns Associés**:

#### API Gateway
```typescript
// API Gateway avec routing
@Controller()
export class ApiGateway {
  constructor(
    private readonly userService: UserServiceClient,
    private readonly orderService: OrderServiceClient
  ) {}
  
  @Get('/users/:id')
  async getUser(@Param('id') id: string) {
    return await this.userService.getUserById(id);
  }
  
  @Get('/orders/:id')
  async getOrder(@Param('id') id: string) {
    return await this.orderService.getOrderById(id);
  }
  
  // Aggregation pattern
  @Get('/users/:id/profile')
  async getUserProfile(@Param('id') userId: string) {
    const [user, orders, preferences] = await Promise.all([
      this.userService.getUserById(userId),
      this.orderService.getOrdersByUserId(userId),
      this.preferencesService.getPreferences(userId)
    ]);
    
    return { user, orders, preferences };
  }
}
```

#### Circuit Breaker
```typescript
import { CircuitBreaker } from 'opossum';

const options = {
  timeout: 3000,
  errorThresholdPercentage: 50,
  resetTimeout: 30000
};

const breaker = new CircuitBreaker(async (orderId: string) => {
  return await fetch(`${inventoryServiceUrl}/check/${orderId}`);
}, options);

breaker.on('open', () => {
  console.log('Circuit breaker opened - using fallback');
});

// Usage
try {
  const result = await breaker.fire(orderId);
} catch (error) {
  // Fallback logic
  return getCachedInventory(orderId);
}
```

#### Saga Pattern
```typescript
// Choreography-based Saga
export class OrderSaga {
  constructor(
    private readonly eventBus: IEventBus,
    private readonly compensations: Map<string, () => Promise<void>>
  ) {}
  
  async createOrder(order: Order): Promise<void> {
    try {
      // Step 1: Reserve inventory
      await this.eventBus.publish(new ReserveInventoryCommand(order));
      this.compensations.set('inventory', () => this.releaseInventory(order));
      
      // Step 2: Process payment
      await this.eventBus.publish(new ProcessPaymentCommand(order));
      this.compensations.set('payment', () => this.refundPayment(order));
      
      // Step 3: Confirm order
      await this.eventBus.publish(new ConfirmOrderCommand(order));
      
    } catch (error) {
      // Compensate in reverse order
      for (const [key, compensate] of Array.from(this.compensations.entries()).reverse()) {
        await compensate();
      }
      throw error;
    }
  }
}
```

### 3. Event-Driven Architecture

**Description**: Communication asynchrone via événements.

**Patterns**:

#### Event Sourcing
```typescript
// Event Store
export class OrderEventStore {
  async saveEvent(event: DomainEvent): Promise<void> {
    await this.db.events.insert({
      aggregateId: event.aggregateId,
      type: event.type,
      data: event.data,
      timestamp: event.timestamp,
      version: event.version
    });
  }
  
  async getEvents(aggregateId: string): Promise<DomainEvent[]> {
    const rows = await this.db.events.find({ aggregateId })
      .sort({ version: 1 });
    
    return rows.map(row => this.deserialize(row));
  }
}

// Aggregate reconstruction
export class Order {
  private events: DomainEvent[] = [];
  
  static fromHistory(events: DomainEvent[]): Order {
    const order = new Order();
    events.forEach(event => order.apply(event));
    return order;
  }
  
  private apply(event: DomainEvent): void {
    switch (event.type) {
      case 'OrderCreated':
        this.id = event.data.orderId;
        this.status = OrderStatus.Created;
        break;
      case 'ItemAdded':
        this.items.push(event.data.item);
        break;
      case 'OrderConfirmed':
        this.status = OrderStatus.Confirmed;
        break;
    }
  }
  
  addItem(item: OrderItem): void {
    const event = new ItemAddedEvent(this.id, item);
    this.apply(event);
    this.events.push(event);
  }
}
```

#### CQRS (Command Query Responsibility Segregation)
```typescript
// Write Model (Commands)
export class CreateOrderCommandHandler {
  constructor(
    private readonly repository: IOrderRepository,
    private readonly eventBus: IEventBus
  ) {}
  
  async handle(command: CreateOrderCommand): Promise<void> {
    const order = Order.create(command.items);
    await this.repository.save(order);
    
    // Publish events for read model
    await this.eventBus.publish(new OrderCreatedEvent(order));
  }
}

// Read Model (Queries)
export class OrderQueryService {
  constructor(private readonly readDb: IReadDatabase) {}
  
  async getOrderById(orderId: string): Promise<OrderView> {
    // Optimized read model
    return await this.readDb.orders.findOne({ id: orderId });
  }
  
  async getOrdersByCustomer(customerId: string): Promise<OrderView[]> {
    // Denormalized for fast reads
    return await this.readDb.ordersByCustomer.find({ customerId });
  }
}

// Projection (Event Handler)
export class OrderProjection {
  constructor(private readonly readDb: IReadDatabase) {}
  
  @EventHandler(OrderCreatedEvent)
  async onOrderCreated(event: OrderCreatedEvent): Promise<void> {
    await this.readDb.orders.insert({
      id: event.orderId,
      customerId: event.customerId,
      items: event.items,
      total: event.total,
      status: 'created',
      createdAt: event.timestamp
    });
    
    // Update denormalized view
    await this.readDb.ordersByCustomer.insert({
      customerId: event.customerId,
      orderId: event.orderId,
      total: event.total,
      createdAt: event.timestamp
    });
  }
}
```

### 4. Hexagonal Architecture (Ports and Adapters)

**Description**: Isoler la logique métier des détails techniques.

```
         ┌─────────────────────────────────┐
         │      External Systems           │
         │  (UI, APIs, Databases, etc.)   │
         └─────────────────────────────────┘
                       │
              ┌────────▼────────┐
              │    Adapters     │  ← Infrastructure
              │   (Secondary)   │
              └────────┬────────┘
                       │
              ┌────────▼────────┐
              │      Ports      │  ← Interfaces
              └────────┬────────┘
                       │
         ┌─────────────▼─────────────┐
         │     Domain / Core         │  ← Business Logic
         │  (Technology Agnostic)    │
         └─────────────┬─────────────┘
                       │
              ┌────────▼────────┐
              │      Ports      │  ← Interfaces
              └────────┬────────┘
                       │
              ┌────────▼────────┐
              │    Adapters     │  ← Infrastructure
              │    (Primary)    │
              └────────┬────────┘
                       │
         ┌─────────────▼─────────────┐
         │      Driving Systems      │
         │  (Controllers, Tests)     │
         └───────────────────────────┘
```

**Exemple**:
```typescript
// Core Domain (Port)
export interface IOrderRepository {
  save(order: Order): Promise<void>;
  findById(id: string): Promise<Order | null>;
}

// Core Domain (Business Logic)
export class OrderService {
  constructor(private readonly repository: IOrderRepository) {}
  
  async placeOrder(items: OrderItem[]): Promise<Order> {
    const order = Order.create(items);
    await this.repository.save(order);
    return order;
  }
}

// Infrastructure (Adapter - PostgreSQL)
export class PostgresOrderRepository implements IOrderRepository {
  constructor(private readonly pool: Pool) {}
  
  async save(order: Order): Promise<void> {
    await this.pool.query(
      'INSERT INTO orders (id, items, status) VALUES ($1, $2, $3)',
      [order.id, JSON.stringify(order.items), order.status]
    );
  }
  
  async findById(id: string): Promise<Order | null> {
    const result = await this.pool.query('SELECT * FROM orders WHERE id = $1', [id]);
    return result.rows[0] ? this.mapToOrder(result.rows[0]) : null;
  }
}

// Infrastructure (Adapter - MongoDB)
export class MongoOrderRepository implements IOrderRepository {
  constructor(private readonly db: Db) {}
  
  async save(order: Order): Promise<void> {
    await this.db.collection('orders').insertOne({
      _id: order.id,
      items: order.items,
      status: order.status
    });
  }
  
  async findById(id: string): Promise<Order | null> {
    const doc = await this.db.collection('orders').findOne({ _id: id });
    return doc ? this.mapToOrder(doc) : null;
  }
}

// Application (Dependency Injection)
const repository = new PostgresOrderRepository(pool);
// or: const repository = new MongoOrderRepository(db);
const orderService = new OrderService(repository);
```

## 🎯 Patterns Cloud-Native

### 1. Strangler Fig Pattern

**Description**: Migrer progressivement un monolithe vers des microservices.

```
Phase 1: Monolithe          Phase 2: Hybrid             Phase 3: Microservices
┌─────────────┐            ┌─────────────┐             ┌─────────────┐
│             │            │   Facade/   │             │   Gateway   │
│  Monolith   │            │   Gateway   │             └──────┬──────┘
│             │            └──────┬──────┘                    │
└─────────────┘                   │                           │
                         ┌────────┼────────┐           ┌──────┴──────┐
                         │                 │           │             │
                    ┌────▼────┐      ┌────▼────┐ ┌───▼───┐    ┌───▼───┐
                    │Monolith │      │ Service │ │Service│    │Service│
                    │(reduced)│      │    A    │ │   B   │    │   C   │
                    └─────────┘      └─────────┘ └───────┘    └───────┘
```

**Implémentation**:
```typescript
// Gateway routing
@Controller()
export class StranglerFacade {
  constructor(
    private readonly legacyService: LegacyMonolithClient,
    private readonly newOrderService: OrderServiceClient
  ) {}
  
  @Get('/orders/:id')
  async getOrder(@Param('id') id: string) {
    // Route new orders to new service
    if (await this.isNewOrder(id)) {
      return await this.newOrderService.getOrder(id);
    }
    
    // Route legacy orders to monolith
    return await this.legacyService.getOrder(id);
  }
  
  private async isNewOrder(id: string): Promise<boolean> {
    // Logic to determine if order is in new system
    return id.startsWith('ORD-2024');
  }
}
```

### 2. Sidecar Pattern

**Description**: Déployer des fonctionnalités auxiliaires dans un conteneur séparé.

```yaml
# Kubernetes Deployment with Sidecar
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp
spec:
  template:
    spec:
      containers:
        # Main application
        - name: app
          image: myapp:latest
          ports:
            - containerPort: 8080
        
        # Logging sidecar
        - name: log-collector
          image: fluentd:latest
          volumeMounts:
            - name: logs
              mountPath: /var/log/app
        
        # Proxy sidecar (Envoy)
        - name: envoy-proxy
          image: envoyproxy/envoy:latest
          ports:
            - containerPort: 9901
```

### 3. Ambassador Pattern

**Description**: Proxy client-side pour gérer la connectivité réseau.

```typescript
// Ambassador pour retry et circuit breaker
export class ServiceAmbassador {
  private readonly circuitBreaker: CircuitBreaker;
  
  constructor(
    private readonly targetUrl: string,
    private readonly retryPolicy: RetryPolicy
  ) {
    this.circuitBreaker = new CircuitBreaker(
      async (request) => this.makeRequest(request),
      { timeout: 3000, errorThresholdPercentage: 50 }
    );
  }
  
  async call<T>(endpoint: string, options?: RequestOptions): Promise<T> {
    return await this.circuitBreaker.fire({ endpoint, options });
  }
  
  private async makeRequest({ endpoint, options }): Promise<any> {
    return await retry(
      async () => {
        const response = await fetch(`${this.targetUrl}${endpoint}`, options);
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        return await response.json();
      },
      this.retryPolicy
    );
  }
}

// Usage
const ambassador = new ServiceAmbassador(
  'https://api.external-service.com',
  { maxRetries: 3, backoff: 'exponential' }
);

const data = await ambassador.call('/users/123');
```

## 📋 Choosing the Right Pattern

| Contexte | Pattern Recommandé | Alternative |
|----------|-------------------|-------------|
| Application simple CRUD | Layered Architecture | Clean Architecture |
| Scaling indépendant nécessaire | Microservices | Modular Monolith |
| Audit et temporal queries | Event Sourcing | Change Data Capture |
| Reads >> Writes | CQRS | Read Replicas |
| Migration progressive | Strangler Fig | Big Bang Rewrite |
| Infrastructure agnostic | Hexagonal | Layered |
| Asynchronous processing | Event-Driven | Message Queue |
| Service mesh | Sidecar | API Gateway |
