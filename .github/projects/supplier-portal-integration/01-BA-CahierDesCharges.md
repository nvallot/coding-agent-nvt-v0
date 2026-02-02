# CAHIER DES CHARGES
## Supplier Performance Assessment - Intégrations Azure

**Document de référence** : MW_SBM_SupplierPortalAssessment  
**Flux traité** : IT 05a - NADIA to Supplier Performance Assessment  
**Auteur** : Business Analyst Agent  
**Date** : 2026-02-02  
**Statut** : DRAFT - En attente de validation

---

## 1. CONTEXTE PROJET

### 1.1 Vue d'ensemble

Le projet **Supplier Performance Assessment (SPA)** vise à centraliser dans Microsoft Dataverse les données nécessaires à l'évaluation de la performance des fournisseurs de SBM Offshore. 

Les données proviennent de 3 systèmes sources :
- **MDM** : Données maîtres des fournisseurs (IT 03)
- **NADIA** : Métadonnées des Purchase Orders (IT 05a) ← **FOCUS DE CE DOCUMENT**
- **IFS** : Données supplémentaires (IT 05b - non spécifié)

### 1.2 Périmètre fonctionnel

Ce document couvre **uniquement le flux IT 05a : NADIA → SPA**.

**Objectif** : Transférer quotidiennement les métadonnées des Purchase Orders depuis la base de données NADIA vers la table de staging Dataverse `sbm_stagedpurchaseorder`.

**Découpage en demi-flux (projets distincts)** :
- **Demi-flux entrant (NADIA → Service Bus)** : implémenté dans le **projet NADIA**
- **Demi-flux sortant (Service Bus → Dataverse + Lucy)** : implémenté dans le **projet Supplier Performance**

**Systèmes impliqués** :
- **Source** : NADIA SQL Database (SQL Server)
- **Enrichissement** : Lucy API (récupération des informations Package Manager)
- **Destination** : Microsoft Dataverse (Supplier Performance Assessment)

---

## 2. EXIGENCES FONCTIONNELLES

### 2.1 Trigger et Planification

**REQ-001 : Déclenchement quotidien**
- Le flux doit s'exécuter **1 fois par jour** selon un schedule configurable
- Heure recommandée : En dehors des heures de forte activité NADIA
- Le flux doit supporter le déclenchement manuel pour maintenance

**REQ-002 : Gestion de la date de dernière exécution**
- Le système doit enregistrer la date et l'heure de dernière exécution réussie
- Chaque exécution doit récupérer uniquement les données modifiées depuis la dernière exécution
- Le stockage de cette date doit être persistant (Table Storage Azure)

### 2.2 Extraction des Données NADIA

**REQ-003 : Appel de stored procedure**
- Nom : `NADIA_SPA_SUPHEADERMETADATA_AZURE`
- Paramètres :
  - `@LastExecutionDate` (NVARCHAR) : Date de dernière exécution
  - `@LastExecutionTime` (NVARCHAR) : Heure de dernière exécution
- La stored procedure applique les filtres métier (voir REQ-004)

**REQ-004 : Filtres métier appliqués**
La stored procedure NADIA doit retourner UNIQUEMENT les Purchase Orders qui respectent :
1. **Montant** : `Total PO Value (FCY) >= 100 000`
2. **Product Code** : Les 3 premiers caractères doivent être dans la liste `PKG`, `EQT`, `BLK`, `SER`, `LOG`
3. **Exclusion conditionnelle** : 
   - Si `PO Close Out Boolean = False` → Exclure les PO avec Product Code commençant par `SER` ou `LOG`

**REQ-005 : Données extraites**
Les champs suivants doivent être extraits de NADIA (voir section 4.1 pour mapping détaillé) :
- PO Number (with revision)
- MDM Vendor ID
- Package Manager GUID + Email
- Product Id (Category/Equipment Code)
- Total PO Value (FCY)
- First/Last Delivery Date
- PO Close Out Boolean (calculé)
- Last Modified Date + Time
- Project Number
- Description

### 2.3 Enrichissement via Lucy API

**REQ-006 : Récupération des informations Package Manager**
- Pour chaque PO, appeler l'API Lucy avec l'email du Package Manager
- **Base URL** : `https://apimgt-stg.sbmoffshore.com/lucy`
- **Endpoint** :
   ```
   GET /UserAccount?$format=json&$expand=user&$select=personIdExternal,email&$filter=tolower(user/email) eq tolower('{pkm_email}')
   ```
- **Données à récupérer** :
  - `PersonIdExternal` (ObjectId Entra ID)
  - `givenName` (Prénom)
  - `surName` (Nom)

