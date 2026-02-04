---
applyTo: "**/Deployment/**,**/*.tf"
excludeAgent: "code-review"
---

# 🏗️ Infrastructure as Code (Terraform)

## Standard Structure
```
terraform/
├── main.tf              # Main configuration
├── variables.tf         # Input variables
├── outputs.tf           # Outputs
├── locals.tf            # Local variables
├── providers.tf         # Provider config
├── terraform.tfvars     # Values (gitignore)
├── terraform.tfvars.example  # Template
├── modules/
│   ├── storage/
│   ├── compute/
│   └── networking/
├── environments/        # If per-environment
│   ├── dev/
│   ├── staging/
│   └── prod/
└── README.md           # Setup instructions
```

## Variables Best Practices

**Required (no defaults)**:
```hcl
variable "project" {
  type = string
  description = "Project name (e.g., 'nadia')"
}

variable "environment" {
  type = string
  description = "Environment (dev/stg/prod)"
  validation {
    condition = contains(["dev", "stg", "prod"], var.environment)
  }
}
```

**Optional (with defaults)**:
```hcl
variable "location" {
  type = string
  description = "Azure region"
  default = "eastus"
}
```

**Never default for secrets**:
```hcl
variable "sql_password" {
  type = string
  description = "SQL admin password"
  sensitive = true  # Hide in logs
  # NO default!
}
```

## Naming Convention (Azure CAF)

Format: `{prefix}-{resource-type}-{environment}`

Examples:
```
adls-datalake-dev        (Data Lake Storage)
sql-analytics-prod       (SQL Database)
kv-nadia-dev            (Key Vault)
adf-ingestion-stg       (Data Factory)
appi-monitoring-prod    (App Insights)
```

## Tags on All Resources

```hcl
tags = {
  Environment      = var.environment
  Project          = var.project
  Owner            = var.owner
  CostCenter       = var.cost_center
  ManagedBy        = "Terraform"
  CreatedDate      = "2026-02-04"
  BackupRequired   = "true"
  Compliance       = "GDPR"
}
```

## State Management

**Remote Backend** (mandatory for prod):
```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-tfstate-prod"
    storage_account_name = "tfstateprod"
    container_name       = "tfstate"
    key                  = "nadia/terraform.tfstate"
  }
}
```

**Local** (dev only):
```bash
# Never commit tfstate to git!
echo "*.tfstate" >> .gitignore
echo "*.tfstate.*" >> .gitignore
```

## Module Pattern

```hcl
# modules/storage/main.tf
resource "azurerm_storage_account" "adls" {
  name                     = lower("adls${var.project}${var.environment}")
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  https_traffic_only_enabled = true
  
  network_rules {
    default_action = "Deny"
    bypass         = ["AzureServices"]
  }
  
  tags = var.tags
}

# modules/storage/variables.tf
variable "project" { type = string }
variable "environment" { type = string }
variable "location" { type = string }
variable "resource_group_name" { type = string }
variable "tags" { type = map(string) }

# modules/storage/outputs.tf
output "storage_account_id" {
  value = azurerm_storage_account.adls.id
}
```

## Deployment Commands

```bash
# Validate
terraform validate

# Format
terraform fmt -recursive

# Plan (always review!)
terraform plan -out=tfplan

# Apply
terraform apply tfplan

# Destroy (dev only)
terraform destroy

# Migrate state (careful!)
terraform state mv <old> <new>
```

## Secrets Management

**Never in terraform.tfvars**:
```hcl
# ❌ WRONG
sql_password = "MyPassword123"

# ✅ RIGHT
sql_password = var.sql_admin_password

# Use Azure KeyVault or environment variables
export TF_VAR_sql_admin_password=$(az keyvault secret show -n sql-admin-pwd -v myVault -q value)
```

## Security Checklist

✅ Private Endpoints for all PaaS services
✅ Network isolation (VNet, NSG)
✅ Managed Identity for authentication
✅ Secrets in Key Vault (never hardcoded)
✅ Encryption at-rest enabled
✅ Audit logging enabled
✅ RBAC configured (no Owner assignments)
✅ Storage firewall enabled (default Deny)
