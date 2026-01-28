"""
Configuration et gestion de la base de données.
"""

from typing import AsyncGenerator, Optional

from sqlalchemy.ext.asyncio import (
    AsyncSession,
    async_sessionmaker,
    create_async_engine,
)
from sqlalchemy.orm import DeclarativeBase
from sqlalchemy import text

from app.core.config import settings
from app.core.logging import get_logger


logger = get_logger(__name__)


class Base(DeclarativeBase):
    """Classe de base pour les modèles SQLAlchemy."""
    pass


# Engine global
_engine = None
_session_factory: Optional[async_sessionmaker[AsyncSession]] = None


async def init_db() -> None:
    """Initialise la connexion à la base de données."""
    global _engine, _session_factory
    
    logger.info("initializing_database", url=settings.DATABASE_URL.split("@")[-1])
    
    _engine = create_async_engine(
        settings.DATABASE_URL,
        pool_size=settings.DATABASE_POOL_SIZE,
        max_overflow=settings.DATABASE_MAX_OVERFLOW,
        echo=settings.DEBUG,
    )
    
    _session_factory = async_sessionmaker(
        bind=_engine,
        class_=AsyncSession,
        expire_on_commit=False,
    )
    
    logger.info("database_initialized")


async def close_db() -> None:
    """Ferme la connexion à la base de données."""
    global _engine
    
    if _engine:
        await _engine.dispose()
        logger.info("database_connection_closed")


async def get_db_session() -> AsyncSession:
    """Retourne une session de base de données."""
    if _session_factory is None:
        raise RuntimeError("Database not initialized. Call init_db() first.")
    
    return _session_factory()


async def get_db() -> AsyncGenerator[AsyncSession, None]:
    """Générateur de session pour injection de dépendances."""
    session = await get_db_session()
    try:
        yield session
        await session.commit()
    except Exception:
        await session.rollback()
        raise
    finally:
        await session.close()


async def check_db_health() -> bool:
    """Vérifie la santé de la connexion à la base de données."""
    try:
        session = await get_db_session()
        await session.execute(text("SELECT 1"))
        await session.close()
        return True
    except Exception as e:
        logger.error("database_health_check_failed", error=str(e))
        return False
