---
applyTo: "tests/**"
---

# Instructions Tests

Quand tu travailles dans le dossier tests:

- Tests en 3 parties: Arrange, Act, Assert
- Nommage clair et descriptif
- Un seul concept testé par test
- Mocks et stubs appropriés

```typescript
describe('UserService', () => {
  let service: UserService;
  let mockRepository: jest.Mocked<IUserRepository>;
  
  beforeEach(() => {
    mockRepository = createMockRepository();
    service = new UserService(mockRepository);
  });
  
  describe('createUser', () => {
    it('should create user with valid data', async () => {
      // Arrange
      const dto = { name: 'John', email: 'john@example.com' };
      const expectedUser = new User(dto);
      mockRepository.save.mockResolvedValue(expectedUser);
      
      // Act
      const result = await service.createUser(dto);
      
      // Assert
      expect(result).toEqual(expectedUser);
      expect(mockRepository.save).toHaveBeenCalledWith(
        expect.objectContaining(dto)
      );
    });
  });
});
```
