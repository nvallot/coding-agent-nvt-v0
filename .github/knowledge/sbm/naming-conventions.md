# Naming Conventions – SBM (v0.1)

## Règles générales
- Tout en minuscule quand le service l’autorise (ex: Storage Accounts)
- Séparer par tirets quand c’est possible (ex: Service Bus, APIM, Key Vault)
- Garder un préfixe stable (site/plateforme) sur l’ensemble des ressources

## Environnements (codes observés)
- `DV` = dev
- `ST` = stg
- `PR` = prd

## Nommage standard (ressources avec tirets)

**Modèle observé** :
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
| `API` | Application Insights |
| `LAP` | Log Analytics |
| `WEB` | Web App / Site |

> Ces codes sont basés sur les noms réellement observés dans Azure et les documents internes. Ils doivent être confirmés par le standard SBM officiel.

## Nommage lower-case (ressources contraintes)

**Pattern observé** :
`sbwe1isp<env><service><index>`

Exemples observés :
- `sbwe1ispdevdsta01`
- `sbwe1ispdevdlog01`

## Resource Groups

Patterns observés :
- `IntegrationServicesDEV-<DOMAINE>-RG`
- `IntegrationServicesSTG-<DOMAINE>-RG`
- `IntegrationServices-<DOMAINE>-RG` (PRD sans préfixe ENV)

Exemples observés :
- `IntegrationServicesDEV-CMN-RG`
- `IntegrationServicesSTG-CMN-RG`
- `IntegrationServices-IFS-RG`

Domaines observés :
`CMN`, `IFS`, `EDB`, `ESH`, `MDM`, `MON`, `B2C`, `APA`, `NDA`, `SPL`, etc.

## Service Bus (noms d’entités)

**Pattern recommandé** :
`<project>.<type>.<entity>.<event>`

- `<project>`: trigramme projet (lowercase)
- `<type>`: `q` (queue), `t` (topic), `s` (subscription)
- `<entity>`: entité métier (lowercase)
- `<event>`: action/événement (optionnel)

Exemples :
- `ifs.q.purchaseorder.created`
- `lucy.t.employee.updated`
- `lucy.s.employee.payroll`

## Tags obligatoires (projet)

| Tag | Description | Exemple |
|-----|-------------|---------|
| `env` | Environnement | `DEV`, `STG`, `PRD` |
| `flow` | Code flow/projet | `IFS`, `LUCY` |
| `desc` | Description courte | `ProcessOrders` |

## App Configuration (clés)

**Pattern** : `<Project>:<Component>:<Setting>`

Exemples :
- `ISP:IFS:ApiUrl`
- `ISP:ServiceBus:QueueName`

## Remarques
Ces conventions sont **déduites des ressources visibles** et des documents internes. Dès que le Terraform officiel est accessible dans le workspace, consolider et remplacer par la règle SBM officielle.
