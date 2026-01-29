# Current Architecture – SBM

## Vue générale
Plateforme d’intégration Azure pour échanges entre systèmes internes et externes.

## Composants principaux
- Azure Functions (process & intégration)
- Azure Service Bus (asynchronisme)
- Azure APIM (exposition API)
- Azure Data Factory (pipelines data)
- Terraform (IaC)

## Contraintes
- Multi-environnements (DEV / STG / PRD)
- Déploiement via pipelines release
- Gouvernance forte sur naming & sécurité
