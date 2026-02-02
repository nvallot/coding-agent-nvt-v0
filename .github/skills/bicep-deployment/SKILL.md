# Skill: Bicep Infrastructure Deployment

**Type**: Infrastructure as Code  
**Niveau**: Avancé  
**Prérequis**: Azure CLI, Bicep CLI, droits Contributor sur la souscription

## Description

Cette skill explique comment déployer l'infrastructure Azure via **Bicep templates**, en respectant les standards client (naming, tagging, resource organization). Adapter les noms et tags selon `knowledge/<client>/naming-conventions.md`.

## Prérequis

- Bicep CLI installé (`az bicep version`)
- Azure CLI authentifié (`az login`)
- Souscription SBM sélectionnée (`az account set --subscription <sub-id>`)
- Fichiers Bicep préparés dans `infrastructure/bicep/`

## Étapes

### 1. Valider la syntaxe Bicep

```powershell
az bicep build --file infrastructure/bicep/main.bicep
```

Corriger toute erreur de syntaxe ou de paramètres.

### 2. Créer le Resource Group (si nécessaire)

```powershell
az group create `
  --name <resource-group-name> `
  --location <location> `
  --tags "Project=<project>" "Environment=<env>" "ManagedBy=<team>"
```

Utiliser les conventions de tagging du client (voir `knowledge/<client>/`).

### 3. Valider le déploiement (What-If)

```powershell
az deployment group what-if `
  --resource-group <resource-group-name> `
  --template-file <bicep-file> `
  --parameters <parameters-file>
```

Vérifier les ressources qui seront créées/modifiées.

### 4. Déployer l'infrastructure

```powershell
az deployment group create `
  --name "<project>-deployment-$(Get-Date -Format 'yyyyMMdd-HHmmss')" `
  --resource-group <resource-group-name> `
  --template-file <bicep-file> `
  --parameters <parameters-file>
```

### 5. Récupérer les outputs

```powershell
az deployment group show `
  --name <deployment-name> `
  --resource-group <resource-group-name> `
  --query properties.outputs
```

Stocker les outputs (connection strings, endpoints) dans Azure Key Vault ou variables de pipeline.

## Best Practices

- Utiliser des **parameter files** par environnement (dev, test, prod)
- Tagguer toutes les ressources avec `Project`, `Environment`, `ManagedBy`
- Versionner les templates Bicep dans le repo
- Utiliser des **modules Bicep** pour la réutilisabilité

## Références

- Naming conventions: `knowledge/<client>/naming-conventions.md`
- [Bicep best practices (Azure)](https://learn.microsoft.com/azure/azure-resource-manager/bicep/best-practices)
- Client architecture guidelines: `clients/<client>/instructions/`

## Related Skills

- `azure-function-deployment`: Déployer l'application après l'infra
- `service-bus-setup`: Configuration détaillée Service Bus
