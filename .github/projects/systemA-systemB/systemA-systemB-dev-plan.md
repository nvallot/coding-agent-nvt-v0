# Plan de Développement – systemA-systemB

**Version** : 1.0  
**Date** : 27 janvier 2026  
**Client** : client-demo  
**Auteur** : Developer  
**Source** : systemA-systemB-architecture.md  

---

## 1. Stack technologique retenue

| Composant | Technologie | Justification |
|-----------|-------------|---------------|
| **Langage** | Python 3.11+ | Productivité, écosystème riche, équipes disponibles |
| **Message Broker** | RabbitMQ | HA native, persistance, DLQ intégrée, léger |
| **API Framework** | FastAPI | Async natif, OpenAPI auto, performant |
| **Persistence** | PostgreSQL | ACID, fiabilité, JSON support |
| **Monitoring** | Prometheus + Grafana | Standard industrie, open source |
| **Logging** | Structlog + Loki | Logs structurés, centralisation |
| **Tracing** | OpenTelemetry | Standard ouvert, vendor-neutral |
| **Tests** | Pytest + Testcontainers | Isolation, tests d'intégration réalistes |
| **Infrastructure** | Docker + Docker Compose | Portabilité, reproductibilité |

---

## 2. Arborescence projet

```
systemA-systemB-integration/
├── README.md
├── pyproject.toml
├── docker-compose.yml
├── docker-compose.prod.yml
├── .env.example
├── Makefile
│
├── src/
│   ├── __init__.py
│   ├── config/
│   │   ├── __init__.py
│   │   └── settings.py              # Configuration centralisée
│   │
│   ├── core/
│   │   ├── __init__.py
│   │   ├── models.py                # Modèles de données canoniques
│   │   ├── exceptions.py            # Exceptions métier
│   │   └── interfaces.py            # Contrats/interfaces abstraites
│   │
│   ├── adapters/
│   │   ├── __init__.py
│   │   ├── base.py                  # Adapter abstrait
│   │   ├── adapter_a.py             # Connecteur systemA
│   │   └── adapter_b.py             # Connecteur systemB
│   │
│   ├── messaging/
│   │   ├── __init__.py
│   │   ├── broker.py                # Client RabbitMQ
│   │   ├── publisher.py             # Publication de messages
│   │   ├── consumer.py              # Consommation de messages
│   │   └── router.py                # Routage intelligent
│   │
│   ├── transformation/
│   │   ├── __init__.py
│   │   ├── engine.py                # Moteur de transformation
│   │   ├── canonical.py             # Format canonique
│   │   └── mappers/
│   │       ├── __init__.py
│   │       ├── a_to_canonical.py    # Mapping A → Canonical
│   │       └── canonical_to_b.py    # Mapping Canonical → B
│   │
│   ├── persistence/
│   │   ├── __init__.py
│   │   ├── database.py              # Connexion PostgreSQL
│   │   ├── repositories.py          # Repositories
│   │   └── models.py                # Modèles ORM
│   │
│   ├── monitoring/
│   │   ├── __init__.py
│   │   ├── metrics.py               # Métriques Prometheus
│   │   ├── health.py                # Health checks
│   │   └── tracing.py               # OpenTelemetry setup
│   │
│   ├── api/
│   │   ├── __init__.py
│   │   ├── main.py                  # FastAPI app
│   │   ├── routes/
│   │   │   ├── __init__.py
│   │   │   ├── health.py            # /health endpoints
│   │   │   ├── exchanges.py         # Consultation historique
│   │   │   └── replay.py            # Rejeu manuel
│   │   └── middleware/
│   │       ├── __init__.py
│   │       ├── auth.py              # Authentification
│   │       └── logging.py           # Request logging
│   │
│   └── workers/
│       ├── __init__.py
│       ├── processor_a_to_b.py      # Worker flux A→B
│       └── processor_b_to_a.py      # Worker flux B→A
│
├── tests/
│   ├── __init__.py
│   ├── conftest.py                  # Fixtures pytest
│   ├── unit/
│   │   ├── __init__.py
│   │   ├── test_models.py
│   │   ├── test_transformation.py
│   │   └── test_mappers.py
│   ├── integration/
│   │   ├── __init__.py
│   │   ├── test_messaging.py
│   │   ├── test_persistence.py
│   │   └── test_api.py
│   └── e2e/
│       ├── __init__.py
│       └── test_full_flow.py
│
├── scripts/
│   ├── init_db.py
│   ├── setup_rabbitmq.py
│   └── generate_test_data.py
│
├── docs/
│   ├── api.md
│   ├── deployment.md
│   └── runbook.md
│
└── infra/
    ├── docker/
    │   ├── Dockerfile
    │   ├── Dockerfile.worker
    │   └── rabbitmq/
    │       └── rabbitmq.conf
    ├── grafana/
    │   └── dashboards/
    │       └── integration-platform.json
    └── prometheus/
        └── prometheus.yml
```

---

## 3. Responsabilités par module

| Module | Responsabilité | Exigences couvertes |
|--------|----------------|---------------------|
| `core/` | Modèles canoniques, exceptions, interfaces | RF-003, RF-008 |
| `adapters/` | Connexion aux systèmes A et B | RF-001, RF-002, RF-009 |
| `messaging/` | Gestion RabbitMQ, queues, DLQ | RF-010, RNF-003 |
| `transformation/` | Conversion des formats | R-001 mitigation |
| `persistence/` | Stockage messages, journal audit | RF-004, RNF-012/13/14 |
| `monitoring/` | Métriques, health, traces | RNF-016, RF-005 |
| `api/` | Admin API REST | RF-006, RF-007 |
| `workers/` | Traitement asynchrone des messages | RF-001, RF-002 |

