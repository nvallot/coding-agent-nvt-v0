# Knowledge Base (v0.1)

## Purpose

This folder contains **reference information** dynamically loaded by agents as needed:
- Naming conventions
- Architecture guidelines
- Integration principles
- Security policies
- Monitoring standards
- Decision records
- etc.

## Organization

Knowledge is organized by client:

```
knowledge/
└── sbm/                           # SBM Offshore
    ├── README.md                  # Index of SBM knowledge
    ├── naming-conventions.md
    ├── security-guidelines.md
    ├── monitoring-and-observability.md
    ├── integration-principles.md
    ├── current-architecture.md
    ├── target-principles.md
    ├── data-contracts.md
    ├── data-quality-rules.md
    ├── retry-and-recovery.md
    ├── incident-handling.md
    ├── sla-and-slo.md
    ├── decision-records.md
    ├── deployment-pipeline.md
    ├── sbm-integration-services-platform.md
    └── sbm-isp-projects-overview.md
```

## Usage

- **Knowledge is NOT loaded at agent startup** (unlike instructions)
- **Agents retrieve knowledge dynamically** based on context and user queries
- **Knowledge files are the source of truth** for client-specific standards

## Difference with Instructions

| Instructions | Knowledge |
|--------------|-----------|
| Define agent behavior | Provide reference information |
| Loaded at startup | Loaded dynamically |
| Limited files (5-7) | Unlimited files |
| How to behave | What to know |

## Adding New Knowledge

1. Create markdown file in `knowledge/<client>/`
2. Update `knowledge/<client>/README.md` index
3. Agent will discover it via semantic search or explicit reference
