# CAHIER DES CHARGES FONCTIONNEL
## IT 05A - NADIA to Supplier Performance Assessment

**Date**: 30 janvier 2026  
**Version**: 1.0  
**Projet**: Integration Services Platform (ISP)  
**Flow**: IT 05a - NADIA → Supplier Performance Assessment (SPA)

---

## 1. CONTEXTE ET OBJECTIFS

### 1.1 Contexte Métier

SBM Offshore gère un volume important de commandes d'achat (Purchase Orders) dans son système ERP NADIA. Dans le cadre de l'amélioration de la relation fournisseurs, SBM souhaite évaluer la performance des fournisseurs via une application dédiée : **Supplier Performance Assessment (SPA)**, hébergée sur Dataverse.

### 1.2 Objectif du Flux

Synchroniser quotidiennement les métadonnées des Purchase Orders depuis NADIA vers SPA pour permettre :
- L'évaluation de la performance des fournisseurs
- Le suivi des commandes importantes (>100K)
- La gestion du cycle de vie des commandes
- L'identification des Package Managers responsables

### 1.3 Périmètre Fonctionnel

**Inclus** :
- ✅ Purchase Orders avec montant > 100 000 (toutes devises)
- ✅ PO liées aux catégories : PKG (Package), EQT (Equipment), BLK (Block), SER (Service), LOG (Logistique)
- ✅ Synchronisation des métadonnées PO Header uniquement
- ✅ Identification du Package Manager (PKM) responsable
- ✅ Calcul automatique du statut "Close Out" (6 mois après dernière livraison)

**Exclu** :
- ❌ PO Service (SER) non clôturées
- ❌ PO Logistique (LOG) de shipment non clôturées
- ❌ PO avec montant ≤ 100 000
- ❌ Lignes détaillées des PO (uniquement header)
- ❌ Documents attachés

### 1.4 Parties Prenantes

| Rôle | Équipe/Personne | Responsabilités |
|------|-----------------|-----------------|
| **Product Owner** | SBM Procurement | Validation fonctionnelle, UAT |
| **Architecte Solution** | MiddleWay | Architecture technique, design |
| **Équipe Développement** | MiddleWay | Implémentation, tests |
| **Administrateur NADIA** | SBM IT | Accès base de données, stored procedures |
| **Administrateur Dataverse** | SBM IT | Configuration environnement SPA |
| **Équipe Support** | MiddleWay | Monitoring, incidents |

---

## 2. EXIGENCES FONCTIONNELLES

### RF-001 : Récupération des Purchase Orders depuis NADIA

**Priorité** : Critique  
**Description** : Le système doit récupérer quotidiennement les Purchase Orders de NADIA répondant aux critères de sélection.

**Critères de sélection** :
1. Date de modification ≥ dernière date d'exécution
2. Montant PO > 100 000 (en devise locale)
3. Code produit commençant par : PKG, EQT, BLK, SER, ou LOG
4. Exclusion des PO Service non clôturées
5. Exclusion des PO Logistique de shipment non clôturées

**Règles de gestion** :
- RG-001.1 : La dernière date d'exécution est stockée dans Azure Table Storage
- RG-001.2 : En cas de première exécution, récupérer les PO des 30 derniers jours
- RG-001.3 : Les PO supprimées dans NADIA ne sont pas propagées (pas de suppression dans SPA)

**Données d'entrée** :
- LastExecutionDate (NVARCHAR format ISO 8601)
- LastExecutionTime (NVARCHAR format HH:MM:SS)

**Données de sortie** :
- Liste des Purchase Orders avec leurs métadonnées (voir RF-002)

---

### RF-002 : Mapping des Données PO

**Priorité** : Critique  
**Description** : Les données NADIA doivent être transformées selon le mapping défini pour Dataverse SPA.

**Table de mapping** :

