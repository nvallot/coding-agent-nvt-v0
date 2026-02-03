# Skill: Solution Design

## 🎯 Objectif

Concevoir des solutions techniques complètes et robustes qui répondent aux besoins métier tout en respectant les contraintes techniques, budgétaires et temporelles.

## 📋 Méthodologie

### Phase 1: Discovery (Découverte)

#### 1.1 Comprendre le Besoin Métier
**Questions clés**:
- Quel problème cherche-t-on à résoudre?
- Qui sont les utilisateurs finaux?
- Quels sont les objectifs mesurables?
- Quelles sont les contraintes réglementaires?

**Livrables**:
- User Stories
- Cas d'usage
- Critères d'acceptation
- KPIs métier

#### 1.2 Analyse des Contraintes
**Contraintes Techniques**:
- Stack technologique existante
- Compétences de l'équipe
- Infrastructure disponible
- Intégrations nécessaires

**Contraintes Non-Techniques**:
- Budget
- Timeline
- Ressources humaines
- Conformité (RGPD, SOC2, etc.)

#### 1.3 Définir les Exigences
**Exigences Fonctionnelles**:
```markdown
## User Authentication
- [ ] FR-001: Users can sign up with email/password
- [ ] FR-002: Users can sign in with social providers (Google, Microsoft)
- [ ] FR-003: Users can reset password via email
- [ ] FR-004: Session expires after 24h of inactivity
```

**Exigences Non-Fonctionnelles**:
```markdown
## Performance
- [ ] NFR-001: API response time < 200ms (p95)
- [ ] NFR-002: Support 10,000 concurrent users
- [ ] NFR-003: Page load time < 2 seconds

## Security
- [ ] NFR-004: All data encrypted at rest and in transit
- [ ] NFR-005: OWASP Top 10 vulnerabilities addressed
- [ ] NFR-006: Audit logs for all sensitive operations

## Availability
- [ ] NFR-007: 99.9% uptime SLA
- [ ] NFR-008: RTO < 1 hour
- [ ] NFR-009: RPO < 5 minutes

## Scalability
- [ ] NFR-010: Horizontal scaling capability
- [ ] NFR-011: Support 10x traffic spike
```

### Phase 2: Architecture Design

#### 2.1 Choisir le Pattern d'Architecture
**Critères de décision**:
- Complexité du domaine
- Besoins de scalabilité
- Maturité de l'équipe
- Budget et timeline

**Options**:
1. **Monolith Modulaire**: Projets simples, équipe petite
2. **Microservices**: Scalabilité indépendante nécessaire
3. **Serverless**: Charges variables, réduction OpEx
4. **Event-Driven**: Asynchronisme, découplage

#### 2.2 Conception de Haut Niveau

