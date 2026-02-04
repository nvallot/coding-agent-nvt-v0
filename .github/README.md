# 🤖 GitHub Copilot Agents Configuration

Configuration et instructions pour les agents GitHub Copilot utilisés dans ce workspace.

## 📋 Structure Globale

```
.github/
├── README.md (ce fichier)
├── agents/                         # Définition des agents
│   ├── architecte.md              # Solution Architect Azure
│   ├── business-analyst.md        # Business Analyst Expert
│   ├── developpeur.md             # Developer Azure Expert
│   └── reviewer.md                # Code Reviewer
├── config/
│   └── copilot-config.json        # Configuration agents + routing
├── instructions/                  # Instructions détaillées (modulaires)
│   ├── README.md                  # Index & guide navigation
│   ├── base/                      # Directives communes
│   │   ├── agent-roles.md         # Rôles & workflow
│   │   ├── conventions.md         # Standards techniques
│   │   └── azure-reference.md     # Services Azure
│   ├── agents/                    # Instructions par agent (core)
│   │   ├── architecte.md
│   │   ├── business-analyst.md
│   │   ├── developpeur.md
│   │   └── reviewer.md
│   ├── domains/                   # Spécialités techniques détaillées
│   │   ├── azure-patterns.md      # Medallion, Lambda, CDC
│   │   ├── data-architecture.md   # Modélisation, gouvernance
│   │   ├── iac-terraform.md       # Infrastructure as Code
│   │   └── testing.md             # Stratégies tests
│   └── contracts/                 # Contrats de livrables
│       └── artefacts.md           # Format BRD, TAD, ADR
├── clients/                       # Configuration par client
│   ├── active-client.json         # Client actif
│   ├── default/
│   │   ├── CLIENT.md              # Profil client
│   │   ├── instructions/          # Client-specific conventions
│   │   └── knowledge/             # Client-specific knowledge
│   └── sbm/                       # Exemple client SBM
│       ├── CLIENT.md
│       ├── instructions/
│       └── knowledge/
├── knowledge/                     # Base de connaissances
│   └── azure/                     # Documentation Azure
├── skills/                        # Skills spécialisées
│   └── diagram-creation/
├── tools/                         # Utilitaires
│   └── client-manager.ps1         # Script gestion clients
└── prompts/                       # Prompt files
    └── tad.prompt                 # Prompt TAD
```

## 🎯 Flux de Travail Standard

```
┌─────────────────────────────────────────────────────────────┐
│                    WORKFLOW AGENTS                          │
└─────────────────────────────────────────────────────────────┘

   BA (Business Analyst)
        │
        │ (BRD + User Stories)
        ↓
   ARCHI (Architecte)
        │
        │ (TAD + Terraform + ADRs)
        ↓
   DEV (Développeur)
        │
        │ (Code + Tests + Documentation)
        ↓
   REVIEWER (Reviewer)
        │
        │ (Code Review)
        ↓
   PR MERGED → PRODUCTION
```

### 1️⃣ Phase Analyse (@ba)
**Input**: Besoins métier  
**Produit**: BRD, Data Mapping, User Stories  
**Instructions**: `.github/instructions/agents/business-analyst.md`

```bash
@ba "Analyser les exigences pour [projet]"
```

### 2️⃣ Phase Conception (@architecte)
**Input**: BRD + User Stories  
**Produit**: TAD, Diagrammes, ADRs, Terraform  
**Instructions**: `.github/instructions/agents/architecte.md`

```bash
@architecte "Concevoir l'architecture pour [projet]"
```

### 3️⃣ Phase Développement (@dev)
**Input**: TAD + Architecture  
**Produit**: Code, Tests, Pipelines  
**Instructions**: `.github/instructions/agents/developpeur.md`

```bash
@dev "Implémenter [composant] selon [architecture]"
```

### 4️⃣ Phase Revue (@reviewer)
**Input**: Pull Request  
**Produit**: Code Review Report  
**Instructions**: `.github/instructions/agents/reviewer.md`

