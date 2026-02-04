# GitHub Copilot Agents Configuration

Ce projet utilise des agents GitHub Copilot personnalisés pour automatiser différents aspects du workflow de développement.

## Vue d'ensemble

```
Exigences Métier → Analyse → Architecture → Développement → Revue → Production
     @ba         →   @ba   →   @archi    →     @dev      → @rev  →
```

## Agents disponibles

### 🏗️ Agent Architecte (`@architecte`)
- **Rôle**: Solution Architect senior
- **Spécialité**: Conception d'architecture Azure, design système
- **S'active sur**: Fichiers dans `/docs`, `/Deployment`, `/architecture`
- **Livrables**: TAD, Diagrammes (C4), ADRs, Infrastructure as Code (Terraform), Estimation coûts

**Instructions**: [.github/instructions/architecte.instructions.md](.github/instructions/architecte.instructions.md)

### 👤 Agent Business Analyst (`@ba`)
- **Rôle**: Expert en analyse métier
- **Spécialité**: Recueil d'exigences, analyse de données, user stories
- **S'active sur**: Fichiers dans `/requirements`, `/specifications`, `/docs`
- **Livrables**: BRD, Data Mapping, User Stories, Acceptance criteria

**Instructions**: [.github/instructions/business-analyst.instructions.md](.github/instructions/business-analyst.instructions.md)

### 💻 Agent Développeur (`@dev`)
- **Rôle**: Développeur expert Azure
- **Spécialité**: Implémentation code, pipelines data, Azure services
- **S'active sur**: Fichiers `.cs`, `.py`, `.sql`, `.tf`, `/src`, `/Functions`, `/Development`
- **Livrables**: Code production, tests, documentation, Infrastructure as Code

**Instructions**: [.github/instructions/developpeur.instructions.md](.github/instructions/developpeur.instructions.md)

### 🔍 Agent Reviewer (`@reviewer`)
- **Rôle**: Expert en revue de code
- **Spécialité**: Qualité, sécurité, performance, compliance
- **S'active sur**: Pull requests, fichiers `.cs`, `.py`, `.sql`
- **Livrables**: Rapport de revue détaillé, security audit, recommandations

**Instructions**: [.github/instructions/reviewer.instructions.md](.github/instructions/reviewer.instructions.md)

## Structure du projet

```
agent-nvt-v1/
├── .github/
│   ├── agents/
│   │   ├── architecte.md           # Définition agent (format original)
│   │   ├── business-analyst.md
│   │   ├── developpeur.md
│   │   └── reviewer.md
│   ├── instructions/               # Instructions GitHub Copilot (NEW)
│   │   ├── README.md
│   │   ├── architecte.instructions.md
│   │   ├── business-analyst.instructions.md
│   │   ├── developpeur.instructions.md
│   │   ├── reviewer.instructions.md
│   │   └── copilot-instructions.md (repository-wide)
│   ├── clients/                    # Configuration par client
│   ├── knowledge/                  # Base de connaissances
│   └── skills/                     # Compétences spécialisées
├── docs/                           # Documentation
├── src/                            # Code source
├── infrastructure/                 # Infrastructure as Code
├── tests/                          # Tests automatisés
├── VALIDATION.md
├── README.md
└── [autres fichiers]
```

## Format des instructions

Les instructions respektent le standard GitHub Copilot avec frontmatter YAML:

```yaml
---
applyTo: "glob/pattern/**"
excludeAgent: "code-review" | "coding-agent"
---

# Instructions en Markdown
```

### Paramètres

| Paramètre | Format | Exemple | Description |
|-----------|--------|---------|-------------|
| `applyTo` | glob pattern | `**/*.py`, `src/**/*.ts` | Fichiers auxquels appliquer les instructions |
| `excludeAgent` | string enum | `"code-review"` ou `"coding-agent"` | Agent à exclure (optionnel) |

### Glob patterns valides

```
*                    - Tous les fichiers du dossier courant
**/*.py              - Tous les fichiers .py récursivement
src/**/*.ts          - Tous les .ts sous src/
**/test/**           - Tous les fichiers dans n'importe quel dossier test/
docs|src|tests       - Dossiers alternatifs (pipe = OR)
```

## Workflow recommandé

### 1️⃣ Phase d'analyse (Business Analyst)

```bash
@ba "Analyser les exigences pour [projet]"
# → Produit BRD, Data Mapping, User Stories
```

