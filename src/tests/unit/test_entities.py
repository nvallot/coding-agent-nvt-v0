"""
Tests unitaires pour les entités du domaine.
"""

from uuid import uuid4

import pytest

from app.domain.entities.user import User
from app.domain.entities.item import Item


class TestUserEntity:
    """Tests pour l'entité User."""
    
    def test_create_user(self) -> None:
        """Test de création d'un utilisateur."""
        user = User(
            email="test@example.com",
            name="Test User",
            hashed_password="hashed_password",
        )
        
        assert user.email == "test@example.com"
        assert user.name == "Test User"
        assert user.is_active is True
        assert user.id is not None
        assert user.created_at is not None
        assert user.updated_at is not None
    
    def test_user_str(self) -> None:
        """Test de la représentation string."""
        user = User(
            email="test@example.com",
            name="Test",
            hashed_password="hash",
        )
        
        assert str(user) == "User(test@example.com)"
    
    def test_user_touch(self) -> None:
        """Test de la mise à jour du timestamp."""
        user = User(
            email="test@example.com",
            name="Test",
            hashed_password="hash",
        )
        
        original_updated_at = user.updated_at
        user.touch()
        
        assert user.updated_at >= original_updated_at


class TestItemEntity:
    """Tests pour l'entité Item."""
    
    def test_create_item(self) -> None:
        """Test de création d'un item."""
        owner_id = uuid4()
        item = Item(
            name="Test Item",
            description="Description",
            price=99.99,
            owner_id=owner_id,
        )
        
        assert item.name == "Test Item"
        assert item.description == "Description"
        assert item.price == 99.99
        assert item.is_available is True
        assert item.owner_id == owner_id
    
    def test_item_without_description(self) -> None:
        """Test de création d'un item sans description."""
        item = Item(
            name="Simple Item",
            price=10.0,
            owner_id=uuid4(),
        )
        
        assert item.description is None
    
    def test_item_str(self) -> None:
        """Test de la représentation string."""
        item = Item(
            name="My Item",
            price=50.0,
            owner_id=uuid4(),
        )
        
        assert str(item) == "Item(My Item)"
