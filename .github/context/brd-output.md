---
created: 2026-02-05
updated: 2026-02-05
status: validated
agent: ba
client: demo-client
---

# Business Requirements Document (BRD)

## Projet : Azure Function – Transformation CSV vers Service Bus

---

## 1. Résumé Exécutif

### 1.1 Vision

Automatiser le traitement des fichiers CSV de commandes clients déposés dans Azure Blob Storage, en les transformant en messages JSON normalisés et en les publiant sur un topic Service Bus pour consommation par le système aval.

### 1.2 Problème à Résoudre

Actuellement, les fichiers CSV arrivent quotidiennement et nécessitent un traitement manuel ou semi-automatisé. Ce processus est source d'erreurs, de délais et ne garantit pas l'idempotence.

### 1.3 Bénéfices Attendus

| Bénéfice | Mesure |
|----------|--------|
| Automatisation complète | 0 intervention manuelle |
| Fiabilité | Idempotence garantie |
| Performance | < 5 sec par fichier |
| Traçabilité | Logs centralisés |

---

## 2. Parties Prenantes

| Rôle | Responsabilité |
|------|----------------|
| Product Owner | Validation des exigences métier |
| Équipe Data | Fournisseur des fichiers CSV |
| Équipe Backend | Consommateur des messages Service Bus |
| Équipe DevOps | Déploiement et monitoring |

---

## 3. Périmètre

### 3.1 In Scope

- ✅ Détection automatique des fichiers CSV (Blob Trigger)
- ✅ Parsing et validation du format CSV
- ✅ Transformation en JSON selon schéma défini
- ✅ Publication sur topic Service Bus
- ✅ Gestion de l'idempotence
- ✅ Logging et monitoring

### 3.2 Out of Scope

- ❌ Création/gestion du Blob Storage (existant)
- ❌ Création/gestion du Service Bus (existant)
- ❌ Traitement côté consommateur (système aval)
- ❌ Interface utilisateur

---

## 4. Exigences Fonctionnelles

### FR-001 : Déclenchement Automatique
| Attribut | Valeur |
|----------|--------|
| **Description** | L'Azure Function se déclenche automatiquement lors du dépôt d'un fichier CSV dans le container Blob désigné |
| **Priorité** | Must |
| **Critères d'acceptation** | - Le trigger se déclenche en moins de 30 secondes après dépôt<br>- Seuls les fichiers .csv sont traités<br>- Les autres formats sont ignorés avec log |

### FR-002 : Parsing CSV
| Attribut | Valeur |
|----------|--------|
| **Description** | Le système parse le fichier CSV et extrait les données de chaque ligne |
| **Priorité** | Must |
| **Critères d'acceptation** | - Support du séparateur point-virgule (;) et virgule (,)<br>- Première ligne = en-têtes<br>- Gestion des valeurs avec guillemets<br>- Encodage UTF-8 |

### FR-003 : Validation des Données
| Attribut | Valeur |
|----------|--------|
| **Description** | Chaque ligne est validée avant transformation |
| **Priorité** | Must |
| **Critères d'acceptation** | - Champs obligatoires présents<br>- Formats de données respectés (dates, nombres)<br>- Lignes invalides loguées mais n'arrêtent pas le traitement |

### FR-004 : Transformation JSON
| Attribut | Valeur |
|----------|--------|
| **Description** | Chaque ligne valide est transformée en message JSON conforme au schéma |
| **Priorité** | Must |
| **Critères d'acceptation** | - JSON valide et conforme au schéma défini<br>- Métadonnées ajoutées (timestamp, source, correlationId)<br>- Mapping des champs documenté |

### FR-005 : Publication Service Bus
| Attribut | Valeur |
|----------|--------|
| **Description** | Les messages JSON sont publiés sur le topic Service Bus |
| **Priorité** | Must |
| **Critères d'acceptation** | - Messages publiés en batch (optimisation)<br>- Gestion des erreurs de publication<br>- Retry automatique en cas d'échec transitoire |

### FR-006 : Idempotence
| Attribut | Valeur |
|----------|--------|
| **Description** | Un fichier ne peut être traité qu'une seule fois |
| **Priorité** | Must |
| **Critères d'acceptation** | - Mécanisme de tracking des fichiers traités<br>- Rejet des fichiers déjà traités avec log<br>- Possibilité de forcer un retraitement (manuel) |

