# Skill: Azure Function Deployment

**Type**: Deployment  
**Niveau**: Intermédiaire  
**Prérequis**: Azure CLI, `func` CLI, souscription Azure

## Description

Cette skill décrit comment déployer une **Azure Function** sur Azure, en respectant les conventions de nommage et les pratiques de déploiement. Adapter les noms de ressources selon le client (voir `knowledge/<client>/naming-conventions.md`).

## Prérequis

- Azure CLI authentifié (`az login`)
- Azure Functions Core Tools v4 (`func --version`)
- Resource Group existant (`rg-<client>-<project>-<env>-<instance>`)
- Function App créé (`func-<client>-<project>-<name>-<env>-<instance>`)

Voir `knowledge/<client>/naming-conventions.md` pour les conventions spécifiques.

## Étapes

### 1. Valider le projet localement

```powershell
cd src/NADIA/FAP-XX.FunctionName
func start
```

Vérifier que la fonction démarre sans erreur.

### 2. Builder le projet

```powershell
dotnet build -c Release
```

### 3. Publier vers Azure

```powershell
func azure functionapp publish <function-app-name>
```

Options:
- `--build remote` : build sur Azure (recommandé)
- `--dotnet-isolated` : pour .NET 8 isolated

### 4. Configurer Application Settings

```powershell
az functionapp config appsettings set `
  --name <function-app-name> `
  --resource-group <resource-group-name> `
  --settings "SettingName=Value"
```

Synchroniser avec `local.settings.json`.

### 5. Vérifier le déploiement

```powershell
# Logs de la fonction
func azure functionapp logstream <function-app-name>

# Status de la Function App
az functionapp show --name <function-app-name> --resource-group <resource-group-name> --query state
```

## Références

- Naming conventions: `knowledge/<client>/naming-conventions.md`
- Azure Functions best practices: [Microsoft Docs](https://learn.microsoft.com/azure/azure-functions/functions-best-practices)
- Client deployment guide: `clients/<client>/README.md`

## Related Skills

- `bicep-deployment`: Provisionner l'infrastructure
- `service-bus-setup`: Configurer Service Bus pour messages
