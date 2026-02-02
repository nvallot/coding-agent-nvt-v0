# Supplier Portal - FAP-57 (SendPOSupplier)

## Overview

**Supplier Portal** is a dedicated Azure Functions project for the **Supplier Performance Assessment (SPA)** integration flow.

This repository handles the second part of the IT 05a integration:
- **FAP-57**: `SendPOSupplier` - Processes Purchase Orders from NADIA, enriches them with PKM data from Lucy API, and upserts to Dataverse staging.

## Architecture

```
Service Bus Topic (purchase-orders)
    ↓
[FAP-57.SendPOSupplier] - Service Bus Trigger
    ├─→ Deserialize PurchaseOrderMessage
    ├─→ Enrich with Lucy API (/api/users/{pkmGuid})
    ├─→ Map to Dataverse DataverseStagingPurchaseOrder
    └─→ Upsert to Dataverse (sbm_stagedpurchaseorder)
         └─→ Monitor via Application Insights
```

## Project Structure

```
src/SupplierPortal/
├── SupplierPortal.sln
├── FAP-57.SendPOSupplier/
│   ├── SendPOSupplierFunction.cs       (Service Bus trigger function)
│   ├── Program.cs                       (DI setup)
│   ├── host.json                        (Function runtime config)
│   ├── local.settings.json              (Dev secrets template)
│   └── Services/
│       ├── ILucyApiService.cs
│       ├── LucyApiService.cs            (HTTP GET + Polly retry: 3 attempts)
│       ├── IDataverseService.cs
│       └── DataverseService.cs          (OAuth 2.0 + upsert: 5 attempts)
├── Shared.Models/                       (Shared DTOs)
│   ├── PurchaseOrderMessage.cs
│   ├── DataverseStagingPurchaseOrder.cs
│   └── LucyUserResponse.cs
└── FAP-57.Tests/
    ├── SendPOSupplierFunctionTests.cs   (xUnit tests)
    └── FAP-57.Tests.csproj
```

## Services

### LucyApiService
- **Purpose**: HTTP GET enrichment from Lucy API endpoint `/api/users/{pkmGuid}`
- **Retry Policy**: 3 attempts, exponential backoff (2^n seconds)
- **Handles**: 404 (user not found), 429 (throttling), timeouts
- **Logging**: Structured with correlation IDs, debug + warnings

### DataverseService
- **Purpose**: OAuth 2.0 Client Credentials authentication + upsert to Dataverse
- **Entity**: `sbm_stagedpurchaseorder`
- **Fields**: 15 mapped fields (poNumber, mdmNumber, pkm*, product*, amount, dates, etc.)
- **Status Code**: 918860002 (Ready to be Processed)
- **Retry Policy**: 5 attempts, exponential backoff, respects Retry-After headers
- **Logic**: Query by poNumber → Update existing or Create new

## Configuration

### Environment Variables

```json
{
  "ServiceBusConnection__fullyQualifiedNamespace": "sbwe1-isp-dv-sbn-02.servicebus.windows.net",
  "LucyApiBaseUrl": "https://lucy-api-dev.sbm.com",
  "DataverseApiBaseUrl": "https://sbmsupplierportaltest.crm4.dynamics.com/api/data/v9.2",
  "DataverseClientId": "<YOUR_CLIENT_ID>",
  "DataverseClientSecret": "<YOUR_CLIENT_SECRET>",
  "DataverseTenantId": "<YOUR_TENANT_ID>",
  "APPINSIGHTS_INSTRUMENTATIONKEY": "<YOUR_INSTRUMENTATION_KEY>"
}
```

## Building & Testing

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools v4

### Build
```bash
cd src/SupplierPortal
dotnet restore
dotnet build
```

### Run Locally
```bash
cd src/SupplierPortal/FAP-57.SendPOSupplier
func start
```

### Run Tests
```bash
cd src/SupplierPortal
dotnet test
```

## Deployment

See [DEPLOYMENT.md](../NADIA/DEPLOYMENT.md) for step-by-step instructions covering:
1. Azure resource provisioning (Terraform)
2. Key Vault secrets setup
3. Function App deployment (zip publish)
4. App Settings configuration
5. Service Bus subscription creation
6. Monitoring dashboard import

## Monitoring & Alerts

Integrates with **Application Insights** for:
- Execution count & timestamps
- Error tracking (dead letters, retries)
- Exception logging
- Dataverse upsert confirmation
- PKM not found events
- Service Bus delivery count tracking

### Key Telemetry Events
- `SPA_Message_Received` - Service Bus message arrived
- `SPA_PKM_Enriched` - Lucy API enrichment succeeded
- `SPA_PKM_NotFound` - Lucy API returned 404
- `SPA_Dataverse_Sent` - Dataverse upsert succeeded
- `SPA_Dataverse_Error` - Dataverse operation failed

## Error Handling

### Dead Letter Scenarios
1. **InvalidMessageFormat**: Deserialization failed
2. **PKMNotFound**: Lucy API returned 404 for pkmGuid
3. **MaxDeliveryAttemptsExceeded**: Retried 5+ times

### Retry Scenarios
- Transient HTTP errors (5xx, 429, timeout)
- Dataverse throttling (429, 503)
- Automatic Service Bus retries on AbandonMessage

## Related Projects

- **NADIA Repository**: Contains FAP-65 (NADIA data retrieval) + Shared infrastructure configuration
- **Shared Models**: `PurchaseOrderMessage`, `DataverseStagingPurchaseOrder`, `LucyUserResponse`

## License & Support

Internal SBM project (IT 05a Integration Scenario Platform)

For questions on architecture or deployment, refer to [Architecture Documentation](../NADIA/DEPLOYMENT.md#6-monitoring--kql-queries).
