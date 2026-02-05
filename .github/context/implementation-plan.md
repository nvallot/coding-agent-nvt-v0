---
created: 2026-02-05
updated: 2026-02-05
status: completed
agent: dev
client: demo-client
tad-reference: tad-output.md
---

# Plan d'Implémentation

## Projet : Azure Function – CSV to Service Bus

---

## 1. Résumé

Implémentation complète d'une Azure Function en C# (.NET 8) qui :
- Se déclenche automatiquement sur dépôt de fichiers CSV (Blob Trigger)
- Parse et valide les lignes CSV avec CsvHelper + FluentValidation
- Transforme en messages JSON conformes au schéma défini
- Publie en batch sur Service Bus
- Garantit l'idempotence via Azure Table Storage

**Exigences couvertes** : FR-001 à FR-007, NFR-001 à NFR-005

---

## 2. Structure du Projet

```
src/
├── CsvProcessor.Functions/
│   ├── Configuration/
│   │   └── AppSettings.cs
│   ├── Functions/
│   │   └── CsvBlobTriggerFunction.cs
│   ├── Models/
│   │   ├── CsvOrderLine.cs
│   │   ├── OrderMessage.cs
│   │   ├── ProcessedFileEntity.cs
│   │   └── ProcessingResult.cs
│   ├── Services/
│   │   ├── ICsvParserService.cs
│   │   ├── CsvParserService.cs
│   │   ├── IIdempotencyService.cs
│   │   ├── IdempotencyService.cs
│   │   ├── IJsonTransformerService.cs
│   │   ├── JsonTransformerService.cs
│   │   ├── IServiceBusPublisher.cs
│   │   └── ServiceBusPublisher.cs
│   ├── Validators/
│   │   └── CsvOrderLineValidator.cs
│   ├── CsvProcessor.Functions.csproj
│   ├── Program.cs
│   ├── host.json
│   └── local.settings.json
└── CsvProcessor.Tests/
    ├── Unit/
    │   ├── CsvOrderLineValidatorTests.cs
    │   ├── CsvParserServiceTests.cs
    │   ├── IdempotencyServiceTests.cs
    │   └── JsonTransformerServiceTests.cs
    └── CsvProcessor.Tests.csproj
```

---

## 3. Composants Créés

### 3.1 Models

| Fichier | Description | Exigence |
|---------|-------------|----------|
| `CsvOrderLine.cs` | Modèle CSV avec attributs CsvHelper | FR-002 |
| `OrderMessage.cs` | Message JSON avec métadonnées | FR-004 |
| `ProcessedFileEntity.cs` | Entité Table Storage pour tracking | FR-006 |
| `ProcessingResult.cs` | Résultat de traitement avec statistiques | FR-007 |

### 3.2 Services

| Service | Interface | Description | Exigence |
|---------|-----------|-------------|----------|
| `CsvParserService` | `ICsvParserService` | Parse CSV, détecte séparateur, valide lignes | FR-002, FR-003 |
| `JsonTransformerService` | `IJsonTransformerService` | Transforme CSV → JSON avec métadonnées | FR-004 |
| `ServiceBusPublisher` | `IServiceBusPublisher` | Publication batch avec retry | FR-005, ADR-003 |
| `IdempotencyService` | `IIdempotencyService` | Hash MD5 + Table Storage tracking | FR-006, ADR-002 |

### 3.3 Function

| Fichier | Trigger | Description |
|---------|---------|-------------|
| `CsvBlobTriggerFunction.cs` | Blob Trigger | Orchestration du pipeline complet |

---

## 4. Dépendances NuGet

```xml
<!-- Azure Functions -->
<PackageReference Include="Microsoft.Azure.Functions.Worker" Version="1.21.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.17.2" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Storage.Blobs" Version="6.3.0" />

<!-- Azure SDKs -->
<PackageReference Include="Azure.Data.Tables" Version="12.8.3" />
<PackageReference Include="Azure.Messaging.ServiceBus" Version="7.17.4" />
<PackageReference Include="Azure.Identity" Version="1.11.0" />

<!-- CSV Parsing -->
<PackageReference Include="CsvHelper" Version="31.0.2" />

<!-- Validation -->
<PackageReference Include="FluentValidation" Version="11.9.0" />

<!-- Telemetry -->
<PackageReference Include="Microsoft.ApplicationInsights.WorkerService" Version="2.22.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.ApplicationInsights" Version="1.2.0" />
```

---

## 5. Configuration

### 5.1 App Settings Requises

