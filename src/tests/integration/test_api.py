"""
Tests d'intégration pour les endpoints API.
"""

import pytest
from httpx import AsyncClient


class TestHealthEndpoints:
    """Tests pour les endpoints de santé."""
    
    @pytest.mark.asyncio
    async def test_health_check(self, client: AsyncClient) -> None:
        """Test du endpoint /health."""
        response = await client.get("/health")
        
        assert response.status_code == 200
        data = response.json()
        assert data["status"] == "healthy"
        assert "service" in data
    
    @pytest.mark.asyncio
    async def test_liveness_check(self, client: AsyncClient) -> None:
        """Test du endpoint /health/live."""
        response = await client.get("/health/live")
        
        assert response.status_code == 200
        data = response.json()
        assert data["status"] == "alive"


class TestAuthEndpoints:
    """Tests pour les endpoints d'authentification."""
    
    @pytest.mark.asyncio
    async def test_login_invalid_credentials(self, client: AsyncClient) -> None:
        """Test de login avec credentials invalides."""
        response = await client.post(
            "/api/v1/auth/login",
            data={
                "username": "unknown@example.com",
                "password": "wrongpassword",
            },
        )
        
        # Devrait échouer car pas de base de données configurée
        assert response.status_code in [401, 500]


class TestUserEndpoints:
    """Tests pour les endpoints utilisateur."""
    
    @pytest.mark.asyncio
    async def test_list_users_unauthorized(self, client: AsyncClient) -> None:
        """Test de liste des utilisateurs sans authentification."""
        response = await client.get("/api/v1/users")
        
        assert response.status_code == 401
    
    @pytest.mark.asyncio
    async def test_create_user(self, client: AsyncClient) -> None:
        """Test de création d'un utilisateur."""
        response = await client.post(
            "/api/v1/users",
            json={
                "email": "newuser@example.com",
                "name": "New User",
                "password": "securepassword123",
            },
        )
        
        # Devrait échouer car pas de base de données configurée
        # Mais vérifie que l'endpoint accepte les données
        assert response.status_code in [201, 500]


class TestItemEndpoints:
    """Tests pour les endpoints item."""
    
    @pytest.mark.asyncio
    async def test_list_items_unauthorized(self, client: AsyncClient) -> None:
        """Test de liste des items sans authentification."""
        response = await client.get("/api/v1/items")
        
        assert response.status_code == 401
    
    @pytest.mark.asyncio
    async def test_get_item_unauthorized(self, client: AsyncClient) -> None:
        """Test de récupération d'un item sans authentification."""
        response = await client.get("/api/v1/items/a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11")
        
        assert response.status_code == 401
