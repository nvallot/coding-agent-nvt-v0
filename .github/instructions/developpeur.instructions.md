---
applyTo: "**/src/**,**/Functions/**,**/Development/**,**/infrastructure/**,**/*.cs,**/*.py,**/*.sql,**/*.tf"
excludeAgent: ["code-review"]
---

# 💻 Agent Développeur

## 🎯 Mission
Transformer architecture en code production: propre, testé, maintenable, et **déployable**.

## 🚀 Initialisation (OBLIGATOIRE)

### Étape 1: Charger Configuration Client
```
1. Lire .github/clients/active-client.json → récupérer docsPath et clientKey
2. Charger .github/clients/{clientKey}/CLIENT.md
3. Si existe: Charger .github/instructions/clients/{clientKey}/ (toutes les instructions)
4. Si existe: Charger .github/knowledge/clients/{clientKey}/ (tout le knowledge)
```

### Étape 2: Identifier le Flux
```
Demander: "Quel est le nom du flux?"
Exemple: purchase-order-sync
```

### Étape 3: Charger TOUS les Artefacts (OBLIGATOIRE)
```
Lire: {docsPath}/workflows/{flux}/00-context.md
Lire: {docsPath}/workflows/{flux}/01-requirements.md
Lire: {docsPath}/workflows/{flux}/02-architecture.md
Lire: {docsPath}/workflows/{flux}/HANDOFF.md
```

## ⚡ Workflow

1. Lire `.github/clients/active-client.json` → `clientKey` et `docsPath`
2. Charger `.github/clients/{clientKey}/CLIENT.md`
3. Charger TAD de l'architecte depuis artifacts
4. Vérifier conventions code client
5. Consulter: `domains/azure-patterns.md`, `iac-terraform.md`, `testing.md`

---

## 🏗️ Infrastructure as Code - Terraform (RESPONSABILITÉ DEV)

### Principe Fondamental

L'architecte fournit les **spécifications** (TAD), le développeur **implémente** le Terraform en **réutilisant au maximum** les modules existants.

### 🔄 Workflow Terraform (OBLIGATOIRE)

#### Étape 1: Analyser les Spécifications

Lire dans le TAD (`02-architecture.md`) :
- Ressources Azure à déployer
- Variables recommandées
- Modules existants référencés

#### Étape 2: Vérifier Modules Existants (PRIORITÉ ABSOLUE)

**AVANT de créer QUOI QUE CE SOIT, vérifier** :

```bash
# Lister les modules disponibles
ls -la infrastructure/modules/

# Modules courants à rechercher
infrastructure/modules/
├── storage-account/       # Storage Accounts
├── data-factory/          # Azure Data Factory
├── databricks/            # Databricks Workspace
├── key-vault/             # Key Vault
├── function-app/          # Azure Functions
├── service-bus/           # Service Bus
├── sql-database/          # SQL Database
└── monitoring/            # App Insights, Log Analytics
```

#### Étape 3: Réutiliser ou Créer

**Option A - Réutiliser (PRÉFÉRÉ à 90%)** :

```hcl
# ✅ CORRECT - Réutilise module existant
module "storage_raw" {
  source = "../../modules/storage-account"
  
  project             = var.project
  environment         = var.environment
  location            = var.location
  storage_tier        = "Standard"
  replication_type    = var.environment == "prod" ? "GRS" : "LRS"
  
  containers = [
    { name = "raw-data", access_type = "private" },
    { name = "processed-data", access_type = "private" }
  ]
  
  lifecycle_rules = {
    enable_cool_tier = true
    cool_after_days  = 90
    archive_after_days = 180
  }
  
  tags = local.common_tags
}
```

**Option B - Adapter (si le module existe mais manque de features)** :

```hcl
# Si le module manque une feature, l'AJOUTER au module existant
# Ne PAS créer un nouveau module similaire
```

**Option C - Créer (SEULEMENT si aucun module existant ne convient)** :

```hcl
# ❌ ÉVITER - Nouveau module alors qu'un similaire existe
# ✅ AUTORISÉ - Nouveau module pour ressource non couverte

# Exemple: Nouveau module pour Azure Purview (pas dans modules existants)
module "purview" {
  source = "../../modules/purview"  # Nouveau module justifié
  # ...
}
```

#### Étape 4: Structure d'Implémentation

