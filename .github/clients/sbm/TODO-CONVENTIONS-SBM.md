# TODO: Intégration Conventions SBM Offshore

> **Statut**: ⏳ En attente du fichier de normes SBM
> **Date création**: 2026-02-05
> **Dernière mise à jour**: 2026-02-05

## 📋 Contexte

Ce fichier sera utilisé pour intégrer les conventions et normes spécifiques de SBM Offshore
une fois le fichier d'explication disponible (via MCP DevOps ou document externe).

## 🎯 Actions à effectuer quand le fichier sera disponible

### 1. Analyser le fichier source
- [ ] Identifier les conventions de nommage Azure
- [ ] Identifier les patterns d'architecture imposés
- [ ] Identifier les règles de sécurité spécifiques
- [ ] Identifier les tags obligatoires
- [ ] Identifier les conventions de code C#
- [ ] Identifier les conventions Terraform/Bicep

### 2. Créer les fichiers instructions
- [ ] Créer `instructions/naming.md` avec les conventions de nommage SBM
- [ ] Créer `instructions/architecture.md` avec les patterns SBM
- [ ] Créer `instructions/security.md` si règles spécifiques
- [ ] Mettre à jour `CLIENT.md` avec les nouvelles références

### 3. Valider la hiérarchie
- [ ] Vérifier que les instructions SBM surchargent bien les `default/`
- [ ] Tester avec chaque agent (@ba, @archi, @dev, @reviewer)
- [ ] Documenter les différences avec le client `default`

### 4. Mettre à jour la base de connaissance
- [ ] Ajouter glossaire SBM si nécessaire dans `knowledge/`
- [ ] Ajouter documentation domaine spécifique

## 📂 Structure cible

```
.github/clients/sbm/
├── CLIENT.md                    # ✅ Existant - À enrichir
├── TODO-CONVENTIONS-SBM.md      # Ce fichier - À supprimer après intégration
├── instructions/
│   ├── naming.md                # À créer
│   ├── architecture.md          # À créer
│   └── security.md              # À créer (si nécessaire)
└── knowledge/
    └── glossary.md              # À créer (si nécessaire)
```

## 🔗 Sources potentielles

- [ ] Fichier Word/PDF de normes SBM
- [ ] Azure DevOps Wiki SBM (via MCP DevOps)
- [ ] Documentation Confluence/SharePoint
- [ ] Repo de référence SBM

## 📝 Notes

_Ajouter ici les notes lors de la réception du fichier de normes._
