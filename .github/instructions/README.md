# 📋 Instructions

## 🎯 Purpose

Ce dossier contient les **directives comportementales** (BEHAVIORAL) pour les agents.

**Instructions = COMMENT agir** (règles, workflows, standards)

> ⚠️ **Ne pas confondre avec `knowledge/`** qui contient la documentation de référence (WHAT).

## 📋 Définition

| Type | Dossier | Contenu | Chargement |
|------|---------|---------|------------|
| **BEHAVIORAL** | `instructions/` | Règles MUST/SHOULD, checklists, workflows | Auto via `applyTo` |
| **REFERENCE** | `knowledge/` | Descriptions, exemples code, tables | Manuel via "Lire si besoin" |

### Exemples de contenu Instructions

✅ **Appartient à instructions/**:
- Règles "MUST do X", "NEVER do Y"
- Checklists de validation
- Workflows et processus obligatoires
- Standards de coding (conventions)
- Décisions architecturales (quand utiliser quoi)

❌ **N'appartient PAS à instructions/** (→ knowledge/):
- Descriptions de services (qu'est-ce que X?)
- Templates de code et exemples complets
- Tables de lookup (abréviations, pricing)
- Diagrammes et schémas de référence

## 📁 Structure

```
instructions/
├── README.md                        ← Vous êtes ici
├── INDEX.md                         # Navigation complète
├── *.instructions.md                # Instructions par agent
│   ├── architecte.instructions.md   # @architecte
│   ├── business-analyst.instructions.md  # @ba
│   ├── developpeur.instructions.md  # @dev
│   └── reviewer.instructions.md     # @reviewer
├── base/                            # Règles universelles (tous agents)
│   ├── agent-roles.md              # Définition rôles, workflow
│   ├── azure-reference.md          # Best practices Azure
│   └── conventions.md              # Standards code, sécurité, logging
├── domains/                         # Expertise technique (contextuel)
│   ├── azure-patterns.md           # Quand utiliser Medallion/Lambda
│   ├── bicep-arm.md                # Règles Bicep/ARM
│   ├── csharp-dotnet.md            # Standards C# .NET
│   ├── data-architecture.md        # Gouvernance données
│   ├── draw-io-standards.md        # Standards visuels Draw.io
│   ├── iac-terraform.md            # Règles Terraform
│   └── testing.md                  # Pyramide tests, couverture
└── contracts/                       # Templates livrables
    └── artefacts.md                # BRD, TAD, ADR templates
```

## 🔄 Hiérarchie de Chargement

```
┌─────────────────────────────────────────────────────────────┐
│ PRIORITÉ (décroissante)                                      │
├─────────────────────────────────────────────────────────────┤
│ 1. Client Instructions  (.github/clients/{key}/instructions/)│
│ 2. Agent Instructions   (*.instructions.md)                  │
│ 3. Domain Instructions  (domains/*.md via applyTo)           │
│ 4. Base Instructions    (base/*.md)                          │
│ 5. Repository-wide      (copilot-instructions.md)            │
└─────────────────────────────────────────────────────────────┘
```

## 📖 Format Frontmatter

Chaque fichier instruction doit avoir un frontmatter YAML:

```yaml
---
applyTo: "{glob patterns}"
excludeAgent: "code-review"  # optionnel, format string
---

# Titre du fichier

[Contenu des instructions]
```

### Paramètres

| Paramètre | Format | Description |
|-----------|--------|-------------|
| `applyTo` | glob pattern | Fichiers auxquels appliquer (ex: `**/*.cs`) |
| `excludeAgent` | `"code-review"` ou `"coding-agent"` | Agent à exclure (optionnel) |

### Patterns applyTo valides

```
*                    - Tous les fichiers du dossier courant
**/*.py              - Tous les fichiers .py récursivement
src/**/*.ts          - Tous les .ts sous src/
**/test/**           - Tous les fichiers dans n'importe quel dossier test/
**/*.py,**/*.ts      - Multiple patterns (séparés par virgules)
**/*                 - Tous les fichiers (universel)
```

## 🏷️ Matrice Agent → Fichiers

| Fichier | @ba | @archi | @dev | @reviewer |
|---------|:---:|:------:|:----:|:---------:|
| base/agent-roles.md | ✅ | ✅ | ✅ | ✅ |
| base/azure-reference.md | ✅ | ✅ | ✅ | ✅ |
| base/conventions.md | ✅ | ✅ | ✅ | ✅ |
| domains/azure-patterns.md | ❌ | ✅ | ✅ | ✅ |
| domains/csharp-dotnet.md | ❌ | ⚪ | ✅ | ✅ |
| domains/bicep-arm.md | ❌ | ✅ | ✅ | ❌ |
| domains/data-architecture.md | ⚪ | ✅ | ✅ | ⚪ |
| domains/draw-io-standards.md | ⚪ | ✅ | ❌ | ❌ |
| domains/iac-terraform.md | ❌ | ✅ | ✅ | ❌ |
| domains/testing.md | ❌ | ❌ | ✅ | ✅ |
| contracts/artefacts.md | ✅ | ✅ | ✅ | ✅ |

**Légende**: ✅ Chargé auto | ⚪ Optionnel/Référence | ❌ Non chargé

## 📚 Références vers Knowledge

Les instructions peuvent référencer knowledge via:
```markdown
## Ressources (Lire si besoin)
- `knowledge/azure/service-bus.md` - Patterns Service Bus
- `knowledge/azure/patterns.md` - Descriptions architectures
```

## 🔄 Maintenance

### Ajouter une nouvelle instruction

1. Créer le fichier avec frontmatter approprié
2. Définir `applyTo` pour cibler les bons fichiers
3. Mettre à jour `INDEX.md`
4. Vérifier accès dans la matrice agent

### Splitter un fichier trop gros

Si un fichier dépasse ~200 lignes:
1. Séparer contenu BEHAVIORAL → garde dans instructions/
2. Extraire contenu REFERENCE → déplacer vers knowledge/
3. Ajouter référence "Lire si besoin" dans l'instruction

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-05
