# ARCHITECTURE TECHNIQUE
## Flux IT05a – NADIA → Supplier Performance Assessment (SPA)

**Auteur** : Solution Architect Agent  
**Date** : 2026-02-02  
**Statut** : DRAFT  

---

## 1. Vue d’ensemble

Le flux IT05a est découpé en **deux demi‑flux** opérés par **deux projets distincts** :

1. **Demi‑flux entrant (Projet NADIA)** :
   - Extraction SQL NADIA → Publication Service Bus
2. **Demi‑flux sortant (Projet Supplier Performance)** :
   - Service Bus → Enrichissement Lucy → Upsert Dataverse

Objectif : alimenter la table Dataverse `sbm_stagedpurchaseorder` avec les métadonnées PO, enrichies via Lucy, avec statut `Ready to be Processed`.

---

## 2. Architecture logique

### 2.1 Composants Azure (DEV)

| Type | Nom (SBM) | RG | Rôle |
|------|-----------|----|------|
| Function App | SBWE1-ISP-DV-FAP-65 | IntegrationServices-**NDA**-RG | Demi‑flux entrant (Timer → SQL → SB) |
| Function App | SBWE1-ISP-DV-FAP-57 | IntegrationServices-**SPL**-RG | Demi‑flux sortant (SB → Lucy → Dataverse) |
| Service Bus Namespace | SBWE1-ISP-DV-SBN-02 | IntegrationServices-**CMN**-RG | Topic `purchase-orders` |
| Storage Account | sbwe1ispdvnadia | IntegrationServices-**NDA**-RG | Runtime FAP‑65 |
| Storage Account | sbwe1ispdvsuportal | IntegrationServices-**SPL**-RG | Runtime FAP‑57 |
| Key Vault | SBWE1-ISP-DV-KVA-01 | IntegrationServices-**CMN**-RG | Secrets |
| App Insights | SBWE1-ISP-DV-API-01 | IntegrationServices-**MON**-RG | Observabilité |

> Les mêmes patterns s’appliquent en STG/PRD (ENV = ST/PR).

### 2.2 Nommage Service Bus

- **Topic** : `purchase-orders`
- **Subscription** : dédiée au demi‑flux sortant (FAP‑57)
- **FlowType (Monitoring)** : `scm-SPA`

---

## 3. Diagramme d’architecture (Mermaid)

```mermaid
flowchart LR
  subgraph RG_NDA[RG-ISP-NDA-ENV]
    FAP65[Function App FAP-65\nTimer Trigger]
    SA1[Storage\nsbwe1isp{env}nadia]
  end

  subgraph RG_CMN[RG-ISP-CMN-ENV]
    SB[Service Bus\nSBWE1-ISP-ENV-SBN-02\nTopic: purchase-orders]
    KV[Key Vault\nSBWE1-ISP-ENV-KVA-01]
  end

  subgraph RG_SPL[RG-ISP-SPL-ENV]
    FAP57[Function App FAP-57\nSB Trigger]
    SA2[Storage\nsbwe1isp{env}suportal]
  end

  subgraph EXT[External Systems]
    NADIA[(NADIA SQL)]
    LUCY[Lucy API\nAPIM]
    DV[Dataverse\nsbm_stagedpurchaseorder]
  end

  FAP65 -->|Stored Proc| NADIA
  FAP65 -->|Publish| SB
  FAP57 -->|Consume| SB
  FAP57 -->|Enrich| LUCY
  FAP57 -->|Upsert| DV

  FAP65 --- KV
  FAP57 --- KV
```

---

## 4. Flux détaillé

### 4.1 Demi‑flux entrant (Projet NADIA)

1. **Timer Trigger** (daily) sur FAP‑65
2. Appel stored procedure `NADIA_SPA_SUPHEADERMETADATA_AZURE`
3. Construction message Service Bus selon mapping :
   - **Colonne A** : SQL fields
   - **Colonne B** : propriétés Service Bus
4. Publication sur topic `purchase-orders`
5. Monitoring via `Monitoring.Client` (FlowType = `scm-SPA`)

### 4.2 Demi‑flux sortant (Projet Supplier Performance)

1. Trigger Service Bus sur FAP‑57
2. Appel Lucy API (APIM) pour enrichir `sbm_pkmpersonid`
3. Mapping vers Dataverse (colonne H)
4. Upsert `sbm_stagedpurchaseorder`
5. Ajout `statuscode = OptionSetValue(918860002)`
6. Monitoring via `Monitoring.Client`

---

## 5. Contrat Service Bus

### 5.1 Propriétés attendues (colonne B)

- `PoReferenceRevised`
- `Identity`
- `In Lucy from Email`
- `Email`
- `Product Id`
- `PoCurrAmt`
- `FirstDeliveryDate`
- `LastDeliveryDate`
- `PO Close Out Boolean`
- `DateModified`
- `ProjectId`
- `Description`

### 5.2 Mapping vers Dataverse (colonne H)

- `sbm_ponumber`
- `sbm_mdmnumber`
- `sbm_pkmpersonid` (via Lucy)
- `sbm_pkmemail`
- `sbm_productcode`
- `sbm_amount`
- `sbm_firstdelivery`
- `sbm_lastdelivery`
- `sbm_closeout`
- `sbm_erplastupdate`
- `sbm_projectnumber`
- `sbm_description`
- `sbm_pkmfirstname`
- `sbm_pkmlastname`
- `statuscode` = 918860002

---

## 6. Sécurité & Identités

- **Managed Identity** pour accès Key Vault
- Secrets stockés dans **SBWE1-ISP-ENV-KVA-01**
- Lucy API via APIM :
  - Base URL : `https://apimgt-stg.sbmoffshore.com/lucy`
  - Scope : `api://api.lucy.{envLower}`

---

## 7. Observabilité

- **Monitoring.Client v2.0.5** obligatoire dans les deux demi‑flux
- `FlowType = scm-SPA`
- Application Insights centralisé
- Corrélation via `CorrelationId`

---

## 8. CI/CD (Azure DevOps)

- **Projet NADIA** : pipeline pour FAP‑65
- **Projet Supplier Performance** : pipeline pour FAP‑57
- Variables d’environnement centralisées (env, envLower)
- Déploiement DEV depuis feature branches

---

## 9. Risques techniques & mitigations

| Risque | Impact | Mitigation |
|--------|--------|------------|
| Lucy API indisponible | Enrichissement manquant | Retry + log warning |
| Timeouts SQL | Perte d’exécution | Pagination + tuning SP |
| DLQ volumineuse | Dégradation flux | Alerting + replay |

---

## 10. Livrables Architecte

- ✅ Architecture technique (ce document)
- ✅ Diagramme Mermaid
- ✅ Contrat Service Bus
- ✅ Règles sécurité / observabilité

---

**Prochaine étape** : Handoff vers **Developer** pour implémentation des demi‑flux (NADIA + Supplier Performance).