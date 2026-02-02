# Instructions (v0.1)

Ce dossier contient des règles transverses et standards techniques **génériques** (tous clients).

## Contenu

### Base Agent Rules
- **AGENTS.base.md** : règles universelles pour tous les agents (tous clients)

### Contracts
- **contracts/artefacts-contract.md** : types de livrables par agent
- **contracts/flow-contract.md** : contrat de flux multi-agent

### Azure Standards (Generic)
- **azure.general.md** : principes communs Azure
- **azure.terraform.md** : IaC Terraform Azure
- **azure.adf.md** : Azure Data Factory
- **azure.apim.md** : API Management
- **azure.functions.md** : Azure Functions
- **azure.security.md** : sécurité et secrets
- **azure.observability.md** : logs, métriques, alertes

## Client-Specific Instructions

Les instructions spécifiques à un client sont dans `clients/<client>/instructions/`.

Exemple : `clients/sbm/instructions/`

## Utilisation

Ces fichiers sont référencés par les agents Copilot dans `agents/`.
