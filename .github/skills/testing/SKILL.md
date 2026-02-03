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
- **Outils**: Jest, pytest, xUnit
- **Exemple**: Tester une fonction de calcul

### 2. Tests d'Intégration
- **Scope**: Plusieurs modules ensemble
- **Vitesse**: Modérée (secondes)
- **Coverage**: Flux critiques
- **Outils**: Pytest, Mocha, JUnit
- **Exemple**: Tester un appel API + DB

### 3. Tests End-to-End (E2E)
- **Scope**: Workflow utilisateur complet
- **Vitesse**: Lente (minutes)
- **Coverage**: Scénarios clés
- **Outils**: Cypress, Selenium, Playwright
- **Exemple**: Inscription + achat + paiement

### 4. Tests de Performance
- **Scope**: Temps de réponse, mémoire
- **Vitesse**: Variable
- **Coverage**: Points critiques
- **Outils**: k6, JMeter, Apache Bench
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

### Structure des Tests
```
describe('Feature', () => {
  beforeEach(() => { /* setup */ })
  afterEach(() => { /* cleanup */ })
  
  it('should do something', () => {
    expect(result).toBe(expected)
  })
})
```

### AAA Pattern (Arrange-Act-Assert)
```
// Arrange: Préparer les données
const input = { ... }

// Act: Exécuter le code
const result = function(input)

// Assert: Vérifier le résultat
expect(result).toBe(expected)
```

### Coverage
- **Minimum**: 70% pour du nouveau code
- **Bon**: 80% pour la plupart des projets
- **Excellent**: 90%+ pour code critique
- **Outil**: nyc, coverage.py, JaCoCo

### Test Data
- Utiliser des fixtures
- Générer des données aléatoires
- Mock les dépendances externes
- Tester les edge cases

## Frameworks par Langage

### JavaScript/TypeScript
- **Jest**: Tests unitaires et d'intégration
- **Cypress**: E2E browser testing
- **Playwright**: E2E multi-navigateur
- **Supertest**: API testing

### Python
- **pytest**: Tests unitaires et d'intégration
- **unittest**: Framework standard
- **nose2**: Test runner
- **Robot Framework**: Acceptance testing

### Java
- **JUnit**: Framework standard
- **Mockito**: Mocking
- **Selenium**: Web testing
- **TestNG**: Tests avancés

### C#/.NET
- **xUnit**: Framework modern
- **Moq**: Mocking
- **NUnit**: Framework populaire
- **SpecFlow**: BDD testing

## Mocking et Stubbing

### Quand Mocker
- Dépendances externes (API, DB)
- Services coûteux
- Comportements non-déterministes
- Code non développé

### Quand Ne Pas Mocker
- Logique métier du système
- Tests d'intégration
- Bases de données en test
- Comportements réels

## Exemples d'Utilisation

### Créer des tests unitaires
```
@developpeur /test getUserById function
```

### Ajouter un test d'intégration
```
@developpeur /test API endpoint POST /users
```

### Améliorer la couverture
```
@developpeur /test increase coverage for payment module
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
- [ ] Mocks des dépendances externes
- [ ] Data setup et cleanup
- [ ] Assertions claires et significatives
- [ ] Tests documentés
- [ ] Coverage acceptable

## Ressources
- [Testing Pyramid](https://martinfowler.com/bliki/TestPyramid.html)
- [Test Driven Development](https://en.wikipedia.org/wiki/Test-driven_development)
- [Jest Documentation](https://jestjs.io/)
- [Pytest Documentation](https://docs.pytest.org/)
