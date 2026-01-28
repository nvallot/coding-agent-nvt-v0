"""
Repository de base avec opérations CRUD génériques.
"""

from typing import Generic, List, Optional, Type, TypeVar
from uuid import UUID

from sqlalchemy import select, func
from sqlalchemy.ext.asyncio import AsyncSession

from app.infrastructure.database import Base


T = TypeVar("T", bound=Base)


class BaseRepository(Generic[T]):
    """Repository générique avec opérations CRUD de base."""
    
    def __init__(self, session: AsyncSession, model: Type[T]) -> None:
        self._session = session
        self._model = model
    
    async def get_by_id(self, entity_id: UUID) -> Optional[T]:
        """Récupère une entité par son ID."""
        result = await self._session.execute(
            select(self._model).where(self._model.id == entity_id)
        )
        return result.scalar_one_or_none()
    
    async def get_all(self, skip: int = 0, limit: int = 100) -> List[T]:
        """Récupère toutes les entités avec pagination."""
        result = await self._session.execute(
            select(self._model)
            .offset(skip)
            .limit(limit)
            .order_by(self._model.created_at.desc())
        )
        return list(result.scalars().all())
    
    async def count(self) -> int:
        """Compte le nombre total d'entités."""
        result = await self._session.execute(
            select(func.count()).select_from(self._model)
        )
        return result.scalar() or 0
    
    async def create(self, entity: T) -> T:
        """Crée une nouvelle entité."""
        self._session.add(entity)
        await self._session.commit()
        await self._session.refresh(entity)
        return entity
    
    async def update(self, entity: T) -> T:
        """Met à jour une entité existante."""
        await self._session.merge(entity)
        await self._session.commit()
        return entity
    
    async def delete(self, entity: T) -> None:
        """Supprime une entité."""
        await self._session.delete(entity)
        await self._session.commit()
