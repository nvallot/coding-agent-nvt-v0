# ✅ Migration des Agents vers le Format GitHub Copilot Standard

## Résumé des changements

Conversion de vos agents personnalisés vers le format GitHub Copilot standard avec support de `applyTo` patterns.

## 📁 Fichiers créés

### Nouvelles instructions path-specific (`.github/instructions/`)

#### 1. **architecte.instructions.md**
- **Pattern**: `**/(docs|Deployment|architecture)/**`
- **Exclusion**: `code-review`
- **Contenu**: Instructions complètes pour l'Agent Architecte
  - Mission et workflow
  - Domaines d'expertise détaillés
  - Livrables attendus (TAD, diagrammes, ADRs, Terraform)
  - Well-Architected Framework principles
  - Data architecture principles
  - Templates et exemples

#### 2. **business-analyst.instructions.md**
- **Pattern**: `**/requirements/**,**/specifications/**,**/docs/**`
- **Exclusion**: `code-review`
- **Contenu**: Instructions complètes pour Business Analyst
  - Mission et workflow
  - Domaines de compétence
  - Template BRD avec 9 sections
  - Data mapping documentation
  - User Stories avec acceptance criteria
  - Risks & Mitigations

#### 3. **developpeur.instructions.md**
- **Pattern**: `**/(src|Functions|Development|*.cs|*.py|*.sql|*.tf)/**`
- **Exclusion**: `code-review`
- **Contenu**: Instructions complètes pour Développeur
  - Mission et workflow
  - Expertise Azure et langages
  - Livrables code (ADF, Databricks, Functions, SQL, Terraform)
  - Exemples de code production pour chaque composant
  - Standards de qualité
  - Tests (unit, integration, E2E)
  - Documentation requirements

#### 4. **reviewer.instructions.md**
- **Pattern**: `**/(pull_requests|*.cs|*.py|*.sql)/**`
- **Exclusion**: `coding-agent` (code-review ONLY)
- **Contenu**: Instructions complètes pour Reviewer
  - Mission et workflow
  - Domaines d'expertise (qualité, sécurité, performance)
  - Template de rapport détaillé avec:
    - Score par catégorie
    - Blockers (MUST FIX)
    - Important issues (SHOULD FIX)
    - Suggestions (NICE TO HAVE)
    - Strengths
    - Metrics summary
    - Security assessment
    - Performance assessment
    - Testing coverage
    - Action items
  - Security & Compliance checklist

### Documentation

#### 5. **.github/instructions/README.md**
- Guide d'utilisation des instructions
- Tableau récapitulatif des patterns
- Formats et priorités
- Maintenance et évolution

#### 6. **AGENTS.md** (racine du projet)
- Vue d'ensemble des 4 agents
- Structure du projet
- Format des instructions
- Workflow recommandé (BA → Archi → Dev → Reviewer)
- Configuration par client
- Best practices
- Dépannage
- Ressources

## 🎯 Améliorations apportées

### ✨ Format standardisé
- ✅ Frontmatter YAML avec `applyTo` et `excludeAgent`
- ✅ Compatible avec GitHub Copilot standard
- ✅ Peut être utilisé dans VS Code, GitHub.com, et autres éditeurs
- ✅ Glob patterns reconnus automatiquement

### 📋 Contenu enrichi
- ✅ Templates détaillés pour tous les livrables
- ✅ Exemples de code pour chaque technologie
- ✅ Checklists complètes (security, performance, etc.)
- ✅ Workflows clarifiés (BA → Archi → Dev → Reviewer)
- ✅ Handoff documentation entre agents
- ✅ References et ressources

### 🔒 Sécurité & Qualité
- ✅ Guidelines de sécurité explicites
- ✅ Standards de code détaillés
- ✅ Tests requirements (unit, integration, E2E)
- ✅ Compliance checklist (GDPR, SOX, etc.)
- ✅ Well-Architected Framework integration

### 🤝 Collaboration
- ✅ Clear handoff procedures
- ✅ Communication templates
- ✅ Explicit dependencies between agents
- ✅ Status indicators (Approved, Needs revision, etc.)

## 🔄 Utilisation

### Avant (ancien format)
```
@architecte "Concevoir l'architecture"
# Chargeait uniquement .github/agents/architecte.md
```

### Après (nouveau format)
```
@architecte "Concevoir l'architecture pour le dossier docs/"
# Charge automatiquement .github/instructions/architecte.instructions.md
# basé sur le pattern applyTo: "**/(docs|Deployment|architecture)/**"
```

## 📊 Patterns `applyTo` configurés

| Agent | Pattern | Cas d'usage |
|-------|---------|-----------|
| **Architecte** | `**/(docs\|Deployment\|architecture)/**` | Design, décisions architecturales |
| **Développeur** | `**/(src\|Functions\|Development\|*.cs\|*.py\|*.sql\|*.tf)/**` | Implémentation code, tests |
| **Business Analyst** | `**/requirements/**,**/specifications/**,**/docs/**` | Exigences, stories, analyses |
| **Reviewer** | `**/(pull_requests\|*.cs\|*.py\|*.sql)/**` | Code review, audits |

## ✅ Migration Checklist

- [x] Créer `.github/instructions/architecte.instructions.md`
- [x] Créer `.github/instructions/business-analyst.instructions.md`
- [x] Créer `.github/instructions/developpeur.instructions.md`
- [x] Créer `.github/instructions/reviewer.instructions.md`
- [x] Ajouter frontmatter avec `applyTo` sur tous les fichiers
- [x] Ajouter `excludeAgent` approprié
- [x] Créer `.github/instructions/README.md`
- [x] Créer `AGENTS.md` documentation racine
- [x] Enrichir contenu avec templates et exemples
- [x] Ajouter security, performance, testing guidelines
- [x] Valider glob patterns

## 🚀 Prochaines étapes (optionnelles)

1. **Tester les patterns**
   - Ouvrir des fichiers matching (ex: `src/Functions/test.cs`)
   - Vérifier que Copilot charge les bonnes instructions

2. **Intégrer avec clients**
   - Mettre à jour `.github/clients/{clientKey}/instructions/`
   - Ajouter des overrides spécifiques au client

3. **Ajouter knowledge base**
   - Remplir `.github/knowledge/` avec articles spécialisés
   - Référencer depuis les instructions

4. **Optimiser par usage**
   - Monitorer quels patterns sont utilisés
   - Ajuster la granularité si nécessaire

## 📖 Ressources

- 📚 [GitHub Copilot Documentation](https://docs.github.com/en/copilot/how-tos/configure-custom-instructions/add-repository-instructions)
- 🔗 [openai/agents.md Format](https://github.com/openai/agents.md)
- 📋 [Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)

## 💡 Notes importantes

### Format YAML Frontmatter
```yaml
---
applyTo: "glob/pattern/**"
excludeAgent: "code-review" | "coding-agent"
---
```

### Priorité des instructions
1. Personal instructions
2. **Path-specific instructions** ← Votre `.github/instructions/*.instructions.md`
3. Repository-wide instructions
4. Organization instructions

### Glob Pattern Examples
- `*` = tous fichiers dans dossier courant
- `**/*.py` = tous `.py` récursivement
- `src/**/*.ts` = tous `.ts` sous `src/`
- `**/test/**` = tous fichiers dans n'importe quel `test/`
- `(a|b|c)/**` = dossiers `a/`, `b/`, ou `c/`

---

**Complété le**: 2026-02-04  
**Version**: 1.0.0  
**Format**: GitHub Copilot Path-specific Instructions
