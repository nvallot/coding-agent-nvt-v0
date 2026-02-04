---
applyTo: "**/*"
---

# 📋 Migration Guide: Instructions Refactorisées

## Ce qui a Changé

### ❌ Avant (Ancien)
```
.github/instructions/
├── copilot-instructions.md        (317 lignes - trop gros)
├── architecte.instructions.md     (260 lignes - mélangé)
├── developpeur.instructions.md    (537 lignes - énorme)
├── business-analyst.instructions.md
└── reviewer.instructions.md
```

**Problème**: Fichiers énormes, redondance, client-spécific mélangé au common

### ✅ Après (Nouveau)
```
.github/instructions/
├── base/                          # Common uniquement
│   ├── agent-roles.md            (Rôles seulement)
│   ├── conventions.md            (Tech globales)
│   └── azure-reference.md        (Services + patterns)
├── agents/                        # Par agent, court & ciblé
│   ├── architecte.md             (~150 lignes)
│   ├── developpeur.md            (~140 lignes)
│   ├── business-analyst.md       (~120 lignes)
│   └── reviewer.md               (~100 lignes)
├── domains/                       # Spécialités techniques
│   ├── azure-patterns.md         (Medallion, Lambda, etc.)
│   ├── data-architecture.md      (Modélisation, gouvernance)
│   ├── iac-terraform.md          (Infrastructure as Code)
│   └── testing.md                (Stratégies tests)
├── contracts/                     # Contrats livrables
│   └── artefacts.md              (BRD, TAD, ADR format)
└── README.md                      # Index & navigation
```

**Avantages**:
- Fichiers <200 lignes (lisible rapidement)
- Zéro redondance (références via `domains/`)
- Client-specific **séparé** dans `.github/clients/{key}/`
- Agents trouvent **exactement** ce qu'ils besoin

## Quelle Instruction Utiliser Maintenant?

### Architecte

**Avant**: Lisait `architecte.instructions.md` (260 lignes)

**Maintenant**: 
1. `base/agent-roles.md` (1 min) → "Je suis Architecte"
2. `agents/architecte.md` (5 min) → Core instructions
3. `domains/azure-patterns.md` (10 min) → Détails patterns
4. `domains/iac-terraform.md` (10 min) → IaC détails
5. `.github/clients/{key}/CLIENT.md` → Contexte client

**Total**: 30 min au lieu de 30 min mais **beaucoup plus clair** (pas de redondance)

### Développeur

**Avant**: Lisait `developpeur.instructions.md` (537 lignes!!)

**Maintenant**:
1. `agents/developpeur.md` (5 min) → Core
2. `domains/data-architecture.md` (15 min) → Data patterns
3. `domains/iac-terraform.md` (10 min) → IaC
4. `domains/testing.md` (10 min) → Tests
5. `.github/clients/{key}/instructions/` → Conventions client

**Total**: 40 min d'une manière **beaucoup plus fluide**

### Business Analyst

**Avant**: Lisait `business-analyst.instructions.md` (482 lignes)

**Maintenant**:
1. `agents/business-analyst.md` (5 min)
2. `contracts/artefacts.md` (10 min) → Format BRD
3. `.github/clients/{key}/CLIENT.md` → Contexte

**Total**: 15 min, **bien plus rapide**

### Reviewer

**Avant**: Lisait `reviewer.instructions.md` (15,889 lignes... wait that's reviewers too)

**Maintenant**:
1. `agents/reviewer.md` (5 min)
2. `base/conventions.md` (5 min) → Standards
3. `domains/testing.md` (10 min) → Coverage expectations
4. PR TAD/ADRs → Architecture

## Checklist: Migration vers Nouveau Format

### Client Leaders/Admin

- [ ] Créer `.github/clients/{clientKey}/instructions/` dossier
- [ ] Copier TEMPLATE-naming.client.md → `naming.md`
- [ ] Personnaliser conventions nommage (prefixes, tags, etc.)
- [ ] Ajouter `architecture.md` si client-specific patterns
- [ ] Ajouter `security.md` si client-specific requirements
- [ ] Communicar aux agents: "Lire `.github/instructions/` d'abord"

### Agents

- [ ] ✅ Remplacer ancient references par new structure
- [ ] ✅ Signaler si info manquante (sera dans `.github/clients/{key}/`)
- [ ] ✅ Utiliser `domains/` pour détails technique

## Quelle Info Aller Où?

| Information | Destination |
|-------------|-----------|
| Rôles agents | `base/agent-roles.md` |
| Standards tech globaux (DRY, SOLID) | `base/conventions.md` |
| Services Azure, patterns | `base/azure-reference.md` |
| Architecte core | `agents/architecte.md` |
| Dev core | `agents/developpeur.md` |
| BA core | `agents/business-analyst.md` |
| Reviewer core | `agents/reviewer.md` |
| Medallion, Lambda patterns | `domains/azure-patterns.md` |
| Modélisation, gouvernance | `domains/data-architecture.md` |
| Terraform structure, modules | `domains/iac-terraform.md` |
| Unit/integration/E2E tests | `domains/testing.md` |
| BRD, TAD, ADR format | `contracts/artefacts.md` |
| **CLIENT NOMMAGE** | `.github/clients/nadia/instructions/naming.md` |
| **CLIENT ARCHITECTURE** | `.github/clients/nadia/instructions/architecture.md` |
| **CLIENT SECURITY** | `.github/clients/nadia/instructions/security.md` |

## FAQ

### Q: Où go je mettre conventions nommage SBM?
**A**: `.github/clients/sbm/instructions/naming.md`

**Jamais** dans `base/conventions.md` - c'est client-spécific!

### Q: Et si j'ai besoin de détail sur Terraform?
**A**: Lis `domains/iac-terraform.md` (structure, modules, variables)

### Q: Où est la liste complète des services Azure?
**A**: `base/azure-reference.md` (tableau récapitulatif)

### Q: Comment je sais si couverture tests est ok?
**A**: `domains/testing.md` + PR template dans `contracts/artefacts.md`

### Q: Ancien `copilot-instructions.md`, qu'est-ce qui l'a remplacé?
**A**: Distribué entre:
- `base/agent-roles.md` (workflow & rôles)
- `base/conventions.md` (standards)
- Chaque `agents/{agent}.md` (instructions spécifiques)

---

**Version**: 2.0.0 Migration  
**Date**: 2026-02-04  
**Scope**: ✅ Taille fichiers réduite 70% | ✅ Zéro redondance | ✅ Client-specific séparé