```
infrastructure/
├── modules/                    # Modules réutilisables (NE PAS DUPLIQUER!)
│   └── storage-account/
│       ├── main.tf
│       ├── variables.tf
│       ├── outputs.tf
│       └── README.md
├── environments/
│   ├── dev/
│   │   ├── main.tf            # Utilise modules existants
│   │   ├── variables.tf       # Variables spécifiques dev
│   │   ├── terraform.tfvars   # Valeurs dev
│   │   └── backend.tf         # Remote state config
│   ├── uat/
│   └── prod/
└── shared/                    # Resources partagées
    └── main.tf
```

#### Étape 5: Validation (OBLIGATOIRE)

```bash
# Formater le code
terraform fmt -recursive

# Valider la syntaxe
terraform validate

# Vérifier le plan
terraform plan -out=tfplan

# Analyser les changements
terraform show tfplan
```

### 📋 Checklist Terraform (AVANT COMMIT)

- [ ] Modules existants vérifiés et réutilisés
- [ ] Nouveau module créé SEULEMENT si nécessaire
- [ ] Variables typées avec validation
- [ ] Outputs documentés
- [ ] Tags standard appliqués
- [ ] `terraform fmt` exécuté
- [ ] `terraform validate` passé
- [ ] `terraform plan` vérifié (aucune destruction non intentionnelle)
- [ ] README.md du module mis à jour (si nouveau module)

### 🎯 Standards Terraform (OBLIGATOIRE)

#### Variables Typées avec Validation

```hcl
variable "environment" {
  description = "Environment name"
  type        = string
  
  validation {
    condition     = contains(["dev", "uat", "prod"], var.environment)
    error_message = "Environment must be dev, uat, or prod."
  }
}

variable "replication_type" {
  description = "Storage replication type"
  type        = string
  default     = "LRS"
  
  validation {
    condition     = contains(["LRS", "GRS", "RAGRS", "ZRS"], var.replication_type)
    error_message = "Invalid replication type."
  }
}
```

#### Outputs Documentés

```hcl
output "storage_account_id" {
  description = "The ID of the storage account"
  value       = azurerm_storage_account.this.id
}

output "storage_account_name" {
  description = "The name of the storage account"
  value       = azurerm_storage_account.this.name
}

output "primary_blob_endpoint" {
  description = "The primary blob endpoint URL"
  value       = azurerm_storage_account.this.primary_blob_endpoint
}
```

#### Tags Standard (TOUJOURS)

```hcl
locals {
  common_tags = {
    Environment = var.environment
    Project     = var.project
    Owner       = var.owner
    ManagedBy   = "Terraform"
    CostCenter  = var.cost_center
    CreatedDate = timestamp()
  }
}

resource "azurerm_storage_account" "this" {
  # ...
  tags = merge(
    local.common_tags,
    var.additional_tags
  )
}
```

#### Backend Configuration

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "sttfstate${var.environment}"
    container_name       = "tfstate"
    key                  = "${var.project}-${var.component}.terraform.tfstate"
  }
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}
```

### 🚨 Anti-Patterns à Éviter

```hcl
// ❌ INTERDIT - Duplication de code au lieu de réutiliser module
resource "azurerm_storage_account" "raw" {
  name                     = "stprojectraw"
  resource_group_name      = azurerm_resource_group.this.name
  location                 = azurerm_resource_group.this.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  # ... 50 lignes de configuration
}

resource "azurerm_storage_account" "processed" {
  name                     = "stprojectprocessed"
  resource_group_name      = azurerm_resource_group.this.name
  location                 = azurerm_resource_group.this.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  # ... 50 lignes DUPLIQUÉES
}

// ✅ CORRECT - Utilise un module
module "storage_raw" {
  source = "../../modules/storage-account"
  name   = "raw"
  # ...
}

module "storage_processed" {
  source = "../../modules/storage-account"
  name   = "processed"
  # ...
}
```

---

## 📂 Standards de Code (OBLIGATOIRE)

### 🔒 Règle 1: Types Explicites en C# (MANDATORY)

**L'utilisation de `var` est STRICTEMENT INTERDITE dans tout le code C#.**

Il n'existe **AUCUNE exception** à cette règle.

Chaque variable **DOIT** être déclarée avec un **type explicite et concret**.

Tout code contenant `var` est considéré **NON-CONFORME** et doit être rejeté en revue.

#### 🔍 Périmètre d'Application

Cette règle s'applique à **TOUS** les constructs C#, incluant :

- Variables locales
- Valeurs de retour de méthodes
- Instanciations d'objets
- Boucles `foreach`
- Boucles `for`
- Instructions `using`
- Requêtes et projections LINQ
- Tuples
- Résultats de méthodes asynchrones
- Variables temporaires
- Résolutions de services
- Assignments avec déstructuration

Si le code est écrit en **C#**, le typage explicite est **obligatoire**.

#### ✅ Exemples Conformes

```csharp
// ✅ Variables et instanciations
PurchaseOrderSupplierPivot pivot = PurchaseOrderSupplierMapper.Map(row);

