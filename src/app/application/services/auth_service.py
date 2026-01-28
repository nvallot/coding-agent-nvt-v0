"""
Service d'authentification.
"""

from app.core.security import verify_password, create_access_token
from app.core.exceptions import AuthenticationError
from app.infrastructure.repositories.user_repository import UserRepository
from app.core.logging import get_logger


logger = get_logger(__name__)


class AuthService:
    """Service gérant l'authentification des utilisateurs."""
    
    def __init__(self, user_repository: UserRepository) -> None:
        self._user_repo = user_repository
    
    async def authenticate(self, email: str, password: str) -> str:
        """
        Authentifie un utilisateur et retourne un token JWT.
        
        Args:
            email: Email de l'utilisateur
            password: Mot de passe en clair
            
        Returns:
            Token JWT
            
        Raises:
            AuthenticationError: Si les credentials sont invalides
        """
        logger.info("authentication_attempt", email=email)
        
        user = await self._user_repo.get_by_email(email)
        if user is None:
            logger.warning("authentication_failed", reason="user_not_found", email=email)
            raise AuthenticationError("Email ou mot de passe incorrect")
        
        if not verify_password(password, user.hashed_password):
            logger.warning("authentication_failed", reason="invalid_password", email=email)
            raise AuthenticationError("Email ou mot de passe incorrect")
        
        if not user.is_active:
            logger.warning("authentication_failed", reason="user_inactive", email=email)
            raise AuthenticationError("Compte désactivé")
        
        token = create_access_token(subject=user.email)
        logger.info("authentication_success", email=email, user_id=str(user.id))
        
        return token