### FR-007 : Logging et Traçabilité
| Attribut | Valeur |
|----------|--------|
| **Description** | Tous les traitements sont tracés |
| **Priorité** | Should |
| **Critères d'acceptation** | - Logs structurés (Application Insights)<br>- CorrelationId pour suivi bout-en-bout<br>- Métriques : fichiers traités, lignes, erreurs |

---

## 5. Exigences Non-Fonctionnelles

### NFR-001 : Performance
| Attribut | Valeur |
|----------|--------|
| **Description** | Temps de traitement < 5 secondes par fichier |
| **Mesure** | P95 du temps de traitement |
| **Cible** | ≤ 5 000 ms |

### NFR-002 : Disponibilité
| Attribut | Valeur |
|----------|--------|
| **Description** | Service disponible 24/7 |
| **Mesure** | Uptime mensuel |
| **Cible** | 99.5% |

### NFR-003 : Scalabilité
| Attribut | Valeur |
|----------|--------|
| **Description** | Supporter la volumétrie définie |
| **Mesure** | Capacité de traitement |
| **Cible** | 50 fichiers/jour, 5 000 lignes/fichier |

### NFR-004 : Sécurité
| Attribut | Valeur |
|----------|--------|
| **Description** | Accès sécurisé aux ressources Azure |
| **Mesure** | Conformité |
| **Cible** | Managed Identity, Private Endpoints, pas de secrets en clair |

### NFR-005 : Coûts
| Attribut | Valeur |
|----------|--------|
| **Description** | Optimisation des coûts Azure |
| **Mesure** | Coût mensuel estimé |
| **Cible** | Plan Consumption (évaluer Premium si latence requise) |

---

## 6. User Stories

### US-001 : Traitement Automatique
> **En tant que** système de données  
> **Je veux** que les fichiers CSV soient traités automatiquement  
> **Afin de** éliminer les interventions manuelles

**Critères d'acceptation :**
- [ ] Dépôt d'un fichier CSV déclenche le traitement
- [ ] Aucune action manuelle requise
- [ ] Notification en cas d'erreur

### US-002 : Transformation Fiable
> **En tant que** système aval  
> **Je veux** recevoir des messages JSON conformes au schéma  
> **Afin de** les traiter sans erreur de parsing

**Critères d'acceptation :**
- [ ] JSON valide selon schéma défini
- [ ] Métadonnées de traçabilité présentes
- [ ] Ordre des messages préservé

### US-003 : Monitoring
> **En tant que** opérateur  
> **Je veux** visualiser l'état des traitements  
> **Afin de** détecter rapidement les anomalies

**Critères d'acceptation :**
- [ ] Dashboard Application Insights
- [ ] Alertes en cas d'échec
- [ ] Métriques de performance visibles

---

## 7. Risques et Hypothèses

### 7.1 Hypothèses

| ID | Hypothèse |
|----|-----------|
| H-001 | Le Blob Storage et Service Bus existent et sont configurés |
| H-002 | Le schéma JSON cible est défini et stable |
| H-003 | Le format CSV est standardisé (séparateur, encodage) |
| H-004 | Les Private Endpoints sont déjà configurés sur le réseau |

### 7.2 Risques

| ID | Risque | Impact | Probabilité | Mitigation |
|----|--------|--------|-------------|------------|
| R-001 | Format CSV variable selon source | Moyen | Moyenne | Validation stricte + logs détaillés |
| R-002 | Volumétrie supérieure aux prévisions | Haut | Faible | Plan Premium si nécessaire |
| R-003 | Latence réseau vers Service Bus | Moyen | Faible | Private Endpoint + retry policy |
| R-004 | Fichiers corrompus ou vides | Faible | Moyenne | Validation en entrée + alertes |

---

## 8. Glossaire

| Terme | Définition |
|-------|------------|
| Blob Trigger | Déclencheur Azure Function activé par création/modification de blob |
| Idempotence | Propriété garantissant qu'un traitement produit le même résultat s'il est exécuté plusieurs fois |
| Service Bus Topic | File de messages pub/sub Azure permettant la distribution vers plusieurs abonnés |
| Managed Identity | Identité gérée par Azure permettant l'authentification sans secrets |

---

## Handoff

✅ **Analyse métier complétée**

| Élément | Statut |
|---------|--------|
| Exigences fonctionnelles | 7 FR définies |
| Exigences non-fonctionnelles | 5 NFR définies |
| User Stories | 3 US créées |
| Risques | 4 risques identifiés |

**Prochain agent** : `@archi`  
**Commande** : `#prompt:handoff-to-archi`

Le contexte complet est disponible dans ce fichier pour permettre à l'architecte de concevoir la solution technique.
