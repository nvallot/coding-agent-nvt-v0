# Architecture Cible – systemA-systemB

**Version** : 1.0  
**Date** : 27 janvier 2026  
**Client** : client-demo  
**Auteur** : Solution Architect  
**Statut** : Proposition initiale  
**Source** : systemA-systemB-cahier-des-charges.md  

---

## 1. Vue d'ensemble

### 1.1 Objectif architectural

Concevoir une architecture d'intégration permettant l'échange bidirectionnel de données entre systemA et systemB, garantissant :
- Haute disponibilité (≥ 99,5%)
- Zéro perte de données (RPO = 0)
- Traçabilité complète
- Reprise sur erreur automatisée

### 1.2 Pattern d'intégration retenu

**Event-Driven Architecture (EDA)** avec **Message Broker centralisé**

```
┌─────────────┐                                           ┌─────────────┐
│   systemA   │                                           │   systemB   │
│             │                                           │             │
│  ┌───────┐  │    ┌─────────────────────────────────┐    │  ┌───────┐  │
│  │Adapter│◄─┼────┤                                 ├────┼─►│Adapter│  │
│  │   A   │  │    │                                 │    │  │   B   │  │
│  └───┬───┘  │    │      INTEGRATION PLATFORM       │    │  └───┬───┘  │
│      │      │    │                                 │    │      │      │
│      ▼      │    │  ┌─────────┐    ┌───────────┐   │    │      ▼      │
│  ┌───────┐  │    │  │ Message │    │ Canonical │   │    │  ┌───────┐  │
│  │ API   │──┼───►│  │ Broker  │◄──►│ Transform │   │◄───┼──│ API   │  │
│  │Endpoint│ │    │  │  (HA)   │    │  Engine   │   │    │  │Endpoint│ │
│  └───────┘  │    │  └────┬────┘    └───────────┘   │    │  └───────┘  │
│             │    │       │                         │    │             │
└─────────────┘    │  ┌────▼────────────────────┐    │    └─────────────┘
                   │  │   Persistence Layer     │    │
                   │  │   (Journal + DLQ)       │    │
                   │  └─────────────────────────┘    │
                   │                                 │
                   │  ┌─────────────────────────┐    │
                   │  │  Monitoring & Alerting  │    │
                   │  └─────────────────────────┘    │
                   └─────────────────────────────────┘
```

### 1.3 Justification du pattern

| Critère | EDA + Message Broker | Alternative ESB | Alternative API Point-to-Point |
|---------|---------------------|-----------------|-------------------------------|
| Haute disponibilité | ✅ Clustering natif | ✅ Possible | ⚠️ Complexe |
| RPO = 0 | ✅ Persistance garantie | ✅ Possible | ❌ Risque élevé |
| Découplage | ✅ Fort | ✅ Moyen | ❌ Couplage fort |
| Évolutivité | ✅ Ajout de systèmes simple | ⚠️ Configuration lourde | ❌ Multiplicité des connexions |
| Complexité | ⚠️ Moyenne | ⚠️ Élevée | ✅ Faible |

**Décision** : EDA retenue pour le découplage, la résilience et l'évolutivité (RNF-017).

---

## 2. Architecture des composants

