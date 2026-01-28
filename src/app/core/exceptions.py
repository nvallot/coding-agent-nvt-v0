"""
Exceptions personnalisées de l'application.
"""

from typing import Any, Optional


class AppException(Exception):
    """Exception de base de l'application."""
    
    def __init__(
        self,
        message: str,
        code: str = "APP_ERROR",
        details: Optional[dict[str, Any]] = None,
    ) -> None:
        self.message = message
        self.code = code
        self.details = details or {}
        super().__init__(message)


class NotFoundError(AppException):
    """Ressource non trouvée."""
    
    def __init__(self, resource: str, identifier: Any) -> None:
        super().__init__(
            message=f"{resource} non trouvé(e): {identifier}",
            code="NOT_FOUND",
            details={"resource": resource, "identifier": str(identifier)},
        )


class ValidationError(AppException):
    """Erreur de validation."""
    
    def __init__(self, message: str, field: Optional[str] = None) -> None:
        super().__init__(
            message=message,
            code="VALIDATION_ERROR",
            details={"field": field} if field else {},
        )


class AuthenticationError(AppException):
    """Erreur d'authentification."""
    
    def __init__(self, message: str = "Authentification requise") -> None:
        super().__init__(message=message, code="AUTHENTICATION_ERROR")


class AuthorizationError(AppException):
    """Erreur d'autorisation."""
    
    def __init__(self, message: str = "Action non autorisée") -> None:
        super().__init__(message=message, code="AUTHORIZATION_ERROR")


class ConflictError(AppException):
    """Conflit de données."""
    
    def __init__(self, message: str) -> None:
        super().__init__(message=message, code="CONFLICT")


class ExternalServiceError(AppException):
    """Erreur de service externe."""
    
    def __init__(self, service: str, message: str) -> None:
        super().__init__(
            message=f"Erreur du service {service}: {message}",
            code="EXTERNAL_SERVICE_ERROR",
            details={"service": service},
        )
