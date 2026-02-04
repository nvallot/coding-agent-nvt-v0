---
applyTo: "**/*"
---

# 📋 Contrats de Livrables

## 📄 Business Requirements Document (BRD)

**Fichier**: `docs/brd-{project}.md`

**Structure minimale**:
```markdown
# Business Requirements Document - {Project}

## Executive Summary
- Overview (2-3 phrases)
- Business Problem
- Proposed Solution
- Expected Benefits

## Business Context
- Organization Overview
- Current State & Pain Points

## Functional Requirements (RF)
### Priority MoSCoW
- Must: Critical for success
- Should: Important but not blockers
- Could: Nice-to-have
- Won't: Explicitly excluded

### User Stories
```
As a [role], I want to [action] so that [benefit]

Acceptance Criteria:
- [ ] Criterion 1
- [ ] Criterion 2
```

## Non-Functional Requirements (RNF)
- Performance: SLA, latency targets
- Reliability: Uptime targets
- Security: Compliance, data classification
- Scalability: Volume, growth projections

## Success Criteria (KPIs)
| Metric | Baseline | Target | Timeline |
|--------|----------|--------|----------|
| ... | ... | ... | ... |
```

---

## 🏗️ Technical Architecture Document (TAD)

**Fichier**: `docs/architecture/tad-{project}.md`

**Structure minimale**:
```markdown
# Technical Architecture Document - {Project}

## Executive Summary
- Problem statement
- Solution overview
- Key benefits
- High-level approach

## Business Context
- Requirements summary (reference RF/RNF)
- Success criteria (reference KPIs)

## Architecture Overview
### C4 Context Diagram
[Mermaid or DrawIO]

### C4 Container Diagram
[Mermaid or DrawIO]

## Detailed Component Design
### [Component 1: Azure Data Factory]
- Purpose
- Inputs & Outputs
- Configuration
- Error Handling
- Monitoring & Logging

### [Component 2: Databricks]
- Notebooks
- Cluster configuration
- Partitioning strategy
- Performance optimization

[... repeat for each component]

## Data Model
### Conceptual Model
[ER Diagram]

### Logical Model
[Table structures, relationships]

### Physical Model
### Bronze Layer
- Raw data storage
- Partitioning strategy
- Data format

### Silver Layer
- Data transformations
- Validation rules
- Quality metrics

### Gold Layer
- Business aggregations
- Denormalization patterns
- Performance indexes

## Architecture Decision Records (ADRs)
### ADR-001: [Decision Title]
- **Context**: Problem & drivers
- **Options Considered**: 
  - Option A: Pros/Cons/Costs
  - Option B: Pros/Cons/Costs
- **Decision**: Option chosen + justification
- **Consequences**: Positive/negative impacts
- **Implementation**: Steps to implement

[... repeat for each major decision]

## Non-Functional Requirements Matrix
| Requirement | Status | Evidence |
|-------------|--------|----------|
| Latency <15min | ✅ | ADF trigger interval |
| Availability 99.9% | ✅ | Multi-region setup |
| Cost <$5K/month | ✅ | Reserved instances |

## Cost Estimation
- Ingestion cost (data movement)
- Compute cost (processing)
- Storage cost (data at rest)
- Networking cost (data egress)
- **Total monthly**: $X.XX
- **Annual**: $X.XX

## Deployment Strategy
- Infrastructure as Code: Terraform
- CI/CD: Azure DevOps / GitHub Actions
- Environments: dev → stg → prod
- Rollback strategy

## Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|-----------|
| Data quality issues | High | Validation framework in Silver |
| Cost overrun | High | Monitoring, reserved instances |

## Future Enhancements (Phase 2+)
- Real-time streaming (Phase 2)
- Machine Learning integration (Phase 3)
```

---

## 🔄 Architecture Decision Record (ADR)

**Fichier**: `docs/architecture/adr-{number}-{title}.md`

**Format**:
```markdown
# ADR-001: Use Medallion Architecture

**Status**: Accepted (or Proposed/Deprecated)

**Date**: 2026-02-04

## Context
[Explain the business & technical drivers]
- Need for data quality
- Need for performance
- Cost constraints
- Scalability requirements

## Options Considered

### Option A: Medallion (Bronze → Silver → Gold)
**Pros**:
- Clear data quality progression
- Easy to understand and maintain
- Standard in modern data platforms

**Cons**:
- Storage overhead (data at rest tripled)
- Transformation complexity

**Costs**: ~$5K/month storage

### Option B: Lambda Architecture (Batch + Stream)
**Pros**:
- Real-time + historical data
- Flexible processing

**Cons**:
- Complex to maintain
- Higher cost (~$8K/month)

**Costs**: ~$8K/month

### Option C: Data Warehouse only (no lake)
**Pros**:
- Simple query model
- Direct to reporting

**Cons**:
- Schema-on-write (loses flexibility)
- Difficult to add new sources

**Costs**: ~$3K/month (but long-term inflexible)

## Decision
**We choose Option A: Medallion Architecture** because:
1. Aligns with Azure best practices
2. Provides flexibility for future requirements
3. Reasonable cost vs benefit
4. Team experience with pattern

## Consequences

### Positive
- Easy to maintain and explain
- Standard tooling available
- Good for compliance auditing

### Negative
- Higher storage costs
- Requires discipline in silver layer rules
- More complex than simple warehouse

## Implementation Plan
1. Week 1: Setup ADLS Gen2 with bronze/silver/gold containers
2. Week 2: Implement validation framework
3. Week 3: Build first data flow
4. Week 4: Optimize & document
```

---

## ✅ Code Review Checklist (Pull Request)

**Fichier**: Auto-generated template in `.github/pull_request_template.md`

```markdown
# Code Review: [Feature/Fix Title]

## Description
[Summary of changes]

## Type of change
- [ ] Bug fix
- [ ] New feature
- [ ] Architecture refactor
- [ ] Infrastructure update

## Testing
- [ ] Unit tests added (>80% coverage)
- [ ] Integration tests passed
- [ ] Manual testing done

## Checklist
- [ ] Code follows style guide (conventions.md)
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No secrets hardcoded
- [ ] Error handling proper
- [ ] Logging structured (JSON)
- [ ] No breaking changes

## Reviewer Attention
[Any specific areas to focus on]

## Deployment Considerations
- [ ] Infrastructure changes needed
- [ ] Database migration required
- [ ] Feature flag needed
```

---

## 📚 README Templates

### Terraform Module README
```markdown
# Storage Module

Creates Azure Data Lake Storage with security best practices.

## Usage

\`\`\`hcl
module "storage" {
  source = "./modules/storage"
  
  project     = "nadia"
  environment = "dev"
  location    = "eastus"
}
\`\`\`

## Variables

| Name | Type | Default | Description |
|------|------|---------|-------------|
| project | string | - | Project name |
| environment | string | - | dev/stg/prod |

## Outputs

| Name | Description |
|------|-------------|
| storage_account_id | ID of storage account |
| storage_account_url | HTTPS endpoint |
```

### Notebook README
```markdown
# Transform Orders Notebook

Transforms raw orders from Bronze to Silver layer.

## Purpose
- Deduplicate orders
- Validate data types & ranges
- Standardize column names

## Inputs
- Table: `bronze.orders` (incoming raw data)
- Widgets:
  - `execution_date`: Date to process

## Outputs
- Table: `silver.orders` (cleaned data)
- Metrics: Row count, null %, duplicates

## Quality Rules
- No null in `order_id`
- `order_amount` > 0
- `order_date` between 2020-01-01 and today

## Schedule
- Trigger: Daily at 02:00 UTC
- SLA: <15 min to complete
```
