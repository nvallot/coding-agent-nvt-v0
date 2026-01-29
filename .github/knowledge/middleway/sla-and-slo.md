# SLA & SLO – Middleway

## SLA (Service Level Agreement)

| Métrique | Cible |
|----------|-------|
| Disponibilité globale | 99.9% |
| Temps de réponse API (P95) | < 500ms |
| Temps de traitement message | < 5s |

## SLO (Service Level Objectives)

### Par type de flux
| Type | Latence cible | Taux d'erreur max |
|------|---------------|-------------------|
| Temps réel | < 1s | 0.1% |
| Near real-time | < 5s | 0.5% |
| Batch | < 1h | 1% |

### Monitoring
- Métriques exposées dans Application Insights
- Alertes configurées sur dépassement des SLO
- Revue mensuelle des performances