**REQ-007 : Gestion des erreurs Lucy API**
- Si le Package Manager n'est pas trouvé dans Lucy :
  - Laisser les champs PKM vides (`sbm_pkmfirstname`, `sbm_pkmlastname`, `sbm_pkmpersonid`)
  - Ne PAS bloquer l'insertion de la ligne
  - Logger un avertissement avec l'email PKM concerné

### 2.4 Transformation et Mapping

**REQ-008 : Format des données**
- **PO Number** : Conserver le format avec révision (ex: "PO-12345-R02")
- **Dates** : Convertir en format `DateOnly` (sans heure)
- **Montant** : Decimal avec 2 décimales (Min: -100 000 000 000, Max: 100 000 000 000)
- **Boolean** : `PO Close Out Boolean` = True si `Last Delivery Date + 6 mois < Date du jour`
- **Last Update Date** : Combiner `Last Modified Date` et `Modified Time` en `DateTime`

**REQ-009 : Mapping Dataverse**
Tous les champs doivent être mappés vers la table `sbm_stagedpurchaseorder` selon le mapping détaillé en section 4.1.

### 2.5 Publication Service Bus

**REQ-010 : Message Service Bus par PO**
- Chaque Purchase Order doit générer **1 message** dans le Service Bus
- **Topic** : `purchase-orders` (dans namespace `supplier-events`)
- **Subscription** : Dédiée à la Function App `FAP-57`
- **Format** : JSON avec toutes les propriétés mappées

**REQ-011 : Propriétés du message**
- Inclure toutes les données en tant que **custom properties**
- Ajouter un `CorrelationId` pour traçabilité
- Inclure `SourceSystem = "NADIA"`

**REQ-011a : Contrat Service Bus (basé sur le mapping)**
- **Colonne A** = champs extraits SQL (stored procedure)
- **Colonne B** = **noms des propriétés Service Bus**
- **Colonne H** = champs de sortie Dataverse

**Dictionnaire de propriétés Service Bus (colonne B)** :
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

**Règles supplémentaires** :
- Ajouter `statuscode = OptionSetValue(918860002)` à la sortie Dataverse
- `sbm_pkmpersonid` **doit être enrichi via Lucy** à partir de `Email`

### 2.6 Chargement Dataverse

**REQ-012 : Insertion en staging table**
- Table cible : `sbm_stagedpurchaseorder`
- Opération : **Upsert** basé sur `sbm_ponumber` (clé)
- Statut initial : `statuscode = 1` (Draft)

**REQ-013 : Gestion du statut**
- Après insertion réussie : `statuscode = 918860002` (Ready to be Processed)
- ⚠️ **ATTENTION** : Ce code diffère des autres staging tables SBM

**REQ-014 : Validation des champs obligatoires**
Avant insertion, vérifier :
- `sbm_ponumber` (obligatoire)
- `sbm_mdmnumber` (obligatoire)
- `sbm_projectnumber` (obligatoire)
- `sbm_amount` (obligatoire)

Si un champ obligatoire est manquant → Rejeter la ligne et logger l'erreur.

---

## 3. EXIGENCES NON-FONCTIONNELLES

### 3.1 Performance

**REQ-NFT-001 : Volume de données**
- Capacité à traiter jusqu'à **10 000 PO par jour** (estimation)
- Temps maximum d'exécution : **30 minutes**

**REQ-NFT-002 : Parallélisation**
- Les messages Service Bus doivent être traités en parallèle (Function App scalable)
- Configurer `maxConcurrentCalls` approprié (recommandé : 16)

### 3.2 Fiabilité

**REQ-NFT-003 : Gestion des erreurs**
- **Retry Policy** : 3 tentatives avec exponential backoff
- **Dead Letter Queue** : Après 3 échecs, envoyer le message en DLQ
- **Alerting** : Notification si > 10% des messages échouent

**REQ-NFT-004 : Idempotence**
- L'upsert Dataverse doit être idempotent (réexécution sans duplication)
- Utiliser `sbm_ponumber` comme clé unique

### 3.3 Sécurité

**REQ-NFT-005 : Authentification**
- **NADIA SQL** : Utiliser compte SQL dédié (voir section 5.2)
- **Lucy API** : OAuth 2.0 avec Client Credentials
- **Dataverse** : Service Principal avec Client Secret (Key Vault)

**REQ-NFT-006 : Secrets Management**
- TOUS les secrets doivent être stockés dans **Key Vault** (`SBWE1-ISP-DV-KVA-01`)
- Utiliser **Managed Identity** pour accès Key Vault

