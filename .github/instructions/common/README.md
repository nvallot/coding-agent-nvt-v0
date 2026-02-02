# Common Instructions

Éléments **partagés par plusieurs clients**, mais **pas universels** (donc non-inclus dans `AGENTS.base.md`).

## Contenu

### Azure Standards
- `azure.general.md` – Principes généraux Azure (naming, tags, IAM, réseau, secrets)
- `azure.functions.md` – Azure Functions best practices
- `azure.adf.md` – Azure Data Factory guidelines
- `azure.apim.md` – API Management standards
- `azure.security.md` – Security policies Azure
- `azure.observability.md` – Monitoring et observabilité
- `azure.terraform.md` – Terraform conventions

Ces fichiers sont génériques et applicables à tous les clients utilisant Azure.

## Usage

Ces fichiers sont chargés **optionnellement** par les agents en fonction du besoin client.

Exemple : un client qui utilise Azure Functions chargerait `instructions/common/azure.functions.md`.

## Client Override

Un client peut **surcharger** une instruction commune en créant sa propre version dans `clients/<client-key>/instructions/`.

Exemple :
- `instructions/common/azure.general.md` (par défaut)
- `clients/sbm/instructions/azure-sbm-override.md` (SBM-specific override)

## Ordre de priorité

1. `instructions/AGENTS.base.md` (universel)
2. `instructions/common/` (partagé) ← YOU ARE HERE
3. `clients/<client>/instructions/` (client-specific, priorité sur common)
