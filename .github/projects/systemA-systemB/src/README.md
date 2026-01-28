# systemA-systemB Integration Platform

Plateforme d'intégration pour l'échange bidirectionnel de données entre systemA et systemB.

## 🚀 Quick Start

```bash
# 1. Cloner et installer
cd systemA-systemB-integration
python -m venv .venv
source .venv/bin/activate  # Windows: .venv\Scripts\activate
pip install -e ".[dev]"

# 2. Configuration
cp .env.example .env

# 3. Lancer l'infrastructure
docker-compose up -d

# 4. Initialiser la base
python scripts/init_db.py

# 5. Lancer l'API
uvicorn src.api.main:app --reload --port 8000

# 6. Lancer les workers
python -m src.workers.processor_a_to_b
python -m src.workers.processor_b_to_a
```

## 📁 Structure

```
src/
├── config/      # Configuration centralisée
├── core/        # Modèles, exceptions, interfaces
├── adapters/    # Connecteurs systemA/B
├── messaging/   # RabbitMQ client
├── transformation/  # Moteur de transformation
├── persistence/ # PostgreSQL
├── monitoring/  # Métriques, health, tracing
├── api/         # Admin API FastAPI
└── workers/     # Processeurs async
```

## 🧪 Tests

```bash
pytest                          # Tous les tests
pytest tests/unit -v            # Tests unitaires
pytest --cov=src --cov-report=html  # Avec coverage
```

## 📊 Endpoints

| Endpoint | Description |
|----------|-------------|
| `GET /health/live` | Liveness check |
| `GET /health/ready` | Readiness check |
| `GET /api/v1/exchanges` | Liste des échanges |
| `GET /api/v1/exchanges/{id}` | Détail d'un échange |
| `POST /api/v1/exchanges/{id}/replay` | Rejeu d'un échange |
| `GET /metrics` | Métriques Prometheus |

## 📖 Documentation

- [Plan de développement](systemA-systemB-dev-plan.md)
- [Architecture](systemA-systemB-architecture.md)
- [Cahier des charges](systemA-systemB-cahier-des-charges.md)
