# 📋 Référence rapide - Agents & Patterns

## 🎯 Quick Reference

### Agents disponibles

```
┌─────────────────────────────────────────────────────┐
│ Workflow: BA → ARCHI → DEV → REVIEWER              │
├─────────────────────────────────────────────────────┤
│ @ba        Business Analyst       Requirements      │
│ @archi     Architecte             Design            │
│ @dev       Développeur            Implementation    │
│ @reviewer  Reviewer               Quality check     │
└─────────────────────────────────────────────────────┘
```

## 📁 Patterns `applyTo`

| Agent | Pattern | Fichiers matchés | Dossiers matchés |
|-------|---------|------------------|------------------|
| **@ba** | `**/requirements/**,**/specifications/**,**/docs/**` | Tous dans ces dossiers | requirements/, specifications/, docs/ |
| **@archi** | `**/(docs\|Deployment\|architecture)/**` | Tous dans ces dossiers | docs/, Deployment/, architecture/ |
| **@dev** | `**/(src\|Functions\|Development\|*.cs\|*.py\|*.sql\|*.tf)/**` | *.cs, *.py, *.sql, *.tf + dossiers | src/, Functions/, Development/, tests/ |
| **@reviewer** | `**/(pull_requests\|*.cs\|*.py\|*.sql)/**` | *.cs, *.py, *.sql files + PR | Code files dans PRs |

## 🔄 Exemples de fichiers

### Business Analyst s'applique sur:
```
✅ docs/requirements/...
✅ docs/specifications/...
✅ docs/user-stories.md
✅ docs/brd.md
❌ src/code.py
```

### Architecte s'applique sur:
```
✅ docs/architecture/tad.md
✅ Deployment/Terraform/main.tf
✅ architecture/diagrams.md
❌ src/code.py
❌ docs/requirements/needs.md
```

### Développeur s'applique sur:
```
✅ src/pipelines/pipeline.py
✅ src/Functions/handler.cs
✅ src/SQL/queries.sql
✅ infrastructure/terraform/variables.tf
✅ tests/test_pipeline.py
❌ docs/architecture/tad.md
```

### Reviewer s'applique sur:
```
✅ Automatique sur les Pull Requests
✅ Fichiers: *.cs, *.py, *.sql
✅ Au moment de la revue de code
❌ @dev ne le charge pas (excludeAgent: coding-agent)
```

## 📝 Frontmatter Format

### Standard minimal
```yaml
---
applyTo: "**/*.py"
---

# Contenu
```

### Avec exclusion
```yaml
---
applyTo: "src/**/*.py"
excludeAgent: "code-review"
---

# Contenu
```

### Patterns multiples
```yaml
---
applyTo: "src/**/*.py,tests/**/*.py,scripts/**/*.py"
---

# Contenu
```

## 🎯 Patterns Glob - Cheat Sheet

| Pattern | Exemple match | Non-match |
|---------|---------------|-----------|
| `*` | `file.py` | `dir/file.py` |
| `**/*` | Tout | Nothing |
| `**/*.py` | `a/b/c.py`, `x.py` | `a.txt` |
| `src/**` | `src/file.py`, `src/a/b/c.py` | `other/file.py` |
| `src/*` | `src/file.py` | `src/a/file.py` |
| `src/**/*.py` | `src/a/b.py` | `other/a/b.py` |
| `**/test/**` | `test/a.py`, `a/test/b.py` | `testing/a.py` |
| `(a\|b\|c)/**` | `a/file.py`, `b/x`, `c/y/z` | `d/file.py` |
| `docs\|Deployment\|arch` | Dossiers doc, Deployment, ou arch | Autres |

## 🚀 Commandes rapides

### Utiliser @ba
```bash
cd docs/requirements
@ba "Analyser les exigences pour..."
```

### Utiliser @archi
```bash
cd docs/architecture
@archi "Concevoir l'architecture pour..."
```

### Utiliser @dev
```bash
cd src
@dev "Implémenter..."
```

### Utiliser @reviewer
```bash
# Automatique sur les PRs ou:
@reviewer "Revue du code dans..."
```

## 📊 Structure des livrables par agent

### @ba Livrables
```
docs/
├── brd-{project}.md
├── data-mapping-{project}.md
├── user-stories-{project}.md
└── requirements/
    └── {project}-requirements.md
```

