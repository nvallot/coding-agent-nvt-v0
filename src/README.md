# 🏗️ Generic App - Architecture 3-Tiers

Application générique basée sur une architecture 3-tiers (Présentation, Application, Données) avec Python/FastAPI.

## 📋 Table des matières

- [Architecture](#-architecture)
- [Structure du projet](#-structure-du-projet)
- [Installation](#-installation)
- [Démarrage rapide](#-démarrage-rapide)
- [API Documentation](#-api-documentation)
- [Tests](#-tests)
- [Déploiement](#-déploiement)

## 🏛️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    COUCHE PRÉSENTATION                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │   API v1    │  │   Schemas   │  │Dependencies │             │
│  │  (FastAPI)  │  │  (Pydantic) │  │ (Injection) │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    COUCHE APPLICATION                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │ AuthService │  │ UserService │  │ ItemService │             │
│  │   (Auth)    │  │   (Users)   │  │   (Items)   │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    COUCHE DOMAINE                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │    User     │  │    Item     │  │    Base     │             │
│  │  (Entity)   │  │  (Entity)   │  │  (Entity)   │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    COUCHE INFRASTRUCTURE                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │ Repositories│  │  Database   │  │    Cache    │             │
│  │ (SQLAlchemy)│  │ (PostgreSQL)│  │   (Redis)   │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
```

## 📁 Structure du projet

```
src/
├── app/
│   ├── __init__.py
│   ├── main.py                 # Point d'entrée FastAPI
│   │
│   ├── core/                   # Configuration et utilitaires
│   │   ├── config.py           # Settings avec pydantic-settings
│   │   ├── security.py         # JWT, hashage mots de passe
│   │   ├── logging.py          # Logging structuré
│   │   └── exceptions.py       # Exceptions personnalisées
│   │
│   ├── domain/                 # Modèles de domaine (DDD)
│   │   └── entities/
│   │       ├── base.py         # Entité de base
│   │       ├── user.py         # Entité User
│   │       └── item.py         # Entité Item
│   │
│   ├── application/            # Logique métier
│   │   └── services/
│   │       ├── auth_service.py
│   │       ├── user_service.py
│   │       └── item_service.py
│   │
│   ├── presentation/           # API HTTP
│   │   ├── api/
│   │   │   ├── health.py       # Endpoints de santé
│   │   │   └── v1/
│   │   │       ├── __init__.py # Router principal v1
│   │   │       └── endpoints/
│   │   │           ├── auth.py
│   │   │           ├── users.py
│   │   │           └── items.py
│   │   ├── schemas/            # DTOs Pydantic
│   │   │   ├── auth.py
│   │   │   ├── user.py
│   │   │   └── item.py
│   │   └── dependencies.py     # Injection de dépendances
│   │
│   └── infrastructure/         # Services externes
│       ├── database.py         # Configuration SQLAlchemy
│       ├── cache.py            # Client Redis
│       ├── models/             # Modèles ORM
│       │   ├── user.py
│       │   └── item.py
│       └── repositories/       # Accès données
│           ├── base.py
│           ├── user_repository.py
│           └── item_repository.py
│
├── tests/
│   ├── conftest.py             # Fixtures pytest
│   ├── unit/                   # Tests unitaires
│   │   ├── test_entities.py
│   │   ├── test_security.py
│   │   └── test_services.py
│   └── integration/            # Tests d'intégration
│       └── test_api.py
│
├── scripts/
│   └── init-db.sql             # Script d'initialisation DB
│
├── monitoring/
│   └── prometheus.yml          # Configuration Prometheus
│
├── pyproject.toml              # Configuration projet Python
├── Dockerfile                  # Image Docker multi-stage
├── docker-compose.yml          # Stack complète
└── .env.example                # Variables d'environnement
```

## 🚀 Installation

### Prérequis

- Python 3.11+
- Docker & Docker Compose (recommandé)
- PostgreSQL 16+ (si sans Docker)
- Redis 7+ (si sans Docker)

### Installation locale

```bash
# Cloner le projet
cd src

# Créer l'environnement virtuel
python -m venv .venv
source .venv/bin/activate  # Linux/Mac
# ou
.venv\Scripts\activate     # Windows

# Installer les dépendances
pip install -e ".[dev]"

# Copier la configuration
cp .env.example .env
# Éditer .env avec vos valeurs
```

## ⚡ Démarrage rapide

### Avec Docker (recommandé)

```bash
# Démarrer toute la stack
docker-compose up -d

# Voir les logs
docker-compose logs -f app

# Arrêter
docker-compose down
```

### Sans Docker

```bash
# S'assurer que PostgreSQL et Redis sont en cours d'exécution

# Démarrer l'application
uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
```

L'API est accessible sur `http://localhost:8000`

## 📚 API Documentation

Une fois l'application démarrée :

- **Swagger UI** : http://localhost:8000/api/docs
- **ReDoc** : http://localhost:8000/api/redoc
- **OpenAPI JSON** : http://localhost:8000/api/openapi.json

### Endpoints principaux

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | `/health` | Vérification de santé |
| GET | `/health/ready` | Readiness check |
| POST | `/api/v1/auth/login` | Authentification |
| GET | `/api/v1/users` | Liste des utilisateurs |
| POST | `/api/v1/users` | Créer un utilisateur |
| GET | `/api/v1/items` | Liste des items |
| POST | `/api/v1/items` | Créer un item |

### Authentification

```bash
# Obtenir un token
curl -X POST http://localhost:8000/api/v1/auth/login \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=admin@example.com&password=password123"

# Utiliser le token
curl http://localhost:8000/api/v1/users \
  -H "Authorization: Bearer <token>"
```

## 🧪 Tests

```bash
# Exécuter tous les tests
pytest

# Avec couverture
pytest --cov=app --cov-report=html

# Tests unitaires uniquement
pytest tests/unit/

# Tests d'intégration
pytest tests/integration/

# Mode verbose
pytest -v
```

## 🐳 Déploiement

### Production avec Docker

```bash
# Build de l'image
docker build -t generic-app:latest .

# Exécution
docker run -d \
  -p 8000:8000 \
  -e DATABASE_URL=postgresql+asyncpg://user:pass@host:5432/db \
  -e REDIS_URL=redis://host:6379/0 \
  -e SECRET_KEY=your-production-secret \
  generic-app:latest
```

### Avec monitoring

```bash
# Démarrer avec Prometheus + Grafana
docker-compose --profile monitoring up -d
```

- Prometheus : http://localhost:9090
- Grafana : http://localhost:3000 (admin/admin)

## 🔒 Sécurité

- ✅ Authentification JWT avec expiration
- ✅ Hashage bcrypt des mots de passe
- ✅ Validation des entrées avec Pydantic
- ✅ Protection CORS configurable
- ✅ Utilisateur non-root dans Docker
- ✅ Health checks pour orchestration

## 📝 Licence

MIT License - Voir [LICENSE](LICENSE)
