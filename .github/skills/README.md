# Skills

Les **skills** décrivent **comment accomplir une tâche spécifique** (e.g., déployer un projet Azure Functions, créer une Logic App, configurer un Service Bus).

## Différence avec Instructions

| Type | Quand | Chargement |
|------|-------|-----------|
| **Instructions** | Comportement général des agents | Chargé au démarrage |
| **Skills** | Comment faire une tâche précise | Invoqué dynamiquement lors de l'exécution |

## Structure

```
skills/
├── README.md (ce fichier)
├── azure-function-deployment/  # Déployer Azure Function (generic)
├── bicep-deployment/           # Déployer Bicep template (generic)
└── service-bus-setup/          # Configurer Service Bus (generic)
```

Les skills sont **génériques** et applicables à tous les clients. Les spécificités client (naming, environments) sont référencées depuis `knowledge/<client>/` ou `clients/<client>/instructions/`.

Chaque skill contient un fichier `SKILL.md` avec :
- Description de la tâche
- Prérequis
- Étapes détaillées
- Exemples de commandes
- Références

## Utilisation

Les skills sont référencés dans les instructions génériques ou client-specific, puis invoqués par les agents selon le contexte.

Exemple dans une instruction :
```markdown
Pour déployer une Azure Function, utiliser la skill `skills/azure-function-deployment/SKILL.md`.
```

## Version

- v0.1 – Structure initiale skills génériques Azure
