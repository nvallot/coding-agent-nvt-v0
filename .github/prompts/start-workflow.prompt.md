---
description: Démarrer une nouvelle demande avec le Business Analyst
---

@ba /analyze

## Nouvelle Demande
#file:.github/context/current-request.md

---

## Instructions

1. **Analyse** la demande initiale
2. **Clarifie** les besoins métier avec des questions si nécessaire
3. **Identifie** les exigences fonctionnelles et non-fonctionnelles
4. **Crée** les user stories avec critères d'acceptation
5. **Sauvegarde** le BRD dans `.github/context/brd-output.md`

## Format de Sortie Attendu

Remplace le contenu de `.github/context/brd-output.md` avec :
- Résumé exécutif
- Exigences fonctionnelles (FR-XXX)
- Exigences non-fonctionnelles (NFR-XXX)
- User Stories (US-XXX)
- Risques et hypothèses

## Prochain Agent

Une fois terminé, utiliser : `#prompt:handoff-to-archi`