### 3.4 Observabilité

**REQ-NFT-007 : Logging**
- Logs structurés dans **Application Insights**
- Inclure `CorrelationId` dans tous les logs
- Niveaux : Information (succès), Warning (PKM non trouvé), Error (échec)

**REQ-NFT-008 : Monitoring**
- Dashboard Application Insights avec métriques :
  - Nombre de PO traités
  - Taux d'erreur
  - Durée d'exécution
  - Nombre de messages en DLQ

**REQ-NFT-009 : ESB Dashboard**
- Intégrer le flux dans ESB Dashboard avec **Flow type = `scm-SPA`**
- **Monitoring.Client obligatoire** dans chaque demi-flux (`NADIA` et `Supplier Performance`)
   - Package : `<PackageReference Include="Monitoring.Client" Version="2.0.5" />`

---

## 4. SPÉCIFICATIONS TECHNIQUES

### 4.1 Mapping Détaillé NADIA → Dataverse

| **Source (NADIA)** | **Type** | **Tables** | **Destination (Dataverse)** | **Type** | **Transformation** |
|--------------------|----------|-----------|----------------------------|----------|--------------------|
| `[No.]` (PO Number with revision) | varchar(30) | POH | `sbm_ponumber` | Text(100) | Aucune |
| MDM ID (VendorID) | varchar(30) | Vendor | `sbm_mdmnumber` | Text(100) | Filtrer WHERE `MDM ID != ''` |
| Major Package Manager GUID | varchar(40) | Package/Resource | `sbm_pkmpersonid` | Text(100) | Depuis Lucy API |
| Email | varchar(80) | Resource | `sbm_pkmemail` | Text(100) | Aucune |
| Product Id | varchar(20) | Package | `sbm_productcode` | Text(100) | Format `XXX.XXX.XXX.XXX` |
| Total PO Value (FCY) | decimal(38,20) | Purchase Header Extension | `sbm_amount` | Decimal | 2 décimales |
| First Delivery Date | datetime | Package Lines | `sbm_firstdelivery` | DateOnly | Date uniquement |
| Last Delivery Date | datetime | Package Lines | `sbm_lastdelivery` | DateOnly | Date uniquement |
| PO Close Out Boolean | boolean (calculé) | Package Lines | `sbm_closeout` | Boolean | `Last Delivery Date + 6 mois` |
| Last Modified Date + Time | datetime2(3) | Package Lines | `sbm_erplastupdate` | DateTime | Combinaison date + time |
| Job No. | varchar(20) | POH | `sbm_projectnumber` | Text(100) | Aucune |
| Description | varchar(100) | Package | `sbm_description` | Text(2000) | Aucune |
| *(via Lucy)* | - | - | `sbm_pkmfirstname` | Text(100) | `givenName` depuis Lucy |
| *(via Lucy)* | - | - | `sbm_pkmlastname` | Text(100) | `surName` depuis Lucy |
| *(défaut)* | - | - | `statuscode` | Choice | `1` (Draft) → `918860002` (Ready) |

**Relations SQL** :
- POH = `[SBM$COSL Purchase Header]`
- V = `[SBM$Vendor]` (JOIN : `POH.[Buy-from Vendor No.] = V.[No.]`)
- PK = `[SBM$Package]` (JOIN : `POH.[Procurement Package No.] = PK.[Procurement Package No.]` AND `POH.[Job No.] = PK.[Project Id]` AND `POH.[Suffix] = PK.[Suffix]`)
- PKL = `[SBM$Package Lines]` (JOIN : `POH.[Procurement Package No.] = PKL.[Procurement Package Number]` AND `POH.[Job No.] = PKL.[Project Id]` AND `PKL.[Procurement Package Suffix] = POH.[Suffix]`)
- R = `[SBM$Resource]` (JOIN : `R.[No.] = PK.[Major Package Manager GUID]`)

### 4.2 Règles de Calcul

**PO Close Out Boolean** :
```
IF (Last Delivery Date + 6 mois) < Date du jour THEN
    PO Close Out = True
ELSE
    PO Close Out = False
END IF
```

**Last Update Date** :
```
Last Modified Date + ' ' + Modified Time
Format: YYYY-MM-DD hh:mm:ss.fff
```

### 4.3 Architecture Technique (Résumé)

**Composants Azure** :
1. **Function App FAP-65** (`SBWE1-ISP-DV-FAP-65`)
   - Timer Trigger (daily)
   - Appel stored procedure NADIA
   - Publication Service Bus
   - **Projet** : NADIA

