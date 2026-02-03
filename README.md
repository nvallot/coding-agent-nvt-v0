# 🤖 GitHub Copilot Agents v1 - Architecture Multi-Client

> **Système d'agents GitHub Copilot pour consulting en intégration de données Azure**

## 📋 Vue d'ensemble

Plateforme multi-agents professionnelle pour le consulting en intégration de données sur Microsoft Azure, conçue pour gérer plusieurs clients avec des profils et contextes spécifiques.

### 🎯 Agents Disponibles

1. **@ba** (Business Analyst) - Analyse métier et exigences
2. **@archi** (Architecte) - Conception système et architecture
3. **@dev** (Développeur) - Implémentation et code
4. **@reviewer** (Reviewer) - Revue de code et qualité

### 🏗️ Architecture

```
agent-nvt-v1/
├── .github/
│   ├── agents/                    # 📝 Définitions des agents
│   │   ├── business-analyst.md
│   │   ├── architecte.md
│   │   ├── developpeur.md
│   │   └── reviewer.md
│   │
│   ├── clients/                   # 👥 Espaces clients
│   │   ├── active-client.json    # Client actif
│   │   ├── default/              # Client par défaut
│   │   └── [client-name]/        # Dossier client spécifique
│   │       ├── CLIENT.md         # Contexte client
│   │       ├── instructions/     # Instructions spécifiques
│   │       ├── knowledge/        # Base de connaissance
│   │       ├── config/           # Configuration client
│   │       └── data/             # Données client
│   │
│   ├── instructions/             # 📚 Instructions globales
│   │   ├── copilot-instructions.md  # Instructions principales
│   │   ├── AGENTS.base.md        # Base commune agents
│   │   ├── HIERARCHY.md          # Hiérarchie instructions
│   │   ├── azure/                # Spécifiques Azure
│   │   ├── common/               # Partagées
│   │   └── contracts/            # Contrats livrables
│   │
│   ├── skills/                   # 🎯 Compétences spécialisées
│   │   ├── diagram-creation/
│   │   ├── solution-design/
│   │   ├── code-implementation/
│   │   ├── code-review/
│   │   ├── security-audit/
│   │   ├── testing/
│   │   ├── data-integration/    # Nouveau: focus data
│   │   └── azure-deployment/    # Nouveau: déploiement
│   │
│   ├── knowledge/                # 📖 Base de connaissance globale
│   │   ├── azure/
│   │   │   ├── data-factory.md
│   │   │   ├── synapse.md
│   │   │   ├── databricks.md
│   │   │   └── fabric.md
│   │   ├── patterns/
│   │   │   ├── etl-patterns.md
│   │   │   ├── cdc-patterns.md
│   │   │   └── streaming-patterns.md
│   │   └── best-practices/
│   │       ├── iac-terraform.md
│   │       ├── security.md
│   │       └── observability.md
│   │
│   ├── prompts/                  # 📝 Templates réutilisables
│   │   ├── brainstorm.prompt
│   │   ├── tad.prompt           # Technical Architecture Document
│   │   ├── solution-design.prompt
│   │   ├── cost-estimation.prompt
│   │   ├── data-mapping.prompt  # Nouveau: mapping données
│   │   └── pipeline-design.prompt # Nouveau: pipelines
│   │
│   ├── config/                   # ⚙️ Configuration système
│   │   ├── copilot-config.json
│   │   ├── mcp-servers.json
│   │   └── client-template/     # Template nouveau client
│   │
│   └── tools/                    # 🔧 Outils et scripts
│       ├── client-manager.ps1   # Gestion clients
│       ├── validate-agents.ps1
│       └── deploy-config.ps1
│
├── docs/                         # 📚 Documentation
│   ├── GETTING-STARTED.md
│   ├── ARCHITECTURE.md
│   ├── CLIENT-MANAGEMENT.md
│   └── AGENT-USAGE.md
│
└── examples/                     # 💡 Exemples
    ├── client-setup/
    ├── prompts/
    └── workflows/
```

## 🚀 Démarrage Rapide

### 1. Activer un Client

```powershell
# Définir le client actif
.\\.github\\tools\\client-manager.ps1 -SetActive "client-name"
```

### 2. Utiliser les Agents

