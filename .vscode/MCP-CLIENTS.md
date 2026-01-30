# MCP par client (VS Code)

VS Code n'utilise qu'un seul fichier actif : `.vscode/mcp.json`.
Pour gérer un MCP par client, utilisez les fichiers suivants :

- `.vscode/mcp.middleway.json`
- `.vscode/mcp.sbm.json`
- `.vscode/mcp.client-demo.json`

Par défaut, les configs SBM et SEB sont **figées** sur leur organisation ADO
respective (pas de prompt).

Pour ajouter un client, créez un fichier `.vscode/mcp.<client>.json`.

## Activer un client

Exécuter le script PowerShell :
- `.vscode/switch-mcp.ps1 -Client <client>`

Le script détecte automatiquement les clients disponibles à partir des fichiers
`mcp.<client>.json`.

Après le switch :
1. Redémarrer le serveur MCP dans la vue MCP.
2. Re-sélectionner les outils.

## GitHub par client (VS Code profiles)

Le compte GitHub MCP est **global** tant que les profils partagent le même
`user-data-dir`. Pour isoler le compte GitHub par client, il faut **un
user-data-dir dédié par profil**.

Le script `switch-vscode-profile.ps1` ouvre désormais VS Code avec un
`user-data-dir` isolé par client, ce qui permet de conserver un compte GitHub
différent par profil.

Étapes :
1. Créer un profil VS Code par client.
2. Ouvrir le workspace avec le bon profil via le script ci-dessous.
3. Dans chaque profil (et son user-data-dir), se connecter au compte GitHub voulu.

Script helper : `.vscode/switch-vscode-profile.ps1 -Profile <client>`

Ce script met aussi à jour automatiquement
`.github/agents-framework/active-client.json` avec le nom du profil.

Astuce : vous pouvez aussi lancer la tâche VS Code
"Switch VS Code profile (sync clientKey)" pour faire le switch et la sync.

## Sync rapide depuis le profil courant

Tâche dédiée : "Sync from current profile (clientKey + MCP)"

Script : `.vscode/sync-from-profile.ps1`

Raccourci (workspace) : `Ctrl+Alt+P` (voir `.vscode/keybindings.json`)
