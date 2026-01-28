"""
Entité Item.
"""

from typing import Optional
from uuid import UUID

from pydantic import Field

from app.domain.entities.base import Entity


class Item(Entity):
    """Représente un item (ressource métier exemple)."""
    
    name: str = Field(..., min_length=1, max_length=200)
    description: Optional[str] = Field(None, max_length=2000)
    price: float = Field(..., ge=0)
    is_available: bool = True
    owner_id: UUID
    
    def __str__(self) -> str:
        return f"Item({self.name})"
    
    def __repr__(self) -> str:
        return f"Item(id={self.id}, name={self.name}, price={self.price})"
