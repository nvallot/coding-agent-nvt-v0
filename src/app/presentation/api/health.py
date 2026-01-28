"""
Endpoint de santé de l'application.
"""

from fastapi import APIRouter, Response

from app.infrastructure.database import check_db_health
from app.infrastructure.cache import check_cache_health


router = APIRouter()


@router.get("/health")
async def health_check() -> dict:
    """Vérifie l'état de santé de l'application."""
    return {"status": "healthy", "service": "generic-app"}


@router.get("/health/ready")
async def readiness_check() -> dict:
    """Vérifie que l'application est prête à recevoir du trafic."""
    checks = {
        "database": await check_db_health(),
        "cache": await check_cache_health(),
    }
    
    all_healthy = all(checks.values())
    
    return {
        "status": "ready" if all_healthy else "not_ready",
        "checks": checks,
    }


@router.get("/health/live")
async def liveness_check(response: Response) -> dict:
    """Vérifie que l'application est vivante."""
    # Simple check - si on peut répondre, on est vivant
    return {"status": "alive"}
