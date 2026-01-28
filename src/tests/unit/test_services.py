"""
Tests unitaires pour les services métier.
"""

from unittest.mock import AsyncMock, MagicMock
from uuid import uuid4

import pytest

from app.application.services.auth_service import AuthService
from app.application.services.user_service import UserService
from app.application.services.item_service import ItemService
from app.domain.entities.user import User
from app.domain.entities.item import Item
from app.core.security import hash_password
from app.core.exceptions import AuthenticationError, NotFoundError, ConflictError
from app.presentation.schemas.user import UserCreate, UserUpdate
from app.presentation.schemas.item import ItemCreate, ItemUpdate


class TestAuthService:
    """Tests pour le service d'authentification."""
    
    @pytest.fixture
    def mock_user_repo(self) -> AsyncMock:
        return AsyncMock()
    
    @pytest.fixture
    def auth_service(self, mock_user_repo: AsyncMock) -> AuthService:
        return AuthService(mock_user_repo)
    
    @pytest.mark.asyncio
    async def test_authenticate_success(
        self,
        auth_service: AuthService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test d'authentification réussie."""
        user = User(
            email="test@example.com",
            name="Test",
            hashed_password=hash_password("password123"),
            is_active=True,
        )
        mock_user_repo.get_by_email.return_value = user
        
        token = await auth_service.authenticate("test@example.com", "password123")
        
        assert token is not None
        assert isinstance(token, str)
    
    @pytest.mark.asyncio
    async def test_authenticate_user_not_found(
        self,
        auth_service: AuthService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test d'authentification avec utilisateur inexistant."""
        mock_user_repo.get_by_email.return_value = None
        
        with pytest.raises(AuthenticationError):
            await auth_service.authenticate("unknown@example.com", "password")
    
    @pytest.mark.asyncio
    async def test_authenticate_wrong_password(
        self,
        auth_service: AuthService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test d'authentification avec mauvais mot de passe."""
        user = User(
            email="test@example.com",
            name="Test",
            hashed_password=hash_password("correct_password"),
            is_active=True,
        )
        mock_user_repo.get_by_email.return_value = user
        
        with pytest.raises(AuthenticationError):
            await auth_service.authenticate("test@example.com", "wrong_password")
    
    @pytest.mark.asyncio
    async def test_authenticate_inactive_user(
        self,
        auth_service: AuthService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test d'authentification avec utilisateur inactif."""
        user = User(
            email="test@example.com",
            name="Test",
            hashed_password=hash_password("password123"),
            is_active=False,
        )
        mock_user_repo.get_by_email.return_value = user
        
        with pytest.raises(AuthenticationError):
            await auth_service.authenticate("test@example.com", "password123")


class TestUserService:
    """Tests pour le service utilisateur."""
    
    @pytest.fixture
    def mock_user_repo(self) -> AsyncMock:
        return AsyncMock()
    
    @pytest.fixture
    def mock_cache(self) -> AsyncMock:
        cache = AsyncMock()
        cache.get.return_value = None
        return cache
    
    @pytest.fixture
    def user_service(
        self,
        mock_user_repo: AsyncMock,
        mock_cache: AsyncMock,
    ) -> UserService:
        return UserService(mock_user_repo, mock_cache)
    
    @pytest.mark.asyncio
    async def test_get_by_id_success(
        self,
        user_service: UserService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test de récupération d'un utilisateur par ID."""
        user_id = uuid4()
        expected_user = User(
            id=user_id,
            email="test@example.com",
            name="Test",
            hashed_password="hash",
        )
        mock_user_repo.get_by_id.return_value = expected_user
        
        result = await user_service.get_by_id(user_id)
        
        assert result.id == user_id
        assert result.email == "test@example.com"
    
    @pytest.mark.asyncio
    async def test_get_by_id_not_found(
        self,
        user_service: UserService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test de récupération d'un utilisateur inexistant."""
        mock_user_repo.get_by_id.return_value = None
        
        with pytest.raises(NotFoundError):
            await user_service.get_by_id(uuid4())
    
    @pytest.mark.asyncio
    async def test_create_user_success(
        self,
        user_service: UserService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test de création d'un utilisateur."""
        mock_user_repo.get_by_email.return_value = None
        mock_user_repo.create.return_value = User(
            email="new@example.com",
            name="New User",
            hashed_password="hash",
        )
        
        user_data = UserCreate(
            email="new@example.com",
            name="New User",
            password="password123",
        )
        
        result = await user_service.create(user_data)
        
        assert result.email == "new@example.com"
        mock_user_repo.create.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_create_user_email_exists(
        self,
        user_service: UserService,
        mock_user_repo: AsyncMock,
    ) -> None:
        """Test de création avec email existant."""
        mock_user_repo.get_by_email.return_value = User(
            email="existing@example.com",
            name="Existing",
            hashed_password="hash",
        )
        
        user_data = UserCreate(
            email="existing@example.com",
            name="New User",
            password="password123",
        )
        
        with pytest.raises(ConflictError):
            await user_service.create(user_data)


class TestItemService:
    """Tests pour le service item."""
    
    @pytest.fixture
    def mock_item_repo(self) -> AsyncMock:
        return AsyncMock()
    
    @pytest.fixture
    def mock_cache(self) -> AsyncMock:
        cache = AsyncMock()
        cache.get.return_value = None
        return cache
    
    @pytest.fixture
    def item_service(
        self,
        mock_item_repo: AsyncMock,
        mock_cache: AsyncMock,
    ) -> ItemService:
        return ItemService(mock_item_repo, mock_cache)
    
    @pytest.mark.asyncio
    async def test_create_item(
        self,
        item_service: ItemService,
        mock_item_repo: AsyncMock,
    ) -> None:
        """Test de création d'un item."""
        owner_id = uuid4()
        mock_item_repo.create.return_value = Item(
            name="New Item",
            price=50.0,
            owner_id=owner_id,
        )
        
        item_data = ItemCreate(name="New Item", price=50.0)
        result = await item_service.create(item_data, owner_id=owner_id)
        
        assert result.name == "New Item"
        assert result.price == 50.0
        mock_item_repo.create.assert_called_once()
    
    @pytest.mark.asyncio
    async def test_get_item_not_found(
        self,
        item_service: ItemService,
        mock_item_repo: AsyncMock,
    ) -> None:
        """Test de récupération d'un item inexistant."""
        mock_item_repo.get_by_id.return_value = None
        
        with pytest.raises(NotFoundError):
            await item_service.get_by_id(uuid4())
