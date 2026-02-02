# SBM Client Instructions

Instructions **spécifiques au client SBM Offshore**.

## Contenu

- **sbm.system.md** : System prompt et context SBM
- **sbm-isp-architecture-guidelines.md** : Guidelines architecture pour projet ISP

## Hiérarchie

Ces fichiers sont chargés **après** `instructions/AGENTS.base.md` et **avant** les fichiers de knowledge.

Ordre de chargement pour agent SBM :
1. `instructions/AGENTS.base.md` (universel)
2. `instructions/common/` (si applicable)
3. `clients/sbm/instructions/` (SBM-specific - ce dossier)
4. `clients/sbm/CLIENT.md` (contexte global SBM)
5. `knowledge/sbm/` (knowledge base - dynamique)

## Override Pattern

Si une instruction universelle doit être changée pour SBM, créer un fichier ici avec le même nom/sujet.

Exemple :
- `instructions/common/azure-standards.md` (par défaut)
- `clients/sbm/instructions/azure-standards-sbm.md` (SBM override)

Les agents chargeront le fichier SBM **après** le fichier commun, donc il prend priorité.

## Adding New Instructions

1. Universelles → `instructions/AGENTS.base.md` ou `instructions/contracts/`
2. Communes à plusieurs clients → `instructions/common/`
3. SBM-only → `clients/sbm/instructions/`
