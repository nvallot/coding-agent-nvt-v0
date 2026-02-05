# 📚 Index des Instructions

> Navigation rapide pour les agents et développeurs.

## 🗂️ Structure

```
.github/
├── instructions/               # BEHAVIORAL rules (auto-loaded via applyTo)
│   ├── INDEX.md               ← Vous êtes ici
│   ├── README.md              # Documentation du système
│   ├── *.instructions.md      # Instructions par agent
│   ├── base/                  # Règles universelles
│   ├── domains/               # Expertise technique
│   ├── contracts/             # Templates livrables
│   └── clients/               # Client-specific instructions
│       └── sbm/               # SBM Offshore (manuel via clientKey)
├── knowledge/                 # REFERENCE docs (auto-loaded via applyTo)
│   ├── README.md              # Documentation du système
│   ├── azure/                 # Azure services reference
│   ├── iac/                   # Terraform/Bicep templates
│   ├── coding/                # Code examples
│   ├── data/                  # Data modeling reference
│   └── clients/               # Client-specific knowledge
│       └── sbm/glossary.md    # SBM terminology (manuel via clientKey)
├── prompts/                   # Prompt templates (manual)
│   └── README.md              # When to use which prompt
├── skills/                    # Skills (complex capabilities)
│   └── draw-io-generator/     # Draw.io diagram generation
└── clients/                   # Client profiles ONLY
    └── sbm/CLIENT.md          # SBM profile (metadata)
```

## 📋 Instructions par Agent

| Agent | Fichier | Pattern `applyTo` |
|-------|---------|-------------------|
| @ba | [business-analyst.instructions.md](./business-analyst.instructions.md) | `**/requirements/**,**/specifications/**,**/docs/**` |
| @architecte | [architecte.instructions.md](./architecte.instructions.md) | `**/docs/**,**/Deployment/**,**/architecture/**` |
| @dev | [developpeur.instructions.md](./developpeur.instructions.md) | `**/src/**,**/Functions/**,**/*.cs,**/*.py,**/*.tf` |
| @reviewer | [reviewer.instructions.md](./reviewer.instructions.md) | `**/*.cs,**/*.py,**/*.sql` |

## 📁 Base (Règles Universelles)

Chargées par **tous les agents**, quel que soit le contexte.