| # | Donnée Source | Donnée Cible | Type | Obligatoire | Règle de Transformation |
|---|---------------|--------------|------|-------------|-------------------------|
| 1 | No. | sbm_ponumber | Text(100) | Oui | PO Number avec révision |
| 2 | Identity (MDM ID) | sbm_mdmnumber | Text(100) | Oui | MDM Vendor ID depuis table Vendor |
| 3 | Major Package Manager GUID | sbm_pkmpersonid | Text(100) | Oui | Object ID Entra ID (GUID) |
| 4 | Email | sbm_pkmemail | Text(100) | Oui | Email du PKM |
| 5 | Product Id | sbm_productcode | Text(100) | Oui | Format: XXX.XXX.XXX.XXX |
| 6 | PoCurrAmt | sbm_amount | Decimal | Oui | Montant total PO (devise locale) |
| 7 | FirstDeliveryDate | sbm_firstdelivery | Date | Non | Date de première livraison réelle |
| 8 | LastDeliveryDate | sbm_lastdelivery | Date | Non | Date de dernière livraison réelle |
| 9 | PO Close Out (calculé) | sbm_closeout | Boolean | Oui | True si LastDeliveryDate + 6 mois < Date du jour |
| 10 | DateModified | sbm_erplastupdate | DateTime | Oui | Date + Heure de dernière modification |
| 11 | Job No. | sbm_projectnumber | Text(100) | Non | Numéro de projet |
| 12 | Description | sbm_description | Text(2000) | Non | Description du package |
| 13 | (depuis Lucy API) | sbm_pkmfirstname | Text(100) | Oui | Prénom du PKM depuis Entra ID |
| 14 | (depuis Lucy API) | sbm_pkmlastname | Text(100) | Oui | Nom du PKM depuis Entra ID |
| 15 | (valeur par défaut) | statuscode | Choice | Oui | 918860002 (Ready to be Processed) |

**Règles de gestion** :
- RG-002.1 : Si PKM PersonID n'est pas trouvé dans Lucy/Entra ID, ignorer le PO et logger une erreur
- RG-002.2 : Si MDM Number est vide, ignorer le PO et logger une erreur
- RG-002.3 : Le montant doit être arrondi à 2 décimales
- RG-002.4 : Les dates doivent être au format ISO 8601 (YYYY-MM-DD)
- RG-002.5 : Le statuscode est toujours fixé à 918860002 pour les données en staging

---

### RF-003 : Enrichissement via Lucy API

**Priorité** : Critique  
**Description** : Pour chaque PO, le système doit récupérer les informations du Package Manager depuis Lucy API.

**Endpoint Lucy API** :
- Méthode : GET
- URL : `{LucyApiBaseUrl}/api/users/{PersonIdExternal}`
- Authentification : Managed Identity

**Attributs récupérés** :
- `givenName` → sbm_pkmfirstname
- `surName` → sbm_pkmlastname
- Validation : `PersonIdExternal` doit correspondre à un utilisateur Entra ID valide

**Règles de gestion** :
- RG-003.1 : En cas d'échec de l'appel Lucy API, logger l'erreur et ignorer le PO
- RG-003.2 : Implémenter un retry avec backoff exponentiel (3 tentatives max)
- RG-003.3 : Cache des informations PKM pour 24h pour optimiser les performances

---

### RF-004 : Calcul du Close Out

**Priorité** : Importante  
**Description** : Le système doit calculer automatiquement le statut de clôture d'un PO.

**Formule** :
```
PO Close Out = True SI (LastDeliveryDate + 6 mois < Date du jour)
PO Close Out = False SINON
```

**Règles de gestion** :
- RG-004.1 : Si LastDeliveryDate est NULL, Close Out = False
- RG-004.2 : Le calcul se fait à chaque synchronisation (pas de stockage intermédiaire)
- RG-004.3 : Pour les PO Service et Logistique, ne pas envoyer si Close Out = False

---

### RF-005 : Envoi vers Dataverse SPA

**Priorité** : Critique  
**Description** : Les données mappées et enrichies doivent être envoyées vers la table staging de Dataverse.

**Table de destination** : `sbm_stagedpurchaseorder`

**Méthode d'envoi** :
- API : Dataverse Web API
- Authentification : OAuth 2.0 Client Credentials
- Endpoint : `{DataverseBaseUrl}/api/data/v9.2/sbm_stagedpurchaseorders`

**Règles de gestion** :
- RG-005.1 : Upsert basé sur `sbm_ponumber` (clé unique)
- RG-005.2 : En cas de conflit, la version NADIA (source) fait foi
- RG-005.3 : Batch des envois par groupe de 50 pour optimiser les performances
- RG-005.4 : En cas d'échec partiel du batch, retraiter individuellement les enregistrements en erreur

---

### RF-006 : Planification de l'Exécution

**Priorité** : Importante  
**Description** : Le flux doit s'exécuter automatiquement selon un planning défini.

