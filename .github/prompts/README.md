# 📝 Prompts

## 🎯 Purpose

Ce dossier contient les **templates de prompts** utilisables manuellement avec les agents.

> ⚠️ **Les prompts ne sont PAS auto-chargés** contrairement aux instructions.
> Ils doivent être invoqués explicitement par l'utilisateur.

## 📋 Types de Prompts

### 1️⃣ Pipeline Prompts (`.prompt.md`)

**Objectif**: Workflow complet pour un agent du début à la fin.

**Format**: `.prompt.md`

**Usage**:
```
@architecte Exécuter le pipeline archi pour le flux {FLUX}
```

**Fichiers**:
| Prompt | Agent | Description |
|--------|-------|-------------|
| `archi-pipeline.prompt.md` | @architecte | Workflow complet architecture |
| `ba-pipeline.prompt.md` | @ba | Workflow complet analyse métier |
| `dev-pipeline.prompt.md` | @dev | Workflow complet développement |
| `reviewer-pipeline.prompt.md` | @reviewer | Workflow complet revue de code |

---

### 2️⃣ Handoff Prompts (`handoff-*.prompt.md`)

**Objectif**: Transition entre agents avec vérification des prérequis.

**Format**: `handoff-{source}-to-{target}.prompt.md`

**Usage**: Copier la commande proposée dans un nouveau chat.

**Fichiers**:
| Prompt | Transition | Prérequis |
|--------|------------|-----------|
| `handoff-ba-to-archi.prompt.md` | @ba → @architecte | 00-context.md, 01-requirements.md |
| `handoff-archi-to-dev.prompt.md` | @architecte → @dev | 02-architecture.md, Draw.io |
| `handoff-dev-to-reviewer.prompt.md` | @dev → @reviewer | 03-implementation.md, tests |

**Workflow**:
```
BA ──────────→ Architecte ──────────→ Dev ──────────→ Reviewer
    handoff       handoff        handoff
```

---

### 3️⃣ Template Prompts (`.prompt`)

**Objectif**: Génération d'un document structuré spécifique.

**Format**: `.prompt`

**Usage**:
```
@architecte Générer un TAD pour le projet {PROJECT}
```

**Fichiers**:
| Prompt | Agent | Livrable |
|--------|-------|----------|
| `brd.prompt` | @ba | Business Requirements Document |
| `tad.prompt` | @architecte | Technical Architecture Document |
| `diagram.prompt` | @architecte | Diagramme Mermaid |
| `code-review.prompt` | @reviewer | Rapport de revue structuré |
| `implementation.prompt` | @dev | Plan d'implémentation |

---

### 4️⃣ Utility Prompts

**Objectif**: Initialisation ou actions transverses.

**Fichiers**:
| Prompt | Description |
|--------|-------------|
| `start-workflow.prompt.md` | Démarrer un nouveau workflow complet |

---

## 📖 Format Frontmatter

### Pipeline/Handoff Prompts
```yaml
---
description: "Description courte du prompt"
tools: ["read", "search", "edit", "edit/createFile"]
---

# Contenu du prompt avec {VARIABLES}
```

### Template Prompts
```yaml
---
title: "Nom du template"
description: "Ce que le template génère"
agents: ["architecte"]
variables:
  - name: project_name
    description: "Nom du projet"
    required: true
  - name: flux_name
    description: "Nom du flux"
    required: false
---

# Template avec {{project_name}} placeholders
```

## 🚀 Comment Utiliser

### Méthode 1: Invocation directe
```
@architecte Générer un TAD pour le flux purchase-order-sync
```

### Méthode 2: Copier-coller depuis handoff
Après avoir terminé avec @ba, le handoff propose:
```
👉 Ouvrir un nouveau chat et copier:
@architecte Flux: purchase-order-sync
Contexte: docs/workflows/purchase-order-sync/
```

### Méthode 3: Pipeline complet
```
@ba Exécuter le pipeline BA complet pour le nouveau flux supplier-import
```

## 📊 Matrice Prompts × Agents

| Prompt | @ba | @archi | @dev | @reviewer |
|--------|:---:|:------:|:----:|:---------:|
| ba-pipeline.prompt.md | ✅ | ❌ | ❌ | ❌ |
| archi-pipeline.prompt.md | ❌ | ✅ | ❌ | ❌ |
| dev-pipeline.prompt.md | ❌ | ❌ | ✅ | ❌ |
| reviewer-pipeline.prompt.md | ❌ | ❌ | ❌ | ✅ |
| brd.prompt | ✅ | ❌ | ❌ | ❌ |
| tad.prompt | ❌ | ✅ | ❌ | ❌ |
| diagram.prompt | ❌ | ✅ | ❌ | ❌ |
| code-review.prompt | ❌ | ❌ | ❌ | ✅ |
| implementation.prompt | ❌ | ❌ | ✅ | ❌ |
| handoff-ba-to-archi | ✅ | ❌ | ❌ | ❌ |
| handoff-archi-to-dev | ❌ | ✅ | ❌ | ❌ |
| handoff-dev-to-reviewer | ❌ | ❌ | ✅ | ❌ |

## 🔄 Maintenance

### Ajouter un nouveau prompt

1. Choisir le type approprié (pipeline, handoff, template)
2. Utiliser le format frontmatter correspondant
3. Ajouter l'entrée dans ce README
4. Tester avec l'agent cible

### Convention de nommage

```
{type}-{agent/source}-{target}.prompt.md   # Handoffs
{agent}-pipeline.prompt.md                  # Pipelines
{deliverable}.prompt                        # Templates
```

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-05
