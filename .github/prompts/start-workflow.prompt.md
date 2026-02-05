---
description: "Initialiser un nouveau workflow multi-agents avec persistence du contexte"
tools: ["read", "search", "edit", "edit/createFile"]
---

# 🚀 Démarrer un Nouveau Workflow

Ce prompt initialise un workflow multi-agents complet avec persistence des artefacts.

## 📋 Étape 1: Charger Configuration

Lire `.github/clients/active-client.json` pour récupérer:
- `clientKey` - identifiant client
- `docsPath` - chemin vers la documentation du projet cible

## 📋 Étape 2: Informations Requises

### Identification du Flux

```
Flux: {nom du flux - ex: purchase-order-integration}
```

### Contexte Initial

```
Objectif: {Quel est l'objectif principal de ce flux?}

Contexte métier: {Description du contexte métier}

Périmètre:
- Inclus: {Ce qui est dans le périmètre}
- Exclus: {Ce qui est hors périmètre}

Contraintes:
- {Contrainte 1}
- {Contrainte 2}
```

---

## 🔧 Actions d'Initialisation

Une fois les informations fournies, je vais:

1. **Créer la structure de dossiers**:
   ```
   {docsPath}/workflows/{flux}/
   ├── 00-context.md
   └── HANDOFF.md
   ```

2. **Sauvegarder le contexte initial** dans `00-context.md`

3. **Initialiser le HANDOFF.md** pour le premier agent (BA)

4. **Proposer le démarrage** avec l'agent @ba

---

## 📂 Structure Créée

```
{docsPath}/workflows/{FLUX}/
├── 00-context.md           ← Contexte initial (ce fichier)
├── 01-requirements.md      ← Sera créé par @ba
├── 02-architecture.md      ← Sera créé par @architecte
├── 03-implementation.md    ← Sera créé par @dev
├── 04-review.md            ← Sera créé par @reviewer
└── HANDOFF.md              ← État courant
```

---

## 👉 Prochaine Étape

Une fois le contexte initialisé, **ouvrir un nouveau chat** et copier:

```
@ba Nouveau flux: {FLUX}
Contexte: {docsPath}/workflows/{FLUX}/
```