**Fréquence** : Quotidienne

**Horaires** :
- **DEV** : 02:00 CET
- **STG** : 03:00 CET
- **PRD** : 04:00 CET

**Règles de gestion** :
- RG-006.1 : Fenêtre d'exécution maximale : 2 heures
- RG-006.2 : Si l'exécution précédente n'est pas terminée, ne pas démarrer une nouvelle
- RG-006.3 : En cas d'échec, envoyer une alerte et ne pas mettre à jour la date de dernière exécution

---

### RF-007 : Gestion des Erreurs et Logging

**Priorité** : Importante  
**Description** : Toutes les erreurs et événements importants doivent être tracés.

**Événements à logger** :
1. Début/Fin de l'exécution (avec compteurs : PO traités, succès, erreurs)
2. Erreurs de connexion (NADIA, Lucy API, Dataverse)
3. PO ignorés (avec raison : PKM non trouvé, MDM vide, etc.)
4. Erreurs de transformation/mapping
5. Erreurs d'envoi vers Dataverse

**Niveaux de criticité** :
- **Critical** : Échec complet du flux
- **Error** : Erreur sur un PO individuel
- **Warning** : Comportement inattendu mais non bloquant
- **Info** : Événements normaux (début, fin, compteurs)

**Règles de gestion** :
- RG-007.1 : Tous les logs doivent être envoyés à Application Insights
- RG-007.2 : Les logs doivent inclure un CorrelationId unique par exécution
- RG-007.3 : Les PO en erreur doivent être envoyés à une dead-letter queue Service Bus

---

## 3. EXIGENCES NON FONCTIONNELLES

### RNF-001 : Performance

**Objectif** : Traiter 10 000 PO en moins de 30 minutes

**Mesures** :
- Temps de réponse API NADIA : < 5 secondes
- Temps de réponse API Lucy : < 2 secondes
- Temps de réponse API Dataverse : < 3 secondes (batch de 50)
- Débit global : > 5 PO/seconde

**Règles** :
- RNF-001.1 : Implémenter du parallélisme pour les appels Lucy API (max 10 threads)
- RNF-001.2 : Utiliser des batches pour les envois Dataverse (50 records/batch)

---

### RNF-002 : Disponibilité

**Objectif SLA** : 99.5% de disponibilité (hors maintenance planifiée)

**Mesures** :
- RPO (Recovery Point Objective) : 24 heures (1 exécution manquée acceptable)
- RTO (Recovery Time Objective) : 4 heures

**Règles** :
- RNF-002.1 : Implémenter des retry policies sur tous les appels externes
- RNF-002.2 : Circuit breaker pour protéger les APIs externes
- RNF-002.3 : Health check endpoint disponible pour monitoring

---

### RNF-003 : Sécurité

**Objectifs** :
- Aucune donnée sensible en clair dans le code ou les logs
- Principe du moindre privilège pour tous les accès
- Chiffrement des données en transit et au repos

**Mesures** :
- Authentification : Managed Identity pour tous les services Azure
- Secrets : Stockés dans Key Vault uniquement
- Données en transit : TLS 1.2 minimum
- Données au repos : Chiffrement Azure Storage et SQL

**Règles** :
- RNF-003.1 : Pas de mot de passe ou clé API en clair dans le code
- RNF-003.2 : Rotation automatique des secrets tous les 90 jours
- RNF-003.3 : Audit trail de tous les accès aux données sensibles

---

### RNF-004 : Scalabilité

**Objectif** : Supporter jusqu'à 50 000 PO par exécution sans dégradation

**Mesures** :
- Consommation Function App : < 30 GB/mois
- Consommation Service Bus : < 100 millions d'opérations/mois
- Consommation Storage : < 10 GB

---

### RNF-005 : Maintenabilité

**Objectifs** :
- Code lisible et documenté
- Architecture modulaire et testable
- Monitoring et observabilité complets

**Mesures** :
- Couverture de tests unitaires : > 80%
- Documentation technique à jour
- Dashboards Application Insights configurés
- Runbook opérationnel disponible

---

### RNF-006 : Conformité

**Réglementations applicables** :
- RGPD (données personnelles PKM)
- SOX (traçabilité des données financières)
- Politiques de sécurité SBM

**Mesures** :
- Conservation des logs : 90 jours (Application Insights)
- Anonymisation des données de test
- Pas de transfert de données hors UE