2. **Service Bus** (`SBWE1-ISP-DV-SBN-02`)
   - Topic : `purchase-orders`
   - Subscription : dédiée FAP-57

3. **Function App FAP-57** (`SBWE1-ISP-DV-FAP-57`)
   - Service Bus Trigger
   - Appel Lucy API
   - Upsert Dataverse
   - **Projet** : Supplier Performance

4. **Ressources partagées** :
   - Key Vault (`SBWE1-ISP-DV-KVA-01`)
   - Application Insights (`SBWE1-ISP-DV-API-01`)
   - Storage Accounts (nadia, suportal)

---

## 5. CONNEXIONS ET ACCÈS

### 5.1 NADIA SQL Database

**Environnements** :
- **DEV/STG** : `nadia-db-stg.corpnet.singlebuoy.com`
- **PROD** : `nadia-db-prd.corpnet.singlebuoy.com`

**Database** : `NADIA`

**Compte SQL** :
- **DEV/STG** : `SQL_NADIA_IFS_AZURE_STG`
- **PROD** : `SQL_NADIA_IFS_AZURE`

**Stored Procedure** : `NADIA_SPA_SUPHEADERMETADATA_AZURE`

### 5.2 Lucy API

**Base URL** : `https://apimgt-stg.sbmoffshore.com/lucy`

**Authentification** : OAuth 2.0 Client Credentials
- Client ID : Depuis release pipeline variable
- Client Secret : Stocké dans Key Vault
- Scope : `api://api.lucy.{envLower}` (référence IFS, `CreateOrUpdatePurchaseOrder.cs`)

**Endpoint** :
```
GET /UserAccount?$format=json&$expand=user&$select=personIdExternal,email&$filter=tolower(user/email) eq tolower('{email}')
```

### 5.3 Dataverse API

**Base URL** :
- **DEV** : `https://sbmsupplierportaltest.crm4.dynamics.com/`
- **UAT** : `https://sbmsupplierportaluat.crm4.dynamics.com/`

**Authentification** : Service Principal
- Client ID : Variable `dataverseClientId` dans release pipeline
- Client Secret : `SUPPLIER-PORTAL-DATAVERSE-CLIENT-SECRET` dans Key Vault

---

## 6. USER STORIES

### US-001 : Extraction quotidienne des PO NADIA
**En tant que** système d'intégration  
**Je veux** extraire quotidiennement les Purchase Orders modifiés depuis NADIA  
**Afin que** les données SPA soient à jour pour l'évaluation des fournisseurs

**Critères d'acceptation** :
- [ ] Le flux s'exécute automatiquement 1 fois par jour
- [ ] Seules les PO modifiées depuis la dernière exécution sont récupérées
- [ ] Les filtres métier (montant, product code) sont appliqués
- [ ] La date de dernière exécution est mise à jour après succès

**Story Points** : 5

---

### US-002 : Enrichissement avec informations Package Manager
**En tant que** fonction d'intégration  
**Je veux** récupérer les informations du Package Manager depuis Lucy  
**Afin que** le nom complet du PKM soit disponible dans SPA

**Critères d'acceptation** :
- [ ] L'API Lucy est appelée avec l'email du PKM
- [ ] Les champs `givenName`, `surName`, `ObjectId` sont extraits
- [ ] Si le PKM n'existe pas dans Lucy, les champs PKM restent vides mais la ligne est insérée
- [ ] Un warning est loggé si le PKM n'est pas trouvé

**Story Points** : 3

---

### US-003 : Chargement en staging Dataverse
**En tant que** fonction d'intégration  
**Je veux** insérer/mettre à jour les PO dans la staging table Dataverse  
**Afin que** les données soient prêtes pour traitement par SPA

**Critères d'acceptation** :
- [ ] Les données sont insérées dans `sbm_stagedpurchaseorder`
- [ ] L'upsert utilise `sbm_ponumber` comme clé
- [ ] Le statut passe de `Draft (1)` à `Ready to be Processed (918860002)`
- [ ] Les champs obligatoires sont validés avant insertion

**Story Points** : 3

---

### US-004 : Gestion des erreurs et retry
**En tant que** système  
**Je veux** gérer les erreurs avec retry et dead letter queue  
**Afin que** les échecs temporaires ne bloquent pas le flux

**Critères d'acceptation** :
- [ ] 3 tentatives avec exponential backoff sur erreur
- [ ] Après 3 échecs, message envoyé en Dead Letter Queue
- [ ] Alerte si taux d'erreur > 10%
- [ ] Logs détaillés pour chaque erreur

