# Skill: Code Implementation

## Description
Compétence pour implémenter des fonctionnalités complètes et de qualité production.

## Objectif
Fournir une méthode systématique pour transformer des spécifications en code propre, testé et documenté.

## Étapes de Mise en Œuvre

### 1. Comprendre les Spécifications
- Analyser les requirements
- Identifier les contraintes
- Clarifier les cas d'usage
- Valider l'architecture

### 2. Planifier l'Implémentation
- Définir l'approche technique
- Identifier les dépendances
- Structurer le code
- Prévoir les tests

### 3. Implémenter
- Écrire du code propre et lisible
- Respecter les conventions
- Appliquer les patterns
- Gérer les erreurs

### 4. Tester
- Écrire des tests unitaires
- Couvrir les cas critiques
- Valider l'intégration
- Tester les edge cases

### 5. Documenter
- Commenter le code complexe
- Documenter les APIs
- Créer des exemples
- Maintenir le README

### 6. Réviser
- Auto-review du code
- Optimiser les performances
- Vérifier la sécurité
- Préparer pour la revue d'équipe

## Bonnes Pratiques

### Principes SOLID
- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

### Clean Code
- Noms explicites et significatifs
- Fonctions petites et focalisées
- Pas de code dupliqué (DRY)
- Gestion d'erreurs propre

### Performance
- Algorithmes efficaces
- Pas de requêtes N+1
- Caching intelligent
- Gestion mémoire optimale

## Langages Supportés
- **C# (.NET 8+)** - Primary
- **Python 3.11+** - Primary
- **PowerShell** - Scripting
- **SQL** - T-SQL, PostgreSQL

## Outils et Frameworks
- **Backend C#**: ASP.NET Core, Entity Framework Core, MediatR
- **Backend Python**: FastAPI, SQLAlchemy, Pydantic
- **Data**: Azure Data Factory, Databricks, PySpark
- **Cloud**: Azure Services (Functions, Storage, Service Bus)
- **IaC**: Terraform, Bicep, ARM Templates

## Exemples d'Utilisation

### Implémenter une API REST (C#)
```
@dev /implement API endpoint GET /users/{id} in ASP.NET Core
```

### Implémenter un service Python
```
@dev /implement data processing service with FastAPI
```

### Créer un pipeline ADF
```
@dev /implement ADF pipeline for incremental load
```

### Refactoriser du code
```
@dev /refactor this legacy function
```

## Critères de Succès
- ✅ Code compilé/exécuté sans erreurs
- ✅ Tests passent avec couverture > 80%
- ✅ Pas de warnings du linter
- ✅ Code review approuvée
- ✅ Documentation complète
- ✅ Performances acceptables

## Ressources
- [Clean Code - Robert Martin](https://www.oreilly.com/library/view/clean-code-a/9780136083238/)
- [Design Patterns](https://refactoring.guru/design-patterns)
- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [PEP 8 – Style Guide for Python](https://peps.python.org/pep-0008/)
