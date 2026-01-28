"""
API v1 Router - Point d'entrée de tous les endpoints v1.
"""

from fastapi import APIRouter

from app.presentation.api.v1.endpoints import users, items, auth


router = APIRouter()

router.include_router(auth.router, prefix="/auth", tags=["Authentication"])
router.include_router(users.router, prefix="/users", tags=["Users"])
router.include_router(items.router, prefix="/items", tags=["Items"])
