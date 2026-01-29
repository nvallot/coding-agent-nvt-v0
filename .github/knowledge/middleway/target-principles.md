# Target Architecture Principles – Middleway

## TAP-001 – Découplage
Les systèmes doivent être découplés via des mécanismes intermédiaires
(Service Bus, Event Grid).

## TAP-002 – Observabilité native
Les flux doivent exposer des métriques et logs exploitables via
Application Insights et Log Analytics.

## TAP-003 – Scalabilité horizontale
La solution doit pouvoir monter en charge sans refonte majeure.

## TAP-004 – Infrastructure as Code
Toutes les ressources Azure doivent être déployées via Bicep ou Terraform.

## TAP-005 – Respect des conventions de nommage
Toutes les ressources doivent respecter les conventions de nommage
définies dans `naming-conventions.md`.
