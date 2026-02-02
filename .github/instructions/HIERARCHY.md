# Instructions Hierarchy

Les **instructions** définissent **comment les agents se comportent**.

## Hiérarchie de Chargement

Chaque agent charge les instructions dans cet ordre (croissant de spécificité) :

1. **`AGENTS.base.md`** – Universel (tous les clients, tous les agents)
   - Core principles
   - Quality standards
   - Naming conventions (generic)
   - Deliverable structure

2. **`common/`** – Partagé par plusieurs clients
   - Azure standards (si multi-client Azure)
   - Architecture patterns
   - DevOps processes
   - **Optionnel** : chargé selon le client

3. **`clients/<client-key>/instructions/`** – Client-spécifique
   - System prompt du client
   - Conventions client (naming, security)
   - Project-specific guidelines
   - Overrides des instructions universelles

## Structure

```
instructions/
├── README.md (this file)
├── AGENTS.base.md (UNIVERSAL - required for all agents)
├── contracts/ (universal)
│   ├── artefacts-contract.md
│   └── flow-contract.md
└── common/ (shared, optional)
    └── README.md (when to use)
```

## For New Clients

Créer : `clients/<nouveau-client>/instructions/`

Inclure minimum :
- `<client>.system.md` (system prompt)
- Reference to AGENTS.base.md (hérité automatiquement)
- Overrides spécifiques si besoin

Exemple SBM :
```
clients/sbm/instructions/
├── sbm.system.md (client context)
└── sbm-isp-architecture-guidelines.md (project-specific)
```

## Agent Context Loading

Exemple pour tous les agents (BA, SA, Dev) :

```markdown
## Contexte à charger

- instructions/AGENTS.base.md (universel)
- instructions/common/ (si applicable au client)
- clients/<client-key>/instructions/ (client-spécifique)
- clients/<client-key>/CLIENT.md (contexte client)
- knowledge/<client-key>/ (chargé dynamiquement)
```

Note: `<client-key>` est défini dans `clients/active-client.json`
