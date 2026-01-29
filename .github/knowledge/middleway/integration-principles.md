# Integration Principles – Middleway

## IP-001 – Fiabilité
Tout échange inter-système doit être résilient aux pannes transitoires.

## IP-002 – Asynchronisme privilégié
Les échanges asynchrones via Service Bus sont préférés lorsque la cohérence
immédiate n'est pas requise.

## IP-003 – Idempotence
Toute opération d'écriture doit être idempotente.

## IP-004 – Traçabilité
Chaque message ou transaction doit être traçable de bout en bout
via un identifiant unique (CorrelationId).

## IP-005 – Format pivot
Utiliser des formats pivot pour les échanges multi-tiers
(voir conventions Service Bus : `pivot-{businessentity}`).

## IP-006 – Versionning des APIs
Les APIs exposées via APIM doivent être versionnées.