---

## 4. Phases de développement

### Phase 1 : Fondations (Sprint 1)
- [x] Configuration projet (pyproject.toml, Docker)
- [ ] Core models et exceptions
- [ ] Configuration centralisée
- [ ] Setup RabbitMQ + PostgreSQL

### Phase 2 : Messaging (Sprint 2)
- [ ] Client RabbitMQ avec retry
- [ ] Publisher/Consumer
- [ ] Dead Letter Queue
- [ ] Persistance messages

### Phase 3 : Adapters & Transformation (Sprint 3)
- [ ] Adapter abstrait
- [ ] Adapter A (mock initial)
- [ ] Adapter B (mock initial)
- [ ] Transformation engine
- [ ] Mappers A↔Canonical↔B

### Phase 4 : Workers & Flux (Sprint 4)
- [ ] Worker A→B
- [ ] Worker B→A
- [ ] Retry logic
- [ ] Error handling

### Phase 5 : Admin API (Sprint 5)
- [ ] FastAPI setup
- [ ] Endpoints health
- [ ] Endpoints consultation
- [ ] Endpoints rejeu
- [ ] Authentification

### Phase 6 : Observabilité (Sprint 6)
- [ ] Métriques Prometheus
- [ ] Dashboards Grafana
- [ ] Alerting rules
- [ ] Tracing OpenTelemetry

### Phase 7 : Tests & Hardening (Sprint 7)
- [ ] Tests unitaires (>80% coverage)
- [ ] Tests d'intégration
- [ ] Tests E2E
- [ ] Tests de charge
- [ ] Documentation

---

## 5. Stratégie de test

| Niveau | Scope | Outils | Coverage cible |
|--------|-------|--------|----------------|
| **Unit** | Fonctions isolées, mappers, validation | Pytest, unittest.mock | > 80% |
| **Integration** | Composants avec dépendances (DB, RabbitMQ) | Testcontainers | > 70% |
| **E2E** | Flux complet A→B et B→A | Docker Compose test | 100% des UC |
| **Charge** | Performance sous load | Locust | RNF-004, RNF-005 |
| **Chaos** | Résilience aux pannes | Toxiproxy | RNF-001, RNF-002 |

---

## 6. Points d'observabilité

### 6.1 Métriques (Prometheus)

| Métrique | Type | Labels |
|----------|------|--------|
| `integration_messages_total` | Counter | source, destination, status |
| `integration_message_duration_seconds` | Histogram | source, destination |
| `integration_queue_depth` | Gauge | queue_name |
| `integration_dlq_messages` | Gauge | queue_name |
| `integration_errors_total` | Counter | error_type, source |
| `integration_retry_count` | Counter | queue_name, attempt |

### 6.2 Logs structurés

```json
{
  "timestamp": "2026-01-27T10:30:00Z",
  "level": "INFO",
  "message": "Message processed",
  "correlation_id": "uuid-xxx",
  "source": "systemA",
  "destination": "systemB",
  "duration_ms": 150,
  "status": "success"
}
```

### 6.3 Health checks

| Endpoint | Check |
|----------|-------|
| `/health/live` | Process alive |
| `/health/ready` | RabbitMQ + PostgreSQL connectés |
| `/health/startup` | Initialisation complète |

---

## 7. Instructions de run

### 7.1 Prérequis

- Python 3.11+
- Docker & Docker Compose
- Make (optionnel)

### 7.2 Installation

```bash
# Cloner le projet
cd systemA-systemB-integration

# Environnement virtuel
python -m venv .venv
source .venv/bin/activate  # Linux/Mac
# ou .venv\Scripts\activate  # Windows

# Dépendances
pip install -e ".[dev]"

# Variables d'environnement
cp .env.example .env
```

### 7.3 Lancer l'infrastructure

```bash
# Démarrer RabbitMQ + PostgreSQL + Monitoring
docker-compose up -d

# Initialiser la base
python scripts/init_db.py
```

### 7.4 Lancer les services

```bash
# API Admin
uvicorn src.api.main:app --reload --port 8000

# Workers (dans des terminaux séparés)
python -m src.workers.processor_a_to_b
python -m src.workers.processor_b_to_a
```

### 7.5 Tests

```bash
# Tests unitaires
pytest tests/unit -v

# Tests intégration (nécessite Docker)
pytest tests/integration -v

# Tous les tests avec coverage
pytest --cov=src --cov-report=html
```

---

## 8. Handoff

### 8.1 État d'implémentation

| Composant | Statut |
|-----------|--------|
| Structure projet | ✅ Créée |
| Core models | ✅ Implémenté |
| Configuration | ✅ Implémenté |
| Messaging | ✅ Implémenté |
| Adapters | ✅ Implémenté (mock) |
| Transformation | ✅ Implémenté |
| Persistence | ✅ Implémenté |
| Workers | ✅ Implémenté |
| Admin API | ✅ Implémenté |
| Monitoring | ✅ Implémenté |
| Tests | ✅ Implémenté |
| Docker | ✅ Implémenté |

### 8.2 Points bloquants

- Aucun bloquant identifié
- Les adapters A et B sont en mode mock, à connecter aux vrais systèmes

### 8.3 Zones à risque / dette technique

| Zone | Risque | Action recommandée |
|------|--------|-------------------|
| Adapters mock | Non connectés aux vrais systèmes | Ateliers avec équipes A et B |
| Tests de charge | Non exécutés | Planifier avant MEP |
| Secrets management | .env file | Migrer vers vault en prod |

---

*Document généré conformément au framework AGENTS.base.md v1.0*
