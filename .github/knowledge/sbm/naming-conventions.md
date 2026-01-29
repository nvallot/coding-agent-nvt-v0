# Naming Conventions – SBM

## Règles générales
- Tout en minuscule quand le service l’autorise (ex: Storage Accounts)
- Séparer par tirets quand c’est possible (ex: Service Bus, APIM, Key Vault)
- Garder un préfixe stable (site/plateforme) sur l’ensemble des ressources

## Environnements (codes observés)
- `DV` = dev
- `ST` = stg
- `PR` = prd

## Préfixe et structure standard (observée)
La majorité des ressources suivent ce modèle :

`<PREFIXE>-<PLATEFORME>-<ENV>-<SERVICE>-<INDEX>`

Exemples observés :
- `SBWE1-ISP-DV-DFA-01`
- `SBWE1-ISP-ST-APM-01`
- `SBWE1-ISP-PR-SBN-02`

### Tokens
- **PREFIXE** : `SBWE1` (site/zone)
- **PLATEFORME** : `ISP`
- **ENV** : `DV` / `ST` / `PR`
- **INDEX** : `01`, `02`, etc.

### Codes service (observés)
| Code | Ressource |
|------|-----------|
| `DFA` | Data Factory |
| `SBN` | Service Bus Namespace |
| `APM` | API Management |
| `KVA` | Key Vault |
| `FAP` | Function App |
| `ASP` | App Service Plan |
| `API` | Application Insights (ex: `SBWE1-ISP-DV-API-01`) |
| `LAP` | Log Analytics (ex: `SBWE1-ISP-PR-LAP-MONITOR`) |
| `WEB` | Web App / Site (ex: `SBWE1-ISP-DV-WEB-01`) |

> Ces codes sont basés sur les noms réellement observés dans Azure. Ils doivent être confirmés par le standard SBM.

## Resource Groups
Patterns observés :

- `IntegrationServicesDEV-<DOMAINE>-RG`
- `IntegrationServicesSTG-<DOMAINE>-RG`
- `IntegrationServices-<DOMAINE>-RG` (PRD sans préfixe ENV)

Exemples observés :
- `IntegrationServicesDEV-CMN-RG`
- `IntegrationServicesSTG-CMN-RG`
- `IntegrationServices-IFS-RG`

### Domaines observés
`CMN`, `IFS`, `EDB`, `ESH`, `MDM`, `MON`, `B2C`, `APA`, `NDA`, `SPL`, etc.

## Storage Accounts
Pattern observé (lowercase, sans tirets) :

`sbwe1isp<env><service><index>`

Exemples observés :
- `sbwe1ispdevdsta01`
- `sbwe1ispdevdlog01`

## APIs & Functions
- Function App : `SBWE1-ISP-<ENV>-FAP-<NN>`
- APIs exposées via APIM : `SBWE1-ISP-<ENV>-APM-<NN>`

## Remarques
Ces conventions sont **déduites des ressources visibles** et de la pipeline release. Dès que le Terraform officiel est accessible dans le workspace, je consolide et remplace par la règle officielle SBM.
