---
applyTo: "**/*.bicep,**/Deployment/**/*.json"
excludeAgent: ["code-review"]
---

# Bicep & ARM Templates - Standards

## Quand utiliser quoi ?

| Critère | Bicep | Terraform | ARM JSON |
|---------|-------|-----------|----------|
| Équipe Azure-only | ✅ Recommandé | ⚪ OK | ❌ Éviter |
| Multi-cloud | ❌ | ✅ Recommandé | ❌ |
| Existant ARM JSON | ✅ Migration facile | ⚪ Réécriture | ⚪ Maintenir |
| CI/CD Azure DevOps | ✅ Natif | ⚪ Task externe | ✅ Natif |
| State management | ✅ Azure natif | ⚠️ Backend requis | ✅ Azure natif |

## MUST (Bloquant)

### Naming Azure CAF
```bicep
// Pattern: {resource-type}-{workload}-{environment}-{region}-{instance}
var naming = {
  resourceGroup: 'rg-${workload}-${env}-${location}'
  storageAccount: 'st${workload}${env}${uniqueString(resourceGroup().id)}'
  functionApp: 'func-${workload}-${env}-${location}'
  keyVault: 'kv-${workload}-${env}-${location}'
}
```

### Tags Obligatoires
```bicep
var commonTags = {
  Environment: env
  Project: workload
  Owner: owner
  ManagedBy: 'Bicep'
  CostCenter: costCenter
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  tags: commonTags
  // ...
}
```

### Sécurité
- Jamais de secrets dans les fichiers → Key Vault references
- Utiliser Managed Identity (pas de connection strings)
- HTTPS only pour tous les endpoints
- Désactiver accès public si non nécessaire

## SHOULD (Fortement recommandé)

### Structure Projet
```
Deployment/
├── Bicep/
│   ├── main.bicep              # Orchestrateur principal
│   ├── parameters/
│   │   ├── dev.bicepparam
│   │   ├── uat.bicepparam
│   │   └── prd.bicepparam
│   └── modules/
│       ├── storage.bicep
│       ├── functions.bicep
│       └── networking.bicep
```

### Module Pattern
```bicep
// modules/storage.bicep
@description('Storage account name')
param name string

@description('Location for resources')
param location string = resourceGroup().location

@allowed(['Standard_LRS', 'Standard_GRS'])
param sku string = 'Standard_LRS'

param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name
  location: location
  tags: tags
  sku: { name: sku }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
  }
}

@description('Storage account resource ID')
output id string = storageAccount.id

@description('Storage account primary endpoint')
output primaryEndpoint string = storageAccount.properties.primaryEndpoints.blob
```

### Main Orchestrator
```bicep
// main.bicep
targetScope = 'subscription'

@description('Environment name')
@allowed(['dev', 'uat', 'prd'])
param env string

@description('Workload name')
param workload string

@description('Azure region')
param location string = 'westeurope'

// Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-${workload}-${env}-${location}'
  location: location
  tags: {
    Environment: env
    Project: workload
  }
}

// Deploy modules
module storage 'modules/storage.bicep' = {
  scope: rg
  name: 'storage-deployment'
  params: {
    name: 'st${workload}${env}${uniqueString(rg.id)}'
    location: location
    tags: rg.tags
  }
}
```

### Parameters File (.bicepparam)
```bicep
using './main.bicep'

param env = 'dev'
param workload = 'nadia'
param location = 'westeurope'
```

## Déploiement

### Azure CLI
```bash
# What-if (preview changes)
az deployment sub what-if \
  --location westeurope \
  --template-file main.bicep \
  --parameters parameters/dev.bicepparam

# Deploy
az deployment sub create \
  --location westeurope \
  --template-file main.bicep \
  --parameters parameters/dev.bicepparam
```

### Azure DevOps Pipeline
```yaml
- task: AzureCLI@2
  displayName: 'Deploy Bicep'
  inputs:
    azureSubscription: '$(serviceConnection)'
    scriptType: 'bash'
    scriptLocation: 'inlineScript'
    inlineScript: |
      az deployment sub create \
        --location $(location) \
        --template-file Deployment/Bicep/main.bicep \
        --parameters Deployment/Bicep/parameters/$(env).bicepparam
```

## Migration ARM → Bicep

```bash
# Décompiler ARM JSON vers Bicep
az bicep decompile --file template.json

# Valider syntaxe
az bicep build --file main.bicep
```
