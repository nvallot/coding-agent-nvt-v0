# Data Contracts – Middleway

## Principes
- Les contrats sont versionnés
- Les changements sont rétrocompatibles
- Les schémas sont stockés dans l'Integration Account

## Champs obligatoires
- Identifiant fonctionnel
- Date de création
- Source émettrice
- CorrelationId

## Formats supportés
- JSON (préféré)
- XML (legacy, EDIFACT)
- CSV (imports/exports bulk)

## Versionning
Les contrats suivent le format : `v{major}.{minor}`
Exemple : `v1.0`, `v2.1`
