---
applyTo: "**/*.py"
---

# Instructions Python (3.11+)

## Conventions de Nommage (PEP 8)

| Élément | Convention | Exemple |
|---------|------------|---------|
| Modules, Packages | snake_case | `user_service.py`, `data_models` |
| Fonctions, Variables | snake_case | `get_user_by_id`, `user_name` |
| Classes | PascalCase | `UserService`, `OrderRepository` |
| Constantes | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT`, `DEFAULT_TIMEOUT` |
| Privé | Préfixe _ | `_internal_method`, `_cache` |
| Protected | Préfixe _ | `_protected_method` |

## Structure de Fichier

```python
"""
Module de service utilisateur.

Ce module fournit les opérations CRUD pour les utilisateurs.
"""
from __future__ import annotations

import logging
from dataclasses import dataclass
from datetime import datetime
from typing import Protocol
from uuid import UUID, uuid4

logger = logging.getLogger(__name__)


@dataclass(frozen=True)
class CreateUserDto:
    """Données pour créer un utilisateur."""
    name: str
    email: str


@dataclass(frozen=True)
class UserDto:
    """Représentation d'un utilisateur."""
    id: UUID
    name: str
    email: str
    created_at: datetime


class IUserRepository(Protocol):
    """Interface pour le repository utilisateur."""
    
    async def add(self, user: User) -> User:
        """Ajoute un utilisateur."""
        ...

    async def get_by_id(self, user_id: UUID) -> User | None:
        """Récupère un utilisateur par ID."""
        ...


class UserService:
    """Service de gestion des utilisateurs."""

    def __init__(self, repository: IUserRepository) -> None:
        """
        Initialise le service.

        Args:
            repository: Repository pour la persistance.
        """
        self._repository = repository

    async def create_user(self, dto: CreateUserDto) -> UserDto:
        """
        Crée un nouvel utilisateur.

        Args:
            dto: Données de création.

        Returns:
            L'utilisateur créé.

        Raises:
            ValueError: Si les données sont invalides.
        """
        if not dto.name or not dto.email:
            raise ValueError("Name and email are required")

        user = User(
            id=uuid4(),
            name=dto.name,
            email=dto.email,
            created_at=datetime.utcnow(),
        )

        saved_user = await self._repository.add(user)
        
        logger.info(
            "User created",
            extra={"user_id": str(saved_user.id), "email": saved_user.email},
        )

        return saved_user.to_dto()
```

## Bonnes Pratiques

### Type Hints (PEP 484)
```python
# ✅ Bon - Types explicites
def get_user_by_id(user_id: UUID) -> User | None:
    ...

# ✅ Bon - Collections typées
def get_users(filters: dict[str, str]) -> list[User]:
    ...

# ✅ Bon - Callables
def process(callback: Callable[[User], None]) -> None:
    ...
```

### Docstrings (Google Style)
```python
def calculate_total(items: list[Item], discount: float = 0.0) -> Decimal:
    """
    Calcule le total avec remise.

    Args:
        items: Liste des articles.
        discount: Pourcentage de remise (0.0 à 1.0).

    Returns:
        Le montant total après remise.

    Raises:
        ValueError: Si la remise est hors limites.

    Example:
        >>> calculate_total([Item(price=10)], discount=0.1)
        Decimal('9.00')
    """
```

### Gestion des Erreurs
```python
# ✅ Bon - Exceptions spécifiques
try:
    user = await repository.get_by_id(user_id)
except DatabaseConnectionError as e:
    logger.error("Database connection failed", exc_info=e)
    raise ServiceUnavailableError("Database unavailable") from e
except RecordNotFoundError:
    raise UserNotFoundError(f"User {user_id} not found")

# ❌ Mauvais - except nu
try:
    process()
except:  # Ne jamais faire ça
    pass
```

### Logging Structuré
```python
# ✅ Bon - Logging structuré avec extra
logger.info(
    "Order processed",
    extra={
        "order_id": str(order.id),
        "user_id": str(order.user_id),
        "amount": float(order.total),
    },
)

# ❌ Mauvais - f-string dans le message
logger.info(f"Order {order.id} processed")
```

### Context Managers
```python
# ✅ Bon - Context manager pour les ressources
async with aiohttp.ClientSession() as session:
    async with session.get(url) as response:
        data = await response.json()

# ✅ Bon - Pour les fichiers
async with aiofiles.open("data.json", "r") as f:
    content = await f.read()
```

## Patterns Recommandés

### Dataclasses pour les DTOs
```python
from dataclasses import dataclass, field
from datetime import datetime

@dataclass(frozen=True)
class OrderDto:
    id: UUID
    items: list[ItemDto] = field(default_factory=list)
    created_at: datetime = field(default_factory=datetime.utcnow)
```

### Pydantic pour la Validation
```python
from pydantic import BaseModel, EmailStr, Field, field_validator

class CreateUserRequest(BaseModel):
    name: str = Field(min_length=1, max_length=100)
    email: EmailStr
    age: int | None = Field(default=None, ge=0, le=150)

    @field_validator("name")
    @classmethod
    def validate_name(cls, v: str) -> str:
        return v.strip()
```

### Protocol pour les Interfaces
```python
from typing import Protocol

class IRepository(Protocol):
    async def get(self, id: UUID) -> Model | None: ...
    async def add(self, entity: Model) -> Model: ...
    async def delete(self, id: UUID) -> bool: ...
```

### Async/Await
```python
import asyncio

async def process_orders(orders: list[Order]) -> list[Result]:
    """Traite les commandes en parallèle."""
    tasks = [process_order(order) for order in orders]
    return await asyncio.gather(*tasks, return_exceptions=True)
```
