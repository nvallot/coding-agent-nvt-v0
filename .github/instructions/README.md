# 📚 Instructions GitHub Copilot - Index

## 🗂️ Structure Refactorisée

```
instructions/
├── base/              # Globales (non-client-spécifique)
│   ├── agent-roles.md           → Définition rôles agents
│   ├── conventions.md           → Standards techniques globaux
│   └── azure-reference.md       → Services Azure + patterns
├── agents/            # Instructions par agent (core)
│   ├── architecte.md            → Architecture cloud
│   ├── business-analyst.md      → Analyse métier
│   ├── developpeur.md           → Implémentation code
│   └── reviewer.md              → Revue code & qualité
├── domains/           # Spécialités techniques détaillées
│   ├── azure-patterns.md        → Medallion, Lambda, CDC, etc.
│   ├── data-architecture.md     → Modélisation, gouvernance, qualité
│   ├── iac-terraform.md         → Infrastructure as Code
│   └── testing.md               → Stratégies test, couverture
├── contracts/         # Contrats de livrables
│   └── artefacts.md            → Format BRD, TAD, ADR, templates
└── README.md          # Ce fichier
```

## 🎯 Comment utiliser

### Pour les Agents

**Architecte** (`applyTo: "**/(docs|Deployment|architecture)/**"`):
1. `base/agent-roles.md` → Comprendre le rôle
2. `agents/architecte.md` → Instructions core
3. `domains/azure-patterns.md` + `iac-terraform.md` → Détails techniques
4. `contracts/artefacts.md` → Format TAD/ADR
5. `.github/clients/{key}/CLIENT.md` → Contexte client

**Développeur** (`applyTo: "**/(src|Functions|Development|*.cs|*.py|*.sql|*.tf)/**"`):
1. `agents/developpeur.md` → Instructions core
2. `domains/data-architecture.md` + `testing.md` → Détails techniques
3. `contracts/artefacts.md` → Contrats PR
4. `.github/clients/{key}/instructions/` → Conventions client

**Business Analyst** (`applyTo: "**/requirements/**,**/specifications/**,**/docs/**"`):
1. `agents/business-analyst.md` → Instructions core
2. `contracts/artefacts.md` → Format BRD
3. `.github/clients/{key}/CLIENT.md` → Contexte client

**Reviewer** (`applyTo: "**/(pull_requests|*.cs|*.py|*.sql)/**"`):
1. `agents/reviewer.md` → Checklist & sévérité
2. `base/conventions.md` → Standards
3. `domains/testing.md` → Couverture tests

## 📋 Principes Clés

1. **Client d'abord**: Charger `.github/clients/{key}/CLIENT.md` avant d'agir
2. **Pas de redondance**: Utiliser références plutôt que copier
3. **Concis**: Chaque fichier <500 lignes, l'essentiel seulement
4. **Clair**: Noms explicites, pas d'ambiguïté

## 🚫 Informations Client-Spécifiques

**DANS les instructions client UNIQUEMENT**:
- Conventions nommage (ex: SBM prefix)
- Standards architecture client
- Tags Azure client
- Règles sécurité client
- Formats internes

**PAS dans les fichiers common** (`base/`, `agents/`, `domains/`)

## Utilisation

### Pour les développeurs

Quand vous ouvrez un fichier correspondant à un pattern, Copilot charge automatiquement les instructions pertinentes.

**Exemple**: En ouvrant `src/Functions/ProcessOrder.cs`, Copilot charge `developpeur.instructions.md`

### Pour les agents

Les agents consultent les instructions:
- `@architecte` pour conception → charge `architecte.instructions.md`
- `@dev` pour implémentation → charge `developpeur.instructions.md`  
- `@ba` pour exigences → charge `business-analyst.instructions.md`
- `@reviewer` pour revue → charge `reviewer.instructions.md`

## Priorité des instructions

GitHub Copilot utilise cet ordre de priorité:

1. **Personal instructions** (instructions personnelles)
2. **Path-specific instructions** (`.github/instructions/*.instructions.md`)
3. **Repository-wide instructions** (`.github/copilot-instructions.md`)
4. **Organization instructions** (au niveau organization)

## Exclusions

Certaines instructions sont exclues pour certains agents:

- `architecte.instructions.md` → `excludeAgent: "code-review"`
- `business-analyst.instructions.md` → `excludeAgent: "code-review"`
- `developpeur.instructions.md` → `excludeAgent: "code-review"`
- `reviewer.instructions.md` → `excludeAgent: "coding-agent"` (code-review uniquement)

## Maintenance

### Mise à jour des instructions

1. Modifier le fichier `.instructions.md` concerné
2. Commit et push les changements
3. Les instructions s'appliquent immédiatement au prochain usage

### Ajout de nouvelles instructions

1. Créer un fichier `{agent-name}.instructions.md` dans `.github/instructions/`
2. Ajouter le frontmatter avec `applyTo` pattern
3. Ajouter la nouvelle ligne dans le tableau ci-dessus

## Ressources externes

- [GitHub Copilot Documentation - Repository Instructions](https://docs.github.com/en/copilot/how-tos/configure-custom-instructions/add-repository-instructions)
- [YAML Frontmatter Format](https://github.com/openai/agents.md)

## Questions?

Pour toute question ou amélioration des instructions, consultez:
- Documentation du projet: `.github/`
- Knowledge base: `.github/knowledge/`
- Skills: `.github/skills/`

---

**Last updated**: 2026-02-04  
**Version**: 1.0.0