### @archi Livrables
```
docs/
├── architecture/
│   ├── tad-{project}.md
│   ├── diagrams/
│   │   ├── c4-context.md
│   │   ├── c4-container.md
│   │   └── data-flow.md
│   └── adrs/
│       ├── adr-001-*.md
│       └── adr-002-*.md
└── Deployment/
    └── Terraform/
        ├── main.tf
        ├── variables.tf
        └── outputs.tf
```

### @dev Livrables
```
src/
├── pipelines/
├── functions/
├── notebooks/
├── sql/
└── shared/
tests/
├── unit/
├── integration/
└── e2e/
infrastructure/
└── terraform/
    └── modules/
```

### @reviewer Livrables
```
REVIEW-{pr-number}.md
- Score global
- Blockers
- Important issues
- Suggestions
- Security assessment
- Performance assessment
```

## 🔐 Sécurité & Exclusions

### Qui charge quoi?

```
Pattern: **/*.py
excludeAgent: "code-review"

↓

Coding Agent (Développeur) ✅ Charge
Code Review Agent (Reviewer) ❌ Ignore
```

### Cas d'usage

- Développeur: Charge les instructions (utilise coding agent)
- Reviewer: N'utilise PAS les instructions (code-review only)

```yaml
# Pour Développeur uniquement
---
applyTo: "src/**/*.py"
excludeAgent: "code-review"
---

# Pour Reviewer uniquement  
---
applyTo: "src/**/*.py"
excludeAgent: "coding-agent"
---

# Pour les deux
---
applyTo: "src/**/*.py"
---
```

## 📚 Documentation référencée

```
.github/
├── agents/              # Définitions agents
├── instructions/        # **Path-specific** instructions ← MAIN
├── clients/            # Config par client
├── knowledge/          # Base de connaissances
└── skills/             # Compétences spécialisées
```

## 🎓 Domaines d'expertise par agent

### @ba - Business Analyst
- Recueil d'exigences
- Analyse métier
- Data mapping
- User stories
- Risk analysis

### @archi - Architecte
- Azure architecture
- C4 models
- Design patterns
- Cost estimation
- Terraform/IaC

### @dev - Développeur
- Python/C#/SQL/PowerShell
- Azure services
- Code quality
- Testing
- Documentation

### @reviewer - Reviewer
- Code quality
- Security audit
- Performance review
- Test coverage
- Compliance check

## ⚡ Quick Workflows

### Nouveau projet complet
```
1. cd docs/requirements → @ba "Analyser..."
2. cd docs/architecture → @archi "Concevoir..."
3. cd src → @dev "Implémenter..."
4. PR créée → @reviewer "Revue..."
```

### Ajouter une feature
```
1. cd docs/requirements → @ba "Analyser impact..."
2. cd docs/architecture → @archi "Conception ajout..."
3. cd src → @dev "Implémenter..."
4. PR → @reviewer "Revue code..."
```

### Bug/Hotfix
```
1. cd src → @dev "Fixer le bug..."
2. cd tests → @dev "Ajouter tests..."
3. PR → @reviewer "Revue rapide..."
```

### Audit de sécurité
```
1. cd src → @reviewer "Audit sécurité..."
2. cd docs → @dev "Remédier aux issues..."
3. PR → @reviewer "Vérification..."
```

## 🔍 Troubleshooting

### Instructions ne s'appliquent pas?

```
1. Vérifier le chemin du fichier:
   pwd → /src/functions/test.cs ✅
   
2. Vérifier le pattern:
   Pattern: **/(src|Functions)/**
   Chemin: src/functions/test.cs ✅
   
3. Redémarrer Copilot:
   Fermer et rouvrir le chat
```

### Plusieurs instructions s'appliquent?

```
Path-specific a priorité:
- .github/instructions/*.instructions.md ← Plus spécifique
- .github/copilot-instructions.md ← Moins spécifique
```

### Modifier les instructions?

```bash
# Éditer directement
.github/instructions/{agent}.instructions.md

# Commit et push
git add .github/instructions/
git commit -m "chore: update agent instructions"
git push

# Changements appliqués immédiatement
```

## 📞 Contacts & Support

| Question | Ressource |
|----------|-----------|
| Format instructions? | `.github/instructions/README.md` |
| Exemples d'utilisation? | `AGENT-EXAMPLES.md` |
| Patterns disponibles? | Ce document |
| Best practices? | `AGENTS.md` |
| Migration completed? | `MIGRATION-COMPLETED.md` |

---

**Version**: 1.0.0  
**Format**: GitHub Copilot Path-specific Instructions  
**Updated**: 2026-02-04
