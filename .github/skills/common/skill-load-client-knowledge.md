# Skill – Load Client Knowledge

## Objectif

Fournir aux agents un accès contrôlé à la base de connaissance
du client actif.

## Procédure

1. Lire le fichier `agents-framework/active-client.json`
2. Identifier la valeur `clientKey`
3. Considérer le dossier `knowledge/<clientKey>/` comme source factuelle
4. Charger `clients/<clientKey>/mcp.json` s’il existe (configuration MCP par client)
4. Utiliser les documents trouvés comme références :
   - standards
   - architecture existante
   - règles de données
   - runbooks
5. Ne jamais utiliser de documents provenant d’un autre client

## Règles

- Si aucune base de connaissance n’est disponible pour le client actif,
  continuer avec des hypothèses explicites
- Toujours distinguer :
  - faits issus de la knowledge base
  - hypothèses formulées par l’agent