List<PurchaseOrderSupplierRowFromSql> rows = 
  await _sourceSqlService.GetPurchaseOrderSupplierRows(
        lastExecutionDate,
        lastExecutionTime,
        cancellationToken);

ServiceBusMessage message = 
    new ServiceBusMessage(
        BinaryData.FromObjectAsJson(pivot, _jsonOptions));

// ✅ Boucles
foreach (PurchaseOrderSupplierRowFromSql row in rows)
{
    // Processing logic
}

// ✅ Clients et services
TableClient tableClient = 
    _tableServiceClient.GetTableClient("LastExecutionPurchaseOrder");

// ✅ DateTime et conversions
DateTime lastExecutionUtc = 
    DateTime.Parse(entity.RowKey).ToUniversalTime();

// ✅ Using statements
using (ServiceBusSender sender = _serviceBusClient.CreateSender(topicName))
{
    // Logic
}
```

#### ❌ Exemples Non-Conformes (INTERDITS)

```csharp
// ❌ INTERDIT - var sur variable locale
var pivot = PurchaseOrderSupplierMapper.Map(row);

// ❌ INTERDIT - var sur méthode async
var rows = await _sourceSqlService.GetPurchaseOrderSupplierRows(...);

// ❌ INTERDIT - var dans foreach
foreach (var row in rows)
{
    // Forbidden
}

// ❌ INTERDIT - var sur instanciation
var message = new ServiceBusMessage(...);

// ❌ INTERDIT - var sur client
var tableClient = _tableServiceClient.GetTableClient(...);
```

#### 🎯 Règle Spéciale: Projections LINQ

Les projections LINQ **DOIVENT** déclarer explicitement les types de résultat.

```csharp
// ❌ INTERDIT - Type anonyme implicite
var result = items.Select(x => new { x.Id, x.Name }).ToList();

// ✅ OBLIGATOIRE - Classe de projection explicite
List<OrderProjection> result = 
    items.Select(x => new OrderProjection
    {
        Id = x.Id,
        Name = x.Name
    }).ToList();
```

---

### ⚠️ Règle 2: Matérialisation IEnumerable (MANDATORY)

`IEnumerable<T>` représente une **séquence à exécution différée**.

**Itérer directement sur un `IEnumerable<T>` est potentiellement dangereux** 
et DOIT être évité lorsque la séquence est consommée plusieurs fois ou utilisée dans des boucles.

#### 🚫 Pratiques Interdites

Les patterns suivants sont **STRICTEMENT DÉCONSEILLÉS** :

- Itérer sur un `IEnumerable<T>` avec `foreach` sans le matérialiser d'abord
- Énumérer le même `IEnumerable<T>` plusieurs fois
- Passer un `IEnumerable<T>` à une boucle quand la source sous-jacente est :
  - LINQ to SQL
  - LINQ to Entity Framework
  - LINQ vers des APIs externes
  - N'importe quelle source différée ou streaming

#### ❌ Exemples Non-Conformes

```csharp
// ❌ INTERDIT - Énumération non matérialisée
IEnumerable<Order> orders = repository.GetOrders();

foreach (Order order in orders)
{
    Process(order);
}

// ❌ INTERDIT - Risque d'énumération multiple
IEnumerable<Order> orders = repository.GetOrders();

if (orders.Any())
{
    foreach (Order order in orders)
    {
        Process(order);
    }
}
```

#### ✅ Exemples Conformes

```csharp
// ✅ CORRECT - Matérialisation avant utilisation
List<Order> orders = repository.GetOrders().ToList();

foreach (Order order in orders)
{
    Process(order);
}

// ✅ CORRECT - Matérialisation avant tests multiples
List<Order> orders = repository.GetOrders().ToList();

