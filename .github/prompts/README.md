# Prompts

Ce dossier contient les instructions et templates utilisés par les agents.

## Structure
- `common/` : règles globales (style, sécurité, format)
- `clients/` : instructions spécifiques par client
- `skills/` : templates par rôle (BA, Architecte, Dev)

## Usage
Les agents chargent :
1) `common/*.md`
2) `clients/<client>/*.md`
3) `skills/<role>.md`

## MCP par client
Si une configuration MCP existe, elle doit être chargée depuis :
`clients/<client>/mcp.json`