### 2.1 Vue composants

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                          INTEGRATION PLATFORM                                 │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐                  │
│  │   ADAPTER A    │  │   ADAPTER B    │  │  ADMIN API     │                  │
│  │                │  │                │  │                │                  │
│  │ • Connecteur   │  │ • Connecteur   │  │ • Rejeu        │                  │
│  │ • Validation   │  │ • Validation   │  │ • Consultation │                  │
│  │ • Sérialisation│  │ • Sérialisation│  │ • Monitoring   │                  │
│  └───────┬────────┘  └───────┬────────┘  └───────┬────────┘                  │
│          │                   │                   │                          │
│          ▼                   ▼                   ▼                          │
│  ┌───────────────────────────────────────────────────────────────────┐      │
│  │                      MESSAGE ROUTER                                │      │
│  │                                                                    │      │
│  │  • Routage intelligent basé sur le type de message                │      │
│  │  • Filtrage et validation                                         │      │
│  │  • Enrichissement de contexte (correlation ID, timestamp)         │      │
│  └────────────────────────────┬──────────────────────────────────────┘      │
│                               │                                              │
│          ┌────────────────────┼────────────────────┐                        │
│          ▼                    ▼                    ▼                        │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐                   │
│  │  QUEUE A→B   │    │  QUEUE B→A   │    │  DEAD LETTER │                   │
│  │              │    │              │    │    QUEUE     │                   │
│  │ • Persistante│    │ • Persistante│    │              │                   │
│  │ • FIFO       │    │ • FIFO       │    │ • Analyse    │                   │
│  │ • Durable    │    │ • Durable    │    │ • Rejeu      │                   │
│  └──────┬───────┘    └──────┬───────┘    └──────────────┘                   │
│         │                   │                                                │
│         ▼                   ▼                                                │
│  ┌───────────────────────────────────────────────────────────────────┐      │
│  │                   TRANSFORMATION ENGINE                            │      │
│  │                                                                    │      │
│  │  • Format A → Canonical → Format B                                │      │
│  │  • Format B → Canonical → Format A                                │      │
│  │  • Validation de schéma                                           │      │
│  └───────────────────────────────────────────────────────────────────┘      │
│                                                                              │
│  ┌───────────────────────────────────────────────────────────────────┐      │
│  │                    PERSISTENCE LAYER                               │      │
│  │                                                                    │      │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                │      │
│  │  │ Message     │  │ Audit       │  │ Config      │                │      │
│  │  │ Store       │  │ Journal     │  │ Store       │                │      │
│  │  └─────────────┘  └─────────────┘  └─────────────┘                │      │
│  └───────────────────────────────────────────────────────────────────┘      │
│                                                                              │
│  ┌───────────────────────────────────────────────────────────────────┐      │
│  │                 MONITORING & ALERTING                              │      │
│  │                                                                    │      │
│  │  • Métriques temps réel       • Dashboards                        │      │
│  │  • Alertes sur seuils         • Logs centralisés                  │      │
│  │  • Health checks              • Traçabilité distribuée            │      │
│  └───────────────────────────────────────────────────────────────────┘      │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

### 2.2 Description des composants

| Composant | Responsabilité | Exigences couvertes |
|-----------|----------------|---------------------|
| **Adapter A** | Interface avec systemA, validation entrante, sérialisation | RF-001, RF-008 |
| **Adapter B** | Interface avec systemB, validation entrante, sérialisation | RF-002, RF-008 |
| **Message Router** | Routage, enrichissement (UUID, timestamp), dispatch | RF-003 |
| **Queue A→B** | File persistante pour flux A vers B | RF-010, RNF-003 |
| **Queue B→A** | File persistante pour flux B vers A | RF-010, RNF-003 |
| **Dead Letter Queue** | Stockage des messages en échec pour analyse/rejeu | RF-007, R-002 |
| **Transformation Engine** | Conversion via format canonique | R-001 mitigation |
| **Message Store** | Persistance des messages pour RPO=0 | RNF-003 |
| **Audit Journal** | Traçabilité complète et immuable | RF-004, RNF-012/013/014 |
| **Admin API** | Interface de consultation et rejeu | RF-006, RF-007 |
| **Monitoring** | Supervision, alertes, dashboards | RNF-016, RF-005 |

---

## 3. Flux de données

### 3.1 Flux nominal A → B

```
┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐
│systemA  │───►│Adapter A│───►│ Router  │───►│Queue A→B│───►│Transform│───►│Adapter B│───►systemB
└─────────┘    └────┬────┘    └────┬────┘    └────┬────┘    └────┬────┘    └────┬────┘
                    │              │              │              │              │
                    ▼              ▼              ▼              ▼              ▼
               [Validate]    [Enrich:     [Persist]      [A→Canon     [Deliver +
                             UUID+TS]                     →B format]   ACK]
                    │              │              │              │              │
                    └──────────────┴──────────────┴──────────────┴──────────────┘
                                              │
                                              ▼
                                       [Audit Journal]
```

**Étapes détaillées** :

| # | Étape | Action | Erreur possible | Gestion erreur |
|---|-------|--------|-----------------|----------------|
| 1 | Réception | Adapter A reçoit la requête de systemA | Timeout, format invalide | Rejet + log |
| 2 | Validation | Contrôle du format et des champs obligatoires | Schéma non conforme | Rejet + notification (RF-005) |
| 3 | Enrichissement | Ajout UUID, timestamp UTC, correlation ID | - | - |
| 4 | Persistance | Écriture en queue durable | Échec stockage | Retry + alerte |
| 5 | Accusé source | Retour ACK à systemA | - | - |
| 6 | Transformation | Conversion Format A → Canonique → Format B | Mapping échoué | DLQ + alerte |
| 7 | Livraison | Envoi à systemB via Adapter B | systemB indisponible | Retry exponentiel |
| 8 | Accusé destination | Confirmation de systemB | Pas d'ACK | Retry puis DLQ |
| 9 | Journal | Écriture audit trail | - | - |

