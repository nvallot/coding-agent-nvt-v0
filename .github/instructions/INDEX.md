# 📚 Index des Instructions

> Navigation rapide pour les agents et développeurs.

## 🗂️ Structure

```
instructions/
├── INDEX.md                    ← Vous êtes ici
├── *.instructions.md           # Instructions par agent
├── base/                       # Règles universelles (toujours chargées)
├── domains/                    # Expertise technique (selon contexte)
└── contracts/                  # Templates livrables
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

## 📦 Contracts (Templates Livrables)

| Fichier | Description |
|---------|-------------|
| [artefacts.md](./contracts/artefacts.md) | Templates BRD, TAD, ADR, PR, README |

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
| contracts/artefacts.md | ✅ | ✅ | ✅ | ✅ |

**Légende**: ✅ Chargé | ⚪ Optionnel/Référence | ❌ Non chargé
