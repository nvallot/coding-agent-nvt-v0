# Common Instructions

Éléments **partagés par plusieurs clients**, mais **pas universels** (donc non-inclus dans `AGENTS.base.md`).

Exemples :
- Standards Azure (si plusieurs clients utilisent Azure)
- Patterns architecturaux (si réutilisables)
- Processus DevOps communs

## Usage

Ces fichiers sont chargés **optionnellement** par les agents en fonction du besoin client.

Exemple : un client qui utilise Azure chargerait `instructions/common/azure-standards.md`.

## Client Override

Un client peut **surcharger** une instruction commune en créant sa propre version dans `clients/<client-key>/instructions/`.

Exemple :
- `instructions/common/azure-standards.md` (par défaut)
- `clients/sbm/instructions/azure-standards-sbm.md` (SBM-specific override)
