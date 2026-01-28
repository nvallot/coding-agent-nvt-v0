"""
Repository pour les items.
"""

from typing import List, Optional

from sqlalchemy import select, func, or_
from sqlalchemy.ext.asyncio import AsyncSession

from app.domain.entities.item import Item
from app.infrastructure.repositories.base import BaseRepository
from app.infrastructure.models.item import ItemModel


class ItemRepository(BaseRepository[ItemModel]):
    """Repository pour la gestion des items en base de données."""
    
    def __init__(self, session: AsyncSession) -> None:
        super().__init__(session, ItemModel)
    
    async def get_by_id(self, item_id) -> Optional[Item]:
        """Récupère un item par son ID."""
        model = await super().get_by_id(item_id)
        return self._to_entity(model) if model else None
    
    async def get_all(
        self,
        skip: int = 0,
        limit: int = 100,
        search: Optional[str] = None,
    ) -> List[Item]:
        """Récupère tous les items avec pagination et recherche."""
        query = select(self._model)
        
        if search:
            query = query.where(
                or_(
                    self._model.name.ilike(f"%{search}%"),
                    self._model.description.ilike(f"%{search}%"),
                )
            )
        
        query = query.offset(skip).limit(limit).order_by(self._model.created_at.desc())
        
        result = await self._session.execute(query)
        models = result.scalars().all()
        return [self._to_entity(m) for m in models]
    
    async def count(self, search: Optional[str] = None) -> int:
        """Compte le nombre total d'items."""
        query = select(func.count()).select_from(self._model)
        
        if search:
            query = query.where(
                or_(
                    self._model.name.ilike(f"%{search}%"),
                    self._model.description.ilike(f"%{search}%"),
                )
            )
        
        result = await self._session.execute(query)
        return result.scalar() or 0
    
    async def create(self, item: Item) -> Item:
        """Crée un nouvel item."""
        model = self._to_model(item)
        self._session.add(model)
        await self._session.commit()
        await self._session.refresh(model)
        return self._to_entity(model)
    
    async def update(self, item: Item) -> Item:
        """Met à jour un item."""
        model = await self._session.get(self._model, item.id)
        if model:
            model.name = item.name
            model.description = item.description
            model.price = item.price
            model.is_available = item.is_available
            await self._session.commit()
            await self._session.refresh(model)
        return self._to_entity(model)
    
    async def delete(self, item: Item) -> None:
        """Supprime un item."""
        model = await self._session.get(self._model, item.id)
        if model:
            await self._session.delete(model)
            await self._session.commit()
    
    def _to_entity(self, model: ItemModel) -> Item:
        """Convertit un modèle SQLAlchemy en entité de domaine."""
        return Item(
            id=model.id,
            name=model.name,
            description=model.description,
            price=model.price,
            is_available=model.is_available,
            owner_id=model.owner_id,
            created_at=model.created_at,
            updated_at=model.updated_at,
        )
    
    def _to_model(self, entity: Item) -> ItemModel:
        """Convertit une entité de domaine en modèle SQLAlchemy."""
        return ItemModel(
            id=entity.id,
            name=entity.name,
            description=entity.description,
            price=entity.price,
            is_available=entity.is_available,
            owner_id=entity.owner_id,
            created_at=entity.created_at,
            updated_at=entity.updated_at,
        )
