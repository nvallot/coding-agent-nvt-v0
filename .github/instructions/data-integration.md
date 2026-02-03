---
applyTo: "{data,etl,ingestion,pipelines,adf,synapse,fabric,databricks}/**"
---

# Instructions Data Integration (Azure)

Quand tu travailles sur l’intégration de données, appliquer ces règles :

## 🎯 Objectifs
- Pipelines **robustes**, **observables** et **idempotents**
- Charges **incrémentales** privilégiées
- **Paramétrage** systématique (pas de valeurs en dur)

## 🧱 Stack Azure Prioritaire
- **Orchestration** : Azure Data Factory / Synapse Pipelines / Fabric Data Factory
- **Streaming** : Event Hubs, Stream Analytics
- **Stockage** : ADLS Gen2 / Blob Storage
- **Traitement** : Databricks / Synapse Spark / Fabric Lakehouse
- **Gouvernance** : Microsoft Purview
- **Sécurité** : Managed Identity + Key Vault

## ✅ Bonnes pratiques
- **Naming** clair (source → cible, domaine, fréquence)
- **Linked Services** avec Managed Identity
- **Retries** + **backoff** configurés
- **Checksum** ou watermark pour l’incrémental
- **Schema drift** géré explicitement
- **Data quality checks** (nulls, types, volumes, doublons)
- **Alerting** sur failures et SLA dépassées

## 📌 Exemples de conventions

### Dataset naming
- `ds_src_sales_orders`
- `ds_curated_customer_dim`

### Pipeline naming
- `pl_ingest_sales_daily`
- `pl_curate_customer_dim`

### Paramètres standards
- `p_run_id`, `p_watermark`, `p_source`, `p_sink`

## 🔍 Observabilité
- Loguer : `run_id`, `source`, `rows_read`, `rows_written`, `duration`
- Générer un rapport d’exécution en sortie

## 🔐 Sécurité
- Secrets dans **Key Vault**
- Identités managées (pas de credentials locaux)
- Accès storage via RBAC + ACLs ADLS
