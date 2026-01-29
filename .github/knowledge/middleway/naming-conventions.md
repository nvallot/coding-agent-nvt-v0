# Naming Conventions – Middleway

## Règle générale

Dans ce document, nous listons les règles de nommage pour l'ensemble des **ressources principales** (directement incluses dans un resource group) et des ressources secondaires (incluses dans une ressource principale).

### Philosophie générale

- **Tout est en minuscule**
- Le nom d'une ressource doit être composé comme suit : **ENV**(-)**TYPE**(-)**CONTEXT**
- `(-)` signifie mettre un tiret quand c'est possible (pour certaines ressources, le tiret est interdit)

### Composants du nommage

| Composant | Description | Valeurs possibles |
|-----------|-------------|-------------------|
| **ENV** | Environnement | `dev`, `rec`, `prd` |
| **TYPE** | Type de ressource | `logic`, `st`, `func`, `sb`, etc. (voir abréviations ci-dessous) |
| **CONTEXT** | Contexte de la ressource | Voir règles de scope |

### Règles de scope pour CONTEXT

| Scope | Format |
|-------|--------|
| Global ou région | `mw(-)(instance ou version)` |
| Subscription ou ResourceGroup | `{ACTIVITE}(-){TIERS}(-)(detail)(-)(instance ou version)` |

### Définitions

- **ACTIVITE** : Nom du secteur d'activité (voir tableau des activités)
- **TIERS** : Application Middleway ou partenaire

---

## Cas particuliers

### Flux sans 1/2 flux

- Si l'échange de données n'est pas représenté par des 1/2 flux, alors TIERS contiendra **TIERSSOURCE-TIERSDESTINATAIRE**
- Si TIERSOURCE ou TIERSDESTINATAIRE représente un partenaire (et non une application Middleway), alors les ressources sont dans le Resource Group lié à l'application Middleway
- Si TIERSOURCE et TIERSDESTINATAIRE sont 2 applications Middleway, alors les ressources sont dans le Resource Group dont le tiers est TIERSOURCE-TIERSDESTINATAIRE

---

## Activités

| Nom | Diminutif |
|-----|-----------|
| Automotive | `auto` |
| BI | `bi` |
| Combined | `comb` |
| DevOps | `dops` |
| Management | `mgmt` |
| Storage | `stor` |
| Transport | `trsp` |
| Transverse | `trsv` |

---

## Composants Azure

### Compute

#### Logic App

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Logic App | `{env}-logic-{activité}-{tiers}-{description}` | `dev-logic-automotive-transportorders-toservicebus` |
| API Connection | `{env}-co-{protocole}-{description}` | `dev-co-ftp-lubrizol-orders` |
| Custom Connector | `{env}-cco-{activité}-{tiers}-{description}` | |

#### App Service Plan

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| App Service Plan | `{env}-plan-{activité}` | `dev-plan-common`, `prd-plan-automotive` |
| App Service Plan (Conso) | `{env}-planc-{activité}` | `dev-planc-management-niva` |

#### Function App

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Function App | `{env}-func-{activité}-{tiers}` | `dev-func-automotive-transportorders` |
| Solution | `Mw.{Activité}.<Api\|Common\|Flow>.{Nom}` | `Mw.Automotive.Api.TransportTrackingEvents` |
| Projet C# | `Mw.{Activité}.<Api\|Common\|Flow>.{Nom}[.Description][.Indice]` | `Mw.Transport.Flow.Cocacola.Helpers.02` |
| Namespaces | `{nom du projet azure function}` (par défaut) | |
| Classes | PascalCase (majuscule devant chaque mot) | |
| Azure Functions (API) | `{type}-{verbe http}-{opération}-{version}` | `http-get-orders-v2` |
| Azure Functions (autres) | `{type}-{action}-{entity}[-desc][-version]` | `sb-send-invoices`, `sched-retrieve-status` |

#### Relay

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Relay | `{env}-relay-mw-{activité}` | `dev-relay-mw-automotive` |
| Hybrid Connection | `{env}-hyco-{app.}[-{desc.}]` | `dev-hyco-proovstation` |

