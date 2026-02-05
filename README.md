# 🤖 GitHub Copilot Multi-Agent System

> Système d'agents pour consulting Azure Data Integration (C# .NET 10, Terraform, Bicep)

## 🚀 Démarrage Rapide

```bash
# 1. Vérifier le client actif
cat .github/clients/active-client.json

# 2. Utiliser un agent
@ba "Analyser les exigences pour [projet]"
@architecte "Concevoir l'architecture pour [projet]"
@dev "Implémenter [composant]"
@reviewer "Faire la revue de code"
```

## 🎯 Agents

| Agent | Rôle | Livrables |
|-------|------|-----------|
| `@ba` | Business Analyst | BRD, User Stories, Data Mapping |
| `@architecte` | Solution Architect | TAD, ADRs, Diagrammes C4, IaC |
| `@dev` | Developer | Code C#, Azure Functions, Tests |
| `@reviewer` | Code Reviewer | Revue qualité, sécurité, perf |

**Workflow**: `@ba` → `@architecte` → `@dev` → `@reviewer`

## 📚 Documentation

| Document | Description |
|----------|-------------|
| [AGENTS.md](AGENTS.md) | Documentation complète des agents |
| [docs/GETTING-STARTED.md](docs/GETTING-STARTED.md) | Guide de démarrage |
| [.github/instructions/INDEX.md](.github/instructions/INDEX.md) | Index des instructions |

## 📁 Structure

```
.github/
├── agents/           # 4 agents (ba, architecte, dev, reviewer)
├── instructions/     # Instructions par contexte
│   ├── base/         # Règles universelles
│   ├── domains/      # C#, Bicep, Terraform, Testing...
│   └── contracts/    # Templates livrables
├── clients/          # Configuration multi-client
├── prompts/          # Prompt files réutilisables
├── skills/           # Skills spécialisés
└── knowledge/        # Base de connaissance Azure
```

## ⚡ Stack Technique

- **Code**: C# .NET 10, Azure Functions (Isolated Worker)
- **IaC**: Terraform, Bicep
- **Cloud**: Azure (ADF, Databricks, CosmosDB, Service Bus...)
- **CI/CD**: Azure DevOps

---

**Version**: 2.0.0 | **Updated**: 2026-02-05
