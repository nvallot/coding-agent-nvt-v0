# System Index

Complete overview of the GitHub Copilot Multi-Agent System for Azure Data Integration.

## 📁 Directory Structure

```
nvt-agents-final/
├── README.md                          # Main documentation
├── START-HERE.md                      # Quick start guide
├── INDEX.md                           # This file
│
└── .github/
    ├── agents/                        # 4 specialized agents
    │   ├── business-analyst.md        # @ba - Requirements & BRDs
    │   ├── solution-architect.md      # @archi - Architecture & TADs
    │   ├── developer.md               # @dev - Implementation
    │   └── code-reviewer.md           # @reviewer - Quality & Security
    │
    ├── clients/                       # Multi-client support
    │   ├── active-client.json         # Currently active client
    │   ├── default/                   # Default configuration
    │   │   └── CLIENT.md              # Default client context
    │   └── template/                  # Template for new clients
    │       └── CLIENT.md              # Client configuration template
    │
    ├── config/
    │   └── copilot-config.json        # GitHub Copilot configuration
    │
    ├── instructions/                  # Auto-applied coding standards (11 files)
    │   ├── azure-instructions.md      # General Azure guidelines
    │   ├── backend.md                 # Backend development
    │   ├── conventions.md             # General conventions
    │   ├── data-integration.md        # ⭐ Data pipeline standards
    │   ├── docs.md                    # Documentation standards
    │   ├── frontend.md                # Frontend development
    │   ├── infrastructure.md          # Infrastructure patterns
    │   ├── path-based-instructions.md # Path-specific rules
    │   ├── terraform.md               # ⭐ Terraform/IaC standards
    │   ├── tests.md                   # Testing standards
    │   └── workflows.md               # CI/CD standards
    │
    ├── knowledge/                     # Knowledge base
    │   ├── architecture/
    │   │   └── patterns.md            # Architecture patterns
    │   ├── azure/
    │   │   ├── best-practices.md      # ⭐ Azure best practices
    │   │   └── services.md            # ⭐ Azure services guide
    │   └── best-practices/            # (empty - ready for expansion)
    │
    ├── prompts/                       # Reusable prompt templates
    │   ├── brd.prompt.md              # Business Requirements Document
    │   ├── code-review.prompt.md      # Code review template
    │   └── tad.prompt.md              # Technical Architecture Document
    │
    ├── skills/                        # 7 specialized skills
    │   ├── code-implementation/
    │   │   └── SKILL.md               # How to implement code
    │   ├── code-review/
    │   │   └── SKILL.md               # How to review code
    │   ├── debugging/
    │   │   └── SKILL.md               # Debugging techniques
    │   ├── diagram-creation/
    │   │   └── SKILL.md               # Creating diagrams
    │   ├── security-audit/
    │   │   └── SKILL.md               # Security assessment
    │   ├── solution-design/
    │   │   └── SKILL.md               # ⭐ Complete solution design
    │   └── testing/
    │       └── SKILL.md               # Testing strategies
    │
    ├── tools/                         # Management scripts
    │   └── client-manager.sh          # ⭐ Client management tool
    │
    └── workflows/                     # CI/CD pipelines
        ├── adf-deploy.yml             # ⭐ Azure Data Factory CI/CD
        ├── dotnet-build-deploy.yml    # ⭐ .NET application CI/CD
        └── terraform-deploy.yml       # ⭐ Terraform infrastructure CI/CD
```

## 🤖 Agents Quick Reference

| Agent | Mention | Primary Role | Key Commands |
|-------|---------|--------------|--------------|
| Business Analyst | `@ba` | Requirements, BRDs | `/analyze`, `/requirements`, `/user-stories`, `/brd` |
| Solution Architect | `@archi` | Architecture, TADs | `/design`, `/diagram`, `/tad`, `/adr` |
| Developer | `@dev` | Implementation | `/implement`, `/refactor`, `/test`, `/pipeline`, `/iac` |
| Code Reviewer | `@reviewer` | Quality, Security | `/review`, `/security`, `/performance`, `/quality` |

## 📝 Instructions (Auto-Applied)

| Instruction File | Applied To | Purpose |
|-----------------|------------|---------|
| `azure-instructions.md` | General | Azure services overview and patterns |
| `data-integration.md` | `{data,etl,pipelines,adf,synapse}/**` | ⭐ Data pipeline best practices |
| `terraform.md` | `**/*.tf` | ⭐ IaC standards & Azure CAF naming |
| `backend.md` | Backend code | API and service development |
| `frontend.md` | Frontend code | UI development |
| `tests.md` | Test files | Testing standards |
| `workflows.md` | CI/CD files | Pipeline conventions |
| `docs.md` | Documentation | Documentation standards |
| `conventions.md` | All code | General coding conventions |
| `infrastructure.md` | Infrastructure | Infrastructure patterns |
| `path-based-instructions.md` | Various paths | Path-specific rules |

