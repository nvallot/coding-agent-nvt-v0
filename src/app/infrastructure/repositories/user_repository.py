"""
Repository pour les utilisateurs.
"""

from typing import Optional

from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession

from app.domain.entities.user import User
from app.infrastructure.repositories.base import BaseRepository
from app.infrastructure.models.user import UserModel


class UserRepository(BaseRepository[UserModel]):
    """Repository pour la gestion des utilisateurs en base de données."""
    
    def __init__(self, session: AsyncSession) -> None:
        super().__init__(session, UserModel)
    
    async def get_by_email(self, email: str) -> Optional[User]:
        """Récupère un utilisateur par son email."""
        result = await self._session.execute(
            select(self._model).where(self._model.email == email)
        )
        model = result.scalar_one_or_none()
        return self._to_entity(model) if model else None
    
    async def get_by_id(self, user_id) -> Optional[User]:
        """Récupère un utilisateur par son ID."""
        model = await super().get_by_id(user_id)
        return self._to_entity(model) if model else None
    
    async def create(self, user: User) -> User:
        """Crée un nouvel utilisateur."""
        model = self._to_model(user)
        self._session.add(model)
        await self._session.commit()
        await self._session.refresh(model)
        return self._to_entity(model)
    
    async def update(self, user: User) -> User:
        """Met à jour un utilisateur."""
        model = await self._session.get(self._model, user.id)
        if model:
            model.email = user.email
            model.name = user.name
            model.hashed_password = user.hashed_password
            model.is_active = user.is_active
            await self._session.commit()
            await self._session.refresh(model)
        return self._to_entity(model)
    
    async def delete(self, user: User) -> None:
        """Supprime un utilisateur."""
        model = await self._session.get(self._model, user.id)
        if model:
            await self._session.delete(model)
            await self._session.commit()
    
    def _to_entity(self, model: UserModel) -> User:
        """Convertit un modèle SQLAlchemy en entité de domaine."""
        return User(
            id=model.id,
            email=model.email,
            name=model.name,
            hashed_password=model.hashed_password,
            is_active=model.is_active,
            created_at=model.created_at,
            updated_at=model.updated_at,
        )
    
    def _to_model(self, entity: User) -> UserModel:
        """Convertit une entité de domaine en modèle SQLAlchemy."""
        return UserModel(
            id=entity.id,
            email=entity.email,
            name=entity.name,
            hashed_password=entity.hashed_password,
            is_active=entity.is_active,
            created_at=entity.created_at,
            updated_at=entity.updated_at,
        )
