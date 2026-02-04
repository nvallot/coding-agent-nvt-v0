# 🔄 Architecture des Agents & Instructions - Diagramme Explicatif

## 1. FLUX GLOBAL: De l'Exigence à la Production

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          WORKFLOW COMPLET                                   │
└─────────────────────────────────────────────────────────────────────────────┘

                            STAKEHOLDER
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │   1️⃣  BUSINESS ANALYST │ (@ba)
                    │    ANALYSE MÉTIER      │
                    └────────────────────────┘
                                 │
                    Lire: .github/clients/active-client.json
                    Charger: .github/clients/{clientKey}/CLIENT.md
                    Utiliser: .github/instructions/agents/business-analyst.md
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │  📄 BRD + User Stories │
                    │  Success Criteria      │
                    └────────────────────────┘
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │   2️⃣  ARCHITECTE       │ (@architecte)
                    │    CONCEPTION SYSTÈME   │
                    └────────────────────────┘
                                 │
                    Lire: .github/instructions/README.md (INDEX)
                    Puis: .github/instructions/agents/architecte.md
                    Puis: .github/instructions/domains/azure-patterns.md
                    Puis: .github/instructions/domains/iac-terraform.md
                    Client: .github/clients/{key}/instructions/architecture.md
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │  TAD + Diagrammes C4   │
                    │  ADRs + Terraform      │
                    │  Estimation coûts      │
                    └────────────────────────┘
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │   3️⃣  DÉVELOPPEUR      │ (@dev)
                    │    IMPLÉMENTATION CODE  │
                    └────────────────────────┘
                                 │
                    Lire: .github/instructions/agents/developpeur.md
                    Puis: .github/instructions/domains/data-architecture.md
                    Puis: .github/instructions/domains/iac-terraform.md
                    Puis: .github/instructions/domains/testing.md
                    Client: .github/clients/{key}/instructions/naming.md
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │  Code + Tests          │
                    │  Pipelines ADF         │
                    │  Terraform modules     │
                    └────────────────────────┘
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │   4️⃣  REVIEWER         │ (@reviewer)
                    │    REVUE DE CODE        │
                    └────────────────────────┘
                                 │
                    Lire: .github/instructions/agents/reviewer.md
                    Puis: .github/instructions/base/conventions.md
                    Puis: .github/instructions/domains/testing.md
                    Architecture: TAD + ADRs
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │  Review Report         │
                    │  Blockers / Important  │
                    │  / Mineur              │
                    └────────────────────────┘
                                 │
                                 ↓
                    ┌────────────────────────┐
                    │    🚀 PRODUCTION       │
                    │    CODE MERGED ✅       │
                    └────────────────────────┘
```

---

## 2. STRUCTURE DES INSTRUCTIONS: Comment Tout S'Interconnecte

```
┌──────────────────────────────────────────────────────────────────────┐
│                    .github/instructions/                             │
└──────────────────────────────────────────────────────────────────────┘

                          README.md (INDEX)
                    ↓        ↓        ↓        ↓
            ┌───────┴────────┴────────┴────────┴─────┐
            │                                        │
            │  "Lire moi EN PREMIER!"                │
            │  - Navigation guide                    │
            │  - Workflow pour chaque agent          │
            │  - Où aller pour quoi                  │
            └────────────────────────────────────────┘

        ┌───────────────────────────────────────────────────────────┐
        │                    Hiérarchie de Priorité                  │
        │  (Ce que charge chaque agent, par ordre)                   │
        └───────────────────────────────────────────────────────────┘

    1️⃣  CLIENT-SPECIFIC (Priorité MAXIMUM)
        .github/clients/{clientKey}/
            ├── CLIENT.md                ← Contexte client
            └── instructions/
                ├── naming.md           ← Conventions client (ex: SBM prefix)
                ├── architecture.md     ← Patterns client
                └── security.md         ← Standards sécurité client

    2️⃣  AGENT-SPECIFIC CORE
        .github/instructions/agents/
            ├── architecte.md           ← Quoi faire (Architecte)
            ├── business-analyst.md     ← Quoi faire (BA)
            ├── developpeur.md          ← Quoi faire (Dev)
            └── reviewer.md             ← Quoi faire (Reviewer)

    3️⃣  DOMAINES TECHNIQUES DÉTAILLÉS
        .github/instructions/domains/
            ├── azure-patterns.md       ← Medallion, Lambda, CDC, etc.
            ├── data-architecture.md    ← Modélisation, gouvernance, qualité
            ├── iac-terraform.md        ← Structure, modules, variables
            └── testing.md              ← Unit, integration, E2E

    4️⃣  DIRECTIVES COMMUNES (Priorité MINIMUM)
        .github/instructions/base/
            ├── agent-roles.md          ← Rôles & workflow global
            ├── conventions.md          ← Standards techniques globaux
            └── azure-reference.md      ← Services Azure + patterns

    5️⃣  CONTRATS DE LIVRABLES
        .github/instructions/contracts/
            └── artefacts.md            ← Format exact BRD / TAD / ADR / PR

        ┌─────────────────────────────────────────┐
        │  EXEMPLE: Agent Développeur charge...  │
        ├─────────────────────────────────────────┤
        │ 1. .github/clients/active-client.json   │ ← Quel client
        │ 2. .github/clients/{key}/CLIENT.md      │ ← Contexte client
        │ 3. .github/clients/{key}/instructions/  │ ← Conventions client
        │    └─ naming.md                         │
        │ 4. .github/instructions/agents/         │ ← Instructions dev core
        │    └─ developpeur.md                    │
        │ 5. .github/instructions/domains/        │ ← Détails techniques
        │    ├─ data-architecture.md              │
        │    ├─ iac-terraform.md                  │
        │    └─ testing.md                        │
        │ 6. .github/instructions/base/           │ ← Standards globaux
        │    └─ conventions.md                    │
        │ 7. .github/instructions/contracts/      │ ← Contrats
        │    └─ artefacts.md (PR template)        │
        └─────────────────────────────────────────┘
