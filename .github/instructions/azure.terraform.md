# Azure – Terraform Standards

## Structure recommandée
- `main.tf`, `variables.tf`, `outputs.tf`, `providers.tf`
- Modules par domaine : `modules/network`, `modules/functionapp`, `modules/apim`

## Règles
- Backend distant (Storage Account) obligatoire.
- `terraform.tfvars` ne contient aucun secret.
- Variables typées et documentées.
- `locals` pour le naming.
- Exposer les outputs utiles (id, name, fqdn).

## Naming (Terraform)
- Créer un `local.naming` avec concaténation normalisée.
- Exposer `name_prefix`, `env`, `region_code`.

## Sécurité
- Utiliser `azurerm_key_vault_secret` si besoin, mais **ne pas** stocker la valeur en clair.
- Preferer les références Key Vault côté app.

## Exemple de variables minimales
- `env`, `location`, `resource_group_name`, `tags`
