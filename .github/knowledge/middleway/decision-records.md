# Architecture Decision Records – Middleway

## ADR-001 – Gestion des erreurs
**Décision** : Utiliser des mécanismes de retry contrôlés avec backoff exponentiel.
**Justification** : Éviter les reprises manuelles et gérer les erreurs transitoires.
**Conséquences** : Configuration systématique des retry policies.

## ADR-002 – Mode d'échange
**Décision** : Privilégier les échanges événementiels via Service Bus.
**Justification** : Réduire le couplage entre systèmes.
**Conséquences** : Utilisation de topics/queues avec conventions de nommage MW.

## ADR-003 – Conventions de nommage
**Décision** : Appliquer strictement les conventions de nommage Middleway.
**Justification** : Cohérence, maintenabilité, automatisation.
**Conséquences** : Validation automatique lors des déploiements.

## ADR-004 – Observabilité centralisée
**Décision** : Centraliser les logs et métriques dans Application Insights.
**Justification** : Faciliter le debugging et le monitoring.
**Conséquences** : CorrelationId obligatoire sur tous les messages.

## ADR-005 – Secrets management
**Décision** : Stocker tous les secrets dans Azure Key Vault.
**Justification** : Sécurité et rotation facilitée.
**Conséquences** : Utilisation de Managed Identity pour l'accès.
