# Incident Handling – Middleway

## Détection
Les incidents sont détectés via :
- Alertes Azure Monitor
- Dashboards Application Insights
- Remontées utilisateurs

## Analyse

### Étapes
1. Identifier le flux concerné
2. Récupérer le CorrelationId
3. Analyser les logs dans Application Insights
4. Identifier la cause racine

### Outils
- Application Insights : `{env}-appi-mw`
- Log Analytics : `{env}-log-mw`
- Kusto queries

## Résolution

### Actions
1. Appliquer une reprise automatique si possible (rejeu DLQ)
2. Corriger la cause si identifiée
3. Escalader si nécessaire

### Post-mortem
- Documenter l'incident
- Identifier les améliorations
- Mettre à jour les ADR si nécessaire