```

---

## 3. COMMENT CHAQUE AGENT NAVIGUE

### 🏗️ Architecte Flow

```
┌─────────────────────────────────────────────────────────┐
│  ARCHITECTE: "Je dois concevoir l'architecture"         │
└─────────────────────────────────────────────────────────┘

1️⃣  IDENTIFIER CLIENT
    Lire: .github/clients/active-client.json
    ↓
    "Le client actif est: NADIA"

2️⃣  CHARGER CONTEXTE CLIENT
    Lire: .github/clients/nadia/CLIENT.md
    ↓
    Comprendre: secteur, contraintes, précédents projets

3️⃣  CHARGER CONVENTIONS CLIENT
    Lire: .github/clients/nadia/instructions/architecture.md
    ↓
    "NADIA préfère Medallion architecture"
    "NADIA utilise prefix 'nadia' pour ressources"

4️⃣  CHARGER INSTRUCTIONS CORE
    Lire: .github/instructions/agents/architecte.md
    ↓
    "Je dois produire: TAD + Diagrammes + ADRs + Terraform"

5️⃣  CHARGER DÉTAILS TECHNIQUES
    Lire: .github/instructions/README.md (pour naviguer)
    ↓
    Lire: .github/instructions/domains/azure-patterns.md
    ↓
    Lire: .github/instructions/domains/iac-terraform.md
    ↓
    "Voici comment structurer Terraform, les modules à créer"

6️⃣  CHARGER CONTRATS DE LIVRABLES
    Lire: .github/instructions/contracts/artefacts.md
    ↓
    "TAD doit avoir sections X, Y, Z"
    "ADR doit suivre ce format"

7️⃣  CHARGER DIRECTIVES COMMUNES
    Lire: .github/instructions/base/conventions.md
    ↓
    "Standards globaux: DRY, SOLID, logging structuré"

✅  PRODUIT FINALISÉ
    - TAD complet
    - Diagrammes C4
    - ADRs
    - Terraform + variables.tf + modules/
    - Estimation coûts
```

### 💻 Développeur Flow

```
┌─────────────────────────────────────────────────────────┐
│  DÉVELOPPEUR: "Je dois implémenter l'architecture"      │
└─────────────────────────────────────────────────────────┘

1️⃣  IDENTIFIER CLIENT & CHARGER CONTEXTE
    (Même que Architecte, steps 1-3)

2️⃣  CHARGER TAD DE L'ARCHITECTE
    Fichier: docs/tad-{project}.md
    ↓
    Comprendre: architecture, composants, décisions

3️⃣  CHARGER INSTRUCTIONS CORE
    Lire: .github/instructions/agents/developpeur.md
    ↓
    "Je dois produire: Code + Tests + Terraform + Doc"

4️⃣  CHARGER CONVENTIONS DE NOMMAGE CLIENT
    Lire: .github/clients/{key}/instructions/naming.md
    ↓
    "C# = PascalCase"
    "Python = snake_case"
    "Ressources Azure = {prefix}-{type}-{env}"

