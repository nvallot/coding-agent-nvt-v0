"""
Tests unitaires pour le module de sécurité.
"""

import pytest
from datetime import timedelta

from app.core.security import (
    verify_password,
    hash_password,
    create_access_token,
    decode_access_token,
)


class TestPasswordHashing:
    """Tests pour le hashage des mots de passe."""
    
    def test_hash_password(self) -> None:
        """Test du hashage d'un mot de passe."""
        password = "secure_password_123"
        hashed = hash_password(password)
        
        assert hashed != password
        assert hashed.startswith("$2b$")  # bcrypt prefix
    
    def test_verify_password_correct(self) -> None:
        """Test de vérification avec mot de passe correct."""
        password = "my_password"
        hashed = hash_password(password)
        
        assert verify_password(password, hashed) is True
    
    def test_verify_password_incorrect(self) -> None:
        """Test de vérification avec mot de passe incorrect."""
        hashed = hash_password("correct_password")
        
        assert verify_password("wrong_password", hashed) is False
    
    def test_different_passwords_different_hashes(self) -> None:
        """Test que deux mots de passe identiques ont des hashes différents (salt)."""
        password = "same_password"
        hash1 = hash_password(password)
        hash2 = hash_password(password)
        
        assert hash1 != hash2  # Different salts
        assert verify_password(password, hash1) is True
        assert verify_password(password, hash2) is True


class TestJWT:
    """Tests pour les tokens JWT."""
    
    def test_create_access_token(self) -> None:
        """Test de création d'un token."""
        token = create_access_token(subject="user@example.com")
        
        assert token is not None
        assert isinstance(token, str)
        assert len(token) > 0
    
    def test_decode_access_token(self) -> None:
        """Test de décodage d'un token valide."""
        subject = "user@example.com"
        token = create_access_token(subject=subject)
        
        decoded = decode_access_token(token)
        
        assert decoded is not None
        assert decoded.sub == subject
    
    def test_decode_invalid_token(self) -> None:
        """Test de décodage d'un token invalide."""
        decoded = decode_access_token("invalid.token.here")
        
        assert decoded is None
    
    def test_create_token_with_custom_expiry(self) -> None:
        """Test de création avec expiration personnalisée."""
        token = create_access_token(
            subject="user@example.com",
            expires_delta=timedelta(hours=1),
        )
        
        decoded = decode_access_token(token)
        
        assert decoded is not None