---

## 4. CONTRAINTES TECHNIQUES

### 4.1 Environnements

| Environnement | Base NADIA | Dataverse URL | Horaire Sync |
|---------------|------------|---------------|--------------|
| **DEV** | nadia-db-stg.corpnet.singlebuoy.com | sbmsupplierportaltest.crm4.dynamics.com | 02:00 CET |
| **STG** | nadia-db-stg.corpnet.singlebuoy.com | sbmsupplierportaluat.crm4.dynamics.com | 03:00 CET |
| **PRD** | nadia-db-prd.corpnet.singlebuoy.com | (TBD) | 04:00 CET |

### 4.2 Composants Azure

| Composant | Nom | Resource Group |
|-----------|-----|----------------|
| Function App (NADIA) | SBWE1-ISP-{ENV}-FAP-65 | IntegrationServices-VEN-RG |
| Function App (SPA) | SBWE1-ISP-{ENV}-FAP-57 | IntegrationServicesSTG-SPL-RG |
| Service Bus Topic | purchase-orders | IntegrationServicesDEV-CMN-RG |
| Service Bus Namespace | supplier-events | IntegrationServicesDEV-CMN-RG |
| Storage Account (NADIA) | sbwe1isp{env}nadia | IntegrationServices{ENV}-NDA-RG |
| Storage Account (SPA) | sbwe1isp{env}suportal | IntegrationServices{ENV}-SPL-RG |
| Key Vault | SBWE1-ISP-{ENV}-KVA-01 | IntegrationServices{ENV}-CMN-RG |

### 4.3 Dépendances Externes

| Système | Responsable | SLA | Impact si indisponible |
|---------|-------------|-----|------------------------|
| NADIA SQL Server | SBM IT | 99% | Flux bloqué - retry automatique |
| Lucy API | KJA / MiddleWay | 99% | Erreur enrichissement - PO ignoré |
| Dataverse API | Microsoft / SBM IT | 99.9% | Flux bloqué - retry automatique |
| Entra ID | Microsoft | 99.9% | Erreur auth - flux bloqué |

---

## 5. CAS D'USAGE

### CU-001 : Synchronisation Quotidienne Nominale

**Acteur** : Planificateur (Timer Trigger)

**Préconditions** :
- Le flux a déjà été exécuté au moins une fois
- NADIA, Lucy et Dataverse sont disponibles

**Scénario principal** :
1. Le timer déclenche la Function App FAP-65 à 04:00 CET
2. FAP-65 récupère la dernière date d'exécution depuis Table Storage
3. FAP-65 exécute la stored procedure `NADIA_SPA_SUPHEADERMETADATA_AZURE`
4. NADIA retourne 150 PO modifiés depuis la dernière exécution
5. FAP-65 envoie chaque PO dans Service Bus topic `purchase-orders`
6. FAP-57 est déclenché par chaque message Service Bus
7. Pour chaque PO, FAP-57 :
   - Appelle Lucy API pour enrichir les données PKM
   - Mappe les données vers le modèle Dataverse
   - Envoie vers `sbm_stagedpurchaseorder` (batch de 50)
8. FAP-65 met à jour la date de dernière exécution dans Table Storage
9. Un rapport de synthèse est loggé : 150 PO traités, 150 succès, 0 erreur

**Postconditions** :
- 150 nouveaux/mis à jour records dans `sbm_stagedpurchaseorder`
- LastExecutionDate mis à jour
- Logs disponibles dans Application Insights

---

### CU-002 : Gestion d'un PO avec PKM Non Trouvé

**Acteur** : Function App FAP-57

**Préconditions** :
- Un message PO est reçu depuis Service Bus
- Le PKM GUID référencé n'existe pas dans Lucy/Entra ID

**Scénario principal** :
1. FAP-57 reçoit le message PO n° "PO-12345-R01"
2. FAP-57 appelle Lucy API avec le PKM GUID
3. Lucy API retourne une erreur 404 "User not found"
4. FAP-57 :
   - Loggue une erreur : "PKM not found for PO-12345-R01, GUID: xxx-xxx-xxx"
   - Envoie le message vers la dead-letter queue
   - Ne met PAS à jour Dataverse
   - Incrémente le compteur d'erreurs
5. Une alerte est envoyée au support si > 10 PO avec PKM non trouvé