**Template de Solution Design**:
```markdown
# Solution Design: [Project Name]

## 1. Executive Summary
Brief overview of the solution (2-3 paragraphs).

## 2. Business Context
- Problem statement
- Success criteria
- Stakeholders

## 3. Technical Architecture

### 3.1 High-Level Architecture
[Insert C4 Context Diagram]

### 3.2 Components
| Component | Technology | Purpose | Owner |
|-----------|------------|---------|-------|
| Web Frontend | React | User interface | Frontend Team |
| API Gateway | APIM | Request routing | Platform Team |
| Auth Service | Azure AD B2C | Authentication | Security Team |

### 3.3 Data Model
[Insert Entity-Relationship Diagram]

### 3.4 Integration Points
[Insert Integration Diagram]

## 4. Technology Stack

### Frontend
- Framework: React 18
- State Management: Redux Toolkit
- UI Library: Material-UI
- Build Tool: Vite

### Backend
- Runtime: Node.js 20
- Framework: NestJS
- ORM: Prisma
- API Style: REST + GraphQL

### Infrastructure
- Cloud: Azure
- Compute: Container Apps
- Database: PostgreSQL (Flexible Server)
- Cache: Redis
- Messaging: Service Bus

## 5. Security Architecture

### Authentication & Authorization
- Azure AD B2C for user authentication
- JWT tokens with 1h expiration
- Role-Based Access Control (RBAC)

### Data Protection
- TLS 1.3 for data in transit
- AES-256 encryption at rest
- Secrets stored in Key Vault

### Network Security
- Private endpoints for databases
- Application Gateway with WAF
- DDoS protection enabled

## 6. Deployment Strategy

### Environments
- Development: Auto-deploy from `develop` branch
- Staging: Auto-deploy from `main` branch
- Production: Manual approval required

### CI/CD Pipeline
[Insert Pipeline Diagram]

### Rollback Strategy
- Blue-green deployment for zero downtime
- Automated rollback on health check failures
- Database migrations handled separately

## 7. Monitoring & Observability

### Metrics
- Application Insights for APM
- Custom metrics for business KPIs
- Infrastructure metrics via Azure Monitor

### Logging
- Structured logging (JSON format)
- Centralized in Log Analytics
- 90-day retention

### Alerting
- P0: Critical (page on-call)
- P1: High (notify during business hours)
- P2: Medium (ticket created)

## 8. Disaster Recovery

### Backup Strategy
- Database: Automated daily backups (35-day retention)
- Configuration: Git repository
- Secrets: Key Vault with soft delete

### Recovery Procedures
- RTO: 1 hour
- RPO: 5 minutes
- Documented runbooks

## 9. Cost Estimation

| Component | Monthly Cost | Notes |
|-----------|-------------|-------|
| Container Apps | $150 | 2 replicas, 0.5 vCPU |
| PostgreSQL | $200 | Flexible Server, Standard D2s v3 |
| Redis | $50 | Basic C1 |
| Service Bus | $10 | Standard tier |
| Application Insights | $100 | 10GB ingestion |
| **Total** | **$510** | |

### Cost Optimization
- Enable auto-scaling to reduce idle costs
- Use reserved capacity for databases (-40%)
- Implement caching to reduce database load

## 10. Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Third-party API downtime | High | Medium | Implement circuit breaker, cache responses |
| Database performance degradation | High | Low | Query optimization, read replicas |
| Team lacks Azure experience | Medium | High | Training sessions, pair programming |

## 11. Timeline

- Week 1-2: Infrastructure setup
- Week 3-4: Backend services implementation
- Week 5-6: Frontend development
- Week 7: Integration testing
- Week 8: UAT and production deployment

## 12. Success Metrics

- System uptime > 99.9%
- API response time < 200ms (p95)
- Zero critical security vulnerabilities
- User satisfaction score > 4.5/5
```

#### 2.3 Décisions d'Architecture (ADR)

**Template ADR**:
```markdown
# ADR-001: Use PostgreSQL for Relational Data

## Status
Accepted

## Context
We need to choose a database for storing transactional data (orders, users, products).

## Decision Drivers
- ACID compliance required
- Complex queries needed
- Relational data model fits well
- Team has SQL expertise
- Budget constraints

## Considered Options
1. PostgreSQL (Flexible Server)
2. Azure SQL Database
3. Cosmos DB

## Decision
We will use PostgreSQL (Azure Database for PostgreSQL - Flexible Server).

## Consequences

### Positive
- Open-source, no licensing costs
- Rich feature set (JSON support, full-text search)
- Strong community and tooling
- Team familiarity

### Negative
- Manual scaling required (vertical)
- Less integrated with Azure ecosystem than SQL Database
- Need to manage connection pooling

### Neutral
- Standard SQL, portable to other clouds

## Implementation Notes
- Use Flexible Server (better performance/cost ratio)
- Enable high availability in production
- Configure automated backups (35-day retention)
- Use pgBouncer for connection pooling
```

### Phase 3: Detailed Design

#### 3.1 API Design

**OpenAPI Specification**:
```yaml
openapi: 3.0.0
info:
  title: Order Management API
  version: 1.0.0
  description: API for managing orders

paths:
  /orders:
    post:
      summary: Create a new order
      tags:
        - Orders
      security:
        - bearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateOrderRequest'
      responses:
        '201':
          description: Order created successfully
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Order'
        '400':
          description: Invalid request
        '401':
          description: Unauthorized

components:
  schemas:
    CreateOrderRequest:
      type: object
      required:
        - items
      properties:
        items:
          type: array
          items:
            $ref: '#/components/schemas/OrderItem'
        shippingAddress:
          $ref: '#/components/schemas/Address'
    
    Order:
      type: object
      properties:
        id:
          type: string
          format: uuid
        status:
          type: string
          enum: [pending, confirmed, shipped, delivered, cancelled]
        items:
          type: array
          items:
            $ref: '#/components/schemas/OrderItem'
        total:
          type: number
          format: decimal
        createdAt:
          type: string
          format: date-time
```

#### 3.2 Data Model Design

