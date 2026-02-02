# NADIA Repository - FAP-65 (RetrivePOVendor)

[![Build Status](https://dev.azure.com/sbm-offshore/Integration-Services-Platform/_apis/build/status/nadia-spa-integration)](https://dev.azure.com/sbm-offshore/Integration-Services-Platform/_build/latest)

## 📋 Vue d'ensemble

Extraction quotidienne des Purchase Orders depuis NADIA SQL Server et publication dans Azure Service Bus.

**IT**: 05a - NADIA to Supplier Portal  
**Composant**: FAP-65 (First part of the flow)  
**Environnements**: DEV, STG, PRD

## ⚠️ Multi-Repository Architecture

This project is now **split across two repositories**:

1. **NADIA Repository** (this one) - Handles FAP-65 data extraction
2. **[Supplier Portal Repository](../SupplierPortal/)** - Handles FAP-57 data enrichment & Dataverse integration

```
┌─────────────────────────────────────────────────────────────┐
│                    NADIA REPOSITORY                          │
│  Timer (04:00 CET) → [FAP-65] NADIA Retrieval               │
│                          ↓                                    │
│                    Service Bus Topic (purchase-orders)       │
│                          ↓                                    │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│              SUPPLIER PORTAL REPOSITORY                       │
│  [FAP-57] SPA Sender (Service Bus Trigger)                   │
│       ↓ (Enrich with Lucy API)                               │
│  Dataverse (sbm_stagedpurchaseorder)                         │
└─────────────────────────────────────────────────────────────┘
```

## 📦 Composants

### FAP-65: RetrivePurchaseOrderVendor (this repository)
- **Type**: Timer Trigger Function
- **Runtime**: .NET 8 Isolated
- **Trigger**: Quotidien 04:00 CET
- **Responsabilité**: Extraire PO depuis NADIA SQL et publier dans Service Bus

**Dépendances**:
- NADIA SQL Server (corpnet)
- Service Bus Topic `purchase-orders`
- Table Storage (LastExecutionDate)

### FAP-57: SendPurchaseOrderSupplier (Supplier Portal Repository)
**MOVED**: See [Supplier Portal Repository](../SupplierPortal/README.md)
- Service Bus Trigger Function
- Enrichissement avec Lucy API
- Upsert vers Dataverse

## 🚀 Démarrage rapide

### Prérequis
- .NET 8 SDK
- Azure Functions Core Tools v4
- Visual Studio 2022 ou VS Code
- Accès réseau NADIA (corpnet)
- Accès Azure resources (DEV environment)

### Configuration locale

1. **Cloner le repository**
```powershell
git clone https://sbm-offshore@dev.azure.com/sbm-offshore/Integration-Services-Platform/_git/nadia-spa-integration
cd nadia-spa-integration/src/NADIA
```

2. **Configurer les secrets locaux**

Créer `FAP-65.RetrivePOVendor/local.settings.json`:
```json
{
  "Values": {
    "NadiaConnectionString": "Server=nadia-db-stg.corpnet.singlebuoy.com;Database=NADIA;User Id=SQL_NADIA_IFS_AZURE_DEV;Password=<PASSWORD>;",
    "ServiceBusConnection__fullyQualifiedNamespace": "sbwe1-isp-dv-sbn-02.servicebus.windows.net",
    "StorageAccount": "DefaultEndpointsProtocol=https;AccountName=sbwe1ispdvnadia;AccountKey=<KEY>;"
  }
}
```

Créer `FAP-57.SendPOSupplier/local.settings.json`:
```json
{
  "Values": {
    "ServiceBusConnection__fullyQualifiedNamespace": "sbwe1-isp-dv-sbn-02.servicebus.windows.net",
    "LucyApiBaseUrl": "https://lucy-api-dev.sbm.com",
    "DataverseApiBaseUrl": "https://sbmsupplierportaltest.crm4.dynamics.com/api/data/v9.2",
    "DataverseClientId": "<CLIENT_ID>",
    "DataverseClientSecret": "<CLIENT_SECRET>",
    "DataverseTenantId": "<TENANT_ID>"
  }
}
```

3. **Restaurer et build**
```powershell
dotnet restore
dotnet build
```

4. **Run tests**
```powershell
dotnet test
```

5. **Run Functions localement**

Terminal 1 (FAP-65):
```powershell
cd FAP-65.RetrivePOVendor
func start
```

Terminal 2 (FAP-57):
```powershell
cd FAP-57.SendPOSupplier
func start
```

## 🧪 Tests

### Exécuter tous les tests
```powershell
dotnet test --logger "console;verbosity=detailed"
```

### Coverage report
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**Couverture actuelle**: 85% (objectif: 80%+)

## 📊 Monitoring

### Application Insights

**Workspace**: `SBWE1-ISP-{ENV}-API-01`

### Dashboard standard ISP (obligatoire)

- Template Workbook: [src/NADIA/monitoring/ISP-NADIA-SPA-Dashboard.workbook.json](src/NADIA/monitoring/ISP-NADIA-SPA-Dashboard.workbook.json)
- Guide d'import: [src/NADIA/monitoring/README.md](src/NADIA/monitoring/README.md)

**Custom Events**:
- `NADIA_Execution_Started`
- `NADIA_Execution_Completed`
- `NADIA_PO_Published`
- `SPA_PKM_Enriched`
- `SPA_Dataverse_Sent`

### KQL Queries

**Voir les dernières exécutions**:
```kql
customEvents
| where name == "NADIA_Execution_Completed"
| project timestamp, customDimensions.CorrelationId, customDimensions.POProcessed, customDimensions.Duration
| order by timestamp desc
| take 10
```

**Erreurs PKM Not Found**:
```kql
customEvents
| where name == "SPA_PKM_NotFound"
| project timestamp, customDimensions.PkmGuid, customDimensions.PoNumber
| order by timestamp desc
```

## 🔒 Sécurité

- **Managed Identity**: Utilisé pour Service Bus, Storage, Key Vault
- **OAuth 2.0**: Dataverse authentication (Client Credentials flow)
- **Key Vault**: Stockage des secrets (SQL passwords, Client secrets)
- **NSG**: Restrictions réseau sur Function Apps

## 🔄 CI/CD

### Pipelines Azure DevOps

**Build Pipeline**: `nadia-spa-build.yml`
- Compile .NET 8 projects
- Run unit tests
- Publish artifacts
- Terraform validate

**Release Pipeline**: `nadia-spa-release.yml`
- Deploy DEV (auto)
- Deploy STG (manual approval)
- Deploy PRD (CAB approval)

### Déploiement manuel

```powershell
# Build & Publish
dotnet publish FAP-65.RetrivePOVendor -c Release -o ./publish/fap65

# Deploy via Azure CLI
az functionapp deployment source config-zip \
  --resource-group IntegrationServicesDEV-NDA-RG \
  --name SBWE1-ISP-DV-FAP-65 \
  --src ./publish/fap65.zip
```

## 📝 Logs & Diagnostics

### Activer verbose logging

Modifier `host.json`:
```json
{
  "logging": {
    "logLevel": {
      "default": "Debug"
    }
  }
}
```

### Accéder aux logs en temps réel

```powershell
func azure functionapp logstream SBWE1-ISP-DV-FAP-65
```

## 🐛 Troubleshooting

### ❌ NADIA Connection Failed

**Erreur**: `SqlException: A network-related or instance-specific error`

**Solutions**:
1. Vérifier réseau corpnet (VPN)
2. Tester connectivité: `Test-NetConnection nadia-db-stg.corpnet.singlebuoy.com -Port 1433`
3. Vérifier credentials dans Key Vault

### ❌ PKM Not Found

**Erreur**: Messages en Dead Letter Queue avec `PKMNotFound`

**Solutions**:
1. Vérifier GUID PKM dans NADIA vs Lucy API
2. Query Lucy API: `GET /api/users/{pkmGuid}`
3. Reprocesser dead letters après correction

### ❌ Dataverse Throttling

**Erreur**: `429 TooManyRequests`

**Solutions**:
1. Réduire `maxConcurrentCalls` dans `host.json`
2. Activer retry avec backoff (déjà configuré)
3. Contacter admin Dataverse pour augmenter quotas

## 📚 Documentation

- [Business Requirements](../../.github/projects/nadia-spa-integration/01-BA-CahierDesCharges.md)
- [Technical Architecture](../../.github/projects/nadia-spa-integration/02-ARCHI-SpecificationsTechniques.md)
- [Deployment Guide](./DEPLOYMENT.md)
- [ISP Platform Guide](../../.github/knowledge/sbm-integration-services-platform.md)

## 👥 Équipe

| Rôle | Contact |
|------|---------|
| **Product Owner** | john.doe@sbm.com |
| **Tech Lead** | jane.smith@sbm.com |
| **DevOps** | devops@sbm.com |
| **Support 24/7** | support@middleway.com |

## 📋 Checklist déploiement

- [ ] Tests unitaires passent (80%+ coverage)
- [ ] Configuration secrets dans Key Vault
- [ ] Managed Identities configurées (FAP-65, FAP-57)
- [ ] Service Bus Topic + Subscription créés
- [ ] Table Storage `LastExecutionDate` initialisée
- [ ] Dataverse table `sbm_stagedpurchaseorder` accessible
- [ ] Application Insights connecté
- [ ] Alerts configurées
- [ ] Documentation mise à jour

## 📄 Licence

© 2026 SBM Offshore - Internal Use Only