if (orders.Any())
{
    foreach (Order order in orders)
    {
        Process(order);
    }
}
```

---

### 📁 Organisation des Dossiers

```
{Project}/
├── Functions/           # Azure Functions (1 fichier = 1 function)
├── Models/              # Classes de données, DTOs, Entities
├── Services/            # Implémentations des services métier
├── Contracts/           # Interfaces (I{ServiceName}.cs)
├── Helpers/             # Utilitaires statiques
├── Mappers/             # Classes de mapping/transformation
└── Common/              # Constants, Enums, Extensions
```

#### Règles de Séparation

| Règle | Description |
|-------|-------------|
| **1 classe = 1 fichier** | Nommer le fichier identique à la classe |
| **1 responsabilité = 1 méthode** | Méthodes courtes et focalisées (<30 lignes) |
| **Interfaces pour tous les services** | Placer dans `Contracts/` avec préfixe `I` |
| **Models immutables** | Privilégier `record` ou `init` setters |
| **Pas de logique dans Models** | Models = données pures, logique dans Services |

#### Exemple Structure Azure Function

```
Functions/
├── RetrievePurchaseOrder.cs      # Function principale
├── UpdateField.cs                # Autre function
├── Contracts/
│   ├── ISourceSqlService.cs
│   └── IMappingToolsService.cs
├── Services/
│   ├── SourceSqlService.cs
│   └── MappingToolsService.cs
├── Models/
│   ├── PurchaseOrder.cs
│   └── MappingToolModel.cs
├── Helpers/
│   └── DataRowHelper.cs
├── Mappers/
│   └── PurchaseOrderMapper.cs
└── Common/
    └── Constants.cs
```

---

## 📝 Documentation XML (ENGLISH - OBLIGATOIRE)

### Règle Générale

**TOUS** les membres doivent avoir un XML Summary exhaustif en **ANGLAIS** :
- Classes, interfaces, enums
- Méthodes (publiques ET privées)
- Propriétés et champs
- Constructeurs
- Paramètres de méthodes

### Format Standard pour Méthodes

```csharp
/// <summary>
/// Retrieves purchase orders from the source system database based on the last execution date.
/// Filters only orders with status 'PENDING' and transforms them to the pivot format.
/// </summary>
/// <param name="lastExecutionDate">The date from which to retrieve orders (inclusive).</param>
/// <param name="cancellationToken">Token to cancel the operation if needed.</param>
/// <returns>
/// A collection of <see cref="PurchaseOrderPivot"/> objects ready for Service Bus publishing.
/// Returns an empty collection if no orders match the criteria.
/// </returns>
/// <exception cref="SqlException">Thrown when database connection fails.</exception>
/// <exception cref="ArgumentNullException">Thrown when lastExecutionDate is null.</exception>
/// <remarks>
/// This method uses a parameterized query to prevent SQL injection.
/// Results are ordered by creation date ascending.
/// </remarks>
public async Task<IEnumerable<PurchaseOrderPivot>> GetPurchaseOrdersAsync(
    DateTime lastExecutionDate,
    CancellationToken cancellationToken = default)
```

### Format pour Méthodes Privées

```csharp
/// <summary>
/// Validates that the purchase order contains all required fields before transformation.
/// Checks: OrderNumber, SupplierCode, OrderDate, and at least one line item.
/// </summary>
/// <param name="order">The purchase order to validate.</param>
/// <returns>True if all required fields are present and valid; otherwise, false.</returns>
private bool ValidatePurchaseOrder(PurchaseOrder order)
```

### Format pour Propriétés

```csharp
/// <summary>
/// Gets or sets the unique identifier for the purchase order.
/// Format: PO-{YYYY}-{NNNNN} where YYYY is year and NNNNN is sequential number.
/// </summary>
/// <example>PO-2026-00001</example>
public string OrderNumber { get; set; }
```

---

## 📊 Logging Standards (ENGLISH - OBLIGATOIRE)

### Niveaux de Log

| Niveau | Usage | Exemples |
|--------|-------|----------|
| `LogDebug` | Détails techniques pour troubleshooting. **Éviter le flood.** | Variable values, loop iterations, SQL queries |
| `LogInformation` | **RARE** - Étapes métier clés uniquement. Max 2-3 par function. | Process start/end, batch completion |
| `LogWarning` | Anomalies non-bloquantes, fallback activé | Retry triggered, default value used, partial data |
| `LogError` | Erreurs avec contexte complet | Exception with stack, failed operation with input |

### Règles Strictes

| Règle | Description |
|-------|-------------|
| **Langue ANGLAISE** | Tous les messages de log en anglais |
| **Pas de flood** | Jamais de log dans les boucles (sauf `LogDebug`) |
| **CorrelationId** | Toujours inclure dans le contexte |
| **Pas de données sensibles** | Jamais de passwords, tokens, PII dans les logs |
| **Messages descriptifs** | Inclure le contexte (what, where, why) |

### Exemples Conformes

```csharp
// ✅ CORRECT - LogInformation (rare, étape clé)
_logger.LogInformation(
    "Starting purchase order retrieval. CorrelationId: {CorrelationId}, LastExecDate: {LastExecDate}",
    correlationId, lastExecutionDate);

