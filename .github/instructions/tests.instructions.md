---
applyTo: "tests/**"
---

# Instructions Tests (C# / Python)

Quand tu travailles dans le dossier tests:

- Tests en 3 parties: Arrange, Act, Assert
- Nommage clair et descriptif
- Un seul concept testé par test
- Mocks et stubs appropriés

## Intended Agents

- `@dev` (Developer) - Primary user for writing tests
- `@reviewer` (Code Reviewer) - For review purposes

---

## C# - xUnit / NUnit

```csharp
// Structure de test xUnit
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _sut; // System Under Test

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _sut = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ReturnsCreatedUser()
    {
        // Arrange
        var dto = new CreateUserDto { Name = "John", Email = "john@example.com" };
        var expectedUser = new User { Id = Guid.NewGuid(), Name = dto.Name, Email = dto.Email };
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _sut.CreateUserAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateUserAsync_WithInvalidEmail_ThrowsArgumentException(string? email)
    {
        // Arrange
        var dto = new CreateUserDto { Name = "John", Email = email! };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateUserAsync(dto));
    }
}
```

## Python - pytest

```python
# Structure de test pytest
import pytest
from unittest.mock import Mock, AsyncMock
from uuid import uuid4

class TestUserService:
    @pytest.fixture
    def mock_repository(self):
        return Mock(spec=IUserRepository)
    
    @pytest.fixture
    def service(self, mock_repository):
        return UserService(repository=mock_repository)

    @pytest.mark.asyncio
    async def test_create_user_with_valid_data_returns_created_user(self, service, mock_repository):
        # Arrange
        dto = CreateUserDto(name="John", email="john@example.com")
        expected_user = User(id=uuid4(), name=dto.name, email=dto.email)
        mock_repository.add = AsyncMock(return_value=expected_user)

        # Act
        result = await service.create_user(dto)

        # Assert
        assert result is not None
        assert result.email == dto.email
        mock_repository.add.assert_called_once()

    @pytest.mark.asyncio
    @pytest.mark.parametrize("email", [None, ""])
    async def test_create_user_with_invalid_email_raises_value_error(self, service, email):
        # Arrange
        dto = CreateUserDto(name="John", email=email)

        # Act & Assert
        with pytest.raises(ValueError):
            await service.create_user(dto)
```

## Bonnes Pratiques

### Nommage des tests
- C#: `MethodName_Scenario_ExpectedBehavior`
- Python: `test_method_name_scenario_expected_behavior`

### Coverage
- Minimum: 70% pour du nouveau code
- Bon: 80% pour la plupart des projets
- Critique: 90%+ pour code métier important

### Outils
- **C#**: xUnit, NUnit, Moq, FluentAssertions, Coverlet
- **Python**: pytest, pytest-asyncio, pytest-cov, unittest.mock
