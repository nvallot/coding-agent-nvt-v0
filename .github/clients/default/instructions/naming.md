# Naming Conventions - Default

## 🏷️ Azure Resources

### Format Général

```
{resource-abbreviation}-{project}-{environment}-{region}-{instance}
```

### Abréviations Azure CAF

| Ressource | Abréviation | Exemple |
|-----------|-------------|---------|
| Resource Group | rg | rg-dataplatform-dev-weu-001 |
| Storage Account | st | stdataplatformdevweu001 |
| Data Factory | adf | adf-dataplatform-dev-weu-001 |
| Databricks | dbw | dbw-dataplatform-dev-weu-001 |
| Synapse | syn | syn-dataplatform-dev-weu-001 |
| Key Vault | kv | kv-dataplatform-dev |
| SQL Database | sqldb | sqldb-dataplatform-dev |
| Event Hub | evh | evh-dataplatform-dev |
| Function App | func | func-dataplatform-dev |
| Log Analytics | log | log-dataplatform-dev |
| Application Insights | appi | appi-dataplatform-dev |

### Environments

- `dev` - Development
- `stg` - Staging / Pre-production
- `prd` - Production

### Regions

- `weu` - West Europe
- `neu` - North Europe
- `eus` - East US
- `wus` - West US

## 📁 Data Lake Structure

```
container-name/
├── bronze/              # Raw data
│   ├── {source}/
│   │   ├── {entity}/
│   │   │   └── {year}/{month}/{day}/
│   │   │       └── data.parquet
├── silver/              # Cleaned data
│   ├── {domain}/
│   │   ├── {entity}/
│   │   │   └── {year}/{month}/
│   │   │       └── data.parquet
└── gold/                # Curated data
    ├── {domain}/
    │   ├── {entity}/
    │   │   └── data.parquet
```

### Exemples

```
bronze/erp/customers/2026/02/03/customers_20260203.parquet
silver/sales/customers/2026/02/customers.parquet
gold/analytics/customer_360/customer_360.parquet
```

## 💻 Code Naming

### Python

```python
# Variables et fonctions: snake_case
customer_name = "John Doe"
total_amount = 100.50

def calculate_total_price(items: list) -> float:
    """Calculate total price of items."""
    return sum(item.price for item in items)

# Classes: PascalCase
class CustomerService:
    def __init__(self, repository: CustomerRepository):
        self.repository = repository
    
    def get_customer_by_id(self, customer_id: str) -> Customer:
        return self.repository.find_by_id(customer_id)

# Constants: UPPER_SNAKE_CASE
MAX_RETRY_ATTEMPTS = 3
DEFAULT_TIMEOUT = 30
```

### SQL

```sql
-- Tables: snake_case
CREATE TABLE customer_orders (
    order_id INT PRIMARY KEY,
    customer_id INT,
    order_date DATE,
    total_amount DECIMAL(10,2)
);

-- Views: snake_case with 'vw_' prefix
CREATE VIEW vw_customer_summary AS
SELECT customer_id, COUNT(*) as order_count
FROM customer_orders
GROUP BY customer_id;

-- Stored Procedures: snake_case with 'sp_' prefix
CREATE PROCEDURE sp_get_customer_orders(
    @customer_id INT
)
AS
BEGIN
    SELECT * FROM customer_orders
    WHERE customer_id = @customer_id;
END;

-- Keywords: UPPERCASE
SELECT customer_id, order_date, total_amount
FROM customer_orders
WHERE order_date >= '2026-01-01'
ORDER BY order_date DESC;
```

### Terraform

```hcl
# Resources: snake_case
resource "azurerm_resource_group" "data_platform" {
  name     = "rg-dataplatform-dev-weu-001"
  location = var.location
  tags     = local.common_tags
}

# Variables: snake_case
variable "project_name" {
  description = "Project name"
  type        = string
}

# Outputs: snake_case
output "resource_group_id" {
  value = azurerm_resource_group.data_platform.id
}
```

### Fichiers

```
kebab-case pour tous les fichiers

customer-service.py
data-factory-pipeline.json
terraform-variables.tf
database-schema.sql
```

## 🔖 Git

### Branches

```
main                           # Production
develop                        # Intégration
feature/add-customer-api      # Nouvelles fonctionnalités
bugfix/fix-auth-error         # Corrections
hotfix/critical-security-fix  # Urgences production
```

### Commits

Format: `<type>(<scope>): <description>`

Types:
- `feat`: Nouvelle fonctionnalité
- `fix`: Correction de bug
- `docs`: Documentation
- `style`: Formatting
- `refactor`: Refactoring
- `test`: Tests
- `chore`: Maintenance

Exemples:
```
feat(api): add customer search endpoint
fix(auth): resolve managed identity token refresh
docs(readme): update installation instructions
refactor(etl): simplify transformation logic
```

## 📋 Tags Obligatoires

Toutes les ressources Azure doivent avoir ces tags:

```hcl
tags = {
  Environment  = "dev"           # dev, stg, prd
  Project      = "dataplatform"
  Owner        = "data-team"
  CostCenter   = "IT-1000"
  ManagedBy    = "Terraform"
  CreatedDate  = "2026-02-03"
  Application  = "data-integration"
}
```

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-03
