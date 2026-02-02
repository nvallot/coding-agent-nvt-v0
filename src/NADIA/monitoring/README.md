# Monitoring Dashboard - ISP NADIA SPA

Ce dossier contient le **dashboard standard ISP** obligatoire pour le flux IT 05a (NADIA → SPA).

## Fichiers

- Template Workbook: [src/NADIA/monitoring/ISP-NADIA-SPA-Dashboard.workbook.json](src/NADIA/monitoring/ISP-NADIA-SPA-Dashboard.workbook.json)

## Import du dashboard (Azure Portal)

1. Ouvrir Application Insights `SBWE1-ISP-{ENV}-API-01`
2. Aller dans **Workbooks** → **New** → **Advanced Editor**
3. Coller le JSON du template et **Apply**
4. Sauvegarder sous le nom **ISP - NADIA SPA - Monitoring Dashboard**

## KPIs couverts

- Exécutions FAP-65 (dernière exécution + total)
- Dernières exécutions détaillées
- Erreurs clés FAP-57 (PKM Not Found, Dataverse Error)
- Exceptions récentes FAP-65/FAP-57
- Upserts Dataverse

## Personnalisation (optionnel)

- Remplacer `fallbackResourceIds` par l’ID de l’Application Insights cible
- Ajuster les plages temporelles (par défaut 24h / 7j)
- Ajouter des widgets spécifiques si besoin métier
