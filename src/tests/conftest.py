"""
Configuration des fixtures pytest.
"""

import asyncio
from typing import AsyncGenerator, Generator
from uuid import uuid4

import pytest
from httpx import AsyncClient, ASGITransport
from sqlalchemy.ext.asyncio import AsyncSession, create_async_engine, async_sessionmaker

from app.main import app
from app.infrastructure.database import Base, get_db_session
from app.infrastructure.cache import CacheClient
from app.domain.entities.user import User
from app.core.security import hash_password, create_access_token


# Base de données de test en mémoire
TEST_DATABASE_URL = "sqlite+aiosqlite:///:memory:"


@pytest.fixture(scope="session")
def event_loop() -> Generator[asyncio.AbstractEventLoop, None, None]:
    """Crée une event loop pour toute la session de test."""
    loop = asyncio.get_event_loop_policy().new_event_loop()
    yield loop
    loop.close()


@pytest.fixture(scope="function")
async def test_db() -> AsyncGenerator[AsyncSession, None]:
    """Crée une base de données de test."""
    engine = create_async_engine(TEST_DATABASE_URL, echo=False)
    
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.create_all)
    
    session_factory = async_sessionmaker(
        bind=engine,
        class_=AsyncSession,
        expire_on_commit=False,
    )
    
    async with session_factory() as session:
        yield session
    
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.drop_all)
    
    await engine.dispose()


@pytest.fixture
async def client() -> AsyncGenerator[AsyncClient, None]:
    """Client HTTP asynchrone pour les tests d'API."""
    transport = ASGITransport(app=app)
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        yield client


@pytest.fixture
def test_user() -> User:
    """Utilisateur de test."""
    return User(
        id=uuid4(),
        email="test@example.com",
        name="Test User",
        hashed_password=hash_password("password123"),
        is_active=True,
    )


@pytest.fixture
def test_token(test_user: User) -> str:
    """Token JWT de test."""
    return create_access_token(subject=test_user.email)


@pytest.fixture
def auth_headers(test_token: str) -> dict:
    """Headers d'authentification pour les requêtes."""
    return {"Authorization": f"Bearer {test_token}"}


class MockCache:
    """Cache mock pour les tests."""
    
    def __init__(self) -> None:
        self._store: dict[str, str] = {}
    
    async def get(self, key: str) -> str | None:
        return self._store.get(key)
    
    async def set(self, key: str, value: str, ttl: int | None = None) -> None:
        self._store[key] = value
    
    async def delete(self, key: str) -> None:
        self._store.pop(key, None)
    
    async def exists(self, key: str) -> bool:
        return key in self._store
    
    async def ping(self) -> bool:
        return True


@pytest.fixture
def mock_cache() -> MockCache:
    """Cache mock pour les tests."""
    return MockCache()
