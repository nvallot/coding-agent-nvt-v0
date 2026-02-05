# 🔄 WORKFLOW STATE - {FLUX_NAME}

> **Projet**: {PROJECT_NAME}  
> **Flux**: {FLUX_NAME}  
> **Créé le**: {DATE}  
> **Dernière mise à jour**: {DATE}

---

## 📊 État du Workflow

| # | Phase | Agent | Status | Fichier | Date |
|---|-------|-------|--------|---------|------|
| 0 | Context Initial | - | ⏳ Pending | `00-context.md` | - |
| 1 | Requirements | @ba | ⏳ Pending | `01-requirements.md` | - |
| 2 | Architecture | @architecte | ⏳ Pending | `02-architecture.md` | - |
| 3 | Implementation | @dev | ⏳ Pending | `03-implementation.md` | - |
| 4 | Review | @reviewer | ⏳ Pending | `04-review.md` | - |

### Légende des Status
- ⏳ **Pending**: Non commencé
- 🔄 **In Progress**: En cours
- ✅ **Complete**: Terminé
- ❌ **Blocked**: Bloqué (voir notes)
- 🔁 **Revision**: Corrections demandées

---

## 📝 Notes par Phase

### Phase 0: Context Initial
```
Status: ⏳ Pending
Agent: -
Notes: [À compléter]
```

### Phase 1: Requirements (@ba)
```
Status: ⏳ Pending
Agent: @ba
Notes: [À compléter]
```

### Phase 2: Architecture (@architecte)
```
Status: ⏳ Pending
Agent: @architecte
Notes: [À compléter]
Diagrammes: [ ] C4 Context [ ] C4 Container [ ] Data Flow
```

### Phase 3: Implementation (@dev)
```
Status: ⏳ Pending
Agent: @dev
Notes: [À compléter]
Tests: [ ] Unit [ ] Integration
Coverage: ___%
```

### Phase 4: Review (@reviewer)
```
Status: ⏳ Pending
Agent: @reviewer
Notes: [À compléter]
Verdict: [ ] Approve [ ] Request Changes [ ] Block
```

---

## 🔗 Liens Rapides

| Artefact | Lien |
|----------|------|
| Context | [00-context.md](./00-context.md) |
| Requirements | [01-requirements.md](./01-requirements.md) |
| Architecture | [02-architecture.md](./02-architecture.md) |
| Implementation | [03-implementation.md](./03-implementation.md) |
| Review | [04-review.md](./04-review.md) |
| Handoff | [HANDOFF.md](./HANDOFF.md) |

---

## 📈 Historique des Handoffs

| Date | De | Vers | Action | Notes |
|------|-----|------|--------|-------|
| {DATE} | - | @ba | Start | Workflow initié |

---

## ✅ Checklist Finale (avant merge)

- [ ] Tous les fichiers (00 à 04) créés
- [ ] HANDOFF.md à jour
- [ ] Diagrammes draw.io exportés (PNG + SVG)
- [ ] Tests passent (>80% coverage)
- [ ] Review approuvée
- [ ] Documentation complète

---

## 📚 Validation du Workflow

Pour valider l'état actuel du workflow, exécuter :

```powershell
.\.github\tools\validate-workflow.ps1 -DocsPath "{DOCS_PATH}" -Flux "{FLUX_NAME}" -Phase "all"
```

---

*Ce fichier est automatiquement mis à jour par les agents à chaque handoff.*
