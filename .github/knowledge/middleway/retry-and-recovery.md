# Retry & Recovery – Middleway

## Retry Strategy

### Paramètres par défaut
- Nombre de tentatives : 3
- Backoff : exponentiel
- Délai initial : 1 seconde
- Délai max : 30 secondes

### Par type de ressource
| Ressource | Retry | Backoff |
|-----------|-------|---------|
| Service Bus | 3 | Exponentiel |
| HTTP calls | 3 | Exponentiel |
| Storage | 5 | Linéaire |

## Recovery

### Principes
- Rejouabilité des messages via Dead Letter Queue
- Pas de duplication fonctionnelle (idempotence)
- Alerting sur DLQ non vide

### Dead Letter Queue
- Monitoring obligatoire
- Procédure de reprise documentée
- Retention : 14 jours