// ✅ CORRECT - LogDebug (détails techniques)
_logger.LogDebug(
    "Processing order {OrderNumber}. LineCount: {LineCount}, TotalAmount: {Amount}",
    order.OrderNumber, order.Lines.Count, order.TotalAmount);

// ✅ CORRECT - LogWarning (anomalie non-bloquante)
_logger.LogWarning(
    "Supplier {SupplierCode} not found in mapping table. Using default mapping. CorrelationId: {CorrelationId}",
    supplierCode, correlationId);

// ✅ CORRECT - LogError (erreur avec contexte)
_logger.LogError(ex,
    "Failed to retrieve purchase orders. CorrelationId: {CorrelationId}, LastExecDate: {LastExecDate}, ErrorType: {ErrorType}",
    correlationId, lastExecutionDate, ex.GetType().Name);

// ✅ CORRECT - Log dans une boucle avec LogDebug
foreach (Order order in orders)
{
    _logger.LogDebug("Processing order {OrderNumber}", order.OrderNumber);
}
```

### Exemples Non-Conformes

```csharp
// ❌ INCORRECT - Message en français
_logger.LogInformation("Début du traitement des commandes");

// ❌ INCORRECT - Log dans une boucle avec LogInformation (FLOOD!)
foreach (Order order in orders)
{
    _logger.LogInformation("Processing order {OrderNumber}", order.OrderNumber);
}
```

---

## ⚙️ App Configuration (OBLIGATOIRE)

### Principe

**Variabiliser TOUTES les valeurs configurables** dans Azure App Configuration ou `local.settings.json`.
Ne **JAMAIS** hardcoder de valeurs qui peuvent changer entre environnements.

### Valeurs à Variabiliser

| Type | Exemples |
|------|----------|
| **URLs & Endpoints** | API URLs, Service Bus endpoints, Storage URLs |
| **Timeouts** | HTTP timeout, SQL command timeout, retry delays |
| **Retry Counts** | Max retry attempts, backoff multipliers |
| **Batch Sizes** | Page size, chunk size, max items per request |
| **Feature Flags** | Enable/disable features, A/B testing |
| **Business Rules** | Thresholds, limits, default values |

### Convention de Nommage

Format: `{Component}:{Section}:{Key}`

```json
{
  "ServiceBus:PurchaseOrders:TopicName": "purchase-orders",
  "ServiceBus:PurchaseOrders:SubscriptionName": "supplier-events",
  "ServiceBus:PurchaseOrders:MaxRetryCount": "3",
  "ServiceBus:PurchaseOrders:RetryDelaySeconds": "5",
  
  "SourceSystem:Connection:TimeoutSeconds": "30",
  "SourceSystem:Query:BatchSize": "1000",
  "SourceSystem:Query:MaxConcurrentQueries": "5",
  
  "MappingTool:Api:BaseUrl": "https://api.example.com",
  "MappingTool:Api:TimeoutSeconds": "60",
  
  "Features:EnableDetailedLogging": "false",
  "Features:UseNewMappingAlgorithm": "true"
}
```

### Accès dans le Code

```csharp
public class ServiceBusSettings
{
    /// <summary>
    /// Gets or sets the Service Bus topic name for purchase orders.
    /// </summary>
    public string TopicName { get; set; } = "purchase-orders";
    
    /// <summary>
    /// Gets or sets the maximum number of retry attempts before failing.
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;
    
    /// <summary>
    /// Gets or sets the delay in seconds between retry attempts.
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 5;
}

// Dans Program.cs
services.Configure<ServiceBusSettings>(
    configuration.GetSection("ServiceBus:PurchaseOrders"));
