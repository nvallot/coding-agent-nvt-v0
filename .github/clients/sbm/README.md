# Client SBM (v0.1)

## Structure

```
clients/sbm/
├── CLIENT.md                    # Client context and priorities
├── mcp.json                     # MCP servers and tools config
├── instructions/                # SBM-specific instructions
│   ├── AGENTS.base.md          # Base rules for SBM agents
│   ├── sbm.system.md           # System prompt for SBM
│   └── sbm-isp-architecture-guidelines.md  # Architecture best practices
└── README.md                    # This file
```

## Loading Order

When working on SBM projects, agents load:

1. `instructions/AGENTS.base.md` (universal rules)
2. `clients/sbm/instructions/AGENTS.base.md` (SBM agent rules)
3. `clients/sbm/instructions/sbm.system.md` (SBM system prompt)
4. `clients/sbm/CLIENT.md` (context and priorities)
5. `clients/sbm/mcp.json` (tool restrictions)
6. `knowledge/sbm/` (dynamically loaded as needed)

## Knowledge Base

SBM knowledge is located in `knowledge/sbm/`:
- `naming-conventions.md`: Azure resource naming (SBWE1-ISP-...)
- `security-guidelines.md`: Key Vault, RBAC, Managed Identity
- `monitoring-and-observability.md`: Logs, metrics, CorrelationId
- `integration-principles.md`: ISP integration patterns
- And more...

## Tools (MCP)

See `mcp.json` for configured MCP servers and tool restrictions.

## Version

v0.1 – Initial clean structure
