# Azure API Management – Standards

## Versioning
- APIs versionnées (ex: `v1`, `v2`) avec suffixe clair.
- Chaque version = backend distinct si changement incompatible.

## Policies
- Politique globale pour auth + correlation.
- Policies par API pour transformation payload.

## Subscriptions
- Par produit / client / équipe
- Nommage conforme aux conventions client

## Monitoring
- App Insights connecté
- Logs activés pour tracing