**Entity Design**:
```typescript
// Domain Model
export class Order {
  id: string;
  customerId: string;
  status: OrderStatus;
  items: OrderItem[];
  shippingAddress: Address;
  billingAddress: Address;
  subtotal: Money;
  tax: Money;
  shipping: Money;
  total: Money;
  createdAt: Date;
  updatedAt: Date;
  
  constructor(data: OrderData) {
    // Validation
    if (!data.items || data.items.length === 0) {
      throw new ValidationError('Order must have at least one item');
    }
    
    // Initialization
    this.items = data.items.map(item => new OrderItem(item));
    this.calculateTotals();
  }
  
  // Business logic
  addItem(item: OrderItem): void {
    if (this.status !== OrderStatus.Draft) {
      throw new BusinessRuleError('Cannot modify confirmed order');
    }
    this.items.push(item);
    this.calculateTotals();
  }
  
  confirm(): void {
    if (this.status !== OrderStatus.Draft) {
      throw new BusinessRuleError('Order already confirmed');
    }
    this.status = OrderStatus.Confirmed;
    this.updatedAt = new Date();
  }
  
  private calculateTotals(): void {
    this.subtotal = this.items.reduce((sum, item) => 
      sum.add(item.price.multiply(item.quantity)), Money.zero()
    );
    this.tax = this.subtotal.multiply(0.20); // 20% VAT
    this.total = this.subtotal.add(this.tax).add(this.shipping);
  }
}
```

**Database Schema**:
```sql
-- Orders table
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES users(id),
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    subtotal DECIMAL(10, 2) NOT NULL,
    tax DECIMAL(10, 2) NOT NULL,
    shipping DECIMAL(10, 2) NOT NULL DEFAULT 0,
    total DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
    CONSTRAINT valid_status CHECK (status IN ('pending', 'confirmed', 'shipped', 'delivered', 'cancelled')),
    CONSTRAINT positive_amounts CHECK (subtotal >= 0 AND tax >= 0 AND shipping >= 0)
);

CREATE INDEX idx_orders_customer ON orders(customer_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_created_at ON orders(created_at DESC);

-- Order items table
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES products(id),
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    total_price DECIMAL(10, 2) NOT NULL,
    
    CONSTRAINT positive_quantity CHECK (quantity > 0),
    CONSTRAINT positive_price CHECK (unit_price >= 0)
);

CREATE INDEX idx_order_items_order ON order_items(order_id);
```

## 📊 Trade-Off Analysis

**Template de comparaison**:
```markdown
## Option Comparison: Database Selection

### Option 1: PostgreSQL
**Pros**:
- ✅ Open-source, lower cost
- ✅ Rich SQL features
- ✅ Team expertise
- ✅ JSON support

**Cons**:
- ❌ Manual scaling
- ❌ Limited global distribution
- ❌ Connection management needed

**Cost**: ~$200/month
**Complexity**: Medium
**Risk**: Low

### Option 2: Cosmos DB
**Pros**:
- ✅ Global distribution
- ✅ Automatic scaling
- ✅ Multi-model support
- ✅ High availability built-in

**Cons**:
- ❌ Higher cost
- ❌ Different query language
- ❌ Team learning curve
- ❌ Vendor lock-in

**Cost**: ~$500/month
**Complexity**: High
**Risk**: Medium

### Decision: PostgreSQL
**Rationale**: 
- Current scale doesn't justify Cosmos DB cost
- Team has PostgreSQL expertise
- Can migrate to Cosmos DB later if needed
- Sufficient for 99.9% SLA requirement
```

## 🎯 Validation Checklist

- [ ] **Fonctionnel**: Répond à tous les besoins métier
- [ ] **Performance**: Atteint les objectifs NFR
- [ ] **Sécurité**: Vulnérabilités adressées
- [ ] **Scalabilité**: Peut gérer la croissance prévue
- [ ] **Maintenabilité**: Code propre, testé, documenté
- [ ] **Opérabilité**: Monitoring, alertes, runbooks
- [ ] **Coût**: Dans le budget
- [ ] **Timeline**: Réalisable dans les délais
- [ ] **Risques**: Identifiés et mitigés
- [ ] **Conformité**: Respect des réglementations

## 📚 Livrables

1. **Solution Design Document**: Vue d'ensemble complète
2. **Architecture Diagrams**: C4, UML, infrastructure
3. **ADRs**: Décisions majeures documentées
4. **API Specifications**: OpenAPI/Swagger
5. **Data Model**: ERD et schémas SQL
6. **Cost Estimation**: Breakdown détaillé
7. **Risk Register**: Risques et mitigations
8. **Timeline**: Planning détaillé avec jalons
