"""
Schemas d'authentification.
"""

from pydantic import BaseModel, EmailStr


class LoginRequest(BaseModel):
    """Requête de connexion."""
    email: EmailStr
    password: str


class TokenResponse(BaseModel):
    """Réponse contenant le token JWT."""
    access_token: str
    token_type: str = "bearer"
