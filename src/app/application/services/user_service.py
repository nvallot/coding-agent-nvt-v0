"""
Service de gestion des utilisateurs.
"""

from typing import List, Optional
from uuid import UUID

from app.domain.entities.user import User
from app.infrastructure.repositories.user_repository import UserRepository
from app.infrastructure.cache import CacheClient
from app.core.security import hash_password
from app.core.exceptions import NotFoundError, ConflictError
from app.core.logging import get_logger
from app.presentation.schemas.user import UserCreate, UserUpdate


logger = get_logger(__name__)


class UserService:
    """Service gérant la logique métier des utilisateurs."""
    
    CACHE_PREFIX = "user:"
    CACHE_TTL = 300  # 5 minutes
    
    def __init__(
        self,
        user_repository: UserRepository,
        cache: Optional[CacheClient] = None,
    ) -> None:
        self._user_repo = user_repository
        self._cache = cache
    
    async def get_by_id(self, user_id: UUID) -> User:
        """
        Récupère un utilisateur par son ID.
        
        Args:
            user_id: ID de l'utilisateur
            
        Returns:
            Utilisateur trouvé
            
        Raises:
            NotFoundError: Si l'utilisateur n'existe pas
        """
        # Check cache first
        if self._cache:
            cached = await self._cache.get(f"{self.CACHE_PREFIX}{user_id}")
            if cached:
                return User.model_validate_json(cached)
        
        user = await self._user_repo.get_by_id(user_id)
        if user is None:
            raise NotFoundError("Utilisateur", user_id)
        
        # Cache the result
        if self._cache:
            await self._cache.set(
                f"{self.CACHE_PREFIX}{user_id}",
                user.model_dump_json(),
                ttl=self.CACHE_TTL,
            )
        
        return user
    
    async def get_by_email(self, email: str) -> User:
        """
        Récupère un utilisateur par son email.
        
        Args:
            email: Email de l'utilisateur
            
        Returns:
            Utilisateur trouvé
            
        Raises:
            NotFoundError: Si l'utilisateur n'existe pas
        """
        user = await self._user_repo.get_by_email(email)
        if user is None:
            raise NotFoundError("Utilisateur", email)
        return user
    
    async def get_all(self, skip: int = 0, limit: int = 100) -> List[User]:
        """Liste tous les utilisateurs avec pagination."""
        return await self._user_repo.get_all(skip=skip, limit=limit)
    
    async def count(self) -> int:
        """Compte le nombre total d'utilisateurs."""
        return await self._user_repo.count()
    
    async def create(self, data: UserCreate) -> User:
        """
        Crée un nouvel utilisateur.
        
        Args:
            data: Données de création
            
        Returns:
            Utilisateur créé
            
        Raises:
            ConflictError: Si l'email existe déjà
        """
        # Check if email already exists
        existing = await self._user_repo.get_by_email(data.email)
        if existing:
            raise ConflictError(f"L'email {data.email} est déjà utilisé")
        
        user = User(
            email=data.email,
            name=data.name,
            hashed_password=hash_password(data.password),
            is_active=data.is_active,
        )
        
        created = await self._user_repo.create(user)
        logger.info("user_created", user_id=str(created.id), email=created.email)
        
        return created
    
    async def update(self, user_id: UUID, data: UserUpdate) -> User:
        """
        Met à jour un utilisateur.
        
        Args:
            user_id: ID de l'utilisateur
            data: Données de mise à jour
            
        Returns:
            Utilisateur mis à jour
            
        Raises:
            NotFoundError: Si l'utilisateur n'existe pas
        """
        user = await self.get_by_id(user_id)
        
        update_data = data.model_dump(exclude_unset=True)
        
        # Hash password if provided
        if "password" in update_data:
            update_data["hashed_password"] = hash_password(update_data.pop("password"))
        
        for field, value in update_data.items():
            setattr(user, field, value)
        
        updated = await self._user_repo.update(user)
        
        # Invalidate cache
        if self._cache:
            await self._cache.delete(f"{self.CACHE_PREFIX}{user_id}")
        
        logger.info("user_updated", user_id=str(user_id))
        
        return updated
    
    async def delete(self, user_id: UUID) -> None:
        """
        Supprime un utilisateur.
        
        Args:
            user_id: ID de l'utilisateur
            
        Raises:
            NotFoundError: Si l'utilisateur n'existe pas
        """
        user = await self.get_by_id(user_id)
        await self._user_repo.delete(user)
        
        # Invalidate cache
        if self._cache:
            await self._cache.delete(f"{self.CACHE_PREFIX}{user_id}")
        
        logger.info("user_deleted", user_id=str(user_id))
