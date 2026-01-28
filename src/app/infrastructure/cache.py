"""
Configuration et gestion du cache Redis.
"""

from typing import Optional

import redis.asyncio as redis

from app.core.config import settings
from app.core.logging import get_logger


logger = get_logger(__name__)


class CacheClient:
    """Client de cache Redis."""
    
    def __init__(self, client: redis.Redis) -> None:
        self._client = client
    
    async def get(self, key: str) -> Optional[str]:
        """Récupère une valeur du cache."""
        value = await self._client.get(key)
        return value.decode("utf-8") if value else None
    
    async def set(
        self,
        key: str,
        value: str,
        ttl: Optional[int] = None,
    ) -> None:
        """Stocke une valeur dans le cache."""
        ttl = ttl or settings.CACHE_TTL_SECONDS
        await self._client.setex(key, ttl, value)
    
    async def delete(self, key: str) -> None:
        """Supprime une valeur du cache."""
        await self._client.delete(key)
    
    async def exists(self, key: str) -> bool:
        """Vérifie si une clé existe."""
        return await self._client.exists(key) > 0
    
    async def ping(self) -> bool:
        """Vérifie la connexion au cache."""
        try:
            await self._client.ping()
            return True
        except Exception:
            return False


# Cache global
_cache_client: Optional[CacheClient] = None
_redis_pool: Optional[redis.Redis] = None


async def init_cache() -> None:
    """Initialise la connexion au cache Redis."""
    global _cache_client, _redis_pool
    
    logger.info("initializing_cache", url=settings.REDIS_URL.split("@")[-1])
    
    try:
        _redis_pool = redis.from_url(
            settings.REDIS_URL,
            encoding="utf-8",
            decode_responses=False,
        )
        _cache_client = CacheClient(_redis_pool)
        
        # Test connection
        await _redis_pool.ping()
        logger.info("cache_initialized")
    except Exception as e:
        logger.warning("cache_initialization_failed", error=str(e))
        _cache_client = None


async def close_cache() -> None:
    """Ferme la connexion au cache."""
    global _redis_pool
    
    if _redis_pool:
        await _redis_pool.close()
        logger.info("cache_connection_closed")


async def get_cache() -> Optional[CacheClient]:
    """Retourne le client de cache."""
    return _cache_client


async def check_cache_health() -> bool:
    """Vérifie la santé de la connexion au cache."""
    if _cache_client is None:
        return False
    
    try:
        return await _cache_client.ping()
    except Exception as e:
        logger.error("cache_health_check_failed", error=str(e))
        return False