```bash
@reviewer "Faire la revue PR #[N]"
```

## 🤖 Agents Disponibles

### 🏗️ Architecte
- **Rôle**: Solution Architect senior Azure
- **Trigger**: Fichiers dans `/docs`, `/Deployment`, `/architecture`
- **Produit**: TAD, Diagrammes C4, ADRs, Terraform, Coûts
- **Handoffs**: → @dev (implémentation), → @ba (clarifications)

### 👤 Business Analyst
- **Rôle**: Expert analyse métier
- **Trigger**: Fichiers dans `/requirements`, `/specifications`
- **Produit**: BRD, Data Mapping, User Stories, Acceptance Criteria
- **Handoffs**: → @architecte (conception)

### 💻 Développeur
- **Rôle**: Developer expert Azure
- **Trigger**: Fichiers `.cs`, `.py`, `.sql`, `.tf`, `/src`, `/Functions`
- **Produit**: Code, Tests, Documentation, IaC
- **Handoffs**: → @reviewer (code review)

### 🔍 Reviewer
- **Rôle**: Expert revue de code
- **Trigger**: Pull Requests, fichiers `.cs`, `.py`, `.sql`
- **Produit**: Code Review Report, Security Audit, Recommandations
- **Handoffs**: → @dev (clarifications)

## 📚 Comment Utiliser

### Pour les Utilisateurs

Les agents s'activent **automatiquement** selon le fichier ouvert:

```
Si vous ouvrez:           Active:
─────────────────────────────────
requirements/*.md   →     @ba
docs/tad-*.md      →     @architecte
src/functions.py   →     @dev
pull_request       →     @reviewer
```

### Pour les Agents

**AVANT TOUTE ACTION** (workflow obligatoire):

1. **Charger le client actif**:
   ```bash
   Lire: .github/clients/active-client.json → obtenir clientKey
   Charger: .github/clients/{clientKey}/CLIENT.md
   ```

2. **Charger les instructions détaillées**:
   ```bash
   Lire: .github/instructions/README.md (index + guide)
   Puis: .github/instructions/agents/{agent}.md
   Puis: .github/instructions/domains/*.md (selon besoin)
   ```

3. **Appliquer les conventions client**:
   ```bash
   Lire: .github/clients/{clientKey}/instructions/
   - naming.md (conventions nommage)
   - architecture.md (patterns client)
   - security.md (standards sécurité)
   ```

## 🔑 Fichiers Clés

### `.github/clients/active-client.json`
Identifie le client actif. Les agents lisent ce fichier **en premier**.

```json
{
  "clientKey": "nadia",
  "name": "NADIA",
  "loadedAt": "2026-02-04T..."
}
```

### `.github/config/copilot-config.json`
Configuration globale: agents, triggers, handoffs, routing.

**Contient**:
- Liste des agents et leurs propriétés
- Triggers (fichiers qui activent chaque agent)
- Handoffs (passages de relais)
- Routing (quelle instruction pour quel domaine)

### `.github/instructions/README.md`
**INDEX ET GUIDE** pour naviguer dans les instructions.
**À lire en premier** par tous les agents.

### `.github/instructions/agents/{agent}.md`
Instructions **détaillées** pour chaque agent:
- Mission précise
- Workflow obligatoire
- Expertise
- Livrables attendus
- Handoffs

### `.github/instructions/domains/*.md`
Détails **techniques** par spécialité:
- `azure-patterns.md` → Medallion, Lambda, CDC
- `data-architecture.md` → Modélisation, gouvernance
- `iac-terraform.md` → Structure, modules, sécurité
- `testing.md` → Stratégies, couverture

### `.github/instructions/contracts/artefacts.md`
**Contrats de livrables**: Format exact pour BRD, TAD, ADR, PR.

### `.github/clients/{clientKey}/CLIENT.md`
Profil du **client spécifique**:
- Contexte métier
- Contraintes
- Précédents projets
- Contacts

