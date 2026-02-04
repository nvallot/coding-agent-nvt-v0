---
applyTo: "**/*"
---

# Référence Azure Services

Groupe | Services | Usage
-------|----------|------
**Ingestion** | Data Factory, Event Hubs, Functions | Ingestion batch/streaming
**Traitement** | Databricks, Synapse, Stream Analytics | Transformation, analytics
**Stockage** | ADLS Gen2, SQL Database, Cosmos DB | Persistence données
**Orchestration** | Data Factory, Synapse Pipelines, Logic Apps | Workflow management
**Gouvernance** | Purview, Key Vault, Policy | Metadata, secrets, compliance
**Monitoring** | App Insights, Log Analytics, Monitor | Observabilité
**Réseau** | VNet, Private Endpoints, NSG | Isolation, sécurité
**Identité** | Managed Identity, RBAC, Entra ID | Auth, authorization

## 💡 Patterns Recommandés
- **Medallion**: Bronze → Silver → Gold layers
- **Lambda**: Batch + Stream parallel
- **CDC**: Change Data Capture pour sync
- **Data Mesh**: Federated ownership

## 🎯 Best Practices (Rapide)
1. Utiliser Managed Identity (jamais connection strings)
2. Private Endpoints pour isolation réseau
3. Key Vault pour tous secrets
4. Tags sur toutes ressources (client, env, owner)
5. Monitoring dès le jour 1
