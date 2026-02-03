# Knowledge: Azure Data Factory

## 📋 Vue d'ensemble

**Azure Data Factory (ADF)** est le service ETL/ELT managé d'Azure pour orchestrer et automatiser les mouvements et transformations de données.

## 🎯 Use Cases

- Orchestration de pipelines ETL/ELT
- Copie de données entre sources
- Transformation de données (Data Flows)
- Intégration hybride (on-prem vers cloud)
- Planification et monitoring

## 🏗️ Architecture

### Composants Principaux

**Pipeline**:
- Unité logique d'activités
- Peut contenir plusieurs activités
- Supporte paramètres et variables

**Activity**:
- Étape individuelle (Copy, Transform, Control flow)
- Types: Copy Data, Data Flow, Execute Pipeline, Web, etc.

**Dataset**:
- Référence aux données (source ou sink)
- Lié à un Linked Service

**Linked Service**:
- Connexion à une source de données
- Credentials stockés (Key Vault recommandé)

**Integration Runtime**:
- Compute pour exécuter activités
- Types: Azure, Self-Hosted, Azure-SSIS

**Trigger**:
- Planifie l'exécution
- Types: Schedule, Tumbling Window, Event-based

## 💻 Exemples

### Pipeline Simple: Copy Files to ADLS

```json
{
  "name": "CopyFilesToADLS",
  "properties": {
    "activities": [
      {
        "name": "CopyData",
        "type": "Copy",
        "inputs": [
          {
            "referenceName": "SourceBlobDataset",
            "type": "DatasetReference"
          }
        ],
        "outputs": [
          {
            "referenceName": "SinkADLSDataset",
            "type": "DatasetReference"
          }
        ],
        "typeProperties": {
          "source": {
            "type": "BlobSource"
          },
          "sink": {
            "type": "AzureBlobFSSink"
          }
        }
      }
    ],
    "parameters": {
      "sourcePath": {
        "type": "string"
      },
      "sinkPath": {
        "type": "string"
      }
    }
  }
}
```

### Linked Service avec Managed Identity

```json
{
  "name": "ADLSGen2LinkedService",
  "type": "Microsoft.DataFactory/factories/linkedservices",
  "properties": {
    "type": "AzureBlobFS",
    "typeProperties": {
      "url": "https://<storage-account>.dfs.core.windows.net/"
    },
    "annotations": [],
    "connectVia": {
      "referenceName": "AutoResolveIntegrationRuntime",
      "type": "IntegrationRuntimeReference"
    }
  }
}
```

**Note**: Pas de credentials = Managed Identity utilisé automatiquement.

### Trigger Schedule (Quotidien à 6h)

```json
{
  "name": "DailyTrigger",
  "properties": {
    "type": "ScheduleTrigger",
    "typeProperties": {
      "recurrence": {
        "frequency": "Day",
        "interval": 1,
        "startTime": "2026-02-03T06:00:00Z",
        "timeZone": "UTC"
      }
    },
    "pipelines": [
      {
        "pipelineReference": {
          "referenceName": "ETLPipeline",
          "type": "PipelineReference"
        }
      }
    ]
  }
}
```

## ✅ Bonnes Pratiques

### 1. Paramétrage

**Toujours paramétrer**:
- Chemins source/destination
- Noms de fichiers avec dates
- Configurations environnement

```json
{
  "parameters": {
    "sourceContainer": "bronze",
    "sinkContainer": "silver",
    "processDate": "@utcnow('yyyy-MM-dd')"
  }
}
```

### 2. Error Handling

**Gérer les erreurs**:
- Activités Retry (retry count, interval)
- Activités conditionnelles (If/Switch)
- Logging dans tables ou ADLS
- Alerting via Logic Apps/Functions

### 3. Sécurité

**Managed Identity partout**:
```json
{
  "typeProperties": {
    "url": "https://myaccount.dfs.core.windows.net/",
    "authenticationType": "MI"
  }
}
```

**Secrets dans Key Vault**:
```json
{
  "secretName": {
    "type": "AzureKeyVaultSecret",
    "store": {
      "referenceName": "KeyVaultLS",
      "type": "LinkedServiceReference"
    },
    "secretName": "DatabasePassword"
  }
}
```

### 4. Performance

**Optimisations**:
- DIU (Data Integration Units): Augmenter pour gros volumes
- Parallel Copies: Paralléliser le copy
- Staging: Utiliser pour transformations lourdes
- Compression: Activer pour réduire I/O

```json
{
  "typeProperties": {
    "source": { ... },
    "sink": { ... },
    "enableStaging": true,
    "stagingSettings": {
      "linkedServiceName": { ... }
    },
    "dataIntegrationUnits": 32,
    "parallelCopies": 8
  }
}
```

### 5. Monitoring

**Logs structurés**:
- Output chaque activité
- Variables pour tracking
- Logging dans tables custom

```json
{
  "name": "LogActivity",
  "type": "Lookup",
  "dependsOn": [
    {
      "activity": "CopyData",
      "dependencyConditions": ["Succeeded"]
    }
  ],
  "userProperties": [
    {
      "name": "RowsCopied",
      "value": "@activity('CopyData').output.rowsCopied"
    }
  ]
}
```

## 🔗 Intégrations Courantes

### ADF + Databricks

```json
{
  "name": "ExecuteDatabricksNotebook",
  "type": "DatabricksNotebook",
  "linkedServiceName": {
    "referenceName": "DatabricksLinkedService",
    "type": "LinkedServiceReference"
  },
  "typeProperties": {
    "notebookPath": "/Notebooks/ETL/Transform",
    "baseParameters": {
      "input_path": "@pipeline().parameters.inputPath",
      "output_path": "@pipeline().parameters.outputPath"
    }
  }
}
```

### ADF + Synapse

```json
{
  "name": "ExecuteSynapseStoredProcedure",
  "type": "SqlServerStoredProcedure",
  "linkedServiceName": {
    "referenceName": "SynapseLinkedService",
    "type": "LinkedServiceReference"
  },
  "typeProperties": {
    "storedProcedureName": "sp_load_fact_sales",
    "storedProcedureParameters": {
      "process_date": {
        "value": "@pipeline().parameters.processDate",
        "type": "String"
      }
    }
  }
}
```

## 💰 Coûts

**Modèle de pricing**:
- Pipeline orchestration: $1 par 1000 exécutions
- Data movement (Copy): DIU heures
- Data Flow: vCore heures
- Self-Hosted IR: Gratuit (compute à votre charge)

**Optimisation**:
- Utiliser triggers event-based vs polling
- Réduire fréquence si possible
- Optimiser DIU et parallelism
- Utiliser Azure IR (moins cher que Self-Hosted)

## 📚 Références

- [ADF Documentation](https://learn.microsoft.com/azure/data-factory/)
- [ADF Best Practices](https://learn.microsoft.com/azure/data-factory/concepts-best-practices)
- [ADF Pricing](https://azure.microsoft.com/pricing/details/data-factory/)

---

**Version**: 1.0.0  
**Type**: Knowledge  
**Catégorie**: Azure Services