| Setting | Description | Exemple |
|---------|-------------|---------|
| `SourceBlobConnection__blobServiceUri` | URI Blob Storage source | `https://st.blob.core.windows.net` |
| `TableStorageConnection__tableServiceUri` | URI Table Storage | `https://st.table.core.windows.net` |
| `ServiceBusConnection__fullyQualifiedNamespace` | Namespace Service Bus | `sb.servicebus.windows.net` |
| `SourceContainerName` | Container des CSV | `csv-input` |
| `ServiceBusTopicName` | Topic cible | `orders-topic` |
| `IdempotencyTableName` | Table pour tracking | `ProcessedFiles` |

### 5.2 RBAC Requis (Managed Identity)

| Rôle | Scope | Usage |
|------|-------|-------|
| Storage Blob Data Reader | Storage Account source | Lecture des CSV |
| Storage Table Data Contributor | Storage Account fonction | Écriture Table Storage |
| Azure Service Bus Data Sender | Service Bus Namespace | Publication messages |

---

## 6. Tests Créés

### 6.1 Tests Unitaires

| Classe de Test | Couverture | Tests |
|----------------|------------|-------|
| `CsvOrderLineValidatorTests` | Validation des règles métier | 8 tests |
| `CsvParserServiceTests` | Parsing CSV, détection séparateur | 6 tests |
| `JsonTransformerServiceTests` | Transformation et métadonnées | 6 tests |
| `IdempotencyServiceTests` | Hash, vérification, marquage | 8 tests |

**Total : 28 tests unitaires**

### 6.2 Exécution des Tests

```bash
cd src/CsvProcessor.Tests
dotnet test --verbosity normal
```

---

## 7. Instructions de Déploiement

### 7.1 Prérequis Azure

1. **Resource Group** créé
2. **Storage Account** avec :
   - Container `csv-input` pour les fichiers source
   - Table `ProcessedFiles` pour l'idempotence
3. **Service Bus Namespace** avec Topic `orders-topic`
4. **Application Insights** pour le monitoring

### 7.2 Déploiement Local

```bash
# 1. Démarrer Azurite pour émulation locale
azurite --silent

# 2. Configurer local.settings.json avec les URIs

# 3. Lancer la fonction
cd src/CsvProcessor.Functions
func start
```

### 7.3 Déploiement Azure

```bash
# 1. Build
dotnet publish -c Release -o ./publish

# 2. Déployer (via Azure CLI ou CI/CD)
az functionapp deployment source config-zip \
    --resource-group rg-csv-processor \
    --name func-csv-processor \
    --src ./publish.zip

# 3. Configurer les App Settings
az functionapp config appsettings set \
    --resource-group rg-csv-processor \
    --name func-csv-processor \
    --settings "SourceContainerName=csv-input" \
               "ServiceBusTopicName=orders-topic" \
               "IdempotencyTableName=ProcessedFiles"
```

### 7.4 Test de Validation

1. Uploader un fichier CSV de test dans le container `csv-input`
2. Vérifier les logs dans Application Insights
3. Vérifier les messages dans Service Bus Explorer
4. Vérifier l'entrée dans Table Storage

---

## 8. Points d'Attention pour Review

### 8.1 Sécurité
- ✅ Aucun secret en dur (Managed Identity)
- ✅ Validation des entrées (FluentValidation)
- ✅ Pas de données sensibles dans les logs

### 8.2 Performance
- ✅ Publication batch (ADR-003)
- ✅ Streaming pour parsing CSV
- ✅ Pas de chargement complet en mémoire

### 8.3 Fiabilité
- ✅ Idempotence (ADR-002)
- ✅ Retry avec backoff exponentiel
- ✅ Gestion des erreurs partielles

### 8.4 Testabilité
- ✅ Interfaces pour tous les services
- ✅ TimeProvider injectable
- ✅ 28 tests unitaires

---

## 9. Métriques de Qualité

| Métrique | Cible | Actuel |
|----------|-------|--------|
| Couverture de code | > 80% | À mesurer |
| Fichiers créés | - | 17 |
| Tests unitaires | > 20 | 28 |
| Violations de conventions | 0 | 0 |

---

## Handoff

✅ **Implémentation complétée**

| Élément | Statut |
|---------|--------|
| Code Function | ✅ Complet |
| Services | ✅ 4 services implémentés |
| Tests unitaires | ✅ 28 tests |
| Configuration | ✅ Documentée |
| Déploiement | ✅ Instructions fournies |

**Prochain agent** : `@reviewer`  
**Commande** : `#prompt:handoff-to-reviewer`

Le code est prêt pour revue de code, audit de sécurité et validation de performance.