#### Integration Account

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Integration Account | `{env}-ia-mw` | `dev-ia-mw`, `rec-ia-mw` |
| Agreements | `{Type}-{Host partner}-{Guest partner}[-Format][-Version]` | `AS2-MW-Henkel`, `EDIFACT-GreenModal-Hapag-IFTMIN-D95B` |
| Partners | `{Nom}` (PascalCase) | `Hapag`, `MW`, `DirectEnergy` |
| Certificates | `{Public\|Private}-{Activité\|Common}-{Partner}` | `Private-Common-MW` |

---

### Storage

#### Storage Account

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Storage Account | `{env}st{activité}{tiers}` | `devstautomotiveinspec` |
| Storage Account (général) | `{env}stmw{tiers}` | `devstmwdevops` |
| Blob containers | `{nom}` (minuscules, tout attaché) | `backup`, `invoices` |
| Tables | `{nom}` (minuscules, tout attaché) | `devopscapacities` |

---

### Messaging

#### Service Bus Namespace

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Service Bus Namespace | `{env}-sb-mw-{activité}` | `dev-sb-mw-automotive` |
| SB Topic/Queue Common | `common-{service}[-{description}]` | `common-notification` |
| SB Topic/Queue Multi-tiers | `pivot-{businessentity}[-{description}]` | `pivot-commandeclient` |
| SB Topic/Queue Autres | `{source}-{businessentity}[-{description}]` | `hapag-commandemultimodal-toakanea` |
| SB Subscription | `{topic name}-{filter desc.}` | `common-notification-email` |

---

### Configuration

#### App Configuration

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| App Configuration | `{env}-appc-mw` | `dev-appc-mw` |
| Key | `{Activité}:{API\|Flows\|Services\|Common}:{Nom}:{Description}` | `Transport:API:Orders:Url:Base` |

#### Key Vault

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Key Vault | `{env}-kv-mw` | `dev-kv-mw`, `prd-kv-mw` |
| Secrets | `{Activité}--{API\|Flows\|Services\|Common}--{Nom}--{Description}` | `Transport--API--Orders--Secret1` |
| Certificates | `{Type}--{Activité\|Common}--{Description}` | `Encrypt--Automotive--VehicleDocuments` |
| Key | `{Activité\|Common}--{Description}` | `Transport--P44` |

---

### Monitoring

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| App Insights | `{env}-appi-mw` | `dev-appi-mw`, `prd-appi-mw` |
| Log Analytics Workspace | `{env}-log-mw` | `dev-log-mw`, `prd-log-mw` |

---

### APIM

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| APIM | `{env}-apim-mw` | `dev-apim-mw`, `prd-apim-mw` |
| Backends (interne) | `int-{composant}-{activité}-{API name}-{version}` | `int-func-storage-masterdata-v1` |
| Backends (externe) | `ext-{service}-{activité}-{API name}-{version}` | `ext-doctransfert-storage-masterdata-v1` |
| Name Values (communes) | `common-{description}` | `common-keyvault` |
| Name Values | `{activité}-{API name}-{version}-{description}` | `automotive-transporttrackingevents-v2-login` |
| Subscription Keys (app) | `mw-{application}-{produit}` | `mw-bringmycar-externalautomotivelogistics` |
| Subscription Keys (team) | `mw-team-{produit}` | `mw-team-internaltransport` |
| Subscription Keys (ext) | `ext-{entité}-{produit}` | `ext-fieldeas-externalautomotivelogistics` |

---

### Data Factory

| Composant | Template nommage | Exemple |
|-----------|------------------|---------|
| Data Factory | `{env}-adf-mw-{activité}` | `rec-adf-mw-bi` |
| Dossier | `{tiers}` (minuscules, tout attaché) | `thalesgabriel` |
| Pipelines | `pl-{action}-{entity}[-description]` | `pl-get-invoices-tobw` |
| Datasets | `ds-{type}-{entity}[-description]` | `ds-rest-order` |
| Link services | `ls-{type}-{système}[-description]` | `ls-rest-keeptracking-stockmanagement` |
| Triggers | `tr-{type}-{tiers}[-description]` | `tr-scheduled-thales-stockmanagement` |
| Data Flows | `df-{action}-{entity}[-description]` | |

---

## Ressources logiques

- Noms explicites
- Pas d'abréviations ambiguës

## Identifiants fonctionnels

- Préfixe fonctionnel obligatoire
- Exemple : `EXCH-ORDER-001`

## Documents

Format : `<source>-<target>-<artefact>.<extension>`
