"""
Service de gestion des items.
"""

from typing import List, Optional
from uuid import UUID

from app.domain.entities.item import Item
from app.infrastructure.repositories.item_repository import ItemRepository
from app.infrastructure.cache import CacheClient
from app.core.exceptions import NotFoundError
from app.core.logging import get_logger
from app.presentation.schemas.item import ItemCreate, ItemUpdate


logger = get_logger(__name__)


class ItemService:
    """Service gérant la logique métier des items."""
    
    CACHE_PREFIX = "item:"
    CACHE_TTL = 300  # 5 minutes
    
    def __init__(
        self,
        item_repository: ItemRepository,
        cache: Optional[CacheClient] = None,
    ) -> None:
        self._item_repo = item_repository
        self._cache = cache
    
    async def get_by_id(self, item_id: UUID) -> Item:
        """
        Récupère un item par son ID.
        
        Args:
            item_id: ID de l'item
            
        Returns:
            Item trouvé
            
        Raises:
            NotFoundError: Si l'item n'existe pas
        """
        # Check cache first
        if self._cache:
            cached = await self._cache.get(f"{self.CACHE_PREFIX}{item_id}")
            if cached:
                return Item.model_validate_json(cached)
        
        item = await self._item_repo.get_by_id(item_id)
        if item is None:
            raise NotFoundError("Item", item_id)
        
        # Cache the result
        if self._cache:
            await self._cache.set(
                f"{self.CACHE_PREFIX}{item_id}",
                item.model_dump_json(),
                ttl=self.CACHE_TTL,
            )
        
        return item
    
    async def get_all(
        self,
        skip: int = 0,
        limit: int = 100,
        search: Optional[str] = None,
    ) -> List[Item]:
        """Liste tous les items avec pagination et recherche optionnelle."""
        return await self._item_repo.get_all(skip=skip, limit=limit, search=search)
    
    async def count(self, search: Optional[str] = None) -> int:
        """Compte le nombre total d'items."""
        return await self._item_repo.count(search=search)
    
    async def create(self, data: ItemCreate, owner_id: UUID) -> Item:
        """
        Crée un nouvel item.
        
        Args:
            data: Données de création
            owner_id: ID du propriétaire
            
        Returns:
            Item créé
        """
        item = Item(
            name=data.name,
            description=data.description,
            price=data.price,
            is_available=data.is_available,
            owner_id=owner_id,
        )
        
        created = await self._item_repo.create(item)
        logger.info("item_created", item_id=str(created.id), owner_id=str(owner_id))
        
        return created
    
    async def update(self, item_id: UUID, data: ItemUpdate) -> Item:
        """
        Met à jour un item.
        
        Args:
            item_id: ID de l'item
            data: Données de mise à jour
            
        Returns:
            Item mis à jour
            
        Raises:
            NotFoundError: Si l'item n'existe pas
        """
        item = await self.get_by_id(item_id)
        
        update_data = data.model_dump(exclude_unset=True)
        for field, value in update_data.items():
            setattr(item, field, value)
        
        updated = await self._item_repo.update(item)
        
        # Invalidate cache
        if self._cache:
            await self._cache.delete(f"{self.CACHE_PREFIX}{item_id}")
        
        logger.info("item_updated", item_id=str(item_id))
        
        return updated
    
    async def delete(self, item_id: UUID) -> None:
        """
        Supprime un item.
        
        Args:
            item_id: ID de l'item
            
        Raises:
            NotFoundError: Si l'item n'existe pas
        """
        item = await self.get_by_id(item_id)
        await self._item_repo.delete(item)
        
        # Invalidate cache
        if self._cache:
            await self._cache.delete(f"{self.CACHE_PREFIX}{item_id}")
        
        logger.info("item_deleted", item_id=str(item_id))
