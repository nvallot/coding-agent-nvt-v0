# Architecture Decision Records – SBM

## ADR-001 – IaC Terraform
**Décision** : Déploiement via Terraform.
**Justification** : Reproductibilité et gouvernance.

## ADR-002 – Pipeline en 2 phases
**Décision** : Infra d’abord (Terraform), puis Functions.
**Justification** : Éviter les déploiements applicatifs sans infra.

## ADR-003 – Naming standardisé
**Décision** : Respect strict des conventions de nommage SBM.
**Justification** : Cohérence et automatisation.