### `.github/clients/{clientKey}/instructions/`
Conventions **spécifiques au client**:
- `naming.md` → Conventions nommage (ex: SBM prefix)
- `architecture.md` → Patterns préférés client
- `security.md` → Standards sécurité client

## 🚀 Commandes Utiles

### Invoquer un Agent
```bash
@ba "Description de la tâche..."
@architecte "Description..."
@dev "Description..."
@reviewer "Revue PR #123"
```

### Charger Contexte Client
```bash
# Avant une tâche
"Client: NADIA. [Description tâche]"

# L'agent chargera automatiquement:
# .github/clients/nadia/CLIENT.md
# .github/clients/nadia/instructions/
```

### Utiliser une Instruction Spécifique
```bash
"Selon .github/instructions/domains/iac-terraform.md, [question]"
```

## 📊 Architecture des Instructions

```
Hiérarchie de Priorité (décroissante):
1. Instructions client    (.github/clients/{key}/instructions/)
2. Instructions agent     (.github/instructions/agents/)
3. Domaines techniques   (.github/instructions/domains/)
4. Directives communes   (.github/instructions/base/)
```

**Principe**: Client-specific > Agent-specific > Technique commune > Global

## 🔄 Handoffs Entre Agents

Les agents se passent automatiquement le relais:

```
@ba (BRD) → @architecte (TAD) → @dev (Code) → @reviewer (Review)
```

**Format de Handoff**:
```markdown
## 🔄 Handoff vers @{agent}

**Contexte**: [Résumé du travail effectué]

**Livrables**:
- Livrable 1
- Livrable 2

**Attentes**:
- Ce qui est attendu de l'agent suivant

**Points en suspens**:
- Question 1
```

## 🔐 Sécurité & Best Practices

### Charger le Client OBLIGATOIREMENT
```bash
❌ WRONG: @dev "Implémenter la fonction X"
✅ RIGHT: "Client: NADIA. Implémenter la fonction X"
```

L'agent doit toujours charger `.github/clients/{key}/CLIENT.md` et les conventions client.

### Pas de Duplication
Utiliser les **références** plutôt que copier:
```bash
❌ "Python uses snake_case, C# uses PascalCase..."
✅ "Voir base/conventions.md"
```

### Concision des Fichiers
Chaque fichier instruction < 500 lignes.
Trop long → splitter en domaine + fichier.

## 📞 Troubleshooting

### Q: Agent ne charge pas le client
**A**: Vérifier `.github/clients/active-client.json` existe et est valide

### Q: Instruction non trouvée
**A**: Vérifier le pattern `applyTo` dans le frontmatter YAML

### Q: Client-specific mélangé au common
**A**: Déplacer vers `.github/clients/{key}/instructions/`

### Q: Fichier trop long, agent lent
**A**: Splitter selon hiérarchie (agent > domain > base)

## 🔄 Workflow Complet: Exemple

```
1. Client: NADIA
   Contexte: Migration ERP vers Azure Synapse

2. @ba /brd
   ↓ Produit: docs/brd-erp-migration.md

3. @architecte /tad
   ↓ Produit: docs/tad-erp-migration.md + Terraform

4. @dev /implement
   ↓ Produit: src/pipelines + tests

5. @reviewer "PR #42"
   ↓ Produit: Code Review Report

6. Merge → Production ✅
```

## 📚 Documentation

- `.github/instructions/README.md` - Guide complet des instructions
- `.github/clients/{clientKey}/CLIENT.md` - Profil client
- `.github/agents/` - Définition des agents
- `.github/config/copilot-config.json` - Configuration système

## 🔗 Ressources Externes

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Azure Architecture Center](https://learn.microsoft.com/azure/architecture/)
- [Well-Architected Framework](https://learn.microsoft.com/azure/architecture/framework/)

---

**Version**: 2.0.0  
**Format**: GitHub Copilot Path-specific Instructions  
**Dernière mise à jour**: 2026-02-04  
**Maintenance**: Chaque fichier modulaire <500 lignes, client-specific séparé