5️⃣  CHARGER PATTERNS DATA
    Lire: .github/instructions/domains/data-architecture.md
    ↓
    "Bronze/Silver/Gold layer structure"
    "Validation rules"
    "Data quality tests"

6️⃣  CHARGER PATTERNS TERRAFORM
    Lire: .github/instructions/domains/iac-terraform.md
    ↓
    "Module structure"
    "Variable management"
    "State backend"

7️⃣  CHARGER STRATÉGIES TESTS
    Lire: .github/instructions/domains/testing.md
    ↓
    "Unit tests >80% coverage"
    "Integration tests for workflows"
    "Test fixtures & mocking"

8️⃣  CHARGER CONTRATS PR
    Lire: .github/instructions/contracts/artefacts.md
    ↓
    "PR template format"
    "Required checklist items"

✅  CODE PRÊT À REVIEW
    - Code production (Python/C#/SQL)
    - Tests (>80%)
    - Terraform modules
    - Documentation
    - PR avec checklist complète
```

---

## 4. ACTIVATION DES AGENTS: Triggers & Routing

```
┌──────────────────────────────────────────────────────────┐
│  Comment GitHub Copilot sait QUEL AGENT activer         │
└──────────────────────────────────────────────────────────┘

Configuration: .github/config/copilot-config.json

                    Fichier Ouvert
                            │
                ┌───────────┴───────────┐
                │                       │
                ↓                       ↓
        [Vérifie pattern]     [Vérifie domaine]
                │                       │
                ↓                       ↓
        ┌─────────────────┐  ┌────────────────┐
        │ requirements/** │  │  "requirements"│
        │   → @ba         │  │   → @ba        │
        └─────────────────┘  └────────────────┘

ROUTING COMPLET:
────────────────────────────────────────────────────────────

Architecture/Conception:
  ├─ docs/**                    → @architecte
  ├─ Deployment/**              → @architecte
  ├─ architecture/**            → @architecte
  └─ *.tf (Terraform)          → @architecte (+ @dev)

Métier/Exigences:
  ├─ requirements/**            → @ba
  ├─ specifications/**          → @ba
  └─ docs/brd*                 → @ba

Développement:
  ├─ src/**                     → @dev
  ├─ Functions/**               → @dev
  ├─ Development/**             → @dev
  ├─ *.cs (C#)                 → @dev
  ├─ *.py (Python)             → @dev
  └─ *.sql (SQL)               → @dev

Revue de Code:
  ├─ pull_requests/**           → @reviewer
  ├─ *.cs (in PR)              → @reviewer
  ├─ *.py (in PR)              → @reviewer
  └─ *.sql (in PR)             → @reviewer
```

---

## 5. HANDOFFS: Comment Les Agents Se Passent le Relais

```
┌────────────────────────────────────────────────────────┐
│  Format Standard pour Passer à l'Agent Suivant        │
└────────────────────────────────────────────────────────┘

        BA → ARCHITECTE
        ┌────────────────────────────────┐
        │ ## 🔄 Handoff vers @architecte │
        │                                │
        │ **BRD Complet**: [Résumé 2-3]  │
        │                                │
        │ **Livrables**:                 │
        │ ✅ BRD détaillé                │
        │ ✅ Data Mapping complet        │
        │ ✅ User Stories + AC           │
        │ ✅ Success Criteria mesurables │
        │                                │
        │ **Attentes**:                  │
        │ 1. Proposer architecture       │
        │ 2. Documenter décisions (ADR)  │
        │ 3. Fournir Terraform           │
        │ 4. Estimer coûts               │
        │                                │
        │ **Questions en suspens**:      │
        │ - [Q1]                         │
        │ - [Q2]                         │
        └────────────────────────────────┘

        ARCHITECTE → DEV
        ┌────────────────────────────────┐
        │ ## 🔄 Handoff vers @dev        │
        │                                │
        │ **Architecture**: [Résumé 2-3] │
        │                                │
        │ **Livrables fournis**:         │
        │ ✅ TAD + Diagrammes C4          │
        │ ✅ ADRs (décisions majeures)   │
        │ ✅ Terraform (prêt déployer)   │
        │ ✅ Estimation coûts            │
        │                                │
        │ **Attentes**:                  │
        │ 1. Implémenter pipelines ADF   │
        │ 2. Code Databricks + tests     │
        │ 3. Scripts SQL Synapse         │
        │ 4. Terraform modules           │
        │ 5. Documentation code          │
        │                                │
        │ **Priorités**:                 │
        │ 1. [Composant critique 1]      │
        │ 2. [Composant critique 2]      │
        │                                │
        │ **Constraints**:                │
        │ - Naming: [Référence]          │
        │ - Secrets: Key Vault           │
        │ - Logging: App Insights        │
        │ - Git: feature/* → main        │
        │                                │
        │ **Points d'attention**:        │
        │ - ⚠️ [Point sensible]          │
        └────────────────────────────────┘

        DEV → REVIEWER
        ┌────────────────────────────────┐
        │ ## PR: [Feature Title]         │
        │                                │
        │ **Implémentation**: [Résumé]   │
        │                                │
        │ **Checklist**:                 │
        │ ✅ Tests unitaires >80%        │
        │ ✅ Documentation code          │
        │ ✅ Logging structuré           │
        │ ✅ Error handling              │
        │ ✅ Pas de secrets              │
        │ ✅ Conventions respectées      │
        │                                │
        │ **Points à reviewer**:         │
        │ - Performance des queries      │
        │ - Sécurité (secrets, input)    │
        │ - Gestion erreurs              │
        └────────────────────────────────┘
```

---

## 6. CONFIGURATION GLOBALE: copilot-config.json

```json
{
  "version": "2.0.0",
  "agents": [
    {
      "id": "ba",
      "name": "Business Analyst",
      "triggers": ["**/requirements/**", "**/docs/brd*"],
      "handoffs": ["architecte"]
    },
    {
      "id": "architecte",
      "name": "Architecte",
      "triggers": ["**/docs/**", "**/Deployment/**", "**/*.tf"],
      "handoffs": ["developpeur", "business-analyst"]
    },
    {
      "id": "developpeur",
      "name": "Developpeur",
      "triggers": ["**/(src|Functions)/**", "**/*.cs", "**/*.py"],
      "handoffs": ["reviewer", "architecte"]
    },
    {
      "id": "reviewer",
      "name": "Reviewer",
      "triggers": ["**/pull_requests/**", "**/*.cs"],
      "handoffs": ["developpeur"]
    }
  ],
  "routing": {
    "requirements": "business-analyst",
    "architecture": "architecte",
    "code": "developpeur",
    "review": "reviewer"
  }
}
```

---

## 7. RÉSUMÉ VISUEL: Les 7 Couches de Contexte

```
┌─────────────────────────────────────────────┐
│           CLIENT-SPECIFIC (Client.md)       │  ← Couche 7 (Priorité MAXIMUM)
├─────────────────────────────────────────────┤
│   CLIENT INSTRUCTIONS (naming, arch, sec)   │  ← Couche 6
├─────────────────────────────────────────────┤
│     AGENT INSTRUCTIONS (core + produits)    │  ← Couche 5
├─────────────────────────────────────────────┤
│   DOMAIN DETAILS (patterns, data, IaC)      │  ← Couche 4
├─────────────────────────────────────────────┤
│   BASE CONVENTIONS (DRY, SOLID, logging)    │  ← Couche 3
├─────────────────────────────────────────────┤
│      CONTRACTS (BRD, TAD, ADR format)       │  ← Couche 2
├─────────────────────────────────────────────┤
│   GITHUB COPILOT BASE (non modifiable)      │  ← Couche 1 (Priorité MINIMUM)
└─────────────────────────────────────────────┘

Chaque agent charge CE QUI LUI EST UTILE
dans cet ordre de priorité.
```

---

## 🎯 Quick Reference: "Où aller pour quoi?"

| Question | Réponse |
|----------|---------|
| Quel client? | `.github/clients/active-client.json` |
| Contexte client? | `.github/clients/{key}/CLIENT.md` |
| Qu'est-ce que j'ai à faire? (Agent) | `.github/instructions/agents/{agent}.md` |
| Comment structurer Terraform? | `.github/instructions/domains/iac-terraform.md` |
| Comment faire les tests? | `.github/instructions/domains/testing.md` |
| Conventions nommage client? | `.github/clients/{key}/instructions/naming.md` |
| Format BRD/TAD/ADR? | `.github/instructions/contracts/artefacts.md` |
| Standards globaux? | `.github/instructions/base/conventions.md` |
| Services Azure? | `.github/instructions/base/azure-reference.md` |
| Index complet? | `.github/instructions/README.md` |

---

**Version**: 2.0.0  
**Format**: Diagramme explicatif + Guide navigation  
**Date**: 2026-02-04  
**Audience**: Tous les agents + utilisateurs
