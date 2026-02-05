---
description: "Handoff rapide Dev → Reviewer"
tools: ["read"]
---

# 🔄 Handoff: Dev → Reviewer

Ce prompt facilite la transition du Développeur vers le Reviewer.

## 📋 Prérequis

Vérifier que ces fichiers existent dans `{docsPath}/workflows/{flux}/`:
- `00-context.md` ✅
- `01-requirements.md` ✅
- `02-architecture.md` ✅
- `03-implementation.md` ✅
- `HANDOFF.md` ✅

## 👉 Commande à Copier

Ouvrir un **nouveau chat** et copier:

```
@reviewer Flux: {FLUX}
Contexte: {docsPath}/workflows/{FLUX}/
```

## 🎯 Mission du Reviewer

Effectuer une revue de code complète selon les standards définis.
