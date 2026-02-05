# Skill: Testing

## Description
Compétence pour créer des tests de qualité assurant la fiabilité et la maintenabilité du code.

## Objectif
Fournir une approche systématique pour tester efficacement à tous les niveaux (unitaire, intégration, E2E).

## Types de Tests

### 1. Tests Unitaires
- **Scope**: Fonction ou méthode isolée
- **Vitesse**: Très rapide (ms)
- **Coverage**: Cible > 80%
- **Outils C#**: xUnit, NUnit, Moq
- **Outils Python**: pytest, unittest.mock
- **Exemple**: Tester une fonction de calcul

### 2. Tests d'Intégration
- **Scope**: Plusieurs modules ensemble
- **Vitesse**: Modérée (secondes)
- **Coverage**: Flux critiques
- **Outils C#**: xUnit + WebApplicationFactory, TestContainers
- **Outils Python**: pytest + httpx, TestContainers
- **Exemple**: Tester un appel API + DB

### 3. Tests End-to-End (E2E)
- **Scope**: Workflow utilisateur complet
- **Vitesse**: Lente (minutes)
- **Coverage**: Scénarios clés
- **Outils**: Playwright, Selenium
- **Exemple**: Test de pipeline ADF complet

### 4. Tests de Performance
- **Scope**: Temps de réponse, mémoire
- **Vitesse**: Variable
- **Coverage**: Points critiques
- **Outils**: k6, BenchmarkDotNet, locust
- **Exemple**: Load test d'une API

### 5. Tests de Sécurité
- **Scope**: Vulnérabilités
- **Vitesse**: Variable
- **Coverage**: Authentification, injection
- **Outils**: OWASP ZAP, Burp Suite
- **Exemple**: Test d'injection SQL

## Méthodologie TDD

### Red-Green-Refactor
```
1. RED: Écrire un test qui échoue
2. GREEN: Implémenter le minimum pour passer
3. REFACTOR: Améliorer le code sans casser le test
```

### Avantages
- Code plus testable
- Design meilleur
- Moins de bugs
- Confiance accrue

## Bonnes Pratiques

### Structure des Tests (C# - xUnit)
```csharp
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _sut; // System Under Test

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _sut = new UserService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ReturnsUser()
    {
        // Arrange
        var dto = new CreateUserDto { Name = "John", Email = "john@test.com" };
        
        // Act
        var result = await _sut.CreateUserAsync(dto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateUserAsync_WithInvalidEmail_ThrowsException(string? email)
    {
        // Arrange
        var dto = new CreateUserDto { Name = "John", Email = email! };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateUserAsync(dto));
    }
}
```

### Structure des Tests (Python - pytest)
```python
import pytest
from unittest.mock import Mock, AsyncMock

class TestUserService:
    @pytest.fixture
    def mock_repository(self):
        return Mock(spec=IUserRepository)
    
    @pytest.fixture
    def service(self, mock_repository):
        return UserService(repository=mock_repository)

    @pytest.mark.asyncio
    async def test_create_user_with_valid_data_returns_user(self, service, mock_repository):
        # Arrange
        dto = CreateUserDto(name="John", email="john@test.com")
        mock_repository.add = AsyncMock(return_value=User(id=uuid4(), **dto.__dict__))
        
        # Act
        result = await service.create_user(dto)
        
        # Assert
        assert result is not None
        assert result.email == dto.email

    @pytest.mark.asyncio
    @pytest.mark.parametrize("email", [None, ""])
    async def test_create_user_with_invalid_email_raises_error(self, service, email):
        # Arrange
        dto = CreateUserDto(name="John", email=email)

        # Act & Assert
        with pytest.raises(ValueError):
            await service.create_user(dto)
```

### AAA Pattern (Arrange-Act-Assert)
```csharp
// C# Example
[Fact]
public async Task Method_Scenario_ExpectedResult()
{
    // Arrange: Préparer les données
    var input = new CreateOrderDto { /* ... */ };
    _mockService.Setup(s => s.ProcessAsync(It.IsAny<Order>())).ReturnsAsync(true);

    // Act: Exécuter le code
    var result = await _sut.CreateOrderAsync(input);

    // Assert: Vérifier le résultat
    Assert.True(result.IsSuccess);
    _mockService.Verify(s => s.ProcessAsync(It.IsAny<Order>()), Times.Once);
}
```

### Coverage
- **Minimum**: 70% pour du nouveau code
- **Bon**: 80% pour la plupart des projets
- **Excellent**: 90%+ pour code critique
- **Outils C#**: Coverlet, dotnet-coverage
- **Outils Python**: pytest-cov, coverage.py

### Test Data
- Utiliser des fixtures
- Générer des données avec AutoFixture (C#) ou Faker (Python)
- Mock les dépendances externes
- Tester les edge cases

## Frameworks par Langage

### C#/.NET (Primary)
- **xUnit**: Framework moderne recommandé
- **NUnit**: Framework populaire
- **Moq**: Mocking
- **FluentAssertions**: Assertions lisibles
- **AutoFixture**: Génération de données
- **TestContainers**: Tests d'intégration avec Docker

### Python (Primary)
- **pytest**: Framework recommandé
- **pytest-asyncio**: Tests async
- **pytest-cov**: Coverage
- **unittest.mock**: Mocking standard
- **Faker**: Génération de données
- **httpx**: Tests HTTP async

## Mocking et Stubbing

### Quand Mocker
- Dépendances externes (API, DB)
- Services Azure (Storage, Service Bus)
- Comportements non-déterministes
- Code non développé

### Quand Ne Pas Mocker
- Logique métier du système
- Tests d'intégration
- Bases de données en test (utiliser TestContainers)
- Comportements réels critiques

## Exemples d'Utilisation

### Créer des tests unitaires (C#)
```
@dev /test CreateUserAsync method with xUnit
```

### Créer des tests unitaires (Python)
```
@dev /test create_user function with pytest
```

### Ajouter un test d'intégration
```
@dev /test API endpoint POST /users with WebApplicationFactory
```

### Améliorer la couverture
```
@dev /test increase coverage for payment module
```

## Critères de Succès
- ✅ Tests écrits avant ou en même temps que le code
- ✅ Tous les tests passent
- ✅ Coverage > 80%
- ✅ Tests maintenables et clairs
- ✅ Pas de flaky tests
- ✅ CI/CD intégré

## Checklist de Test

- [ ] Tests unitaires pour logique métier
- [ ] Tests d'intégration pour flux critiques
- [ ] Tests E2E pour workflows clés
- [ ] Mocks des dépendances Azure
- [ ] Data setup et cleanup
- [ ] Assertions claires et significatives
- [ ] Tests documentés
- [ ] Coverage acceptable

## Ressources
- [Testing Pyramid](https://martinfowler.com/bliki/TestPyramid.html)
- [xUnit Documentation](https://xunit.net/)
- [Pytest Documentation](https://docs.pytest.org/)
- [FluentAssertions](https://fluentassertions.com/)
- [TestContainers .NET](https://dotnet.testcontainers.org/)
