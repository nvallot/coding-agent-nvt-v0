# 📚 Knowledge Base

## 🎯 Purpose

Ce dossier contient la **documentation de référence** (REFERENCE) utilisée par les agents.

**Knowledge = QUOI savoir** (descriptions, exemples, tables de lookup)

> ⚠️ **Ne pas confondre avec `instructions/`** qui contient les directives comportementales (HOW).

## 📋 Définition

| Type | Dossier | Contenu | Chargement |
|------|---------|---------|------------|
| **REFERENCE** | `knowledge/` | Descriptions, exemples code, tables, diagrammes | Manuel via "Lire si besoin" |
| **BEHAVIORAL** | `instructions/` | Règles MUST/SHOULD, checklists, workflows | Auto via `applyTo` |

### Exemples de contenu Knowledge

✅ **Appartient à knowledge/**:
- Descriptions de services Azure (qu'est-ce que Service Bus?)
- Templates de code et exemples
- Tables de lookup (abréviations, SKUs, pricing)
- Diagrammes et schémas de référence
- Glossaires et terminologie

❌ **N'appartient PAS à knowledge/** (→ instructions/):
- Règles "MUST do X", "NEVER do Y"
- Checklists de validation
- Workflows et processus obligatoires
- Standards de coding (conventions)

## 📁 Structure

```
knowledge/
├── README.md                    ← Vous êtes ici
├── azure/                       # Documentation Azure services
│   ├── data-factory.md         # ADF patterns & exemples
│   ├── services.md             # Catalogue services Azure
│   ├── patterns.md             # Medallion, Lambda, Kappa descriptions
│   ├── service-bus.md          # Service Bus documentation
│   ├── dataverse.md            # Dataverse/Power Platform
│   ├── functions.md            # Azure Functions patterns
│   └── key-vault.md            # Key Vault usage
├── iac/                         # Infrastructure as Code templates
│   ├── terraform-patterns.md   # Terraform code examples
│   └── bicep-templates.md      # Bicep code examples
├── coding/                      # Code examples & snippets
│   ├── csharp-examples.md      # C# patterns & samples
│   └── testing-fixtures.md     # Test fixtures & mocks
├── data/                        # Data modeling reference
│   └── modeling-reference.md   # Model types, retention tables
└── integration/                 # Integration patterns
    └── api-patterns.md         # REST, GraphQL, gRPC patterns
```

## 📖 Format Standard

Chaque fichier knowledge doit suivre ce format:

```markdown
---
applyTo: "{patterns pour auto-chargement optionnel}"
type: knowledge
---

# Knowledge: {Nom du Service/Concept}

## 📋 Vue d'ensemble
[Description: qu'est-ce que c'est?]

## 🎯 Use Cases
[Quand utiliser? Cas d'usage typiques]

## 🏗️ Architecture / Composants
[Structure, concepts clés]

## 💻 Exemples
[Code samples, JSON, HCL, etc.]

## ✅ Bonnes Pratiques
[Recommandations - mais PAS de règles MUST]

## 💰 Coûts (si applicable)
[Modèle de pricing]

## 📚 Références
[Liens documentation officielle]
```

## 🔗 Comment Accéder

### Depuis les Instructions Agent

Les agents référencent knowledge via:
```markdown
## Ressources (Lire si besoin)
- `knowledge/azure/service-bus.md` - Patterns Service Bus
- `knowledge/iac/terraform-patterns.md` - Templates Terraform
```

### Frontmatter optionnel

Pour auto-chargement contextuel, ajouter un `applyTo`:
```yaml
---
applyTo: "**/src/**,**/Functions/**"
type: knowledge
---
```

## 📊 Fichiers Disponibles

| Fichier | Description | Agents concernés |
|---------|-------------|------------------|
| `azure/data-factory.md` | ADF pipelines, activities, linked services | @archi, @dev |
| `azure/services.md` | Catalogue services Azure par catégorie | Tous |
| `azure/patterns.md` | Medallion, Lambda, Kappa architectures | @archi, @dev |
| `azure/service-bus.md` | Topics, subscriptions, messaging | @archi, @dev |
| `azure/dataverse.md` | Power Platform, entities, relationships | @dev |
| `azure/functions.md` | Triggers, bindings, isolated worker | @dev |
| `iac/terraform-patterns.md` | Modules, state, variables | @archi, @dev |
| `iac/bicep-templates.md` | Modules, parameters, deployment | @archi, @dev |
| `coding/csharp-examples.md` | Records, patterns, DI exemples | @dev, @reviewer |
| `coding/testing-fixtures.md` | Fixtures, mocks, assertions | @dev, @reviewer |

## 🔄 Maintenance

### Ajouter un nouveau fichier knowledge

1. Créer le fichier dans le sous-dossier approprié
2. Utiliser le format standard ci-dessus
3. Ajouter l'entrée dans ce README
4. Mettre à jour `instructions/INDEX.md` si nécessaire

### Client-specific knowledge

Les fichiers knowledge spécifiques à un client vont dans:
```
.github/clients/{clientKey}/knowledge/
```

Exemple: `.github/clients/sbm/knowledge/glossary.md`

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-05
