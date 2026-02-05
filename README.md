# 🤖 GitHub Copilot Agents v1 - Architecture Multi-Client

> **Système d'agents GitHub Copilot pour consulting en intégration de données Azure**

## 📋 Vue d'ensemble

Plateforme multi-agents professionnelle pour le consulting en intégration de données sur Microsoft Azure, conçue pour gérer plusieurs clients avec des profils et contextes spécifiques.

### 🎯 Agents Disponibles

| Agent | Pattern `applyTo` | Rôle |
|-------|-------------------|------|
| **@ba** | `**/requirements/**,**/specifications/**,**/docs/**` | Business Analyst |
| **@architecte** | `**/docs/**,**/Deployment/**,**/architecture/**` | Solution Architect |
| **@dev** | `**/src/**,**/Functions/**,**/Development/**,**/*.cs,**/*.py,**/*.sql,**/*.tf` | Developer |
| **@reviewer** | `**/*.cs,**/*.py,**/*.sql` | Code Reviewer |

### 🏗️ Architecture

```
agent-nvt-v1/
├── .github/
│   ├── copilot-instructions.md     # Repository-wide instructions
│   ├── agents/                     # Agent definitions
│   │   ├── architecte.md
│   │   ├── business-analyst.md
│   │   ├── developpeur.md
│   │   └── reviewer.md
│   ├── instructions/               # Path-specific instructions
│   │   ├── *.instructions.md       # Per-agent instructions
│   │   ├── base/                   # Common directives
│   │   ├── domains/                # Technical specialties
│   │   └── contracts/              # Deliverable contracts
│   ├── clients/                    # Client configurations
│   │   ├── active-client.json      # Current active client
│   │   └── {clientKey}/            # Client-specific folder
│   ├── prompts/                    # Prompt templates (.prompt)
│   ├── knowledge/                  # Knowledge base
│   └── tools/                      # Utility scripts
├── docs/                           # Documentation
├── AGENTS.md                       # Agents overview
└── README.md                       # This file
```

## 🚀 Démarrage Rapide

### 1. Vérifier le client actif

```bash
cat .github/clients/active-client.json
```

### 2. Utiliser les Agents

```bash
@ba "Analyser les exigences pour [projet]"
@architecte "Concevoir l'architecture pour [projet]"
@dev "Implémenter [composant]"
@reviewer "Faire la revue de code pour PR #[n]"
```

### 3. Utiliser les Prompt Files

```bash
# Référencer un prompt file
#prompt:brd
#prompt:tad
#prompt:diagram
```

## 📊 Structure des Instructions

Selon la documentation GitHub Copilot:

1. **Repository-wide**: `.github/copilot-instructions.md`
2. **Path-specific**: `.github/instructions/*.instructions.md`
3. **Agent definitions**: `.github/agents/*.md`

## 🎨 Fonctionnalités Clés

### ✅ Multi-Client
- Configuration par client
- Instructions spécifiques
- Base de connaissance dédiée

### ✅ Workflow Complet
- **BA**: Exigences et cahier des charges
- **Architecte**: Conception et diagrammes
- **Développeur**: Implémentation
- **Reviewer**: Qualité et conformité

### ✅ Spécialisé Azure Data
- Patterns ETL/ELT optimisés
- Azure Data Factory, Synapse, Fabric
- Databricks, Event Hubs, Stream Analytics
- Terraform pour IaC

## 🔧 Configuration Client

Chaque client suit cette structure:

```
.github/clients/{clientKey}/
├── CLIENT.md              # Contexte client
└── instructions/          # Instructions spécifiques
```

## 🤝 Workflow Agents

```
BA → Architecte → Développeur → Reviewer
     Exigences    Architecture   Code        Quality
```

## 📚 Ressources

- [AGENTS.md](AGENTS.md) - Documentation détaillée des agents
- [GitHub Copilot Docs](https://docs.github.com/en/copilot)
- [Azure Well-Architected Framework](https://learn.microsoft.com/azure/architecture/framework/)

---

**Version**: 2.0.0  
**Last updated**: 2026-02-05  
**Auteur**: Nicolas VALLOT
