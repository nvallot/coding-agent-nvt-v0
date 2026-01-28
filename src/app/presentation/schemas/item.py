"""
Schemas pour les items.
"""

from datetime import datetime
from typing import List, Optional
from uuid import UUID

from pydantic import BaseModel, Field, ConfigDict


class ItemBase(BaseModel):
    """Schema de base pour un item."""
    name: str = Field(..., min_length=1, max_length=200)
    description: Optional[str] = Field(None, max_length=2000)
    price: float = Field(..., ge=0)
    is_available: bool = True


class ItemCreate(ItemBase):
    """Schema pour la création d'un item."""
    pass


class ItemUpdate(BaseModel):
    """Schema pour la mise à jour d'un item."""
    name: Optional[str] = Field(None, min_length=1, max_length=200)
    description: Optional[str] = Field(None, max_length=2000)
    price: Optional[float] = Field(None, ge=0)
    is_available: Optional[bool] = None


class ItemResponse(ItemBase):
    """Schema de réponse pour un item."""
    model_config = ConfigDict(from_attributes=True)
    
    id: UUID
    owner_id: UUID
    created_at: datetime
    updated_at: datetime


class ItemListResponse(BaseModel):
    """Schema de réponse pour une liste d'items."""
    items: List[ItemResponse]
    total: int
    skip: int
    limit: int
