---
applyTo: "src/backend/**"
---

# Instructions Backend (C# / Python)

Quand tu travailles dans le dossier backend:

- Architecture en couches (API → Service → Repository)
- Dependency Injection
- Validation des entrées
- Gestion d'erreurs centralisée
- Logging structuré

## Intended Agents

- `@dev` (Developer) - Primary user
- `@reviewer` (Code Reviewer) - For review purposes

---

## C# (.NET 8+) - Structure de Service

```csharp
// Structure de service ASP.NET Core
public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        // Validation
        ArgumentNullException.ThrowIfNull(dto);

        // Business logic
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        // Persistence
        var savedUser = await _repository.AddAsync(user, ct);

        // Logging structuré
        _logger.LogInformation("User created: {UserId}, {Email}", savedUser.Id, savedUser.Email);

        return savedUser.ToDto();
    }
}
```

## Python (3.11+) - Structure de Service

```python
# Structure de service FastAPI / Python
from abc import ABC, abstractmethod
from dataclasses import dataclass
from datetime import datetime
from uuid import UUID, uuid4
import logging

logger = logging.getLogger(__name__)

@dataclass
class CreateUserDto:
    name: str
    email: str

@dataclass
class UserDto:
    id: UUID
    name: str
    email: str
    created_at: datetime

class IUserRepository(ABC):
    @abstractmethod
    async def add(self, user: "User") -> "User":
        pass

class UserService:
    def __init__(self, repository: IUserRepository):
        self._repository = repository

    async def create_user(self, dto: CreateUserDto) -> UserDto:
        # Validation
        if not dto.name or not dto.email:
            raise ValueError("Name and email are required")

        # Business logic
        user = User(
            id=uuid4(),
            name=dto.name,
            email=dto.email,
            created_at=datetime.utcnow()
        )

        # Persistence
        saved_user = await self._repository.add(user)

        # Logging structuré
        logger.info("User created", extra={"user_id": str(saved_user.id), "email": saved_user.email})

        return saved_user.to_dto()
```

## Bonnes Pratiques

### C#
- Utiliser `async/await` pour les opérations I/O
- Utiliser `CancellationToken` pour les opérations annulables
- Utiliser `ILogger<T>` pour le logging structuré
- Implémenter `IDisposable` si nécessaire
- Utiliser les nullable reference types (`#nullable enable`)

### Python
- Utiliser les type hints (PEP 484)
- Utiliser `dataclasses` ou `Pydantic` pour les DTOs
- Utiliser `logging` avec des champs structurés (`extra={}`)
- Utiliser les context managers (`async with`)
- Gérer les exceptions explicitement (pas de `except:` nu)