### 3.2 Flux nominal B → A

Symétrique au flux A → B, utilisant Queue B→A.

### 3.3 Flux de rejeu (UC-004)

```
┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐
│  Admin  │───►│Admin API│───►│   DLQ   │───►│Re-inject│───► [Flux nominal]
│  User   │    └─────────┘    └─────────┘    └─────────┘
└─────────┘
      │
      └──► [Audit: replay_requested by user X at timestamp]
```

---

## 4. Stratégies de résilience

### 4.1 Haute disponibilité (RNF-001 : ≥ 99,5%)

| Composant | Stratégie HA | Configuration |
|-----------|--------------|---------------|
| Message Broker | Cluster actif-actif | 3 nœuds minimum |
| Adapters | Instances multiples + Load Balancer | 2 instances par adapter |
| Persistence Layer | Réplication synchrone | Réplicas sur zones distinctes |
| Transformation Engine | Stateless + auto-scaling | Min 2 instances |

### 4.2 Garantie RPO = 0 (RNF-003)

```
┌─────────────────────────────────────────────────────────────────┐
│                    STRATÉGIE WRITE-AHEAD LOG                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│   1. Message reçu                                               │
│            │                                                    │
│            ▼                                                    │
│   2. Écriture WAL (Write-Ahead Log) ──► Persistance synchrone  │
│            │                                                    │
│            ▼                                                    │
│   3. ACK envoyé au producteur                                   │
│            │                                                    │
│            ▼                                                    │
│   4. Traitement asynchrone                                      │
│                                                                 │
│   ══► En cas de crash après étape 2 : message récupérable      │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 4.3 Stratégie de retry (RF-010)

| Niveau | Délai | Max tentatives | Action si échec |
|--------|-------|----------------|-----------------|
| 1 | 1 seconde | 3 | Passage niveau 2 |
| 2 | 30 secondes | 5 | Passage niveau 3 |
| 3 | 5 minutes | 10 | Envoi en DLQ + alerte |

**Backoff exponentiel** avec jitter pour éviter les thundering herds.

### 4.4 Dead Letter Queue

```
┌─────────────────────────────────────────────────────────────────┐
│                      DEAD LETTER QUEUE                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Structure du message DLQ :                                     │
│  {                                                              │
│    "originalMessage": { ... },                                  │
│    "errorDetails": {                                            │
│      "code": "TRANSFORM_ERROR",                                 │
│      "message": "Field X not mappable",                         │
│      "timestamp": "2026-01-27T10:30:00Z",                       │
│      "retryCount": 18                                           │
│    },                                                           │
│    "metadata": {                                                │
│      "correlationId": "uuid-xxx",                               │
│      "source": "systemA",                                       │
│      "destination": "systemB"                                   │
│    }                                                            │
│  }                                                              │
│                                                                 │
│  Actions disponibles :                                          │
│  • Analyse manuelle                                             │
│  • Correction + rejeu                                           │
│  • Suppression (avec audit)                                     │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 5. Format canonique

### 5.1 Justification

**Réponse à Q-004** : Oui, un format pivot canonique est recommandé.

| Sans format pivot | Avec format pivot |
|-------------------|-------------------|
| N×(N-1) transformations pour N systèmes | 2×N transformations |
| Couplage fort entre systèmes | Découplage total |
| Maintenance complexe | Évolutivité simple (RNF-017) |

### 5.2 Structure canonique proposée

