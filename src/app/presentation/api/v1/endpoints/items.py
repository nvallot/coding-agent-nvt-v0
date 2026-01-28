"""
Endpoints de gestion des items (exemple de ressource métier).
"""

from typing import List, Optional
from uuid import UUID

from fastapi import APIRouter, Depends, HTTPException, status, Query

from app.application.services.item_service import ItemService
from app.presentation.schemas.item import (
    ItemCreate,
    ItemUpdate,
    ItemResponse,
    ItemListResponse,
)
from app.presentation.dependencies import get_item_service, get_current_user
from app.core.exceptions import NotFoundError
from app.domain.entities.user import User


router = APIRouter()


@router.get("", response_model=ItemListResponse)
async def list_items(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=1000),
    search: Optional[str] = Query(None, min_length=1),
    item_service: ItemService = Depends(get_item_service),
    current_user: User = Depends(get_current_user),
) -> ItemListResponse:
    """Liste tous les items avec pagination et recherche optionnelle."""
    items = await item_service.get_all(skip=skip, limit=limit, search=search)
    total = await item_service.count(search=search)
    
    return ItemListResponse(
        items=[ItemResponse.model_validate(i) for i in items],
        total=total,
        skip=skip,
        limit=limit,
    )


@router.get("/{item_id}", response_model=ItemResponse)
async def get_item(
    item_id: UUID,
    item_service: ItemService = Depends(get_item_service),
    current_user: User = Depends(get_current_user),
) -> ItemResponse:
    """Récupère un item par son ID."""
    try:
        item = await item_service.get_by_id(item_id)
        return ItemResponse.model_validate(item)
    except NotFoundError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))


@router.post("", response_model=ItemResponse, status_code=status.HTTP_201_CREATED)
async def create_item(
    item_data: ItemCreate,
    item_service: ItemService = Depends(get_item_service),
    current_user: User = Depends(get_current_user),
) -> ItemResponse:
    """Crée un nouvel item."""
    item = await item_service.create(item_data, owner_id=current_user.id)
    return ItemResponse.model_validate(item)


@router.put("/{item_id}", response_model=ItemResponse)
async def update_item(
    item_id: UUID,
    item_data: ItemUpdate,
    item_service: ItemService = Depends(get_item_service),
    current_user: User = Depends(get_current_user),
) -> ItemResponse:
    """Met à jour un item."""
    try:
        item = await item_service.update(item_id, item_data)
        return ItemResponse.model_validate(item)
    except NotFoundError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))


@router.delete("/{item_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_item(
    item_id: UUID,
    item_service: ItemService = Depends(get_item_service),
    current_user: User = Depends(get_current_user),
) -> None:
    """Supprime un item."""
    try:
        await item_service.delete(item_id)
    except NotFoundError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))
