---
applyTo: "**/docs/**,**/architecture/**"
type: knowledge
---

# Knowledge: Azure Architecture Patterns

## 📋 Vue d'ensemble

Descriptions des patterns d'architecture data les plus utilisés sur Azure.

---

## 🏅 Medallion Architecture

### Description

Architecture en 3 couches (Bronze → Silver → Gold) pour organiser les données dans un Data Lake.

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│  Sources  ──→  Bronze (Raw)  ──→  Silver (Cleaned)  ──→  Gold   │
│                                                                 │
│              ┌──────────┐     ┌──────────┐     ┌──────────┐    │
│              │ Raw data │     │ Validated│     │ Business │    │
│              │ As-is    │     │ Cleaned  │     │ Ready    │    │
│              │ Immutable│     │ Deduped  │     │ Optimized│    │
│              └──────────┘     └──────────┘     └──────────┘    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Couches

| Couche | Objectif | Caractéristiques |
|--------|----------|------------------|
| **Bronze** | Données brutes | Format source, append-only, historique complet |
| **Silver** | Données nettoyées | Dédupliquées, validées, types standardisés |
| **Gold** | Données métier | Agrégées, dénormalisées, prêtes pour consommation |

### Structure ADLS

```
container/
├── bronze/
│   └── {source}/{entity}/{year}/{month}/{day}/
├── silver/
│   └── {domain}/{entity}/{year}/{month}/
└── gold/
    └── {domain}/{entity}/
```

### Outils Azure

- **Bronze**: ADF Copy Activity, Event Hubs capture
- **Silver**: Databricks notebooks, Synapse Spark
- **Gold**: Synapse SQL pools, Power BI DirectQuery

---

## λ Lambda Architecture

### Description

Architecture combinant traitement batch (historique) et streaming (temps réel).

```
                    ┌─────────────────────────┐
                    │      Batch Layer        │
        ┌──────────→│  (ADF, Databricks)      │──────┐
        │           │  Traitement quotidien   │      │
        │           └─────────────────────────┘      │
        │                                            ↓
┌───────┴───────┐                           ┌───────────────┐
│    Sources    │                           │ Serving Layer │
│ (Events, DB)  │                           │   (Synapse)   │
└───────┬───────┘                           └───────────────┘
        │                                            ↑
        │           ┌─────────────────────────┐      │
        └──────────→│     Speed Layer         │──────┘
                    │ (Stream Analytics, EH)  │
                    │  Traitement temps réel  │
                    └─────────────────────────┘
```

### Composants

| Layer | Objectif | Azure Services |
|-------|----------|----------------|
| **Batch** | Traitement complet, historique | ADF, Databricks, Synapse |
| **Speed** | Traitement temps réel, approximations | Event Hubs, Stream Analytics |
| **Serving** | Vue unifiée des résultats | Synapse SQL, Cosmos DB |

### Use Cases

- Dashboard avec données historiques + temps réel
- Détection anomalies avec contexte historique
- Réconciliation batch/stream

---

## κ Kappa Architecture

### Description

Architecture stream-only où tout est traité comme des événements.

```
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│    Sources    │────→│  Event Log    │────→│   Stream      │
│   (Events)    │     │ (Event Hubs)  │     │  Processing   │
└───────────────┘     │  Append-only  │     │ (Spark/Flink) │
                      └───────────────┘     └───────┬───────┘
                                                    │
                                                    ↓
                                            ┌───────────────┐
                                            │ Serving Layer │
                                            │  (Cosmos DB)  │
                                            └───────────────┘
```

### Différence avec Lambda

| Aspect | Lambda | Kappa |
|--------|--------|-------|
| Layers | Batch + Speed | Stream uniquement |
| Reprocessing | Via batch layer | Rejouer depuis event log |
| Complexité | Plus élevée (2 codebases) | Plus simple (1 codebase) |
| Latence | Variable | Constante (temps réel) |

### Use Cases

- Systèmes event-sourced
- IoT avec traitement continu
- Applications sans besoin de batch historique

---

## 📊 Data Flow Patterns

### Pull Pattern (Polling)

```
ADF/Databricks  ──→  Source System  ──→  Transform  ──→  ADLS
      │                    ↑
      └────── Schedule ────┘
```

**Avantages**: Simple, contrôle côté consommateur  
**Inconvénients**: Latence, polling overhead

### Push Pattern (Event-Driven)

```
Source System  ──→  Event Hub  ──→  Function/Stream Analytics  ──→  ADLS
      │                                        ↑
      └─────────── Webhook/CDC ────────────────┘
```

**Avantages**: Temps réel, réactif  
**Inconvénients**: Plus complexe, gestion backpressure

### CDC (Change Data Capture)

```
Source DB  ──→  CDC Capture  ──→  Bronze  ──→  Silver  ──→  Gold
   │              (Debezium,         │
   │               SQL CDC)          │
   └──── Only changes ───────────────┘
```

**Avantages**: Efficace (deltas only), moins de load sur source  
**Inconvénients**: Infrastructure CDC, complexité

---

## 🔄 Integration Patterns

| Pattern | Use Case | Azure Tools |
|---------|----------|-------------|
| **Batch ETL** | Volumes importants, scheduled | ADF, Databricks |
| **Event-Driven** | Temps réel, réactif | Event Hubs, Functions |
| **API-First** | Microservices, REST | Functions, APIM |
| **CDC** | Sync incrémental | SQL CDC, Debezium, ADF |
| **File Drop** | Intégration legacy | Blob trigger, ADF |

---

## ⚠️ Error Handling Patterns

### Retry with Exponential Backoff

```
Attempt 1: wait 1s
Attempt 2: wait 2s
Attempt 3: wait 4s
Attempt 4: wait 8s
Attempt 5: wait 16s (ou fail)
```

### Dead Letter Queue (DLQ)

```
┌─────────────────┐
│ Main Processing │
└────────┬────────┘
         │
    Error? ──Yes──→ ┌─────────────────┐
         │          │  Dead Letter    │
         │          │  Queue/Blob     │
         │          └────────┬────────┘
         │                   │
         ↓                   ↓
    Continue           Manual Review
                       & Replay
```

### Circuit Breaker

```
State: Closed ──→ Failures > threshold ──→ Open
                                            │
                        ┌───────────────────┘
                        ↓
                     Timeout
                        │
                        ↓
                   Half-Open ──→ Success ──→ Closed
                        │
                        └──→ Failure ──→ Open
```

---

## 📚 Références

- [Medallion Architecture](https://learn.microsoft.com/azure/databricks/lakehouse/medallion)
- [Lambda Architecture](https://lambda-architecture.net/)
- [Kappa Architecture](https://www.oreilly.com/radar/questioning-the-lambda-architecture/)
- [Azure Event-Driven Architecture](https://learn.microsoft.com/azure/architecture/guide/architecture-styles/event-driven)