**Fichiers de sortie**:
- `docs/brd-[project].md`
- `docs/data-mapping.md`
- `docs/user-stories.md`

### 2️⃣ Phase de conception (Architect)

```bash
@architecte "Concevoir l'architecture pour [exigences]"
# → Produit TAD, Diagrammes, ADRs, Terraform, Coûts
```

**Fichiers de sortie**:
- `docs/architecture/tad-[project].md`
- `docs/architecture/diagrams/`
- `infrastructure/terraform/`
- `docs/adrs/`

### 3️⃣ Phase de développement (Developer)

```bash
@dev "Implémenter [composant] selon [architecture]"
# → Produit Code, Tests, Documentation
```

**Fichiers de sortie**:
- `src/`
- `tests/`
- `README.md`

### 4️⃣ Phase de revue (Reviewer)

```bash
@reviewer "Faire la revue de code pour PR #[n]"
# → Produit Rapport détaillé, Recommandations
```

**Fichiers de sortie**:
- `REVIEW-[pr-number].md`

## Configuration par client

Chaque client a sa configuration spécifique:

```
.github/clients/
├── active-client.json              # Client courant
├── {clientKey}/
│   ├── CLIENT.md
│   ├── instructions/
│   │   ├── naming.md
│   │   ├── architecture.md
│   │   └── conventions.md
│   └── knowledge/
```

**Avant de travailler avec un agent**, vérifiez le client actif:

```bash
cat .github/clients/active-client.json
# → { "clientKey": "nadia", "name": "NADIA" }
```

## Best Practices

### ✅ À faire

1. **Charger le client context** avant de lancer un agent
   ```bash
   @architecte "Client: NADIA. Concevoir architecture pour..."
   ```

2. **Utiliser les patterns `applyTo` correctement**
   - Spécifique mais pas trop restrictif
   - Recouvrir tous les types de fichiers pertinents

3. **Mettre à jour les instructions régulièrement**
   - Après un changement de standards
   - Après retours d'expérience
   - Lors d'ajout de nouvelles librairies/frameworks

4. **Documenter les décisions** dans les ADRs
   - Pourquoi cette approche?
   - Alternatives considérées
   - Conséquences

### ❌ À éviter

1. ❌ Secrets ou credentials dans les fichiers
2. ❌ Patterns `applyTo` trop larges (ex: `**`)
3. ❌ Instructions contradictoires
4. ❌ Oublier de charger le CLIENT.md du client

## Dépannage

### Les instructions ne s'appliquent pas

1. Vérifier le pattern `applyTo` avec le chemin du fichier
   ```bash
   # Pattern: **/*.py
   # Fichier: src/transformations.py ✅ Match
   # Fichier: requirements.txt ❌ No match
   ```

2. Vérifier l'`excludeAgent`
   ```bash
   # Si le fichier a excludeAgent: "coding-agent"
   # → Les agents de code (dev) ne le chargeront pas
   ```

3. Recharger le contexte
   - Fermer et rouvrir le chat Copilot
   - Recharger le repo dans GitHub Copilot

### Conflit d'instructions

Si plusieurs instructions s'appliquent:
1. **Path-specific** gagne toujours sur **repository-wide**
2. L'ordre de priorité est défini par la spécificité du pattern

Exemple:
```
Pattern 1: **/*.py           (moins spécifique)
Pattern 2: src/**/*.py       (plus spécifique) ← Gagne
```

## Ressources

- 📖 [GitHub Copilot Documentation](https://github.com/features/copilot/)
- 📚 [Repository Instructions Guide](https://docs.github.com/en/copilot/how-tos/configure-custom-instructions/add-repository-instructions)
- 🔗 [openai/agents.md](https://github.com/openai/agents.md) - Format AGENTS.md
- 📋 [Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)

## Maintenance & Évolution

### Ajouter un nouvel agent

1. Créer `{agent-name}.md` dans `.github/agents/`
2. Créer `{agent-name}.instructions.md` dans `.github/instructions/`
3. Ajouter le frontmatter avec `applyTo`
4. Documenter dans ce README

### Mettre à jour les instructions

```bash
# Éditer directement le fichier
# .github/instructions/{agent-name}.instructions.md

# Commit et push
git add .github/instructions/
git commit -m "chore: update agent instructions"
git push
```

Les instructions s'appliquent immédiatement au prochain usage.

---

**Version**: 1.0.0  
**Last updated**: 2026-02-04  
**Format**: GitHub Copilot Path-specific Instructions
