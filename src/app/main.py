"""
Point d'entrée de l'application FastAPI.
"""

from contextlib import asynccontextmanager
from typing import AsyncGenerator

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.core.config import settings
from app.core.logging import setup_logging
from app.infrastructure.database import init_db, close_db
from app.infrastructure.cache import init_cache, close_cache
from app.presentation.api.v1 import router as api_v1_router
from app.presentation.api.health import router as health_router


@asynccontextmanager
async def lifespan(app: FastAPI) -> AsyncGenerator[None, None]:
    """Gestion du cycle de vie de l'application."""
    # Startup
    setup_logging()
    await init_db()
    await init_cache()
    
    yield
    
    # Shutdown
    await close_cache()
    await close_db()


def create_app() -> FastAPI:
    """Factory pour créer l'application FastAPI."""
    app = FastAPI(
        title=settings.APP_NAME,
        description="API générique - Architecture 3-tiers",
        version="1.0.0",
        docs_url="/api/docs",
        redoc_url="/api/redoc",
        openapi_url="/api/openapi.json",
        lifespan=lifespan,
    )
    
    # CORS
    app.add_middleware(
        CORSMiddleware,
        allow_origins=settings.CORS_ORIGINS,
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )
    
    # Routes
    app.include_router(health_router, tags=["Health"])
    app.include_router(api_v1_router, prefix="/api/v1")
    
    return app


app = create_app()


if __name__ == "__main__":
    import uvicorn
    
    uvicorn.run(
        "app.main:app",
        host="0.0.0.0",
        port=settings.PORT,
        reload=settings.DEBUG,
    )
