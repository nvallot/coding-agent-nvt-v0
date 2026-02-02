# IT 05a - Multi-Repository Architecture

## Overview

The IT 05a (NADIA to Supplier Portal) integration has been split into **two separate Azure repositories** to maintain clean separation of concerns and enable independent deployment.

## Repository Structure

```
workspace/
├── src/
│   ├── NADIA/                          [Repository 1]
│   │   ├── NadiaSpaIntegration.sln
│   │   ├── FAP-65.RetrivePOVendor/    ← Data extraction from NADIA SQL
│   │   ├── FAP-65.Tests/
│   │   ├── Shared.Models/             ← Shared DTOs (local copy)
│   │   ├── README.md
│   │   ├── DEPLOYMENT.md
│   │   └── monitoring/                ← Dashboard & KQL queries
│   │
│   └── SupplierPortal/                 [Repository 2]
│       ├── SupplierPortal.sln
│       ├── FAP-57.SendPOSupplier/     ← Data enrichment & Dataverse upsert
│       ├── FAP-57.Tests/
│       ├── Shared.Models/             ← Shared DTOs (local copy)
│       └── README.md
```

## Workflow Flow

```
┌──────────────────────────────────────────────────────────────────┐
│ NADIA REPOSITORY (FAP-65)                                        │
│                                                                  │
│  Timer Trigger (04:00 CET)                                       │
│      ↓                                                            │
│  [RetrivePOVendorFunction.cs]                                    │
│      ├─→ NadiaDataService.GetPurchaseOrdersAsync()              │
│      │    └─→ NADIA SQL stored proc + Polly retry (3x)          │
│      ├─→ ServiceBusPublisher.PublishAsync()                      │
│      │    └─→ Service Bus Topic "purchase-orders"               │
│      └─→ LastExecutionService.UpdateAsync()                      │
│           └─→ Table Storage tracking                            │
│                                                                  │
│  📊 Monitoring: Application Insights (custom events)             │
└──────────────────────────────────────────────────────────────────┘
              ↓↓↓ Messages published to Service Bus ↓↓↓
┌──────────────────────────────────────────────────────────────────┐
│ SUPPLIER PORTAL REPOSITORY (FAP-57)                              │
│                                                                  │
│  Service Bus Trigger (Topic subscription "spa-processor")        │
│      ↓                                                            │
│  [SendPOSupplierFunction.cs]                                     │
│      ├─→ LucyApiService.GetUserByIdAsync()                      │
│      │    └─→ HTTP GET /api/users/{pkmGuid} + Polly (3x)        │
│      ├─→ DataverseService.UpsertStagedPurchaseOrderAsync()       │
│      │    └─→ OAuth 2.0 + Dataverse upsert + Polly (5x)         │
│      └─→ ServiceBusMessageActions (Complete/DeadLetter/Abandon)  │
│                                                                  │
│  📊 Monitoring: Application Insights (custom events + errors)    │
└──────────────────────────────────────────────────────────────────┘
```

## Key Design Decisions

### 1. **Separation of Concerns**
- **FAP-65** focuses solely on data extraction from NADIA
- **FAP-57** focuses on enrichment and Dataverse integration
- Each repository has its own build, test, and deployment pipeline

### 2. **Shared Models**
Both repositories have **identical copies** of shared DTOs:
- `PurchaseOrderMessage` - Service Bus message contract
- `DataverseStagingPurchaseOrder` - Dataverse entity mapping
- `LucyUserResponse` - Lucy API response model

**Why duplicate?** To allow independent versioning and deployment. Each repository controls its model versions.

### 3. **Service Bus as Integration Point**
- Service Bus **Topic** `purchase-orders` acts as the contract
- FAP-65 publishes `PurchaseOrderMessage`
- FAP-57 subscribes with `spa-processor` subscription
- Topic enables future subscribers (audit, secondary processing, etc.)

### 4. **Managed Identity & RBAC**
Each Function App has its own Managed Identity with specific permissions:
- **FAP-65**: Read NADIA SQL, Write Service Bus Topic, Write Table Storage
- **FAP-57**: Read Service Bus Subscription, Read/Write Dataverse, Read Lucy API

## Deployment Model

### Azure DevOps Pipelines

**NADIA Pipeline** (trigger: src/NADIA)
```
1. Build: dotnet build src/NADIA/
2. Test: dotnet test src/NADIA/FAP-65.Tests/
3. Package: zip FAP-65 binary
4. Deploy: Push to FAP-65 Function App (slot-based rollout)
5. Config: Apply app settings (env-specific)
6. Validation: KQL health checks via Monitoring dashboard
```

