---
applyTo: "**/(src|Functions|Development)/**"
excludeAgent: "code-review"
---

# 🗂️ Data Architecture

## Data Modeling

### Conceptual Model
Entities & Relationships (business language)
```
Customer --< Order >-- Product
  |
  └─ Address
```

### Logical Model
Tables, attributes, constraints (no DBMS specifics)
```
Customer: ID, Name, Email, Created
Order: ID, CustomerID, OrderDate, Total
```

### Physical Model
Implementation-specific (indexes, partitions, storage)
```
Bronze: Raw format, nullable
Silver: Typed, PK defined, FK constraints
Gold: Denormalized, optimized for query
```

## Data Governance

**Lineage**: Document source → transformation → destination
```
ERP System (source)
    ↓ (ADF Copy activity)
Bronze/Orders (raw)
    ↓ (Databricks notebook)
Silver/Orders (cleaned)
    ↓ (SQL aggregation)
Gold/OrderSummary (curated)
```

**Metadata**: Store in Purview
- Data owner
- Classification (public/sensitive/restricted)
- Refresh frequency
- Quality SLA

**Quality Rules**:
- Null check: Required fields non-null
- Type check: Date format YYYY-MM-DD
- Range check: Price > 0
- Uniqueness: ID is PK
- Referential: FK exists in parent

## Data Security

**Classification Levels**:
```
Public        → ADLS public/ → Anyone
Internal      → ADLS internal/ → Employees
Confidential   → ADLS confidential/ → Data owners
Restricted    → ADLS restricted/ → Admin only
```

**Access Control**:
- Storage: RBAC on container/folder level
- Database: Column-level security, Row-level security
- Never grant Individual, use Groups

**Encryption**:
- At-rest: Enabled by default (AES-256)
- In-transit: HTTPS/TLS mandatory
- Key Vault: CMK (Customer Managed Keys) if required

## Data Quality

**Profiling** (initial):
- Distinct count, null %, data types
- Min/max, patterns
- Outlier detection

**Monitoring** (ongoing):
```
Metric              | Threshold | Action
--------------------|-----------|-------
Null % in critical  | >5%       | Alert & investigate
Late arrivals       | >10 min   | Log & continue
Volume deviation    | ±30%      | Alert data owner
Quality score       | <98%      | Block downstream
```

**Validation Framework**:
```python
# Bronze (incoming)
assert df.count() > 0, "No data received"
assert "order_id" in df.columns, "Missing PK"

# Silver (transformed)
assert df.filter(F.col("order_date").isNull()).count() == 0, "Null dates"
assert df.select(F.countDistinct("order_id")).collect()[0][0] == df.count(), "Duplicates"

# Gold (curated)
assert df.filter(F.col("total") < 0).count() == 0, "Negative totals"
```

## Data Retention & Archival

```
Frequency | Retention | Storage | Access
-----------|-----------|---------|--------
Real-time | 7 days    | Hot     | <100ms
Daily     | 90 days   | Warm    | <1s
Monthly   | 7 years   | Cold    | <1h
Compliance| Permanent | Archive | Slow
```

**Policies**:
- Archival: Move to Archive Storage after cold period
- Deletion: Comply with GDPR/regulations (right to be forgotten)
- Backup: Daily snapshots for recovery