**Story Points** : 5

---

### US-005 : Monitoring et observabilité
**En tant qu'** équipe support  
**Je veux** visualiser les métriques d'exécution du flux  
**Afin de** détecter et diagnostiquer rapidement les problèmes

**Critères d'acceptation** :
- [ ] Dashboard Application Insights avec nombre de PO traités
- [ ] Affichage du taux d'erreur et durée d'exécution
- [ ] Intégration ESB Dashboard
- [ ] CorrelationId présent dans tous les logs

**Story Points** : 3

---

## 7. CONTRAINTES ET LIMITES

### 7.1 Contraintes techniques
- La stored procedure NADIA est fournie par l'équipe NADIA (hors périmètre)
- Le format du `Product Code` est imposé : `XXX.XXX.XXX.XXX`
- Le code `statuscode = 918860002` est spécifique à cette staging table

### 7.2 Dépendances
- **Lucy API** : Doit être accessible et retourner les informations utilisateur
- **Dataverse** : La table `sbm_stagedpurchaseorder` doit exister avec le bon schéma
- **NADIA SQL** : Compte SQL doit avoir les droits sur la stored procedure

### 7.3 Points à clarifier

| **Question** | **Impact** | **Priorité** |
|--------------|-----------|--------------|
| Aucun point bloquant restant identifié à ce stade | - | - |

---

## 8. CRITÈRES DE SUCCÈS

### 8.1 Critères fonctionnels
✅ Les PO avec montant >= 100K et product code valide sont transférés quotidiennement  
✅ Les informations Package Manager sont enrichies depuis Lucy  
✅ Les données sont insérées dans Dataverse avec le bon statut  
✅ Les erreurs sont gérées avec retry et DLQ  

### 8.2 Critères techniques
✅ Temps d'exécution < 30 minutes pour 10 000 PO  
✅ Taux de succès > 90%  
✅ Tous les secrets dans Key Vault  
✅ Monitoring opérationnel dans Application Insights  

### 8.3 Critères de validation
- [ ] Tests unitaires : Couverture > 80%
- [ ] Tests d'intégration : Scénarios nominal + erreur
- [ ] Tests de charge : 10 000 PO en < 30 min
- [ ] Validation métier : Échantillon de 100 PO vérifiés manuellement

---

## 9. RISQUES IDENTIFIÉS

| **Risque** | **Probabilité** | **Impact** | **Mitigation** |
|------------|-----------------|------------|----------------|
| Lucy API indisponible | Moyenne | Élevé | Retry + fallback (laisser champs PKM vides) |
| Volume de données > 10K PO | Faible | Moyen | Monitoring + scalabilité Function App |
| Erreurs mapping stored procedure | Faible | Élevé | Tests d'intégration rigoureux |
| Timeouts NADIA SQL | Moyenne | Moyen | Configuration timeout SQL appropriée |

---

## 10. PLANNING ET LIVRABLES

### Phase 1 : Architecture (3 jours)
- [ ] Conception architecture détaillée (diagrammes)
- [ ] Spécifications techniques Function Apps
- [ ] Définition contrats Service Bus

### Phase 2 : Développement (10 jours)
- [ ] Fonction FAP-65 (NADIA → Service Bus)
- [ ] Fonction FAP-57 (Service Bus → Dataverse)
- [ ] Tests unitaires

### Phase 3 : Intégration (5 jours)
- [ ] Tests d'intégration bout en bout
- [ ] Configuration environnements DEV/STG
- [ ] Validation mapping données

### Phase 4 : Déploiement (2 jours)
- [ ] Déploiement PROD
- [ ] Formation équipe support
- [ ] Handover documentation

**Durée totale estimée** : 20 jours ouvrés

---

## 11. ANNEXES

### 11.1 Conventions de nommage SBM
Voir : `knowledge/sbm/naming-conventions.md`

### 11.2 Architecture ISP
Voir : `clients/sbm/instructions/sbm-isp-architecture-guidelines.md`

### 11.3 Documents sources
- `MW_SBM_SupplierPortalAssessment (1).docx` (Section IT 05a)
- `Mapping_Nadia_SPA_V2.xlsx`

---

## SIGNATURES

**Business Analyst** : ___________________  
**Solution Architect** : ___________________ (À compléter)  
**Product Owner** : ___________________  

---

*Ce document est un livrable du Business Analyst Agent dans le cadre du workflow BA → Architect → Developer. Prochaine étape : Handoff → Solution Architect pour conception architecture technique détaillée.*
