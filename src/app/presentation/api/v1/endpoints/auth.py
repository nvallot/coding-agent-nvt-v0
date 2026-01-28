"""
Endpoints d'authentification.
"""

from fastapi import APIRouter, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordRequestForm

from app.application.services.auth_service import AuthService
from app.presentation.schemas.auth import TokenResponse, LoginRequest
from app.presentation.dependencies import get_auth_service
from app.core.exceptions import AuthenticationError


router = APIRouter()


@router.post("/login", response_model=TokenResponse)
async def login(
    form_data: OAuth2PasswordRequestForm = Depends(),
    auth_service: AuthService = Depends(get_auth_service),
) -> TokenResponse:
    """
    Authentifie un utilisateur et retourne un token JWT.
    """
    try:
        token = await auth_service.authenticate(
            email=form_data.username,
            password=form_data.password,
        )
        return TokenResponse(
            access_token=token,
            token_type="bearer",
        )
    except AuthenticationError as e:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail=str(e),
            headers={"WWW-Authenticate": "Bearer"},
        )


@router.post("/token", response_model=TokenResponse)
async def get_token(
    request: LoginRequest,
    auth_service: AuthService = Depends(get_auth_service),
) -> TokenResponse:
    """
    Authentifie un utilisateur via JSON et retourne un token JWT.
    """
    try:
        token = await auth_service.authenticate(
            email=request.email,
            password=request.password,
        )
        return TokenResponse(
            access_token=token,
            token_type="bearer",
        )
    except AuthenticationError as e:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail=str(e),
        )
