---
applyTo: "**/Deployment/**,**/*.tf"
type: knowledge
---

# Knowledge: Terraform Patterns & Templates

## 📋 Vue d'ensemble

Templates et exemples de code Terraform pour Azure.

## 📁 Structure Standard

```
terraform/
├── main.tf              # Configuration principale
├── variables.tf         # Variables d'entrée
├── outputs.tf           # Outputs
├── locals.tf            # Variables locales
├── providers.tf         # Configuration providers
├── terraform.tfvars     # Valeurs (gitignore!)
├── terraform.tfvars.example
├── modules/
│   ├── storage/
│   ├── compute/
│   └── networking/
└── environments/
    ├── dev/
    ├── stg/
    └── prd/
```

## 💻 Templates

### providers.tf

```hcl
terraform {
  required_version = ">= 1.5.0"
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.80"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.45"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "rg-tfstate-prd"
    storage_account_name = "sttfstateprd"
    container_name       = "tfstate"
    key                  = "project/terraform.tfstate"
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy    = false
      recover_soft_deleted_key_vaults = true
    }
  }
}
```

### variables.tf

```hcl
# Required variables
variable "project" {
  type        = string
  description = "Project name (e.g., 'nadia', 'isp')"
  
  validation {
    condition     = can(regex("^[a-z0-9-]+$", var.project))
    error_message = "Project name must be lowercase alphanumeric with hyphens."
  }
}

variable "environment" {
  type        = string
  description = "Environment (dev/stg/prd)"
  
  validation {
    condition     = contains(["dev", "stg", "prd"], var.environment)
    error_message = "Environment must be dev, stg, or prd."
  }
}

variable "location" {
  type        = string
  description = "Azure region"
  default     = "westeurope"
}

# Optional variables
variable "tags" {
  type        = map(string)
  description = "Additional tags"
  default     = {}
}

# Sensitive variables (no default!)
variable "sql_admin_password" {
  type        = string
  description = "SQL Server admin password"
  sensitive   = true
}
```

### locals.tf

```hcl
locals {
  # Naming convention
  name_prefix = "${var.project}-${var.environment}"
  
  # Location abbreviation
  location_abbrev = {
    westeurope  = "weu"
    northeurope = "neu"
    eastus      = "eus"
  }
  loc = local.location_abbrev[var.location]
  
  # Standard tags
  common_tags = merge(var.tags, {
    Environment = var.environment
    Project     = var.project
    ManagedBy   = "Terraform"
    CreatedDate = timestamp()
  })
  
  # Resource names
  resource_names = {
    resource_group  = "rg-${local.name_prefix}-${local.loc}"
    storage_account = "st${replace(local.name_prefix, "-", "")}${local.loc}"
    key_vault       = "kv-${local.name_prefix}-${local.loc}"
    function_app    = "func-${local.name_prefix}-${local.loc}"
    service_bus     = "sb-${local.name_prefix}-${local.loc}"
  }
}
```

### Module: Storage Account

```hcl
# modules/storage/main.tf
resource "azurerm_storage_account" "main" {
  name                     = var.name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = var.account_tier
  account_replication_type = var.replication_type
  account_kind             = "StorageV2"
  
  # Security
  min_tls_version                 = "TLS1_2"
  https_traffic_only_enabled      = true
  allow_nested_items_to_be_public = false
  shared_access_key_enabled       = var.enable_shared_key
  
  # Hierarchical namespace for ADLS Gen2
  is_hns_enabled = var.enable_hns
  
  # Network rules
  network_rules {
    default_action             = "Deny"
    bypass                     = ["AzureServices"]
    virtual_network_subnet_ids = var.allowed_subnet_ids
    ip_rules                   = var.allowed_ip_ranges
  }
  
  # Blob properties
  blob_properties {
    delete_retention_policy {
      days = 7
    }
    container_delete_retention_policy {
      days = 7
    }
    versioning_enabled = var.enable_versioning
  }

  tags = var.tags
}

# Containers
resource "azurerm_storage_container" "containers" {
  for_each = toset(var.containers)
  
  name                  = each.value
  storage_account_name  = azurerm_storage_account.main.name
  container_access_type = "private"
}

# modules/storage/variables.tf
variable "name" {
  type        = string
  description = "Storage account name"
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "account_tier" {
  type    = string
  default = "Standard"
}

variable "replication_type" {
  type    = string
  default = "LRS"
}

variable "enable_hns" {
  type    = bool
  default = false
}

variable "enable_shared_key" {
  type    = bool
  default = false
}

variable "enable_versioning" {
  type    = bool
  default = true
}

variable "containers" {
  type    = list(string)
  default = []
}

variable "allowed_subnet_ids" {
  type    = list(string)
  default = []
}

variable "allowed_ip_ranges" {
  type    = list(string)
  default = []
}

variable "tags" {
  type    = map(string)
  default = {}
}

# modules/storage/outputs.tf
output "id" {
  value = azurerm_storage_account.main.id
}

output "name" {
  value = azurerm_storage_account.main.name
}

output "primary_blob_endpoint" {
  value = azurerm_storage_account.main.primary_blob_endpoint
}

output "primary_dfs_endpoint" {
  value = azurerm_storage_account.main.primary_dfs_endpoint
}
```

