---
type: client-instructions
clientKey: sbm
---

# Instructions Client SBM Offshore

> Ces instructions sont **chargées manuellement** par les agents quand `clientKey=sbm` dans `active-client.json`.
>
> ⚠️ **Important**: Pas de `applyTo` ici ! Le chargement est conditionnel au clientKey.

## 📁 Fichiers

| Fichier | Description | Statut |
|---------|-------------|--------|
| `README.md` | Ce fichier (overview) | ✅ Actif |
| `naming.md` | Conventions de nommage Azure/Code | ⏳ À créer |
| `architecture.md` | Patterns architecture imposés | ⏳ À créer |
| `security.md` | Règles sécurité spécifiques | ⏳ À créer |

## 🔄 Hiérarchie de Chargement

```
1. Agent lit active-client.json → clientKey=sbm
2. Agent charge instructions/clients/sbm/ ← CE DOSSIER
3. Agent charge knowledge/clients/sbm/
4. Agent charge clients/sbm/CLIENT.md
```

## 🎯 Mécanisme de Chargement

**Pas de `applyTo`** - Chargement conditionnel basé sur:
```json
// .github/clients/active-client.json
{ "clientKey": "sbm", "name": "SBM Offshore" }
```

Les agents doivent:
1. Lire `active-client.json` en premier
2. Si `clientKey === "sbm"` → charger ce dossier
3. Applicable à **TOUS** les projets SBM (NADIA, Supplier Portal, et futurs projets)

## 📚 Knowledge Associé

Le knowledge SBM est dans : `.github/knowledge/clients/sbm/`
- `glossary.md` - Terminologie SBM (Lucy, PKM, ISP, etc.)

## 🔗 Références

- **Profil Client**: `.github/clients/sbm/CLIENT.md`
- **TODO**: `.github/clients/sbm/TODO-CONVENTIONS-SBM.md`
