"""
Entité de base.
"""

from datetime import datetime, timezone
from uuid import UUID, uuid4

from pydantic import BaseModel, Field, ConfigDict


class Entity(BaseModel):
    """Classe de base pour toutes les entités du domaine."""
    
    model_config = ConfigDict(from_attributes=True)
    
    id: UUID = Field(default_factory=uuid4)
    created_at: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))
    updated_at: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))
    
    def touch(self) -> None:
        """Met à jour le timestamp de modification."""
        self.updated_at = datetime.now(timezone.utc)
