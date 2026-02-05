# Conventions de Code

## 🎯 Vue d'Ensemble

Ce document définit les conventions de code à respecter dans tous les projets de l'organisation.

## 📋 Conventions Générales

### Nommage

- **Variables**: camelCase (`userName`, `totalCount`)
- **Constantes**: UPPER_SNAKE_CASE (`MAX_RETRY_COUNT`, `API_BASE_URL`)
- **Fonctions/Méthodes**: camelCase (`getUserById`, `calculateTotal`)
- **Classes**: PascalCase (`UserService`, `OrderRepository`)
- **Interfaces** (TypeScript/C#): PascalCase avec préfixe I (`IUserService`, `IRepository`)
- **Fichiers**: kebab-case (`user-service.ts`, `order-repository.cs`)

### Structure de Fichiers

```
src/
├── api/                    # Points d'entrée API
├── domain/                 # Logique métier
│   ├── models/            # Entités et value objects
│   ├── services/          # Services métier
│   └── repositories/      # Interfaces de persistence
├── infrastructure/        # Implémentation technique
│   ├── database/         # Accès données
│   ├── external/         # Services externes
│   └── config/           # Configuration
├── application/          # Cas d'usage / Use cases
└── shared/              # Code partagé
    ├── utils/
    ├── types/
    └── constants/
```

## 🔤 Par Langage

### TypeScript / JavaScript

```typescript
// ✅ Bon
export class UserService {
  private readonly repository: IUserRepository;
  
  constructor(repository: IUserRepository) {
    this.repository = repository;
  }
  
  async getUserById(userId: string): Promise<User | null> {
    if (!userId) {
      throw new ValidationError('userId is required');
    }
    
    return await this.repository.findById(userId);
  }
}

// ❌ Mauvais
export class userservice {
  constructor(public repo: any) {}
  
  getUserById(id) {
    return this.repo.findById(id);
  }
}
```

### Python

```python
# ✅ Bon
from typing import Optional

class UserService:
    """Service pour gérer les utilisateurs."""
    
    def __init__(self, repository: IUserRepository) -> None:
        self._repository = repository
    
    async def get_user_by_id(self, user_id: str) -> Optional[User]:
        """Récupère un utilisateur par son ID."""
        if not user_id:
            raise ValueError("user_id is required")
        
        return await self._repository.find_by_id(user_id)

# ❌ Mauvais
class user_service:
    def __init__(self, repo):
        self.repo = repo
    
    def getUserById(self, id):
        return self.repo.findById(id)
```

### C#

```csharp
// ✅ Bon
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;
    
    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }
        
        return await _repository.FindByIdAsync(userId, cancellationToken);
    }
}

// ❌ Mauvais
public class userService
{
    public async Task<User> GetUserById(string id)
    {
        return await repo.FindById(id);
    }
}
```

## 📝 Commentaires et Documentation

### Documentation de Fonction

```typescript
/**
 * Récupère un utilisateur par son identifiant.
 * 
 * @param userId - L'identifiant unique de l'utilisateur
 * @returns Une promesse contenant l'utilisateur ou null si non trouvé
 * @throws {ValidationError} Si userId est vide ou invalide
 * @throws {DatabaseError} En cas d'erreur de base de données
 * 
 * @example
 * ```typescript
 * const user = await userService.getUserById('user-123');
 * if (user) {
 *   console.log(user.name);
 * }
 * ```
 */
async getUserById(userId: string): Promise<User | null>
```

### Commentaires Inline

```typescript
// ✅ Bon - Explique le POURQUOI
// On utilise un cache de 5 minutes pour réduire la charge sur la DB
// lors des pics de trafic du matin
const CACHE_TTL = 300;

// ❌ Mauvais - Explique le QUOI (déjà visible dans le code)
// Incrémente le compteur
counter++;
```

## 🧪 Tests

### Nommage des Tests

```typescript
describe('UserService', () => {
  describe('getUserById', () => {
    it('should return user when user exists', async () => {
      // Arrange
      const userId = 'user-123';
      const expectedUser = new User(userId, 'John Doe');
      mockRepository.findById.mockResolvedValue(expectedUser);
      
      // Act
      const result = await userService.getUserById(userId);
      
      // Assert
      expect(result).toEqual(expectedUser);
    });
    
    it('should return null when user does not exist', async () => {
      // Arrange
      mockRepository.findById.mockResolvedValue(null);
      
      // Act
      const result = await userService.getUserById('unknown');
      
      // Assert
      expect(result).toBeNull();
    });
    
    it('should throw ValidationError when userId is empty', async () => {
      // Act & Assert
      await expect(userService.getUserById('')).rejects.toThrow(ValidationError);
    });
  });
});
```

## 🔐 Sécurité

### Gestion des Secrets

```typescript
// ✅ Bon
const apiKey = process.env.API_KEY;
if (!apiKey) {
  throw new Error('API_KEY environment variable is required');
}

// ❌ Mauvais
const apiKey = "sk-1234567890abcdef";
```

### Validation des Entrées

```typescript
// ✅ Bon
function processUserInput(input: string): string {
  // Valider
  if (!input || input.length > 1000) {
    throw new ValidationError('Invalid input length');
  }
  
  // Sanitizer
  const sanitized = input.trim().replace(/[<>]/g, '');
  
  return sanitized;
}
```

## 🎨 Formatage

### Indentation
- **Espaces**: 2 espaces (TypeScript, JavaScript)
- **Espaces**: 4 espaces (Python, C#)
- Pas de tabulations

### Longueur de Ligne
- Maximum: 120 caractères
- Préférer 80-100 pour lisibilité

### Imports

```typescript
// ✅ Bon - Groupés et triés
// 1. Modules externes
import { Injectable } from '@nestjs/common';
import { Repository } from 'typeorm';

// 2. Modules internes absolus
import { User } from '@/domain/models/user';
import { IUserRepository } from '@/domain/repositories/user-repository';

// 3. Modules internes relatifs
import { DatabaseConfig } from '../config/database';
import { Logger } from './logger';

// ❌ Mauvais - Non organisés
import { Logger } from './logger';
import { User } from '@/domain/models/user';
import { Injectable } from '@nestjs/common';
```

## 🚫 Anti-Patterns à Éviter

1. **God Objects**: Classes avec trop de responsabilités
2. **Magic Numbers**: Utiliser des constantes nommées
3. **Deep Nesting**: Maximum 3 niveaux d'indentation
4. **Long Methods**: Maximum 50 lignes par fonction
5. **Too Many Parameters**: Maximum 4 paramètres (utiliser un objet)
6. **Callback Hell**: Utiliser async/await

## ✅ Checklist Avant Commit

- [ ] Code respecte les conventions de nommage
- [ ] Fonctions documentées (JSDoc/docstring)
- [ ] Tests unitaires ajoutés/mis à jour
- [ ] Pas de code commenté inutile
- [ ] Pas de console.log ou debug statements
- [ ] Imports organisés et triés
- [ ] Gestion d'erreurs appropriée
- [ ] Variables sensibles externalisées
- [ ] Code formaté (Prettier/Black/etc.)
- [ ] Pas de warnings du linter
