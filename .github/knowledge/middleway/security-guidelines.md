# Security Guidelines – Middleway

## SEC-001 – Authentification
Les échanges entre systèmes doivent être authentifiés via Managed Identity
ou certificats stockés dans le Key Vault.

## SEC-002 – Autorisation
Les accès sont accordés selon le principe du moindre privilège.
Utiliser les RBAC Azure.

## SEC-003 – Données sensibles
Les données sensibles ne doivent jamais transiter en clair.
Utiliser les secrets du Key Vault (`{env}-kv-mw`).

## SEC-004 – Journalisation
Les événements de sécurité doivent être journalisés et conservés
dans Log Analytics (`{env}-log-mw`).

## SEC-005 – Gestion des secrets
- Secrets stockés dans Key Vault
- Rotation régulière
- Naming : `{Activité}--{Type}--{Nom}--{Description}`
