---
applyTo: "**/*"
---

# Conventions Globales (Client-Agnostic)

## 💻 Langages & Frameworks
| Domaine | Standards |
|---------|-----------|
| **Python** | snake_case, pytest, pandas/pyspark |
| **C#** | PascalCase, xUnit, async/await |
| **SQL** | UPPERCASE keywords, indexed views |
| **Terraform** | snake_case, remote state, modules |
| **Fichiers** | kebab-case (sauf source code) |

## 🔍 Qualité Code
- DRY: Pas de duplication
- SOLID: Single Responsibility, Open/Closed, etc.
- KISS: Keep It Simple
- Tests: Cible >80%, critique >95%
- Erreurs: Gestion explicite, pas de swallow silencieux

## 🔐 Sécurité (Principes)
- Defense in Depth: plusieurs couches
- Least Privilege: accès minimal requis
- Zero Trust: vérifier toujours
- Security by Design: intégrer dès le départ

**Checklist universelle**:
- ✅ Aucun secret en clair
- ✅ Validation entrées
- ✅ Gestion erreurs explicite
- ✅ Logging structuré
- ✅ Rate limiting si applicable

## 📊 Logging & Monitoring
```
Niveau       | Quand
-------------|------------------------------------
DEBUG        | Exécution détaillée, variables
INFO         | Étapes clés, transitions
WARNING      | Comportement inattendu mais contrôlé
ERROR        | Problème but can retry/fallback
CRITICAL     | Arrêt du processus
```

**Format obligatoire**: JSON avec `CorrelationId` pour tracer end-to-end

## 📚 Documentation
- Docstrings pour fonctions publiques
- Comments pour logique complexe (pas pour obvieux)
- README pour setup et usage
- ADRs pour décisions majeures
