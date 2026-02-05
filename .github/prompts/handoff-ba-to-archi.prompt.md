---
description: "Handoff rapide BA → Architecte"
tools: ["read"]
---

# 🔄 Handoff: BA → Architecte

Ce prompt facilite la transition du Business Analyst vers l'Architecte.

## 📋 Prérequis

Vérifier que ces fichiers existent dans `{docsPath}/workflows/{flux}/`:
- `00-context.md` ✅
- `01-requirements.md` ✅
- `HANDOFF.md` ✅

## 👉 Commande à Copier

Ouvrir un **nouveau chat** et copier:

```
@architecte Flux: {FLUX}
Contexte: {docsPath}/workflows/{FLUX}/
```

## 🎯 Mission de l'Architecte

Concevoir l'architecture technique Azure basée sur le cahier des charges du BA.

## 📦 Livrables Obligatoires

L'architecte **DOIT** produire:

| # | Livrable | Chemin | Description |
|---|----------|--------|-------------|
| 1 | TAD | `{docsPath}/workflows/{flux}/02-architecture.md` | Technical Architecture Document |
| 2 | **Diagramme Draw.io** | `{docsPath}/workflows/{flux}/diagrams/{flux}-c4-container.drawio` | Diagramme C4 Container |
| 3 | **Export PNG** | `{docsPath}/workflows/{flux}/diagrams/{flux}-c4-container.png` | Export 300 DPI |
| 4 | ADRs | `{docsPath}/workflows/{flux}/adrs/` | Architecture Decision Records |
| 5 | Terraform | `Deployment/Terraform/` | Infrastructure as Code |

### ⚠️ Diagramme Draw.io

Le diagramme est **OBLIGATOIRE** et doit:
- Utiliser les icônes Azure officielles (SVG)
- Respecter les zones de couleur (On-Prem jaune, Azure bleu, External vert)
- Inclure la numérotation des flux (❶❷❸)
- Ne pas avoir de chevauchement de composants
- Inclure une légende

Référence: `.github/instructions/domains/draw-io-standards.md`