**Postconditions** :
- PO-12345-R01 n'est pas créé dans Dataverse
- Message dans dead-letter queue pour investigation
- Erreur tracée dans Application Insights

---

### CU-003 : Première Exécution (Cold Start)

**Acteur** : Administrateur

**Préconditions** :
- Le flux n'a jamais été exécuté
- Aucune entrée dans Table Storage "LastExecutionDate"

**Scénario principal** :
1. L'administrateur déclenche manuellement la Function App FAP-65
2. FAP-65 ne trouve pas de LastExecutionDate dans Table Storage
3. FAP-65 utilise par défaut : Date du jour - 30 jours
4. FAP-65 récupère environ 5000 PO historiques
5. Le traitement se déroule normalement (comme CU-001)
6. À la fin, FAP-65 crée l'entrée LastExecutionDate dans Table Storage

**Postconditions** :
- ~5000 PO chargés dans Dataverse
- LastExecutionDate initialisé
- Le flux est prêt pour les exécutions futures

---

### CU-004 : Échec Complet NADIA Indisponible

**Acteur** : Planificateur

**Préconditions** :
- La base NADIA est en maintenance

**Scénario principal** :
1. Le timer déclenche FAP-65 à 04:00 CET
2. FAP-65 tente de se connecter à NADIA SQL Server
3. Timeout après 30 secondes (3 tentatives avec retry)
4. FAP-65 :
   - Loggue une erreur Critical : "NADIA unreachable"
   - N'envoie AUCUN message Service Bus
   - NE met PAS à jour LastExecutionDate
   - Envoie une alerte critique au support
5. Le flux se terminera en erreur

**Postconditions** :
- Aucune donnée traitée
- LastExecutionDate inchangé (les données seront récupérées lors de la prochaine exécution)
- Alerte envoyée pour intervention manuelle

---

## 6. INTERFACES

### 6.1 Interface NADIA (IN)

**Type** : SQL Server Stored Procedure

**Endpoint** :
```sql
Server: nadia-db-{env}.corpnet.singlebuoy.com
Database: NADIA
Stored Procedure: NADIA_SPA_SUPHEADERMETADATA_AZURE
```

**Paramètres IN** :
- `@LastExecutionDate` (NVARCHAR) : Format "YYYY-MM-DD"
- `@LastExecutionTime` (NVARCHAR) : Format "HH:MM:SS"

**Résultat (ResultSet)** :
- Colonnes : Voir section RF-002 (mapping source)
- Format : Plusieurs rows (une par PO)

**Authentification** :
- SQL Account depuis Key Vault
- DEV/STG : `SQL_NADIA_IFS_AZURE_STG`
- PRD : `SQL_NADIA_IFS_AZURE`

---

### 6.2 Interface Lucy API (IN)

**Type** : REST API

**Endpoint** :
```
GET {LucyApiBaseUrl}/api/users/{PersonIdExternal}
```

**Authentification** : Managed Identity

**Réponse (JSON)** :
```json
{
  "personIdExternal": "guid",
  "givenName": "John",
  "surName": "Doe",
  "email": "john.doe@sbm.com"
}
```

**Codes HTTP** :
- 200 : Succès
- 404 : Utilisateur non trouvé
- 500 : Erreur serveur

---

### 6.3 Interface Dataverse API (OUT)

**Type** : REST API (OData)

**Endpoint** :
```
POST {DataverseBaseUrl}/api/data/v9.2/sbm_stagedpurchaseorders
```

**Authentification** : OAuth 2.0 Client Credentials
- Client ID : Depuis pipeline variable
- Client Secret : Depuis Key Vault (`SUPPLIER-PORTAL-DATAVERSE-CLIENT-SECRET`)

**Body (JSON)** :
```json
{
  "sbm_ponumber": "PO-12345-R01",
  "sbm_mdmnumber": "MDM-67890",
  "sbm_pkmpersonid": "guid-xxx-yyy-zzz",
  "sbm_pkmemail": "john.doe@sbm.com",
  "sbm_pkmfirstname": "John",
  "sbm_pkmlastname": "Doe",
  "sbm_productcode": "PKG.123.456.789",
  "sbm_amount": 250000.50,
  "sbm_firstdelivery": "2025-06-15",
  "sbm_lastdelivery": "2025-12-20",
  "sbm_closeout": false,
  "sbm_erplastupdate": "2025-11-26T14:30:00Z",
  "sbm_projectnumber": "PRJ-2025-001",
  "sbm_description": "Main turbine package for project XYZ",
  "statuscode": 918860002
}
```

