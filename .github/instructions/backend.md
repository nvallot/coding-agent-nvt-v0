---
applyTo: "src/backend/**"
---

# Instructions Backend

Quand tu travailles dans le dossier backend:

- Architecture en couches (API → Service → Repository)
- Dependency Injection
- Validation des entrées
- Gestion d'erreurs centralisée
- Logging structuré

```typescript
// Structure de service
@Injectable()
export class UserService {
  constructor(
    private readonly repository: IUserRepository,
    private readonly logger: ILogger
  ) {}
  
  async createUser(dto: CreateUserDto): Promise<User> {
    // Validation
    this.validateUserDto(dto);
    
    // Business logic
    const user = User.create(dto);
    
    // Persistence
    const savedUser = await this.repository.save(user);
    
    // Logging
    this.logger.info('User created', { userId: savedUser.id });
    
    return savedUser;
  }
}
```
