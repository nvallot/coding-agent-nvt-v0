---
applyTo: "**/*"
---

# Conventions Nommage {CLIENT_NAME}

## Variables & Ressources Azure

Toutes les ressources Azure suivent ce pattern:

```
{prefix}-{resource-type}-{environment}
```

### Préfixes par Client

| Client | Prefix | Exemple |
|--------|--------|---------|
| NADIA | nadia | `nadia-adf-dev` |
| Supplier Portal | spa | `spa-sql-prod` |
| Internal | sbm | `sbm-kv-stg` |

### Types de Ressources

| Type | Code | Exemple |
|------|------|---------|
| Storage Account | adls | `nadia-adls-dev` |
| SQL Database | sql | `nadia-sql-prod` |
| Key Vault | kv | `nadia-kv-dev` |
| Data Factory | adf | `nadia-adf-dev` |
| App Insights | appi | `nadia-appi-prod` |
| Function App | func | `nadia-func-dev` |

### Tags Obligatoires

**Tous les tags doivent être appliqués sur chaque ressource**:

```hcl
tags = {
  Environment  = var.environment        # dev/stg/prod
  Project      = var.project            # nadia / spa / sbm
  Owner        = var.owner              # Team name
  CostCenter   = var.cost_center        # CC code
  BackupPolicy = "daily"                # daily/weekly/none
  Compliance   = "GDPR"                 # ou SOC2, PCI-DSS
  ManagedBy    = "Terraform"
}
```

## Code Conventions

### Python

**Module naming**: `snake_case`
```
✅ transform_orders.py
❌ TransformOrders.py
```

**Functions**: `snake_case`
```python
def transform_orders(df):
    return df
```

**Constants**: `UPPER_SNAKE_CASE`
```python
MAX_RETRIES = 3
DEFAULT_BATCH_SIZE = 1000
```

### C#

**Namespace**: `PascalCase.PascalCase`
```csharp
namespace Company.Nadia.Functions
```

**Classes**: `PascalCase`
```csharp
public class OrderProcessor
```

**Methods**: `PascalCase`
```csharp
public async Task ProcessOrderAsync()
```

**Constants**: `PascalCase`
```csharp
private const int DefaultBatchSize = 1000;
```

### SQL

**Tables**: `snake_case` (lowercase)
```sql
CREATE TABLE bronze.orders (
    order_id INT,
    order_date DATE
);
```

**Stored Procedures**: `usp_{name}`
```sql
CREATE PROCEDURE usp_ProcessOrders
```

**Functions**: `uf_{name}`
```sql
CREATE FUNCTION uf_FormatDate
```

## Databricks Notebooks

**Naming**: `{layer}_{domain}_{process}`

Examples:
```
bronze_ingest_orders.py
silver_transform_orders.py
gold_aggregate_orders.py
```

## Fichiers & Dossiers

**Pattern**: `kebab-case`

```
✅ data-models/
✅ transform-orders.py
❌ DataModels/
❌ TransformOrders.py
```

---

**À ajouter dans**: `.github/clients/{clientKey}/instructions/naming.md`

**Chaque client a ses propres conventions** spécifiées ici, **jamais dans les fichiers common**.
