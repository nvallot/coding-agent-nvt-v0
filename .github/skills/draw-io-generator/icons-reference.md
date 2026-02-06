# Azure Icons Reference

## Overview

This document maps Azure services to their official icon paths for use in Draw.io diagrams.

## Icon Location

```
.github/templates/Azure_Public_Service_Icons/Icons/
```

**Relative base (from `docs/workflows/{flux}/diagrams/`):**

```
../../../../.github/templates/Azure_Public_Service_Icons/Icons/
```

Always embed icons in Draw.io using the relative base (never `file:///`). Example style fragment:
```
shape=image;image=../../../../.github/templates/Azure_Public_Service_Icons/Icons/compute/10029-icon-service-Function-Apps.svg;imageAspect=0;
```

## Icon Index

Reference: `.github/templates/azure-icons-index.md`

## Most Used Icons by Category

### Quick JSON Map (relative paths)

```json
{
	"Function App": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/compute/10029-icon-service-Function-Apps.svg",
	"App Service": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/compute/10035-icon-service-App-Services.svg",
	"Service Bus": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/integration/10836-icon-service-Azure-Service-Bus.svg",
	"Data Factory": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/integration/10126-icon-service-Data-Factories.svg",
	"Key Vault": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/security/10245-icon-service-Key-Vaults.svg",
	"Storage Account": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/storage/10086-icon-service-Storage-Accounts.svg",
	"SQL Database": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/databases/10130-icon-service-SQL-Database.svg",
	"Databricks": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/analytics/10787-icon-service-Azure-Databricks.svg",
	"Virtual Network": "../../../../.github/templates/Azure_Public_Service_Icons/Icons/networking/10061-icon-service-Virtual-Networks.svg"
}
```

### Compute

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Function App | `compute/10029-icon-service-Function-Apps.svg` | Azure Function App |
| App Service | `compute/10035-icon-service-App-Services.svg` | Azure App Service |
| Container Instances | `compute/10104-icon-service-Container-Instances.svg` | Azure Container Instances |
| AKS | `compute/10023-icon-service-Kubernetes-Services.svg` | Azure Kubernetes Service |

### Integration

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Service Bus | `integration/10836-icon-service-Azure-Service-Bus.svg` | Azure Service Bus |
| Data Factory | `integration/10126-icon-service-Data-Factories.svg` | Azure Data Factory |
| Logic Apps | `integration/02631-icon-service-Logic-Apps.svg` | Azure Logic Apps |
| Event Hub | `analytics/10820-icon-service-Event-Hubs.svg` | Azure Event Hubs |
| API Management | `integration/10042-icon-service-API-Management-Services.svg` | Azure API Management |

### Storage

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Storage Account | `storage/10086-icon-service-Storage-Accounts.svg` | Azure Storage Account |
| Blob Storage | `general/10780-icon-service-Blob-Block.svg` | Azure Blob Storage |
| Table Storage | `general/10841-icon-service-Table.svg` | Azure Table Storage |
| Queue Storage | `general/10840-icon-service-Queue-Storage.svg` | Azure Queue Storage |
| Data Lake Gen2 | `storage/10086-icon-service-Storage-Accounts.svg` | Azure Data Lake Storage Gen2 |

### Databases

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| SQL Database | `databases/10130-icon-service-SQL-Database.svg` | Azure SQL Database |
| SQL Server | `databases/10132-icon-service-SQL-Server.svg` | Azure SQL Server |
| Cosmos DB | `databases/10121-icon-service-Azure-Cosmos-DB.svg` | Azure Cosmos DB |
| Synapse Analytics | `analytics/00606-icon-service-Azure-Synapse-Analytics.svg` | Azure Synapse Analytics |

### Analytics

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Databricks | `analytics/10787-icon-service-Azure-Databricks.svg` | Azure Databricks |
| Event Hub | `analytics/10820-icon-service-Event-Hubs.svg` | Azure Event Hubs |
| Stream Analytics | `analytics/00042-icon-service-Stream-Analytics-Jobs.svg` | Azure Stream Analytics |

### Security

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Key Vault | `security/10245-icon-service-Key-Vaults.svg` | Azure Key Vault |
| Entra ID | `identity/10221-icon-service-Azure-Active-Directory.svg` | Azure Active Directory |
| Managed Identity | `identity/10227-icon-service-Managed-Identities.svg` | Managed Identities |

### Monitoring

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Application Insights | `monitor/00012-icon-service-Application-Insights.svg` | Application Insights |
| Log Analytics | `monitor/00009-icon-service-Log-Analytics-Workspaces.svg` | Log Analytics Workspace |
| Azure Monitor | `monitor/00001-icon-service-Monitor.svg` | Azure Monitor |

### Networking

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Virtual Network | `networking/10061-icon-service-Virtual-Networks.svg` | Azure Virtual Network |
| Private Endpoint | `networking/02579-icon-service-Private-Endpoint.svg` | Private Endpoint |
| NSG | `networking/10067-icon-service-Network-Security-Groups.svg` | Network Security Group |

### General

| Service | Icon Path | Draw.io Shape |
|---------|-----------|---------------|
| Resource Group | `general/10007-icon-service-Resource-Groups.svg` | Resource Group |
| Subscription | `general/10002-icon-service-Subscriptions.svg` | Subscription |

## External Services (Non-Azure)

| Service | Recommended Shape | Color |
|---------|-------------------|-------|
| Dataverse | `Common Data Service` or rectangle | Green zone |
| Power Platform | Generic Power Platform icon | Green zone |
| Lucy (HR) | Rectangle with text | Green zone |
| SAP | Rectangle with SAP logo | Yellow zone (On-Prem) |
| ERP (NAV/BC) | Dynamics 365 icon | Yellow zone (On-Prem) |
| File Share | File icon | Yellow zone |

## Icon Import in Draw.io

1. **File** → **Import from** → **Device**
2. Select the `.svg` file
3. Resize to standard size: **60x60 px** or **80x80 px**
4. Group icon with label below

## Label Format

```
{Azure Service Type}
{Instance Name}
{Function/Purpose}
```

Example:
```
Azure Function
SBWE1-ISP-PRD-FAP-65
RetrievePurchaseOrder
```

## Naming in Diagrams

Always use the full naming convention from the project:
- SBM format: `sbm-{project}-{env}-{type}-{region}-{nnn}`
- Show the function/purpose name below
