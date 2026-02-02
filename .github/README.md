# .github – Copilot Custom Agents (v0.1)

Ce dossier contient la configuration minimale et propre pour utiliser des **custom agents** Copilot dans ce repo.

Sources officielles :
- GitHub Copilot – Custom agents configuration: https://docs.github.com/en/copilot/reference/custom-agents-configuration
- VS Code – Custom agents: https://code.visualstudio.com/docs/copilot/customization/custom-agents

## Structure

```
.github/
├── agents/                        # Custom agents (.agent.md)
│   ├── business-analyst.agent.md  # model: gpt-4o, temp: 0.6
│   ├── solution-architect.agent.md # model: claude-sonnet-4.5, temp: 0.5
│   └── developer.agent.md         # model: gpt-4o, temp: 0.3
├── clients/                       # Client-specific configs
│   ├── active-client.json         # Current active client
│   └── sbm/                       # SBM Offshore
│       ├── README.md
│       ├── CLIENT.md              # Context and priorities
│       ├── mcp.json               # MCP servers and tools
│       └── instructions/          # SBM-specific instructions (path-based)
│           ├── README.md
│           ├── sbm.system.md      # applyTo: clients/sbm/**, priority: 10
│           └── sbm-isp-architecture-guidelines.md # priority: 20
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
├── skills/                        # Generic task procedures
│   ├── README.md
│   ├── azure-function-deployment/
│   ├── bicep-deployment/
│   └── service-bus-setup/
├── prompts/                       # Prompt files templates
│   ├── README.md
│   ├── architecture-design.prompt
│   ├── code-review.prompt
│   ├── requirements-analysis.prompt
│   └── brainstorm.prompt
├── copilot-config.json            # Global model config & routing
├── mcp-server.json                # Global MCP servers config
└── README.md
```

## Concepts

### Agents
Custom agents (`.agent.md`) define specialized AI personas with model config:
- **Business Analyst** (gpt-4o, temp: 0.6): requirements and functional specs
- **Solution Architect** (claude-sonnet-4.5, temp: 0.5): technical architecture
- **Developer** (gpt-4o, temp: 0.3): implementation

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
Invoked explicitly by user with `/` command.

Located in `.github/prompts/*.prompt` with variables `{{var}}`.

## Configuration Files

- **copilot-config.json** : Model configuration, routing, temperature
- **mcp-server.json** : Global MCP servers (Azure, GitHub, etc.)
- **clients/active-client.json** : Current active client key

## Démarrage rapide

1. Ouvrir le repo dans VS Code avec model config, instructions path-based, knowledge, skills, prompts
2. Choisir un agent dans la liste **Agents** dropdown
3. Les règles et la base de connaissance sont automatiquement chargées selon le contexte

## Version

- v0.1 – Structure initiale propre (agents, instructions, knowledge, skills SBM)