## 🎓 Skills Overview

| Skill | Purpose | Use Case |
|-------|---------|----------|
| **solution-design** | Complete solution methodology | When designing architectures |
| **diagram-creation** | C4, UML, infrastructure diagrams | When visualizing architecture |
| **code-implementation** | Implementation best practices | When writing code |
| **code-review** | Review methodology | When reviewing code |
| **testing** | Testing strategies | When writing tests |
| **debugging** | Debugging techniques | When fixing bugs |
| **security-audit** | Security assessment | When checking security |

## 📚 Knowledge Base

| Category | Files | Content |
|----------|-------|---------|
| **Azure** | `services.md`, `best-practices.md` | ⭐ Comprehensive Azure guide |
| **Architecture** | `patterns.md` | Architecture patterns & ADRs |
| **Best Practices** | (ready for expansion) | Industry standards |

## 🔄 CI/CD Workflows

### 1. .NET Build & Deploy (`dotnet-build-deploy.yml`)
- **Triggers**: Push/PR to main/develop (C# files)
- **Jobs**: Build → Test → Code Quality → Security → Deploy (dev/staging/prod)
- **Features**: xUnit testing, code coverage, Trivy security scan, Azure deployment

### 2. Terraform Deploy (`terraform-deploy.yml`)
- **Triggers**: Push/PR to main/develop (Terraform files)
- **Jobs**: Validate → Security → Cost Estimate → Plan → Apply (per environment)
- **Features**: TFLint, tfsec, Checkov, Infracost, multi-environment

### 3. Azure Data Factory Deploy (`adf-deploy.yml`)
- **Triggers**: Push/PR to main/develop (ADF files)
- **Jobs**: Validate → Security → Deploy (dev/staging/prod)
- **Features**: JSON validation, naming convention checks, trigger management

## 🏢 Multi-Client Management

### Client Manager Commands

| Command | Purpose |
|---------|---------|
| `./client-manager.sh list` | List all clients |
| `./client-manager.sh show` | Show active client |
| `./client-manager.sh activate <name>` | Switch to a client |
| `./client-manager.sh create <name>` | Create new client |
| `./client-manager.sh validate <name>` | Validate client config |

### Client Structure

Each client folder contains:
- `CLIENT.md` - Comprehensive client context and configuration
- `config/` - Client-specific configuration files (optional)

## 🎯 Common Workflows

### Workflow 1: New Data Integration Project
```
1. @ba /analyze "[business need]"
2. @archi /design "[technical approach]"
3. @dev /pipeline "[implementation details]"
4. @reviewer /review "[check implementation]"
```

### Workflow 2: Infrastructure as Code
```
1. @archi Design Terraform modules for [infrastructure]
2. @dev Create Terraform following architecture
3. @reviewer /iac-review "Verify compliance and security"
```

### Workflow 3: Using Prompts
```
1. #prompt:brd project_name="[name]"
2. #prompt:tad architecture_type="[type]"
3. #prompt:code-review file_path="[path]"
```

## 📊 Statistics

- **Total Files**: 38
- **Total Directories**: 25
- **Agents**: 4
- **Skills**: 7
- **Instructions**: 11
- **Workflows**: 3
- **Knowledge Files**: 3
- **Prompts**: 3

## ✅ Compliance Checklist

This system is compliant with:
- ✅ GitHub Copilot official documentation
- ✅ Microsoft VS Code agent specifications
- ✅ No frontmatter YAML in agent files (common mistake fixed)
- ✅ Correct `.prompt.md` extension (not `.prompt`)
- ✅ Instructions have proper `applyTo` frontmatter
- ✅ MCP configuration separated from agents
- ✅ Azure Cloud Adoption Framework (CAF) naming
- ✅ Terraform best practices
- ✅ Data pipeline best practices

## 🔗 Quick Links

- [Main README](README.md) - Full system documentation
- [Start Here](START-HERE.md) - Quick start guide (5 min)
- [Business Analyst](.github/agents/business-analyst.md) - @ba documentation
- [Solution Architect](.github/agents/solution-architect.md) - @archi documentation
- [Developer](.github/agents/developer.md) - @dev documentation
- [Code Reviewer](.github/agents/code-reviewer.md) - @reviewer documentation
- [Default Client](.github/clients/default/CLIENT.md) - Default configuration
- [Client Template](.github/clients/template/CLIENT.md) - New client template
- [Azure Best Practices](.github/knowledge/azure/best-practices.md) - Azure knowledge
- [Solution Design Skill](.github/skills/solution-design/SKILL.md) - Design methodology

## 📞 Support

For issues or questions:
1. Check [START-HERE.md](START-HERE.md) for common issues
2. Review relevant agent documentation
3. Ask the agents themselves (they can explain their capabilities)
4. Contact the architecture team

---

**Version**: 1.0.0  
**Last Updated**: 2026-02-05  
**Maintained By**: Architecture Team
