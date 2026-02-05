# Context - Espace de Travail Partagé

Ce dossier contient le contexte partagé entre les agents lors d'un workflow.

## Fichiers de Contexte

| Fichier | Agent Source | Description |
|---------|--------------|-------------|
| `current-request.md` | Utilisateur | La demande initiale du client/utilisateur |
| `brd-output.md` | `@ba` | Business Requirements Document |
| `tad-output.md` | `@archi` | Technical Architecture Document |
| `implementation-plan.md` | `@dev` | Plan d'implémentation détaillé |
| `review-output.md` | `@reviewer` | Rapport de revue de code |

## Utilisation

### Démarrer un nouveau workflow

1. Écrire la demande dans `current-request.md`
2. Invoquer l'agent approprié avec `#file:.github/context/current-request.md`

### Passer au prochain agent

Utiliser les prompts de handoff :
```
#prompt:handoff-to-archi
#prompt:handoff-to-dev
#prompt:handoff-to-reviewer
```

### Utiliser un seul agent (sans workflow complet)

Vous pouvez invoquer n'importe quel agent directement avec le contexte existant :
```
@dev #file:.github/context/tad-output.md
Implémente le module X selon le TAD
```

## Convention

- Les fichiers sont écrasés à chaque exécution
- Utilisez Git pour conserver l'historique si nécessaire
- Le format est Markdown avec frontmatter YAML pour les métadonnées