**Codes HTTP** :
- 201 : Créé avec succès
- 204 : Mis à jour avec succès
- 400 : Données invalides
- 401 : Non authentifié
- 500 : Erreur serveur

---

### 6.4 Interface Service Bus (Interne)

**Type** : Message Queue

**Topic** : `purchase-orders`  
**Namespace** : `supplier-events`

**Format Message** :
```json
{
  "poNumber": "PO-12345-R01",
  "mdmNumber": "MDM-67890",
  "pkmGuid": "guid-xxx-yyy-zzz",
  "pkmEmail": "john.doe@sbm.com",
  "productCode": "PKG.123.456.789",
  "amount": 250000.50,
  "firstDelivery": "2025-06-15",
  "lastDelivery": "2025-12-20",
  "closeOut": false,
  "dateModified": "2025-11-26T14:30:00Z",
  "projectNumber": "PRJ-2025-001",
  "description": "Main turbine package for project XYZ"
}
```

**Propriétés Message** :
- `CorrelationId` : GUID unique par exécution
- `MessageId` : GUID unique par PO
- `SessionId` : Date d'exécution (pour ordonnancement)

---

## 7. CRITÈRES D'ACCEPTATION

### 7.1 Tests Fonctionnels

| ID | Scénario de Test | Critère de Succès |
|----|------------------|-------------------|
| TF-001 | Synchronisation de 100 PO valides | 100 PO créés/mis à jour dans Dataverse en < 5 min |
| TF-002 | PO avec montant = 99 000 | PO ignoré (< 100K) |
| TF-003 | PO avec code produit "ABC" | PO ignoré (pas dans PKG/EQT/BLK/SER/LOG) |
| TF-004 | PO Service avec Close Out = False | PO ignoré |
| TF-005 | PO avec PKM GUID invalide | PO dans dead-letter queue, erreur loggée |
| TF-006 | PO avec MDM Number vide | PO ignoré, erreur loggée |
| TF-007 | Calcul Close Out (LastDelivery + 6 mois < aujourd'hui) | Close Out = True |
| TF-008 | Calcul Close Out (LastDelivery + 6 mois > aujourd'hui) | Close Out = False |
| TF-009 | Premier lancement (cold start) | Récupération PO des 30 derniers jours |
| TF-010 | NADIA indisponible | Alerte envoyée, LastExecutionDate non mis à jour |

### 7.2 Tests Non Fonctionnels

| ID | Scénario de Test | Critère de Succès |
|----|------------------|-------------------|
| TNF-001 | Performance - 10 000 PO | Traitement en < 30 minutes |
| TNF-002 | Résilience - Lucy API 50% erreur | Retry automatique, 50% PO OK, 50% en erreur tracés |
| TNF-003 | Sécurité - Scan des secrets | Aucun secret en clair dans le code |
| TNF-004 | Monitoring - Dashboard | Métriques temps réel visibles dans Application Insights |
| TNF-005 | Logs - Recherche par CorrelationId | Tous les logs d'une exécution retrouvés en < 30 sec |

---

## 8. RISQUES ET HYPOTHÈSES

### 8.1 Hypothèses

| ID | Hypothèse | Impact si Faux | Mitigation |
|----|-----------|----------------|------------|
| H-001 | NADIA reste accessible 24/7 | Flux bloqué | Planifier synchro hors heures de maintenance |
| H-002 | Le PKM GUID correspond toujours à un utilisateur Entra ID valide | PO non transféré | Validation préalable des PKM dans NADIA |
| H-003 | Le montant PO est toujours en USD | Mauvais filtrage | Clarifier la devise avec le métier |
| H-004 | Les PO ne sont jamais supprimés, seulement modifiés | Pas de gestion de suppression | Confirmer la règle métier |
| H-005 | Le statut staging est toujours 918860002 | Erreur lors de l'insertion | Valider les choix possibles dans Dataverse |

### 8.2 Risques

| ID | Risque | Probabilité | Impact | Stratégie |
|----|--------|-------------|--------|-----------|
| R-001 | Volume de PO > 50 000 par jour | Faible | Élevé | Scalabilité horizontale Function Apps |
| R-002 | Lucy API lente (> 10 sec) | Moyenne | Élevé | Implémenter circuit breaker + cache |
| R-003 | Dataverse throttling | Moyenne | Moyen | Implémenter retry avec exponential backoff |
| R-004 | Stored procedure NADIA modifiée sans préavis | Faible | Élevé | Tests d'intégration automatisés |
| R-005 | Entra ID users non synchronisés avec NADIA | Moyenne | Moyen | Processus de validation préalable des PKM |

---

## 9. PLANNING ET JALONS

### Phase 1 : Architecture et Design (2 semaines)
- Validation architecture technique
- Design détaillé des Function Apps
- Configuration des environnements (DEV/STG/PRD)
- **Livrable** : Document d'architecture technique

### Phase 2 : Développement (4 semaines)
- Développement FAP-65 (NADIA retrieval)
- Développement FAP-57 (SPA sending)
- Implémentation Service Bus messaging
- Tests unitaires (> 80% couverture)
- **Livrable** : Code source + tests unitaires

### Phase 3 : Tests d'Intégration (2 semaines)
- Tests end-to-end DEV
- Tests de performance (10 000 PO)
- Tests de résilience (pannes simulées)
- **Livrable** : Rapport de tests d'intégration

### Phase 4 : UAT et Déploiement (2 semaines)
- UAT en environnement STG
- Corrections bugs
- Déploiement PRD
- Formation support
- **Livrable** : Flux en production + runbook

---

## 10. QUESTIONS OUVERTES

| ID | Question | Destinataire | Priorité |
|----|----------|--------------|----------|
| Q-001 | La devise des PO est-elle toujours USD ou faut-il gérer le multi-devise ? | SBM Procurement | Haute |
| Q-002 | Faut-il gérer la suppression de PO dans SPA si supprimé dans NADIA ? | SBM Procurement | Haute |
| Q-003 | Quelle est la fenêtre de maintenance NADIA à éviter ? | SBM IT (NADIA Admin) | Moyenne |
| Q-004 | Y a-t-il des contraintes de bande passante réseau pour l'accès NADIA ? | SBM IT Network | Moyenne |
| Q-005 | Faut-il archiver les PO > 2 ans dans un stockage séparé ? | SBM Procurement | Faible |

---

## 11. ANNEXES

### Annexe A : Glossaire

| Terme | Définition |
|-------|------------|
| **PO** | Purchase Order (Commande d'Achat) |
| **PKM** | Package Manager (Responsable du package) |
| **MDM** | Master Data Management |
| **SPA** | Supplier Performance Assessment |
| **NADIA** | ERP système source de SBM |
| **Dataverse** | Plateforme de données Microsoft Power Platform |
| **Entra ID** | Azure Active Directory (nouveau nom) |
| **Close Out** | Statut indiquant qu'un PO est clôturé (6 mois après dernière livraison) |

### Annexe B : Références

- **Document source** : MW_SBM_SupplierPortalAssessment.docx (Section 3.4.2)
- **Mapping détaillé** : Mapping_Nadia_SPA_V2.xlsx (Version 26/11/2025)
- **Wiki ISP** : https://dev.azure.com/sbm-offshore/Integration%20Services%20Platform/_wiki
- **Architecture globale ISP** : BrainBoard - ISP/Common/Core

---

## 12. APPROBATIONS

| Rôle | Nom | Date | Signature |
|------|-----|------|-----------|
| **Product Owner** | [À compléter] | | |
| **Architecte Solution** | [À compléter] | | |
| **Lead Développeur** | [À compléter] | | |
| **Responsable Sécurité** | [À compléter] | | |

---

**Préparé par** : Business Analyst - Mode "Business Analyst"  
**Date** : 30 janvier 2026  
**Version** : 1.0  
**Statut** : 🟡 En Attente d'Approbation

---

## NEXT STEPS

✅ **BA (Business Analyst)** - Cahier des charges fonctionnel ✔️ TERMINÉ

⏭️ **PROCHAINE ÉTAPE** : Architecture Technique
- Désigner l'architecte solution
- Concevoir l'architecture détaillée Azure
- Produire les spécifications techniques
- Générer les diagrammes d'architecture
- Créer les templates Terraform/Bicep

⏭️ **APRÈS** : Développement
- Implémenter les Function Apps
- Configurer Service Bus et APIs
- Développer les tests unitaires
- Préparer les pipelines CI/CD
