---
applyTo: "**/*.bicep,**/Deployment/**"
type: knowledge
---

# Knowledge: Bicep Templates

## 📋 Vue d'ensemble

Templates et exemples de code Bicep pour Azure.

## 📁 Structure Standard

```
Deployment/
├── Bicep/
│   ├── main.bicep              # Orchestrateur principal
│   ├── parameters/
│   │   ├── dev.bicepparam
│   │   ├── stg.bicepparam
│   │   └── prd.bicepparam
│   └── modules/
│       ├── storage.bicep
│       ├── function-app.bicep
│       ├── key-vault.bicep
│       └── networking.bicep
```

## 💻 Templates

### main.bicep

```bicep
targetScope = 'subscription'

// Parameters
@description('Environment name')
@allowed(['dev', 'stg', 'prd'])
param env string

@description('Project/workload name')
param project string

@description('Azure region')
param location string = 'westeurope'

@description('Resource owner email')
param owner string

// Variables
var locationAbbrev = {
  westeurope: 'weu'
  northeurope: 'neu'
  eastus: 'eus'
}
var loc = locationAbbrev[location]
var namePrefix = '${project}-${env}'

var commonTags = {
  Environment: env
  Project: project
  Owner: owner
  ManagedBy: 'Bicep'
}

// Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-${namePrefix}-${loc}'
  location: location
  tags: commonTags
}

// Deploy modules
module storage 'modules/storage.bicep' = {
  scope: rg
  name: 'storage-deployment'
  params: {
    namePrefix: namePrefix
    location: location
    tags: commonTags
  }
}

module keyVault 'modules/key-vault.bicep' = {
  scope: rg
  name: 'keyvault-deployment'
  params: {
    namePrefix: namePrefix
    location: location
    env: env
    tags: commonTags
  }
}

module functionApp 'modules/function-app.bicep' = {
  scope: rg
  name: 'functionapp-deployment'
  params: {
    namePrefix: namePrefix
    location: location
    storageAccountName: storage.outputs.storageAccountName
    keyVaultUri: keyVault.outputs.vaultUri
    tags: commonTags
  }
}

// Grant Function App access to Key Vault
module kvAccess 'modules/key-vault-access.bicep' = {
  scope: rg
  name: 'keyvault-access'
  params: {
    keyVaultName: keyVault.outputs.keyVaultName
    principalId: functionApp.outputs.identityPrincipalId
  }
}

// Outputs
output resourceGroupName string = rg.name
output functionAppName string = functionApp.outputs.functionAppName
output storageAccountName string = storage.outputs.storageAccountName
```

### modules/storage.bicep

```bicep
@description('Name prefix for resources')
param namePrefix string

@description('Azure region')
param location string

@description('Enable hierarchical namespace (ADLS Gen2)')
param enableHns bool = true

@description('Containers to create')
param containers array = ['bronze', 'silver', 'gold']

@description('Tags')
param tags object = {}

// Variables
var storageAccountName = replace('st${namePrefix}', '-', '')

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    isHnsEnabled: enableHns
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

// Blob Service
resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: true
      days: 7
    }
    containerDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

// Containers
resource storageContainers 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = [for container in containers: {
  parent: blobService
  name: container
  properties: {
    publicAccess: 'None'
  }
}]

// Outputs
output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
output primaryBlobEndpoint string = storageAccount.properties.primaryEndpoints.blob
output primaryDfsEndpoint string = storageAccount.properties.primaryEndpoints.dfs
```

### modules/function-app.bicep

```bicep
@description('Name prefix')
param namePrefix string

@description('Azure region')
param location string

@description('Storage account name for Function App')
param storageAccountName string

@description('Key Vault URI')
param keyVaultUri string

@description('Application Insights connection string')
param appInsightsConnectionString string = ''

@description('SKU for App Service Plan')
@allowed(['Y1', 'EP1', 'EP2', 'EP3'])
param skuName string = 'Y1'

@description('Tags')
param tags object = {}

// Variables
var functionAppName = 'func-${namePrefix}'
var appServicePlanName = 'asp-${namePrefix}'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  properties: {
    reserved: true // Linux
  }
}

// Function App
resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccountName), '2023-01-01').keys[0].value}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'KeyVaultUri'
          value: keyVaultUri
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
      ]
    }
  }
}

// Outputs
output functionAppId string = functionApp.id
output functionAppName string = functionApp.name
output identityPrincipalId string = functionApp.identity.principalId
output defaultHostName string = functionApp.properties.defaultHostName
```

### modules/key-vault.bicep

```bicep
@description('Name prefix')
param namePrefix string

@description('Azure region')
param location string

@description('Environment')
@allowed(['dev', 'stg', 'prd'])
param env string

@description('Tags')
param tags object = {}

// Variables
var keyVaultName = 'kv-${namePrefix}'

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: env == 'prd'
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

// Outputs
output keyVaultId string = keyVault.id
output keyVaultName string = keyVault.name
output vaultUri string = keyVault.properties.vaultUri
```

### modules/key-vault-access.bicep

```bicep
@description('Key Vault name')
param keyVaultName string

@description('Principal ID to grant access')
param principalId string

@description('Role to assign')
@allowed(['Key Vault Secrets User', 'Key Vault Secrets Officer', 'Key Vault Administrator'])
param roleName string = 'Key Vault Secrets User'

// Role definition IDs
var roleDefinitions = {
  'Key Vault Secrets User': '4633458b-17de-408a-b874-0445c86b69e6'
  'Key Vault Secrets Officer': 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  'Key Vault Administrator': '00482a5a-887f-4fb3-b363-3b7fe8e74483'
}

// Existing Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

// Role Assignment
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, principalId, roleDefinitions[roleName])
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleDefinitions[roleName])
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}
```

### parameters/dev.bicepparam

```bicep
using '../main.bicep'

param env = 'dev'
param project = 'isp'
param location = 'westeurope'
param owner = 'data-team@company.com'
```

## 🔄 Déploiement

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
  --parameters parameters/dev.bicepparam \
  --name "deployment-$(date +%Y%m%d-%H%M%S)"
```

## 📚 Références

- [Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [Bicep Examples](https://github.com/Azure/bicep/tree/main/docs/examples)