### Module: Function App

```hcl
# modules/function-app/main.tf
resource "azurerm_service_plan" "main" {
  name                = "asp-${var.name}"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = var.sku_name

  tags = var.tags
}

resource "azurerm_linux_function_app" "main" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.main.id
  
  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_key
  
  # Managed Identity
  identity {
    type = "SystemAssigned"
  }
  
  site_config {
    always_on = var.sku_name != "Y1" # Not supported on Consumption
    
    application_stack {
      dotnet_version              = "8.0"
      use_dotnet_isolated_runtime = true
    }
    
    application_insights_connection_string = var.app_insights_connection_string
  }
  
  app_settings = merge(var.app_settings, {
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet-isolated"
    "KeyVaultUri"              = var.key_vault_uri
  })

  tags = var.tags
}
```

### Module: Key Vault

```hcl
# modules/key-vault/main.tf
resource "azurerm_key_vault" "main" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  
  # Security
  enabled_for_deployment          = false
  enabled_for_disk_encryption     = false
  enabled_for_template_deployment = false
  enable_rbac_authorization       = true
  purge_protection_enabled        = var.environment == "prd"
  soft_delete_retention_days      = 90
  
  network_acls {
    default_action             = "Deny"
    bypass                     = "AzureServices"
    virtual_network_subnet_ids = var.allowed_subnet_ids
  }

  tags = var.tags
}

# RBAC: Key Vault Secrets User for Function App
resource "azurerm_role_assignment" "function_app_secrets" {
  count = var.function_app_principal_id != null ? 1 : 0
  
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = var.function_app_principal_id
}

data "azurerm_client_config" "current" {}
```

## 🔄 Usage Pattern

### main.tf (root)

```hcl
# Resource Group
resource "azurerm_resource_group" "main" {
  name     = local.resource_names.resource_group
  location = var.location
  tags     = local.common_tags
}

# Storage Account
module "storage" {
  source = "./modules/storage"
  
  name                = local.resource_names.storage_account
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  enable_hns          = true
  containers          = ["bronze", "silver", "gold"]
  tags                = local.common_tags
}

# Key Vault
module "key_vault" {
  source = "./modules/key-vault"
  
  name                       = local.resource_names.key_vault
  resource_group_name        = azurerm_resource_group.main.name
  location                   = var.location
  environment                = var.environment
  function_app_principal_id  = module.function_app.identity_principal_id
  tags                       = local.common_tags
}

# Function App
module "function_app" {
  source = "./modules/function-app"
  
  name                             = local.resource_names.function_app
  resource_group_name              = azurerm_resource_group.main.name
  location                         = var.location
  storage_account_name             = module.storage.name
  storage_account_key              = module.storage.primary_access_key
  key_vault_uri                    = module.key_vault.vault_uri
  app_insights_connection_string   = module.app_insights.connection_string
  tags                             = local.common_tags
}
```

## 📚 Références

- [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Azure CAF Terraform Modules](https://github.com/Azure/terraform-azurerm-caf)