| Fichier | Description |
|---------|-------------|
| [agent-roles.md](./base/agent-roles.md) | Définition des 4 rôles, workflow obligatoire |
| [azure-reference.md](./base/azure-reference.md) | Services Azure par catégorie, patterns recommandés |
| [conventions.md](./base/conventions.md) | Standards code (C#, Python, SQL, Terraform), sécurité, logging |

## 🎯 Domains (Expertise Technique)

Chargées selon le **pattern `applyTo`** et le contexte du fichier.

| Fichier | Description | Agents |
|---------|-------------|--------|
| [azure-patterns.md](./domains/azure-patterns.md) | Medallion, Lambda, CDC, integration patterns | @archi, @dev, @rev |
| [csharp-dotnet.md](./domains/csharp-dotnet.md) | C# .NET 10, Azure Functions, DI, async | @dev, @rev |
| [bicep-arm.md](./domains/bicep-arm.md) | Bicep templates, modules, deployment | @archi, @dev |
| [data-architecture.md](./domains/data-architecture.md) | Modeling, gouvernance, lineage, qualité | @archi, @dev |
| [iac-terraform.md](./domains/iac-terraform.md) | Structure Terraform, modules, state, secrets | @archi, @dev |
| [testing.md](./domains/testing.md) | Test pyramid, unit, integration, data quality | @dev, @rev |
| [draw-io-standards.md](./domains/draw-io-standards.md) | Visual standards, zones, icons, layout | @archi |

## 📂 Knowledge Files

**Auto-chargés** via `applyTo` pattern (comme les instructions).

| Dossier | Contenu | applyTo |
|---------|---------|--------|
| [knowledge/azure/](../knowledge/azure/) | services.md, patterns.md, functions.md, etc. | Global |
| [knowledge/iac/](../knowledge/iac/) | terraform-patterns.md, bicep-templates.md | `**/*.tf`, `**/*.bicep` |
| [knowledge/coding/](../knowledge/coding/) | csharp-examples.md, testing-fixtures.md | `**/*.cs`, `**/tests/**` |
| [knowledge/data/](../knowledge/data/) | modeling-reference.md | `**/docs/**` |
| [knowledge/clients/sbm/](../knowledge/clients/sbm/) | glossary.md (SBM terminology) | Manuel (via `clientKey=sbm`) |

## 🛠️ Skills

| Skill | Description | Agent |
|-------|-------------|-------|
| [draw-io-generator](../skills/draw-io-generator/SKILL.md) | Generate Draw.io diagrams from architecture | @archi |
| [azure-functions](../skills/azure-functions/SKILL.md) | Azure Functions development | @dev |

## 📦 Contracts (Templates Livrables)

| Fichier | Description |
|---------|-------------|
| [artefacts.md](./contracts/artefacts.md) | Templates BRD, TAD, ADR, PR, README |

## 🔄 Hiérarchie de Chargement

```
┌─────────────────────────────────────────────────────────────┐
│ PRIORITÉ (décroissante)                                      │
├─────────────────────────────────────────────────────────────┤
│ 1. Client Instructions  (instructions/clients/{key}/)        │
│ 2. Client Knowledge     (knowledge/clients/{key}/)           │
│ 3. Agent Instructions   (*.instructions.md)                  │
│ 4. Domain Instructions  (domains/*.md via applyTo)           │
│ 5. Base Instructions    (base/*.md)                          │
│ 6. Repository-wide      (copilot-instructions.md)            │
└─────────────────────────────────────────────────────────────┘
```

### 🎯 Chargement Client (Manuel)

Les fichiers client sont chargés **manuellement** par les agents selon `active-client.json`:

```
1. Agent lit .github/clients/active-client.json
2. Extrait clientKey (ex: "sbm")
3. Charge instructions/clients/{clientKey}/ si existe
4. Charge knowledge/clients/{clientKey}/ si existe
5. Charge clients/{clientKey}/CLIENT.md
```

> ⚠️ **Pas de `applyTo`** pour le contenu client - cela permet de supporter **tous** les projets d'un client (pas seulement ceux nommés explicitement).

## 🏷️ Matrice Agent → Fichiers

| Fichier | @ba | @archi | @dev | @reviewer |
|---------|:---:|:------:|:----:|:---------:|
| base/agent-roles.md | ✅ | ✅ | ✅ | ✅ |
| base/azure-reference.md | ✅ | ✅ | ✅ | ✅ |
| base/conventions.md | ✅ | ✅ | ✅ | ✅ |
| domains/azure-patterns.md | ❌ | ✅ | ✅ | ✅ |
| domains/csharp-dotnet.md | ❌ | ⚪ | ✅ | ✅ |
| domains/bicep-arm.md | ❌ | ✅ | ✅ | ⚪ |
| domains/data-architecture.md | ⚪ | ✅ | ✅ | ⚪ |
| domains/iac-terraform.md | ❌ | ✅ | ✅ | ⚪ |
| domains/testing.md | ❌ | ❌ | ✅ | ✅ |
| domains/draw-io-standards.md | ❌ | ✅ | ❌ | ❌ |
| contracts/artefacts.md | ✅ | ✅ | ✅ | ✅ |

**Légende**: ✅ Auto-chargé | ⚪ Optionnel/Référence | ❌ Non chargé

## 📖 Knowledge vs Instructions

| Type | Dossier | Chargement | Contenu |
|------|---------|------------|---------|
| **Instructions** | `instructions/` | Auto via `applyTo` | Règles COMPORTEMENTALES |
| **Knowledge** | `knowledge/` | Auto via `applyTo` | Docs RÉFÉRENCE |
| **Client Profile** | `clients/{key}/CLIENT.md` | Manuel (lu par agent) | Metadata client |
| **Skills** | `skills/` | Manuel via SKILL.md | Capacités complexes |
| **Prompts** | `prompts/` | Manuel (copier/coller) | Templates d'invocation |

## 📁 Organisation par Client

**Nouveau modèle** (centralisé par type):
```
instructions/clients/sbm/    ← Instructions SBM (auto-chargées)
knowledge/clients/sbm/       ← Knowledge SBM (auto-chargé)
clients/sbm/CLIENT.md        ← Profil client seulement
```

**Avantages**:
- Un seul endroit pour `instructions/` et `knowledge/`
- Pattern `applyTo` unifié
- Isolation client via pattern projet (`**/NADIA/**`, `**/Supplier Portal/**`)

Voir les README.md dans chaque dossier pour plus de détails.
