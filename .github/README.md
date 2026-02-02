# .github – Copilot Custom Agents (v0.1)

Ce dossier contient la configuration minimale et propre pour utiliser des **custom agents** Copilot dans ce repo.

Sources officielles :
- GitHub Copilot – Custom agents configuration: https://docs.github.com/en/copilot/reference/custom-agents-configuration
- VS Code – Custom agents: https://code.visualstudio.com/docs/copilot/customization/custom-agents

## Structure

```
.github/
├── agents/                        # Custom agents (.agent.md)
│   ├── business-analyst.agent.md
│   ├── solution-architect.agent.md
│   └── developer.agent.md
├── clients/                       # Client-specific configs
│   ├── active-client.json         # Current active client
│   └── sbm/                       # SBM Offshore
│       ├── README.md
│       ├── CLIENT.md              # Context and priorities
│       ├── mcp.json               # MCP servers and tools
│       └── instructions/          # SBM-specific instructions
│           ├── AGENTS.base.md
│           ├── sbm.system.md
│           └── sbm-isp-architecture-guidelines.md
├── instructions/                  # Generic instructions (all clients)
│   ├── AGENTS.base.md             # Universal agent rules
│   ├── HIERARCHY.md               # Instruction loading hierarchy
│   ├── README.md                  # Index
│   ├── contracts/                 # Agent contracts
│   │   ├── artefacts-contract.md
│   │   └── flow-contract.md
│   └── common/                    # Shared (multi-client)
│       ├── README.md
│       ├── azure.general.md
│       ├── azure.functions.md
│       ├── azure.adf.md
│       ├── azure.apim.md
│       ├── azure.security.md
│       ├── azure.observability.md
│       └── azure.terraform.md
├── knowledge/                     # Reference information
│   └── sbm/                       # SBM knowledge base
│       ├── README.md
│       ├── naming-conventions.md
│       ├── security-guidelines.md
│       └── ... (dynamically loaded)
└── skills/                        # Generic task procedures
    ├── azure-function-deployment/
    ├── bicep-deployment/
    └── service-bus-setup/
```

## Concepts

### Agents
Custom agents (`.agent.md`) define specialized AI personas:
- Business Analyst: requirements and functional specs
- Solution Architect: technical architecture
- Developer: implementation

### Instructions
Define **how agents behave**:
- `instructions/AGENTS.base.md`: universal rules (all clients)
- `clients/<client>/instructions/`: client-specific behavior

Loaded at agent startup (keep limited: 5-7 files max).

### Knowledge
Reference information **dynamically loaded** as needed:
- Naming conventions
- Architecture guidelines
- Security policies
- etc.

NOT loaded at startup, retrieved via semantic search or explicit reference.

### Skills
Describe **how to perform specific tasks** step-by-step:
- Azure Function deployment
- Bicep infrastructure provisioning
- Service Bus configuration

Referenced in instructions, invoked dynamically when needed.

### Prompt Files
Templates for specific use cases (brainstorm, code review, etc.).
Invoked explicitly by user.

(Note: Prompt files not yet implemented in v0.1)

## Démarrage rapide

1. Ouvrir le repo dans VS Code
2. Choisir un agent dans la liste **Agents** dropdown
3. Les règles et la base de connaissance sont automatiquement chargées selon le contexte

## Version

- v0.1 – Structure initiale propre (agents, instructions, knowledge, skills SBM)
