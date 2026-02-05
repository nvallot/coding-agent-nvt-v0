---
created: 2026-02-05
updated: 2026-02-05
status: draft
client: demo-client
---

# Demande en cours – Azure Function

## Description

Développer une Azure Function permettant de traiter des fichiers CSV déposés
dans un Blob Storage afin de les transformer en JSON normalisé
et de les transmettre à un système aval via Service Bus.

## Contexte

- Les fichiers CSV sont déposés quotidiennement dans un container Blob Azure
- Chaque fichier contient des données de commandes clients
- Le système aval consomme les messages depuis un topic Service Bus
- L’environnement cible est Azure (subscription existante)

## Objectifs

- Déclencher le traitement automatiquement à l’arrivée d’un fichier Blob
- Transformer chaque ligne du CSV en un message JSON conforme au schéma attendu
- Publier les messages sur un topic Service Bus
- Garantir un temps de traitement inférieur à 5 secondes par fichier
- Assurer l’idempotence (un fichier ne doit jamais être traité deux fois)

## Contraintes

- Langage : .NET / C#
- Hébergement : Azure Functions (Consumption ou Premium à définir)
- Sécurité :
  - Managed Identity
  - Accès Blob et Service Bus via Private Endpoint
- Volumétrie :
  - Jusqu’à 5 000 lignes par fichier
  - Jusqu’à 50 fichiers par jour
- Coûts maîtrisés (pas de sur-dimensionnement)
