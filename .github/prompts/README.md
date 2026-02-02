# Prompts Files

Les **prompt files** sont des templates réutilisables pour des tâches spécifiques qui nécessitent une collaboration structurée.

## Contenu

- `architecture-design.prompt` – Guide pour produire un document d'architecture complet
- `code-review.prompt` – Template de revue de code avec checklist
- `requirements-analysis.prompt` – Transformation besoin métier → cahier des charges
- `brainstorm.prompt` – Session de brainstorming structurée

## Usage

Dans VS Code, invoquer avec : `/` puis sélectionner le prompt

Exemple :
```
/architecture-design
```

Les variables `{{variable}}` seront demandées à l'utilisateur.

## Méta-données

Chaque prompt file contient :
- `title` : Nom du prompt
- `description` : Description courte
- `agents` : Liste des agents compatibles
- `applyTo` : Pattern de fichiers (path-based)

## Création de nouveaux prompts

Format :
```markdown
---
title: "Nom du Prompt"
description: "Description"
agents: ["agent1", "agent2"]
applyTo: "pattern/**"
---

# Instructions

{{variable1}}
{{variable2}}

...
```

## Best Practices

- Utiliser pour des tâches **collaboratives** et **répétitives**
- Fournir des templates structurés (tableaux, checklists)
- Inclure des critères de qualité
- Référencer instructions/knowledge pertinents
- Spécifier le format de sortie attendu
