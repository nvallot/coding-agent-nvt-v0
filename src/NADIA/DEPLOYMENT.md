# Guide de Déploiement
## NADIA to Supplier Performance Assessment Integration

Version: 1.0  
Date: 30 janvier 2026

---

## Table des matières

1. [Prérequis](#1-prérequis)
2. [Environnements](#2-environnements)
3. [Infrastructure (Terraform)](#3-infrastructure-terraform)
4. [Déploiement des Function Apps](#4-déploiement-des-function-apps)
5. [Configuration post-déploiement](#5-configuration-post-déploiement)
6. [Validation](#6-validation)
7. [Rollback](#7-rollback)

---

## 1. Prérequis

### Outils requis

| Outil | Version | Installation |
|-------|---------|--------------|
| Azure CLI | 2.54+ | `winget install Microsoft.AzureCLI` |
| Terraform | 1.6+ | `choco install terraform` |
| .NET SDK | 8.0 | `winget install Microsoft.DotNet.SDK.8` |
| Azure Functions Core Tools | 4.x | `npm install -g azure-functions-core-tools@4` |
| PowerShell | 7.4+ | Préinstallé Windows |

### Accès requis

- **Azure**: Contributor sur subscription ISP
- **Azure DevOps**: Contributor sur projet Integration Services Platform
- **NADIA SQL**: Compte `SQL_NADIA_IFS_AZURE_{ENV}`
- **Dataverse**: Service Principal avec role System Administrator
- **Key Vault**: Get/List Secrets permissions

### Secrets requis

Stocker dans Azure Key Vault `SBWE1-ISP-{ENV}-KVA-01`:

```powershell
# NADIA SQL Password
az keyvault secret set \
  --vault-name "SBWE1-ISP-DV-KVA-01" \
  --name "NADIA-SQL-PASSWORD-DEV" \
  --value "SECURE_PASSWORD_HERE"

# Dataverse Client Secret
az keyvault secret set \
  --vault-name "SBWE1-ISP-DV-KVA-01" \
  --name "SUPPLIER-PORTAL-DATAVERSE-CLIENT-SECRET" \
  --value "CLIENT_SECRET_HERE"
```

---

## 2. Environnements

### DEV - Développement

| Resource | Name | Purpose |
|----------|------|---------|
| Resource Group | `IntegrationServicesDEV-NDA-RG` | Ressources NADIA |
| Function App 65 | `SBWE1-ISP-DV-FAP-65` | NADIA Retrieval |
| Function App 57 | `SBWE1-ISP-DV-FAP-57` | SPA Sender |
| Storage NADIA | `sbwe1ispdvnadia` | FAP-65 storage |
| Storage SPA | `sbwe1ispdvsuportal` | FAP-57 storage |
| NADIA DB | `nadia-db-stg.corpnet.singlebuoy.com` | Source (STG) |
| Dataverse | `sbmsupplierportaltest.crm4.dynamics.com` | Target (TEST) |

### STG - Staging

| Resource | Name | Purpose |
|----------|------|---------|
| Resource Group | `IntegrationServicesSTG-NDA-RG` | Ressources NADIA |
| Function App 65 | `SBWE1-ISP-ST-FAP-65` | NADIA Retrieval |
| Function App 57 | `SBWE1-ISP-ST-FAP-57` | SPA Sender |
| Storage NADIA | `sbwe1ispstvnadia` | FAP-65 storage |
| Storage SPA | `sbwe1ispstvsuportal` | FAP-57 storage |
| NADIA DB | `nadia-db-stg.corpnet.singlebuoy.com` | Source (STG) |
| Dataverse | `sbmsupplierportaluat.crm4.dynamics.com` | Target (UAT) |

### PRD - Production

| Resource | Name | Purpose |
|----------|------|---------|
| Resource Group | `IntegrationServicesPRD-NDA-RG` | Ressources NADIA |
| Function App 65 | `SBWE1-ISP-PR-FAP-65` | NADIA Retrieval |
| Function App 57 | `SBWE1-ISP-PR-FAP-57` | SPA Sender |
| Storage NADIA | `sbwe1ispprvnadia` | FAP-65 storage |
| Storage SPA | `sbwe1ispprsuportal` | FAP-57 storage |
| NADIA DB | `nadia-db-prd.corpnet.singlebuoy.com` | Source (PRD) |
| Dataverse | `(TBD)` | Target (PRD) |

---

## 3. Infrastructure (Terraform)

### 3.1 Initialiser Terraform

```powershell
cd terraform

# Login Azure
az login

# Select subscription
az account set --subscription "ISP-Production"

# Initialize Terraform
terraform init \
  -backend-config="resource_group_name=ISP-Terraform-RG" \
  -backend-config="storage_account_name=ispterraformstate" \
  -backend-config="container_name=tfstate" \
  -backend-config="key=nadia-spa.tfstate"
```

### 3.2 Plan et Apply (DEV)

```powershell
# Plan
terraform plan -var-file="environments/dev.tfvars" -out=tfplan-dev

# Review changes
terraform show tfplan-dev

# Apply
terraform apply tfplan-dev
```

**Ressources créées**:
- Resource Group: `IntegrationServicesDEV-NDA-RG`
- Storage Accounts: `sbwe1ispdvnadia`, `sbwe1ispdvsuportal`
- Function Apps: `SBWE1-ISP-DV-FAP-65`, `SBWE1-ISP-DV-FAP-57`
- App Service Plan: Consommation (shared avec Core)
- Service Bus Topic: `purchase-orders` (dans namespace Core)
- Service Bus Subscription: `spa-processor`
- Role Assignments: Managed Identities → Service Bus, Key Vault, Storage

### 3.3 Plan et Apply (STG)

```powershell
terraform plan -var-file="environments/stg.tfvars" -out=tfplan-stg
terraform apply tfplan-stg
```

### 3.4 Plan et Apply (PRD)

⚠️ **Nécessite approbation CAB**

```powershell
terraform plan -var-file="environments/prd.tfvars" -out=tfplan-prd
# Review + CAB approval
terraform apply tfplan-prd
```

---

## 4. Déploiement des Function Apps

### 4.1 Build les artifacts

```powershell
cd src/NADIA

# Restore dependencies
dotnet restore

# Build Release
dotnet build -c Release

# Publish FAP-65
dotnet publish FAP-65.RetrivePOVendor -c Release -o .\publish\fap65

# Publish FAP-57
dotnet publish FAP-57.SendPOSupplier -c Release -o .\publish\fap57

# Créer les packages
Compress-Archive -Path .\publish\fap65\* -DestinationPath .\publish\fap65.zip -Force
Compress-Archive -Path .\publish\fap57\* -DestinationPath .\publish\fap57.zip -Force
```

### 4.2 Déployer FAP-65 (DEV)

```powershell
az functionapp deployment source config-zip `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65 `
  --src .\publish\fap65.zip

# Vérifier le déploiement
az functionapp show `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65 `
  --query "state"
```

**Output attendu**: `"Running"`

### 4.3 Déployer FAP-57 (DEV)

```powershell
az functionapp deployment source config-zip `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-57 `
  --src .\publish\fap57.zip

# Vérifier
az functionapp show `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-57 `
  --query "state"
```

### 4.4 Déployer STG et PRD

Répéter les étapes 4.2 et 4.3 en changeant:
- `IntegrationServicesDEV-NDA-RG` → `IntegrationServicesSTG-NDA-RG` ou `IntegrationServicesPRD-NDA-RG`
- `SBWE1-ISP-DV-FAP-XX` → `SBWE1-ISP-ST-FAP-XX` ou `SBWE1-ISP-PR-FAP-XX`

---

## 5. Configuration post-déploiement

### 5.1 Configurer App Settings (FAP-65)

```powershell
$rgName = "IntegrationServicesDEV-NDA-RG"
$funcName = "SBWE1-ISP-DV-FAP-65"

az functionapp config appsettings set `
  --resource-group $rgName `
  --name $funcName `
  --settings `
    "FUNCTIONS_WORKER_RUNTIME=dotnet-isolated" `
    "WEBSITE_RUN_FROM_PACKAGE=1" `
    "AZURE_FUNCTIONS_ENVIRONMENT=DEV" `
    "TimerSchedule=0 0 4 * * *" `
    "NadiaConnectionString=Server=nadia-db-stg.corpnet.singlebuoy.com;Database=NADIA;User Id=SQL_NADIA_IFS_AZURE_DEV;Password=@Microsoft.KeyVault(SecretUri=https://sbwe1ispdvkva01.vault.azure.net/secrets/NADIA-SQL-PASSWORD-DEV)" `
    "ServiceBusConnection__fullyQualifiedNamespace=sbwe1-isp-dv-sbn-02.servicebus.windows.net"
```

### 5.2 Configurer App Settings (FAP-57)

```powershell
$funcName = "SBWE1-ISP-DV-FAP-57"

az functionapp config appsettings set `
  --resource-group $rgName `
  --name $funcName `
  --settings `
    "FUNCTIONS_WORKER_RUNTIME=dotnet-isolated" `
    "WEBSITE_RUN_FROM_PACKAGE=1" `
    "ServiceBusConnection__fullyQualifiedNamespace=sbwe1-isp-dv-sbn-02.servicebus.windows.net" `
    "LucyApiBaseUrl=https://lucy-api-dev.sbm.com" `
    "DataverseApiBaseUrl=https://sbmsupplierportaltest.crm4.dynamics.com/api/data/v9.2" `
    "DataverseClientId=YOUR_CLIENT_ID" `
    "DataverseClientSecret=@Microsoft.KeyVault(SecretUri=https://sbwe1ispdvkva01.vault.azure.net/secrets/SUPPLIER-PORTAL-DATAVERSE-CLIENT-SECRET)" `
    "DataverseTenantId=YOUR_TENANT_ID"
```

### 5.3 Activer Managed Identity

```powershell
# FAP-65
az functionapp identity assign `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65

# FAP-57
az functionapp identity assign `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-57
```

### 5.4 Assigner les rôles Azure

```powershell
# Get Managed Identity Principal IDs
$fap65PrincipalId = az functionapp identity show `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65 `
  --query principalId -o tsv

$fap57PrincipalId = az functionapp identity show `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-57 `
  --query principalId -o tsv

# Service Bus - FAP-65 (Data Sender)
az role assignment create `
  --role "Azure Service Bus Data Sender" `
  --assignee $fap65PrincipalId `
  --scope "/subscriptions/<SUBSCRIPTION_ID>/resourceGroups/IntegrationServicesDEV-CMN-RG/providers/Microsoft.ServiceBus/namespaces/sbwe1-isp-dv-sbn-02"

# Service Bus - FAP-57 (Data Receiver)
az role assignment create `
  --role "Azure Service Bus Data Receiver" `
  --assignee $fap57PrincipalId `
  --scope "/subscriptions/<SUBSCRIPTION_ID>/resourceGroups/IntegrationServicesDEV-CMN-RG/providers/Microsoft.ServiceBus/namespaces/sbwe1-isp-dv-sbn-02"

# Key Vault - FAP-65
az keyvault set-policy `
  --name SBWE1-ISP-DV-KVA-01 `
  --object-id $fap65PrincipalId `
  --secret-permissions get list

# Key Vault - FAP-57
az keyvault set-policy `
  --name SBWE1-ISP-DV-KVA-01 `
  --object-id $fap57PrincipalId `
  --secret-permissions get list
```

### 5.5 Initialiser Table Storage

```powershell
# Installer module Azure.Data.Tables
Install-Module -Name Az.Storage

$storageAccountName = "sbwe1ispdvnadia"
$tableName = "LastExecutionDate"

$storageAccount = Get-AzStorageAccount `
  -ResourceGroupName IntegrationServicesDEV-NDA-RG `
  -Name $storageAccountName

$ctx = $storageAccount.Context

# Créer la table
New-AzStorageTable -Name $tableName -Context $ctx

# Insérer la valeur initiale (premier run = 30 jours lookback)
# Optionnel - la Function le fera automatiquement
```

### 5.6 Importer le dashboard Monitoring (obligatoire)

> Le dashboard standard ISP doit être importé dans le workspace App Insights associé au flux.

**Template**: [src/NADIA/monitoring/ISP-NADIA-SPA-Dashboard.workbook.json](src/NADIA/monitoring/ISP-NADIA-SPA-Dashboard.workbook.json)

**Étapes (Azure Portal)**:
1. Ouvrir Application Insights `SBWE1-ISP-{ENV}-API-01`
2. Aller dans **Workbooks** → **New** → **Advanced Editor**
3. Coller le JSON du template et **Apply**
4. Sauvegarder sous le nom **ISP - NADIA SPA - Monitoring Dashboard**

---

## 6. Validation

### 6.1 Health Check

```powershell
# FAP-65
$healthUrl = "https://sbwe1-isp-dv-fap-65.azurewebsites.net/api/healthcheck"
Invoke-RestMethod -Uri $healthUrl

# FAP-57
$healthUrl = "https://sbwe1-isp-dv-fap-57.azurewebsites.net/api/healthcheck"
Invoke-RestMethod -Uri $healthUrl
```

### 6.2 Test manuel FAP-65 (Timer Trigger)

```powershell
# Trigger manuellement la function
$functionKey = az functionapp keys list `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65 `
  --query masterKey -o tsv

$triggerUrl = "https://sbwe1-isp-dv-fap-65.azurewebsites.net/admin/functions/RetrivePOVendor"

Invoke-RestMethod -Uri $triggerUrl `
  -Method Post `
  -Headers @{ "x-functions-key" = $functionKey } `
  -Body '{"input":""}' `
  -ContentType "application/json"
```

### 6.3 Vérifier les logs

```powershell
# Stream logs FAP-65
func azure functionapp logstream SBWE1-ISP-DV-FAP-65

# Ou via Azure CLI
az webapp log tail `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65
```

### 6.4 Vérifier Application Insights

```kql
// KQL Query dans Application Insights SBWE1-ISP-DV-API-01

// Dernières exécutions
customEvents
| where name == "NADIA_Execution_Completed"
| project timestamp, customDimensions.CorrelationId, customDimensions.POProcessed
| order by timestamp desc
| take 5

// Erreurs
exceptions
| where cloud_RoleName in ("SBWE1-ISP-DV-FAP-65", "SBWE1-ISP-DV-FAP-57")
| order by timestamp desc
| take 10
```

### 6.5 Vérifier Service Bus

```powershell
# Messages dans le topic
az servicebus topic show `
  --resource-group IntegrationServicesDEV-CMN-RG `
  --namespace-name sbwe1-isp-dv-sbn-02 `
  --name purchase-orders `
  --query "countDetails"

# Dead Letter Queue
az servicebus topic subscription show `
  --resource-group IntegrationServicesDEV-CMN-RG `
  --namespace-name sbwe1-isp-dv-sbn-02 `
  --topic-name purchase-orders `
  --name spa-processor `
  --query "countDetails.deadLetterMessageCount"
```

**Output attendu**: `deadLetterMessageCount: 0`

### 6.6 Vérifier Dataverse

Query Dataverse API:
```powershell
$accessToken = az account get-access-token --resource "https://sbmsupplierportaltest.crm4.dynamics.com" --query accessToken -o tsv

$headers = @{
    "Authorization" = "Bearer $accessToken"
    "OData-MaxVersion" = "4.0"
    "OData-Version" = "4.0"
    "Accept" = "application/json"
}

$uri = "https://sbmsupplierportaltest.crm4.dynamics.com/api/data/v9.2/sbm_stagedpurchaseorders?`$top=5&`$orderby=createdon desc"

Invoke-RestMethod -Uri $uri -Headers $headers -Method Get
```

---

## 7. Rollback

### 7.1 Rollback Function App

```powershell
# Lister les déploiements
az functionapp deployment list `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65

# Rollback vers un deployment précédent
$deploymentId = "PREVIOUS_DEPLOYMENT_ID"

az functionapp deployment source show `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65 `
  --deployment-id $deploymentId
```

### 7.2 Rollback Terraform

```powershell
cd terraform

# Récupérer le state précédent
terraform state pull > state-backup.json

# Plan avec ancienne version
terraform plan -var-file="environments/dev.tfvars" -target="module.function_app_65"

# Apply rollback
terraform apply -auto-approve
```

### 7.3 Désactiver les Function Apps

En cas d'urgence:

```powershell
# Stop FAP-65
az functionapp stop `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-65

# Stop FAP-57
az functionapp stop `
  --resource-group IntegrationServicesDEV-NDA-RG `
  --name SBWE1-ISP-DV-FAP-57
```

---

## Checklist finale

**Avant Go-Live PRD**:

- [ ] Tests unitaires passent (85%+ coverage)
- [ ] Tests d'intégration passent (DEV + STG)
- [ ] Secrets stockés dans Key Vault PRD
- [ ] Managed Identities configurées
- [ ] Role Assignments validés
- [ ] Service Bus Topic + Subscription créés
- [ ] Table Storage initialisée
- [ ] Application Insights configuré
- [ ] Alerts configurées (Critical + High)
- [ ] Runbook disponible pour support
- [ ] Approbation CAB reçue
- [ ] Documentation à jour

---

**Contact Support**: support@middleway.com  
**Escalation**: tech-lead@sbm.com

© 2026 SBM Offshore - Internal Use Only