```
┌─────────────────────────────────────────────────────────────────┐
│                    CANONICAL MESSAGE ENVELOPE                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  {                                                              │
│    "header": {                                                  │
│      "messageId": "uuid-v4",           // RF-003               │
│      "correlationId": "uuid-v4",                                │
│      "timestamp": "ISO-8601 UTC",      // RF-003               │
│      "source": "systemA | systemB",                             │
│      "destination": "systemA | systemB",                        │
│      "messageType": "domain.entity.action",                     │
│      "version": "1.0"                                           │
│    },                                                           │
│    "payload": {                                                 │
│      // Données métier normalisées                              │
│    },                                                           │
│    "metadata": {                                                │
│      "priority": "HIGH | NORMAL | LOW",                         │
│      "ttl": 3600,                                               │
│      "retryPolicy": "standard"                                  │
│    }                                                            │
│  }                                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 6. Sécurité

### 6.1 Architecture de sécurité

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                           SECURITY LAYERS                                     │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │ TRANSPORT SECURITY                                                   │    │
│  │ • TLS 1.3 (minimum 1.2) pour tous les flux           [RNF-008]      │    │
│  │ • Certificats mutuels (mTLS) pour authentification   [RNF-009]      │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │ AUTHENTICATION                                                       │    │
│  │ • Certificats X.509 pour systemA et systemB                         │    │
│  │ • Tokens JWT pour Admin API                                         │    │
│  │ • Rotation automatique des credentials                              │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │ AUTHORIZATION                                                        │    │
│  │ • RBAC pour Admin API                                [RNF-011]      │    │
│  │ • Rôles : Admin, Operator, Reader                                   │    │
│  │ • Principe du moindre privilège                                     │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │ AUDIT & COMPLIANCE                                                   │    │
│  │ • Logs immuables (append-only)                       [RNF-014]      │    │
│  │ • Horodatage cryptographique                                        │    │
│  │ • Rétention configurable                             [RNF-012]      │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

### 6.2 Cloisonnement des environnements (RNF-010)

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│       DEV       │    │       UAT       │    │      PROD       │
│                 │    │                 │    │                 │
│ • Réseau isolé  │    │ • Réseau isolé  │    │ • Réseau isolé  │
│ • Données fictives   │ • Données anonymisées│ • Données réelles│
│ • Accès équipe dev   │ • Accès équipe test  │ • Accès restreint│
│                 │    │                 │    │                 │
│ ══════════════  │    │ ══════════════  │    │ ══════════════  │
│ Pas de connexion│    │ Pas de connexion│    │ Production only │
│ vers PROD       │    │ vers PROD       │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

---

## 7. Monitoring et observabilité

### 7.1 Architecture d'observabilité

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                         OBSERVABILITY STACK                                   │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐           │
│  │     METRICS      │  │      LOGS        │  │     TRACES       │           │
│  │                  │  │                  │  │                  │           │
│  │ • Throughput     │  │ • Centralisés    │  │ • Distributed    │           │
│  │ • Latency P50/95 │  │ • Structurés     │  │   tracing        │           │
│  │ • Error rate     │  │ • Searchable     │  │ • Correlation ID │           │
│  │ • Queue depth    │  │ • Retention      │  │ • Span context   │           │
│  └────────┬─────────┘  └────────┬─────────┘  └────────┬─────────┘           │
│           │                     │                     │                      │
│           └─────────────────────┼─────────────────────┘                      │
│                                 ▼                                            │
│                    ┌──────────────────────┐                                  │
│                    │    DASHBOARDS        │                                  │
│                    │                      │                                  │
│                    │ • Vue temps réel     │                                  │
│                    │ • Historique         │                                  │
│                    │ • Alertes visuelles  │                                  │
│                    └──────────────────────┘                                  │
│                                                                              │
│                    ┌──────────────────────┐                                  │
│                    │    ALERTING          │                                  │
│                    │                      │                                  │
│                    │ • Seuils configurables│                                 │
│                    │ • Escalade           │                                  │
│                    │ • Multi-canal        │                                  │
│                    └──────────────────────┘                                  │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

### 7.2 Métriques clés (KPIs)

| Métrique | Seuil nominal | Alerte warning | Alerte critique |
|----------|---------------|----------------|-----------------|
| Disponibilité | ≥ 99,5% | < 99,5% | < 99% |
| Latence P95 | < 5s | > 5s | > 10s |
| Taux d'erreur | < 0,1% | > 0,5% | > 1% |
| Profondeur queue | < 1000 | > 5000 | > 10000 |
| Messages en DLQ | 0 | > 10 | > 100 |
| Âge plus vieux message | < 1min | > 5min | > 15min |

---

## 8. Réponses aux questions ouvertes

| ID | Question | Réponse architecturale |
|----|----------|----------------------|
| Q-001 | Pattern d'intégration | **Event-Driven + Message Broker** – Découplage maximal, résilience native |
| Q-002 | Garantie RPO = 0 | **Write-Ahead Log + Réplication synchrone** – Persistance avant ACK |
| Q-003 | Mécanisme de monitoring | **Stack complète Metrics/Logs/Traces** – Dashboards + alerting multi-seuils |
| Q-004 | Format pivot canonique | **Oui** – Envelope standardisée pour évolutivité |
| Q-005 | Stratégie retry/DLQ | **Backoff exponentiel 3 niveaux** – DLQ avec structure enrichie |
| Q-006 | Évolutivité | **Architecture en étoile** – Ajout d'adapter par système, format pivot central |

---

## 9. Hypothèses d'architecture

| ID | Hypothèse | Impact si invalide |
|----|-----------|-------------------|
| HA-001 | Les systèmes A et B supportent les appels asynchrones | Nécessité d'adapter le pattern |
| HA-002 | La latence réseau entre composants est < 10ms | Ajustement des timeouts |
| HA-003 | Le stockage persistant offre une durabilité de 99,999999% | Stratégie de backup additionnelle |
| HA-004 | Les équipes opérationnelles peuvent gérer un Message Broker | Formation ou service managé |

---

## 10. Risques architecturaux

| ID | Risque | Probabilité | Impact | Mitigation |
|----|--------|-------------|--------|------------|
| RA-001 | Single Point of Failure sur le broker | Faible | Critique | Cluster multi-nœuds obligatoire |
| RA-002 | Latence excessive en cas de pic | Moyenne | Moyen | Auto-scaling + tests de charge |
| RA-003 | Complexité opérationnelle | Moyenne | Moyen | Documentation + runbooks |
| RA-004 | Coût du stockage persistant | Faible | Faible | Politique de rétention + archivage |

---

## 11. Éléments non-couverts par cette architecture

| Élément | Raison | Phase suivante |
|---------|--------|----------------|
| Choix de produits/technologies | Neutralité technologique | Dossier d'architecture technique |
| Sizing infrastructure | Dépend de la volumétrie réelle | Étude de capacité |
| Schémas de données détaillés | Nécessite ateliers métier | Spécifications techniques |
| Procédures d'exploitation | Dépend de l'implémentation | Dossier d'exploitation |
| Stratégie de migration | Hors scope initial | Plan de migration |

---

## 12. Handoff vers le Developer

### 12.1 Livrables transmis

| Livrable | Statut |
|----------|--------|
| Architecture logique | ✅ Complet |
| Diagrammes de composants | ✅ Complet (ASCII) |
| Flux de données | ✅ Complet |
| Stratégies de résilience | ✅ Complet |
| Format canonique | ✅ Complet |
| Exigences de sécurité | ✅ Complet |

### 12.2 Attentes pour le Developer

1. **Choix technologiques** à valider :
   - Message Broker (options : RabbitMQ, Kafka, Azure Service Bus, etc.)
   - Base de données pour persistence
   - Stack de monitoring

2. **Implémentations à réaliser** :
   - Adapters A et B
   - Transformation Engine
   - Admin API
   - Infrastructure as Code

3. **Tests à prévoir** :
   - Tests unitaires des transformations
   - Tests d'intégration des flux
   - Tests de charge (volumétrie à définir)
   - Tests de résilience (chaos engineering)

### 12.3 Questions pour le Developer

- Quelle est la volumétrie cible pour le sizing ?
- Quels sont les formats de données actuels de systemA et systemB ?
- Existe-t-il des contraintes d'infrastructure (cloud, on-premise, hybride) ?

---

## Annexes

### A. Matrice de traçabilité architecture ↔ exigences

| Exigence | Composant(s) répondant |
|----------|----------------------|
| RF-001, RF-002 | Adapters A/B, Queues, Transformation Engine |
| RF-003 | Message Router (enrichissement UUID/timestamp) |
| RF-004 | Audit Journal |
| RF-005 | Monitoring & Alerting |
| RF-006 | Admin API |
| RF-007 | Dead Letter Queue, Admin API |
| RF-008 | Adapters (validation) |
| RF-009 | Adapters (ACK) |
| RF-010 | Queues persistantes, stratégie retry |
| RNF-001 | Cluster HA, Load Balancer |
| RNF-002 | Cluster HA, auto-restart |
| RNF-003 | Write-Ahead Log, réplication synchrone |
| RNF-007 à RNF-011 | Security Layers |
| RNF-012 à RNF-014 | Audit Journal |
| RNF-015 | Documentation (ce document) |
| RNF-016 | Observability Stack |
| RNF-017 | Architecture en étoile, format pivot |

### B. Glossaire technique

| Terme | Définition |
|-------|------------|
| EDA | Event-Driven Architecture |
| DLQ | Dead Letter Queue |
| WAL | Write-Ahead Log |
| mTLS | Mutual TLS (authentification bidirectionnelle) |
| RBAC | Role-Based Access Control |
| P95 | 95ème percentile |

---

*Document généré conformément au framework AGENTS.base.md v1.0*  
*Prêt pour handoff vers Developer*