**Supplier Portal Pipeline** (trigger: src/SupplierPortal)
```
1. Build: dotnet build src/SupplierPortal/
2. Test: dotnet test src/SupplierPortal/FAP-57.Tests/
3. Package: zip FAP-57 binary
4. Deploy: Push to FAP-57 Function App (slot-based rollout)
5. Config: Apply app settings (env-specific)
6. Validation: KQL health checks via Monitoring dashboard
```

Both pipelines:
- Run on DEV/STG with automatic approval gates
- Require manual approval for PRD
- Use separate app configurations per environment

## Shared Infrastructure

These resources are **shared** between both repositories:

| Resource | Owner | Used By |
|----------|-------|---------|
| Service Bus Namespace | NADIA | FAP-65 (publish), FAP-57 (subscribe) |
| Key Vault (secrets) | Central | Both FAs (Dataverse creds, Lucy API keys) |
| Storage Account (Table Storage) | NADIA | FAP-65 (execution tracking) |
| Application Insights | Central | Both FAs (monitoring & alerts) |
| Azure SQL (NADIA DB) | Infra | FAP-65 only |

**Terraform modules** are stored in NADIA repository (see `terraform/` folder).

## Monitoring & Observability

### Unified Monitoring Dashboard
Single **Application Insights workbook** shows:
- FAP-65 execution metrics
- Service Bus flow status
- FAP-57 processing metrics
- Error aggregation & dead-letter tracking
- Dataverse upsert confirmation

### KQL Queries (included in both READMEs)
- Execution count by function
- Message latency (SB to Dataverse)
- Error rate & exception types
- Dataverse field validation failures
- PKM enrichment success rate

## Development Workflow

### Local Development

**Scenario 1: Modify FAP-65 logic**
```bash
cd src/NADIA/
dotnet restore
dotnet build
dotnet test FAP-65.Tests/
func start  # Run FAP-65 locally
```

**Scenario 2: Modify FAP-57 logic**
```bash
cd src/SupplierPortal/
dotnet restore
dotnet build
dotnet test FAP-57.Tests/
func start  # Run FAP-57 locally
```

### Integration Testing
- Use **Azure Storage Emulator** for Table Storage (FAP-65)
- Use **Azure Service Bus Emulator** or local Service Bus for message testing
- Mock Lucy API & Dataverse in test environment

### CI/CD Integration
Each repository has its own Azure DevOps pipeline:
- Triggered on changes to `src/NADIA/**` → builds NADIA solution
- Triggered on changes to `src/SupplierPortal/**` → builds SupplierPortal solution
- Both pipelines can run in parallel

## Migration Path (if needed)

If you want to recombine into a single solution:

1. **Merge `.sln` files**: Create master `SPA-Integration.sln`
2. **Deduplicate Models**: Choose single source of truth for Shared.Models
3. **Update project references**: Adjust CSPROJ references
4. **Unified pipeline**: Create single build & test pipeline
5. **Update documentation**: Revise README & DEPLOYMENT guides

## Dependencies & Versions

### NuGet Packages (same across both solutions)
```xml
Microsoft.Azure.Functions.Worker: 1.21.0
Microsoft.Azure.Functions.Worker.Extensions.ServiceBus: 5.16.0
Microsoft.ApplicationInsights.WorkerService: 2.22.0
Microsoft.PowerPlatform.Dataverse.Client: 1.1.14
Azure.Identity: 1.11.0
Polly: 8.3.1
```

### .NET Runtime
- **Target Framework**: net8.0
- **Isolated Worker Model**: Yes (not in-process)

## Troubleshooting

### Issue: FAP-57 not processing messages
**Check**:
1. Service Bus Topic has subscription "spa-processor"
2. FAP-65 publishing to "purchase-orders" topic
3. Dataverse credentials valid (Key Vault)
4. Application Insights shows SPA_PKM_NotFound events (expected some)

### Issue: FAP-65 not extracting data
**Check**:
1. Table Storage "LastExecutionDate" table exists
2. NADIA SQL Server connectivity (firewall/NSG)
3. SQL stored proc returns data (query directly)
4. Managed Identity has SQL Reader role

### Issue: Dataverse updates failing
**Check**:
1. OAuth 2.0 token acquisition (Dataverse ClientId/Secret)
2. Entity logical name: `sbm_stagedpurchaseorder`
3. Field mappings (case-sensitive in XML)
4. Status code 918860002 exists in Dataverse

## Related Documentation

- [NADIA Repository README](src/NADIA/README.md)
- [Supplier Portal Repository README](src/SupplierPortal/README.md)
- [Deployment Guide](src/NADIA/DEPLOYMENT.md)
- [Business Requirements](docs/CdC_Fonctionnel_IT05a.md)
- [Solution Architecture](src/NADIA/ARCHITECTURE.md)

---

**Last Updated**: Jan 30, 2026  
**Architecture Owner**: Integration Services Platform Team  
**Status**: Production-ready ✅
