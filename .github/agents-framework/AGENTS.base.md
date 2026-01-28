---
name: "Universal Multi-Agent Framework"
version: "1.0"
scope: "Generic – Client & Technology Agnostic"
---

# Objectif

Ce document définit le socle commun de comportement pour tous les agents
(Business Analyst, Solution Architect, Developer).

Il est **indépendant de tout client, domaine métier ou technologie**.

---

# Règles Globales Applicables à Tous les Agents

## 1. Mode de fonctionnement

- Chaque agent peut fonctionner :
  - en **pipeline** (enchaîné à d’autres agents)
  - en **mode autonome** (entrée minimale ou incomplète)
- En cas d’informations manquantes :
  - l’agent formule explicitement ses **hypothèses**
  - continue la production sur cette base

## 2. Qualité des livrables

Chaque livrable doit inclure explicitement :
- Hypothèses
- Risques
- Non-couvert
- Pré-requis pour l’agent suivant (handoff)

## 3. Nommage universel des fichiers

Format obligatoire :

`<source>-<target>-<artefact>.<extension>`

- source / target : systèmes ou domaines abstraits
- artefact : cahier-des-charges, architecture, mapping, etc.

Exemples valides :
- systemA-systemB-cahier-des-charges.md
- domainX-domainY-architecture.drawio

## 4. Chargement du contexte

Chaque agent doit charger, dans l’ordre :
1. `AGENTS.base.md`
2. `clients/<clientKey>/CLIENT.md` (si présent)
3. `projects/<project>/PROJECT.md` (si présent)

## 5. Neutralité technologique

- Aucun choix technologique ne doit être imposé sans justification.
- Les technologies sont **des options**, jamais des prérequis.

---

# Interaction entre agents

- Chaque agent précise :
  - ce qu’il consomme
  - ce qu’il produit
  - ce qu’il attend du suivant
