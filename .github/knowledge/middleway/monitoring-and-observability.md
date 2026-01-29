# Monitoring & Observability – Middleway

## Application Insights
- Ressource : `{env}-appi-mw`
- Logs structurés obligatoires
- Identifiant de corrélation (CorrelationId) obligatoire

## Log Analytics Workspace
- Ressource : `{env}-log-mw`
- Rétention configurée selon les SLA

## Métriques clés
- Volumétrie des messages
- Taux d'erreur par flux
- Latence de traitement
- Durée des exécutions Function App

## Alertes
- Basées sur seuils et tendances
- Configurées dans Azure Monitor
- Notification via Action Groups

## Dashboards
- Un dashboard par activité
- Métriques temps réel
