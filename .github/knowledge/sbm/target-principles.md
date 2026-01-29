# Target Architecture Principles – SBM

## TAP-001 – IaC obligatoire
Toute ressource Azure doit être déployée via Terraform.

## TAP-002 – Observabilité native
Logs + métriques dans Application Insights et Log Analytics.

## TAP-003 – Scalabilité
Scale horizontal pour Functions et Service Bus.

## TAP-004 – Sécurité par défaut
Secrets dans Key Vault, RBAC strict, MI privilégiée.

## TAP-005 – Standardisation
Conventions de nommage et versionning appliqués partout.
