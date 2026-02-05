---
applyTo: "**/*.tf"
---

# Instructions Terraform (Azure)

## 🎯 Objectif
Générer des fichiers Terraform propres, modulaires et prêts pour Azure DevOps / GitHub Actions.

## ✅ Conventions
- **Structure**: `providers.tf`, `main.tf`, `variables.tf`, `outputs.tf`, `*.tfvars`
- **Naming**: Azure CAF naming (resource naming standard)
- **Tags**: `Owner`, `CostCenter`, `Environment`, `Application`
- **Secrets**: jamais en dur, utiliser Key Vault + Managed Identity
- **Ressources critiques**: diagnostics vers Log Analytics

## 🧱 Providers
- `azurerm` + `aztfmod/azurecaf`
- Provider configuré avec `subscription_id` et `features {}`

## ♻️ Modèles
- Préférer des **modules** pour les ressources récurrentes
- Variables claires et documentées
- Outputs utiles (resource IDs, endpoints, names)

## 🧪 Qualité
- Idempotent
- Reproductible
- Diff minimal entre envs (`dev`, `prod` via tfvars)

## 🚀 CI/CD
- Prévoir: `terraform fmt`, `terraform validate`, `terraform plan`, `terraform apply`
- Stocker l’état dans un backend distant (Azure Storage)
