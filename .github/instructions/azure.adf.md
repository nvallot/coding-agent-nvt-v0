# Azure Data Factory – Standards

## Naming
- Pipelines : `pl-{action}-{entity}[-description]`
- Datasets : `ds-{type}-{entity}[-description]`
- Linked services : `ls-{type}-{system}[-description]`
- Triggers : `tr-{type}-{system}[-description]`
- Data flows : `df-{action}-{entity}[-description]`

## Bonnes pratiques
- Paramétrer tous les endpoints et credentials.
- Pas d’URL codées en dur.
- Datasets génériques réutilisables.
- Réutiliser Integration Runtime commun si défini.

## Observabilité
- Log Analytics activé
- Activity runs + pipeline runs surveillés
