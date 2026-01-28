"""
Entité User.
"""

from pydantic import EmailStr, Field

from app.domain.entities.base import Entity


class User(Entity):
    """Représente un utilisateur du système."""
    
    email: EmailStr
    name: str = Field(..., min_length=1, max_length=100)
    hashed_password: str
    is_active: bool = True
    
    def __str__(self) -> str:
        return f"User({self.email})"
    
    def __repr__(self) -> str:
        return f"User(id={self.id}, email={self.email}, is_active={self.is_active})"
