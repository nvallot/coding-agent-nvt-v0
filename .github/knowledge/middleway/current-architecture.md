# Current Architecture – Middleway

## Vue générale
Middleway dispose d'une plateforme d'intégration Azure permettant
l'échange de données entre plusieurs applications hétérogènes.

## Composants principaux
- **Azure Functions** : Logique métier et transformations
- **Service Bus** : Messagerie asynchrone
- **Logic Apps** : Orchestration et connecteurs
- **APIM** : Exposition des APIs
- **Storage Accounts** : Stockage de fichiers et données

## Problèmes identifiés
- Couplage fort entre certaines applications
- Reprises sur erreur manuelles
- Manque de visibilité sur les flux

## Contraintes
- Systèmes legacy non modifiables
- Volumétrie variable
- Respect des conventions de nommage Middleway
