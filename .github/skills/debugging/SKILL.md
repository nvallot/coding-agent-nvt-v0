# Skill: Debugging

## Description
Compétence pour identifier, analyser et résoudre les bugs et problèmes dans le code.

## Objectif
Fournir une méthode systématique pour déboguer efficacement et résoudre les problèmes de façon durable.

## Étapes du Débogage

### 1. Reproduire le Problème
- Comprendre le comportement inattendu
- Isoler les étapes de reproduction
- Documenter l'environnement
- Vérifier les logs

### 2. Analyser le Problème
- Localiser le code responsable
- Tracer l'exécution (stack trace)
- Examiner l'état des variables
- Identifier les conditions d'erreur

### 3. Former une Hypothèse
- Théoriser la cause racine
- Valider l'hypothèse avec des tests
- Éliminer les fausses pistes
- Confirmer le diagnostic

### 4. Implémenter la Correction
- Écrire un test qui reproduit le bug
- Corriger le code
- Vérifier que le test passe
- Éviter les régressions

### 5. Valider la Solution
- Tester dans tous les environnements
- Vérifier les edge cases
- Analyser l'impact
- Documenter la correction

### 6. Prévenir les Récidives
- Ajouter des tests unitaires
- Améliorer la logging
- Documenter les pièges
- Parler à l'équipe

## Techniques de Débogage

### Logging Structuré
```
Log le contexte: qui, quoi, quand, où, pourquoi
Niveaux: DEBUG, INFO, WARNING, ERROR, CRITICAL
```

### Debugging Interactif
- Breakpoints conditionnels
- Watch expressions
- Stack inspection
- Variable inspection

### Analyse des Logs
- Grep/search les patterns
- Suivre les traces distribuées
- Corréler les timestamps
- Analyser les métriques

### Profiling
- CPU profiling
- Memory profiling
- I/O analysis
- Bottleneck identification

## Outils par Langage

### JavaScript/TypeScript
- Chrome DevTools
- VS Code Debugger
- Node Inspector
- Postman

### Python
- pdb (Python Debugger)
- VS Code Python Extension
- Jupyter Notebooks
- ipdb

### Java
- IntelliJ IDEA Debugger
- Visual VM
- JProfiler
- YourKit

### C#/.NET
- Visual Studio Debugger
- dotTrace
- Application Insights
- Serilog

## Bonnes Pratiques

### Avant le Bug
- Écrire des tests
- Utiliser le type checking
- Faire de la revue de code
- Logger intelligemment

### Pendant le Débogage
- Rester calme et méthodique
- Documenter les hypothèses
- Tester progressivement
- Collaborer avec l'équipe

### Après la Correction
- Écrire un test de régression
- Documenter la cause racine
- Mettre à jour la documentation
- Partager les apprentissages

## Exemples d'Utilisation

### Déboguer une erreur
```
@developpeur /debug "TypeError: undefined is not a function"
```

### Analyser une performance
```
@developpeur /debug slow response time on /api/users
```

### Trouver une fuite mémoire
```
@developpeur /debug memory leak in authentication module
```

## Critères de Succès
- ✅ Bug reproduit et compris
- ✅ Cause racine identifiée
- ✅ Solution implémentée et testée
- ✅ Tests de régression ajoutés
- ✅ Documentation mise à jour
- ✅ Performance acceptée

## Ressources
- [Debug Like a Scientist](https://github.blog/2015-11-05-debugging-like-a-boss/)
- [The Debugging Book](https://www.debuggingbook.org/)
- [Advanced Debugging Techniques](https://www.owasp.org/index.php/Debugging)
