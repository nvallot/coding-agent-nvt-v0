---
applyTo: "**/(docs|Deployment|architecture|src)/**"
excludeAgent: "code-review"
---

# 📐 Azure Patterns & Architectures

## Medallion Architecture (Recommandé)
```
Bronze (Raw)  →  Silver (Cleaned)  →  Gold (Curated)
↓                ↓                      ↓
ADLS raw/       ADLS refined/         ADLS curated/
Unvalidated     Deduplicated          Business-ready
Partitioned     Validated             Aggregated
```

**Bronze**: COPY as-is, format original, partition by ingestion_date
**Silver**: Deduplicate, transform, validate, clean data types, partition by business key
**Gold**: Aggregate, optimize for reporting, add business logic, denormalize for performance

## Lambda Architecture (Batch + Stream)
```
Source → Batch Layer (daily)  ↘
         Stream Layer (RT)     → Merge View
Data Lake (immutable)          ↗ Serving Layer
```

For events: Store raw in Bronze, process both batch & stream in parallel

## Kappa Architecture (Stream Only)
```
Kafka Topic → Stream Processing → Served View
↓
Event Log (append-only)
```

For: Real-time only, no batch reprocessing needed

## Data Flow Patterns

**Pull Pattern**:
```
ADF → Source System → Transform → ADLS
```
Pros: Simple, one-way
Cons: Latency, polling overhead

**Push Pattern**:
```
Source System → Event Hub → Stream Analytics → ADLS
```
Pros: Real-time, event-driven
Cons: More complex

**CDC (Change Data Capture)**:
```
Source DB → CDC Capture → Bronze → Silver → Gold
```
Pros: Only changes, no full refresh
Cons: Infrastructure overhead

## Integration Patterns

| Pattern | Use Case | Tools |
|---------|----------|-------|
| Batch ETL | Scheduled, volumes large | ADF, Databricks |
| Event-Driven | Real-time, react to events | Event Hubs, Stream Analytics |
| API-First | Microservices, REST | Functions, APIM |
| CDC | Incremental sync | SQL CDC, Kafka |

## Error Handling Patterns

**Retry with Exponential Backoff**:
```
Attempt 1: wait 1s
Attempt 2: wait 2s
Attempt 3: wait 4s
...
Max: 5 attempts, 64s total
```

**Dead Letter Queue**:
```
Main Processing ← Errors → DLQ (blob/queue)
                            ↓
                        Investigation & Replay
```

**Graceful Degradation**:
- Continue with available data
- Log degradation
- Alert on threshold

## Optimization Patterns

**Caching**:
- Hot data: Redis/Cache
- Cold data: ADLS with partitioning
- Result caching: 1-24h TTL

**Partitioning**:
- Time: partition by date (daily/monthly)
- Geography: partition by region
- Key: partition by business entity

**Indexing** (SQL/Synapse):
- Clustered: PK (sort order)
- Non-clustered: Frequent filters
- Columnstore: Analytics queries