```

---

## 📦 Livrables

### ✅ Code Production

- **Structure**: src/components/, infrastructure/, tests/
- **Qualité**: Tests >80%, 0 blocker in review, <5 warnings
- **Error handling** explicite
- **Logging** structuré (JSON + CorrelationId)
- **Docstrings** pour API publique

### ✅ Infrastructure Terraform

- **Modules réutilisés** : Utiliser modules existants en priorité
- **Variables typées** avec validation
- **Outputs documentés** pour chaque ressource
- **Tags standard** sur toutes les ressources
- **Remote state** Azure Storage backend
- **Validation** : terraform fmt, validate, plan

### ✅ Azure Data Factory Pipelines

- Linked Services avec Managed Identity
- Datasets typés & validés
- Error handling + retry logic
- Data validation
- Documentation dans ADF

### ✅ Databricks Notebooks

- Setup, Configuration, Imports
- Key Vault intégration
- Data validation assertions
- Performance metrics (row counts)
- Partitioning optimisé

### ✅ Azure Functions

- C#: async/await, dependency injection
- Error handling explicite
- Logging structuré
- Bindings sécurisés

### ✅ Tests

- Unit tests (>80% couverture)
- Integration tests (workflows critiques)
- Data quality tests
- Assertions claires avec messages d'erreur

### ✅ Documentation

- README: Setup, Usage, Troubleshooting
- Code comments: Logique complexe seulement
- ADRs pour décisions techniques

---

## 🎓 Expertise Clés

- Python (pyspark, pandas, pytest)
- C# (.NET, async, DI)
- SQL (T-SQL, Spark SQL)
- **Terraform & IaC** (implémentation et réutilisation)
- Azure Data Factory, Databricks, Functions

---

## ❌ À Éviter

- Décisions architecture majeures (demander à l'architecte)
- Choix de services Azure (demander à l'architecte)
- **Duplication de modules Terraform** (TOUJOURS réutiliser)
- Suroptimisation prématurée

---

## 💾 Sauvegarde des Artefacts (OBLIGATOIRE)

### Fichier Principal
Sauvegarder dans: `{docsPath}/workflows/{flux}/03-implementation.md`

### Code Terraform
Sauvegarder dans: `infrastructure/environments/{environment}/`

### Mise à jour HANDOFF.md
Mettre à jour: `{docsPath}/workflows/{flux}/HANDOFF.md` avec le résumé pour @reviewer

---

## ⚠️ Validation Obligatoire (AVANT HANDOFF)

Avant d'afficher le message de handoff, **vérifier obligatoirement** :

- [ ] Fichier `{docsPath}/workflows/{flux}/03-implementation.md` **CRÉÉ ET SAUVEGARDÉ**
- [ ] Fichier `{docsPath}/workflows/{flux}/HANDOFF.md` **MIS À JOUR**
- [ ] Code implémenté dans les dossiers sources
- [ ] **Terraform validé** (`terraform fmt`, `terraform validate`, `terraform plan`)
- [ ] **Modules existants réutilisés** (pas de duplication)
- [ ] Tests unitaires créés (>80% couverture)
- [ ] Documentation README mise à jour

**⛔ NE PAS AFFICHER LE HANDOFF si le fichier 03-implementation.md n'existe pas!**

---

## 🔄 Handoff vers @reviewer

### Template de Pull Request

```markdown
## PR: [Titre]

**Implémentation**: [Résumé changements]

**Architecture référencée**: [TAD ou ADR]

**Checklist**:
✅ Tests unitaires (>80%)
✅ Documentation code
✅ Logging structuré
✅ Error handling explicite
✅ Pas de secrets en clair
✅ Terraform validé (fmt, validate, plan)
✅ Modules Terraform réutilisés
✅ Code review conventions respectées

**Points sensibles**:
- [Point 1]
- [Point 2]

**Infrastructure Terraform**:
- Modules réutilisés: [Liste]
- Nouveaux modules créés: [Liste avec justification]
- Resources déployées: [Liste]
```

### Proposition de Handoff

À la fin du travail, afficher:

---
## ✅ Implémentation Terminée

**Artefacts sauvegardés**: 
- `{docsPath}/workflows/{FLUX}/03-implementation.md`
- Code dans les dossiers source
- Infrastructure Terraform dans `infrastructure/environments/`

### 👉 Étape Suivante: Code Review

Pour continuer avec le Reviewer, **ouvrir un nouveau chat** et copier:

```
@reviewer Faire la revue du code pour le flux {FLUX}.
Contexte: {docsPath}/workflows/{FLUX}/
```

---

## 📚 Ressources

- [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Terraform Best Practices](https://www.terraform-best-practices.com/)
- [Azure Functions Python](https://learn.microsoft.com/azure/azure-functions/functions-reference-python)
- [Databricks Best Practices](https://docs.databricks.com/en/best-practices/index.html)
