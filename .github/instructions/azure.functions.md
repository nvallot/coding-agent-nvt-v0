# Azure Functions – Standards

## Naming
- Function App : selon convention client (ex: `SBWE1-ISP-<ENV>-FAP-<NN>`)
- Functions HTTP : `http-{verb}-{resource}-v{n}`
- Functions autres : `{type}-{action}-{entity}`

## Paramètres
- Connection strings via Key Vault références
- `APPLICATIONINSIGHTS_CONNECTION_STRING` obligatoire
- CorrelationId obligatoire dans logs

## Sécurité
- Managed Identity par défaut
- Pas de secrets dans `local.settings.json` commités
