"""
Schemas utilisateur.
"""

from datetime import datetime
from typing import List, Optional
from uuid import UUID

from pydantic import BaseModel, EmailStr, Field, ConfigDict


class UserBase(BaseModel):
    """Schema de base pour un utilisateur."""
    email: EmailStr
    name: str = Field(..., min_length=1, max_length=100)
    is_active: bool = True


class UserCreate(UserBase):
    """Schema pour la création d'un utilisateur."""
    password: str = Field(..., min_length=8)


class UserUpdate(BaseModel):
    """Schema pour la mise à jour d'un utilisateur."""
    email: Optional[EmailStr] = None
    name: Optional[str] = Field(None, min_length=1, max_length=100)
    is_active: Optional[bool] = None
    password: Optional[str] = Field(None, min_length=8)


class UserResponse(UserBase):
    """Schema de réponse pour un utilisateur."""
    model_config = ConfigDict(from_attributes=True)
    
    id: UUID
    created_at: datetime
    updated_at: datetime


class UserListResponse(BaseModel):
    """Schema de réponse pour une liste d'utilisateurs."""
    items: List[UserResponse]
    total: int
    skip: int
    limit: int
