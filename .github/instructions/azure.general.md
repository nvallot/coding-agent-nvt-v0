# Azure – Principes généraux

## Objectif
Appliquer des standards Azure cohérents pour des projets data et intégration.

## Règles clés
- **Nommage** : respecter les conventions client (ex: SBM / Middleway) en priorité.
- **Tags** : `Client`, `Env`, `Domain`, `Owner`, `CostCenter` obligatoires.
- **Région** : une région principale + une secondaire si requis.
- **IAM** : RBAC minimal, éviter les rôles larges.
- **Identité** : Managed Identity privilégiée (systémique ou user-assigned).
- **Réseau** : Private Endpoints quand possible, limiter les IP publiques.
- **Secrets** : uniquement dans Key Vault.
- **Observabilité** : App Insights + Log Analytics systématique.

## À éviter
- Secrets dans le code, variables de pipeline ou Terraform tfvars.
- Ressources sans tags.
- Ressources sans logs ni métriques.
