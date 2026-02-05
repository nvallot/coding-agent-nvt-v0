---
applyTo: "**/Deployment/**,**/src/**"
type: knowledge
---

# Knowledge: Azure Key Vault

## 📋 Vue d'ensemble

**Azure Key Vault** est le service managé pour stocker et accéder de manière sécurisée aux secrets, clés et certificats.

## 🎯 Use Cases

- Stockage de secrets (connection strings, API keys)
- Gestion de clés de chiffrement (CMK)
- Certificats SSL/TLS
- Rotation automatique des secrets

## 🏗️ Concepts

### Types d'Objets

| Type | Usage | Exemple |
|------|-------|---------|
| **Secrets** | Valeurs sensibles | Connection strings, passwords |
| **Keys** | Clés cryptographiques | Encryption keys, signing keys |
| **Certificates** | Certificats X.509 | SSL/TLS, code signing |

### Naming Convention

```
kv-{project}-{env}-{region}

Exemples:
- kv-isp-dev-weu
- kv-isp-prd-weu
```

Noms de secrets:
```
{service}--{setting}

Exemples:
- ServiceBus--ConnectionString
- LucyApi--ApiKey
- SqlServer--AdminPassword
```

## 💻 Exemples

### Accès avec Managed Identity

```csharp
// Azure Function / App Service
var secretClient = new SecretClient(
    new Uri("https://kv-isp-prd-weu.vault.azure.net/"),
    new DefaultAzureCredential()
);

KeyVaultSecret secret = await secretClient.GetSecretAsync("LucyApi--ApiKey");
string apiKey = secret.Value;
```

### Configuration .NET avec Key Vault

```csharp
// Program.cs
var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVaultUri"]!),
    new DefaultAzureCredential()
);

// Les secrets sont maintenant accessibles via IConfiguration
// Secret "ServiceBus--ConnectionString" → Configuration["ServiceBus:ConnectionString"]
```

### Key Vault Reference (App Settings)

```json
// Azure Function App Settings
{
  "ServiceBusConnection": "@Microsoft.KeyVault(SecretUri=https://kv-isp-prd-weu.vault.azure.net/secrets/ServiceBus--ConnectionString/)"
}
```

Format court:
```json
{
  "ServiceBusConnection": "@Microsoft.KeyVault(VaultName=kv-isp-prd-weu;SecretName=ServiceBus--ConnectionString)"
}
```

### Terraform - Key Vault

```hcl
resource "azurerm_key_vault" "main" {
  name                = "kv-${var.project}-${var.env}-${var.region}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  
  # Recommandé pour production
  purge_protection_enabled   = true
  soft_delete_retention_days = 90
  
  # Network isolation
  network_acls {
    default_action = "Deny"
    bypass         = "AzureServices"
  }

  tags = var.tags
}

# Secret
resource "azurerm_key_vault_secret" "service_bus" {
  name         = "ServiceBus--ConnectionString"
  value        = azurerm_servicebus_namespace.main.default_primary_connection_string
  key_vault_id = azurerm_key_vault.main.id
}

# Access Policy pour Function App
resource "azurerm_key_vault_access_policy" "function_app" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_function_app.main.identity[0].principal_id

  secret_permissions = ["Get", "List"]
}
```

### Bicep - Key Vault

```bicep
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: 'kv-${project}-${env}-${location}'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
  tags: tags
}

// RBAC plutôt que Access Policies (recommandé)
resource secretUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, functionApp.id, 'Key Vault Secrets User')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
```

## 🔐 Access Control

### Access Policies (Legacy)

| Permission | Usage |
|------------|-------|
| Get | Lire un secret |
| List | Lister les secrets |
| Set | Créer/mettre à jour |
| Delete | Supprimer |
| Purge | Suppression définitive |

### RBAC (Recommandé)

| Role | Permissions |
|------|-------------|
| Key Vault Secrets User | Get, List secrets |
| Key Vault Secrets Officer | Full control secrets |
| Key Vault Administrator | Full control vault |
| Key Vault Crypto User | Crypto operations |

## ✅ Bonnes Pratiques

### Sécurité

- **Toujours** Managed Identity pour accès depuis Azure
- RBAC > Access Policies pour nouveaux déploiements
- Activer soft-delete et purge protection en production
- Network isolation avec Private Endpoints
- Auditer les accès via Diagnostic Settings

### Organisation

- Un Key Vault par environnement
- Naming convention cohérente pour secrets
- Documenter les secrets dans ADRs
- Tags pour identification

### Rotation

- Configurer expiration sur secrets
- Alerter avant expiration
- Automatiser rotation si possible
- Ne jamais hardcoder les secrets

## 💰 Coûts

| Opération | Prix |
|-----------|------|
| Secrets operations | ~$0.03/10K transactions |
| Key operations | ~$0.03/10K transactions |
| Certificate renewals | ~$3/renewal |
| HSM keys | ~$1-5/key/mois |

Le coût est généralement négligeable comparé aux autres services.

## 📚 Références

- [Key Vault Documentation](https://learn.microsoft.com/azure/key-vault/)
- [Key Vault References](https://learn.microsoft.com/azure/app-service/app-service-key-vault-references)
- [RBAC for Key Vault](https://learn.microsoft.com/azure/key-vault/general/rbac-guide)
