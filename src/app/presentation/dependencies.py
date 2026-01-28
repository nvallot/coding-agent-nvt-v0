"""
Dépendances FastAPI pour l'injection de dépendances.
"""

from typing import Annotated

from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer

from app.application.services.auth_service import AuthService
from app.application.services.user_service import UserService
from app.application.services.item_service import ItemService
from app.infrastructure.repositories.user_repository import UserRepository
from app.infrastructure.repositories.item_repository import ItemRepository
from app.infrastructure.database import get_db_session
from app.infrastructure.cache import get_cache
from app.core.security import decode_access_token
from app.core.exceptions import AuthenticationError
from app.domain.entities.user import User


oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/v1/auth/login")


# Repository dependencies
async def get_user_repository() -> UserRepository:
    """Retourne une instance de UserRepository."""
    session = await get_db_session()
    return UserRepository(session)


async def get_item_repository() -> ItemRepository:
    """Retourne une instance de ItemRepository."""
    session = await get_db_session()
    return ItemRepository(session)


# Service dependencies
async def get_auth_service(
    user_repo: UserRepository = Depends(get_user_repository),
) -> AuthService:
    """Retourne une instance de AuthService."""
    return AuthService(user_repo)


async def get_user_service(
    user_repo: UserRepository = Depends(get_user_repository),
    cache = Depends(get_cache),
) -> UserService:
    """Retourne une instance de UserService."""
    return UserService(user_repo, cache)


async def get_item_service(
    item_repo: ItemRepository = Depends(get_item_repository),
    cache = Depends(get_cache),
) -> ItemService:
    """Retourne une instance de ItemService."""
    return ItemService(item_repo, cache)


# Auth dependencies
async def get_current_user(
    token: Annotated[str, Depends(oauth2_scheme)],
    user_service: UserService = Depends(get_user_service),
) -> User:
    """Récupère l'utilisateur courant à partir du token JWT."""
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Token invalide",
        headers={"WWW-Authenticate": "Bearer"},
    )
    
    token_data = decode_access_token(token)
    if token_data is None:
        raise credentials_exception
    
    try:
        user = await user_service.get_by_email(token_data.sub)
        if not user.is_active:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail="Utilisateur désactivé",
            )
        return user
    except Exception:
        raise credentials_exception
