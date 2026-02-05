---
applyTo: "**/*"
type: knowledge
---

# Knowledge: Azure Services Catalog

## 📋 Vue d'ensemble

Catalogue des services Azure par catégorie avec leurs cas d'usage typiques.

## 🗂️ Services par Catégorie

### Ingestion

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Data Factory** | Orchestration ETL/ELT | Batch, volumes importants, transformation complexe |
| **Event Hubs** | Streaming temps réel | Events haute volumétrie, IoT, logs |
| **Azure Functions** | Ingestion légère | HTTP triggers, petits volumes, serverless |
| **Logic Apps** | Intégration low-code | Connecteurs SaaS, workflows simples |

### Traitement

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Databricks** | Big Data processing | Spark, ML, volumes TB+ |
| **Synapse Analytics** | Data Warehouse | Analytics SQL, intégration Spark |
| **Stream Analytics** | Streaming analytics | Temps réel, CEP, alerting |
| **HDInsight** | Hadoop ecosystem | Migration Hadoop existant |

### Stockage

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **ADLS Gen2** | Data Lake | Fichiers, Parquet, Delta Lake |
| **Blob Storage** | Object storage | Fichiers non structurés, backups |
| **SQL Database** | RDBMS managé | Transactionnel, <4TB |
| **Cosmos DB** | NoSQL multi-modèle | Global distribution, faible latence |
| **Table Storage** | NoSQL simple | Key-value, coût faible |

### Orchestration

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Data Factory** | Pipelines data | ETL/ELT, scheduling |
| **Synapse Pipelines** | Pipelines intégrés | Quand Synapse utilisé |
| **Logic Apps** | Workflows | Intégrations SaaS, alerting |
| **Durable Functions** | Orchestration code | Workflows complexes en code |

### Gouvernance

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Purview** | Data catalog | Lineage, classification, discovery |
| **Key Vault** | Secrets management | Clés, certificats, secrets |
| **Policy** | Compliance | Enforcement règles, tagging |
| **Managed Identity** | Authentication | Service-to-service auth |

### Messaging

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Service Bus** | Enterprise messaging | Queues, topics, transactions |
| **Event Grid** | Event routing | Events Azure, webhooks |
| **Event Hubs** | High-throughput streaming | Millions events/sec |
| **Storage Queues** | Simple queuing | Coût faible, >80GB |

### Monitoring

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Application Insights** | APM | Traces, metrics, logs applicatifs |
| **Log Analytics** | Logs centralisés | KQL queries, dashboards |
| **Azure Monitor** | Infrastructure | Alertes, métriques ressources |
| **Sentinel** | SIEM | Sécurité, threat detection |

### Réseau

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Virtual Network** | Isolation réseau | Toujours pour prod |
| **Private Endpoints** | Accès privé PaaS | Sécurité renforcée |
| **NSG** | Firewall niveau 4 | Règles IP/port |
| **Application Gateway** | Load balancer L7 | WAF, SSL offload |
| **API Management** | API Gateway | Versioning, throttling, auth |

### Identité

| Service | Usage | Quand utiliser |
|---------|-------|----------------|
| **Entra ID** | Identity provider | Auth utilisateurs, SSO |
| **Managed Identity** | Service identity | Auth service-to-service |
| **RBAC** | Authorization | Contrôle accès ressources |
| **Conditional Access** | Policies | MFA, device compliance |

## 🔗 Abréviations Standard (Azure CAF)

| Service | Abréviation | Exemple |
|---------|-------------|---------|
| Resource Group | `rg` | rg-dataplatform-dev |
| Storage Account | `st` | stdataplatformdev |
| Data Factory | `adf` | adf-dataplatform-dev |
| Databricks | `dbw` | dbw-dataplatform-dev |
| Synapse | `syn` | syn-dataplatform-dev |
| Key Vault | `kv` | kv-dataplatform-dev |
| SQL Database | `sqldb` | sqldb-dataplatform-dev |
| Event Hub | `evh` | evh-dataplatform-dev |
| Function App | `func` | func-dataplatform-dev |
| Logic App | `logic` | logic-dataplatform-dev |
| Service Bus | `sb` | sb-dataplatform-dev |
| App Insights | `appi` | appi-dataplatform-dev |
| Log Analytics | `log` | log-dataplatform-dev |
| Virtual Network | `vnet` | vnet-dataplatform-dev |
| API Management | `apim` | apim-dataplatform-dev |
| Container Registry | `acr` | acrdataplatformdev |

## 💰 Modèles de Coûts

| Service | Modèle | Facteurs clés |
|---------|--------|---------------|
| Storage | Capacity + transactions | GB stockés, opérations R/W |
| Compute | Instance hours | SKU, durée, région |
| Databricks | DBU hours | Cluster size, workload |
| Functions | Executions + GB-s | Nombre invocations, mémoire |
| Service Bus | Messages + operations | Volume messages, tier |
| Event Hubs | TU/CU + ingress | Throughput units, GB ingérés |

## 📚 Références

- [Azure Architecture Center](https://learn.microsoft.com/azure/architecture/)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
- [CAF Naming Convention](https://learn.microsoft.com/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming)