```markdown
# Analyse métier
@ba /analyze "Besoin de migration Dynamics 365 vers Power Platform"

# Architecture
@archi /design "Pipeline ETL avec Azure Data Factory"

# Développement
@dev /implement "Créer le pipeline de transformation"

# Revue
@reviewer /review "Vérifier la qualité du code"
```

### 3. Utiliser les Prompt Files

```markdown
# Brainstorming sur un sujet
#file:tad.prompt project_name="Migration CRM" project_description="..."
```

## 📊 Hiérarchie de Chargement

Selon le diagramme d'architecture fourni:

1. **Base GitHub Copilot** (non modifiable)
2. **Agent Instructions** (`.github/agents/[agent].md`)
3. **Path-based Instructions** (si workspace match)
4. **Client Instructions** (`.github/clients/[client]/instructions/`)
5. **Knowledge Chunks** (via RAG)
6. **Workspace Context** (fichiers ouverts)
7. **Tools Available** (Built-in + MCP + Custom)

## 🎨 Fonctionnalités Clés

### ✅ Multi-Client
- Configuration par client
- Instructions spécifiques
- Base de connaissance dédiée
- Isolation complète

### ✅ Workflow Complet
- **BA**: Exigences et cahier des charges
- **Architecte**: Conception et diagrammes
- **Développeur**: Implémentation
- **Reviewer**: Qualité et conformité

### ✅ Commandes Spécifiques

Chaque agent dispose de commandes `/command`:

**Business Analyst**:
- `/analyze` - Analyser un besoin
- `/requirements` - Extraire exigences
- `/risks` - Identifier risques

**Architecte**:
- `/design` - Concevoir architecture
- `/diagramme` - Créer diagrammes
- `/tad` - Générer TAD
- `/adr` - Architecture Decision Record

**Développeur**:
- `/implement` - Implémenter fonctionnalité
- `/refactor` - Refactoriser code
- `/test` - Créer tests

**Reviewer**:
- `/review` - Revue de code
- `/security` - Audit sécurité
- `/performance` - Analyse performance

### ✅ Spécialisé Azure Data

- Patterns ETL/ELT optimisés
- Azure Data Factory, Synapse, Fabric
- Databricks, Event Hubs, Stream Analytics
- Terraform pour IaC
- Bonnes pratiques gouvernance

## 🔧 Configuration

### Client Template

Chaque nouveau client suit cette structure:

```
.github/clients/[client-name]/
├── CLIENT.md              # Contexte et priorités
├── instructions/          # Instructions spécifiques
│   ├── naming.md
│   ├── security.md
│   └── architecture.md
├── knowledge/            # Docs spécifiques
│   ├── apis/
│   ├── schemas/
│   └── mapping/
├── config/               # Configuration
│   ├── azure-resources.json
│   ├── mcp.json
│   └── variables.env
└── data/                 # Données de référence
    ├── mappings/
    └── schemas/
```

## 📚 Documentation

- [Getting Started](docs/GETTING-STARTED.md) - Guide de démarrage
- [Architecture](docs/ARCHITECTURE.md) - Architecture détaillée
- [Client Management](docs/CLIENT-MANAGEMENT.md) - Gérer les clients
- [Agent Usage](docs/AGENT-USAGE.md) - Utiliser les agents

## 🤝 Workflow Agents

Les agents peuvent se passer la main via **handoffs**:

```
BA → Architecte → Développeur → Reviewer
↑                                    ↓
└────────── feedback loop ───────────┘
```

## 🛡️ Sécurité

- Secrets dans Key Vault uniquement
- Managed Identity pour authentification
- RBAC strict
- Audit et logging

## 📝 Changelog

### v1.0.0 (Initial)
- ✅ 4 agents (BA, Archi, Dev, Reviewer)
- ✅ Système multi-client
- ✅ Skills et Knowledge modulaires
- ✅ Prompt Files réutilisables
- ✅ Commandes spécifiques
- ✅ Focus Azure Data Integration

## 🎯 Prochaines Évolutions

- [ ] Agent DevOps spécialisé
- [ ] Templates Terraform avancés
- [ ] Intégration CI/CD
- [ ] Dashboard de métriques
- [ ] Agent Testing automatisé

---

**Licence**: Propriétaire - Usage interne uniquement  
**Auteur**: Nicolas VALLOT  
**Contact**: [votre-email]
