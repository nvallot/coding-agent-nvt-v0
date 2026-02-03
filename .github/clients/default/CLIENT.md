# Client: Default

## 📋 Contexte

Client par défaut pour les nouveaux projets sans spécificités particulières.

## 🎯 Priorités

- Suivre les best practices Azure standards
- Architecture Well-Architected Framework
- Sécurité et gouvernance de base

## 🔐 Sécurité

- Managed Identity pour authentification
- Key Vault pour secrets
- RBAC au niveau ressource
- Chiffrement at-rest et in-transit

## 🏗️ Infrastructure

- Terraform pour IaC
- Naming: Azure CAF standard
- Tags: Environment, Project, Owner, CostCenter, ManagedBy

## 📊 Observabilité

- Application Insights pour APM
- Log Analytics pour logs centralisés
- Azure Monitor pour métriques et alertes
- Structured logging avec CorrelationId

## 🧪 Tests

- Unit tests: 80% minimum
- Integration tests pour composants critiques
- Data quality tests

## 📚 Documentation

- README.md à jour
- Architecture diagrams (C4 Model)
- ADRs pour décisions importantes
- Code comments pour logique complexe

## ⚙️ CI/CD

- GitHub Actions ou Azure Pipelines
- Environments: dev, staging, prod
- Automated testing
- Manual approval pour prod

## 📝 Conventions

### Nommage Ressources

```
{resource-type}-{project}-{environment}-{region}-{instance}

Exemples:
- rg-dataplatform-dev-weu-001
- st-dataplatform-dev-weu-001
- adf-dataplatform-dev-weu-001
- dbw-dataplatform-dev-weu-001
```

### Nommage Code

- **Python**: snake_case pour fonctions/variables, PascalCase pour classes
- **SQL**: snake_case pour tables/colonnes, UPPERCASE pour mots-clés
- **Terraform**: snake_case pour ressources/variables
- **Fichiers**: kebab-case

### Git Branching

```
main (production)
├── develop (intégration)
    ├── feature/xxx (nouvelles fonctionnalités)
    ├── bugfix/xxx (corrections)
    └── hotfix/xxx (urgences prod)
```

## 📖 Sources de Vérité

1. `.github/instructions/copilot-instructions.md` (global)
2. `.github/clients/default/instructions/` (spécifique)
3. `.github/knowledge/` (base commune)

## 🎓 Notes

Ce profil sert de base. Les clients spécifiques peuvent surcharger ces conventions.

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-03
