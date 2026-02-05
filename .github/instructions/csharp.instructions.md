---
applyTo: "**/*.cs"
---

# Instructions C# (.NET 8+)

## Conventions de Nommage

| Élément | Convention | Exemple |
|---------|------------|---------|
| Classes, Interfaces, Enums | PascalCase | `UserService`, `IUserRepository` |
| Méthodes, Propriétés | PascalCase | `GetUserAsync`, `FirstName` |
| Champs privés | _camelCase | `_userRepository`, `_logger` |
| Variables locales, Paramètres | camelCase | `userId`, `cancellationToken` |
| Constantes | PascalCase | `MaxRetryCount` |
| Interfaces | Préfixe I | `IUserService` |

## Structure de Fichier

```csharp
#nullable enable

namespace Company.Project.Module;

/// <summary>
/// Description de la classe.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Crée un nouvel utilisateur.
    /// </summary>
    /// <param name="dto">Les données de création.</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>L'utilisateur créé.</returns>
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        var savedUser = await _repository.AddAsync(user, ct);
        
        _logger.LogInformation("User created: {UserId}", savedUser.Id);

        return savedUser.ToDto();
    }
}
```

## Bonnes Pratiques

### Async/Await
- Utiliser `async/await` pour toutes les opérations I/O
- Suffixer les méthodes async avec `Async`
- Toujours accepter `CancellationToken` comme dernier paramètre
- Ne jamais utiliser `.Result` ou `.Wait()` (deadlocks)

### Nullable Reference Types
- Activer `#nullable enable` dans tous les fichiers
- Utiliser `?` pour les types nullables
- Utiliser `ArgumentNullException.ThrowIfNull()` pour la validation

### Dependency Injection
- Injecter les dépendances via le constructeur
- Utiliser des interfaces (`IUserService`, pas `UserService`)
- Préférer `readonly` pour les champs injectés

### Gestion des Erreurs
```csharp
try
{
    await ProcessAsync(data, ct);
}
catch (SpecificException ex) when (ex.ErrorCode == "KNOWN_ERROR")
{
    _logger.LogWarning(ex, "Handled error: {ErrorCode}", ex.ErrorCode);
    // Handle specifically
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error processing {DataId}", data.Id);
    throw;
}
```

### Logging Structuré
```csharp
// ✅ Bon - Logging structuré avec placeholders
_logger.LogInformation("Order {OrderId} created for user {UserId}", order.Id, user.Id);

// ❌ Mauvais - Interpolation de string
_logger.LogInformation($"Order {order.Id} created for user {user.Id}");
```

## Patterns Recommandés

### Record pour les DTOs (C# 10+)
```csharp
public record CreateUserDto(string Name, string Email);
public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);
```

### Primary Constructor (C# 12+)
```csharp
public class UserService(IUserRepository repository, ILogger<UserService> logger) : IUserService
{
    public async Task<UserDto> GetUserAsync(Guid id, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        return user?.ToDto() ?? throw new NotFoundException($"User {id} not found");
    }
}
```

### Pattern Matching
```csharp
var result = status switch
{
    OrderStatus.Pending => ProcessPending(order),
    OrderStatus.Confirmed => ProcessConfirmed(order),
    OrderStatus.Shipped => ProcessShipped(order),
    _ => throw new InvalidOperationException($"Unknown status: {status}")
};
```
