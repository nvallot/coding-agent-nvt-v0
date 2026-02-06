# Repository Custom Instructions for GitHub Copilot

## Project Overview

This repository contains a multi-agent system for GitHub Copilot, designed for Azure Data Integration consulting.

## Project Structure

```
agent-nvt-v1/
├── .github/
│   ├── copilot-instructions.md    # This file (repository-wide)
│   ├── agents/                    # Agent definitions (*.agent.md)
│   ├── instructions/              # Path-specific instructions
│   │   ├── *.instructions.md      # Per-agent instructions
│   │   ├── base/                  # Common directives
│   │   ├── domains/               # Technical specialties
│   │   └── contracts/             # Deliverable contracts
│   ├── clients/                   # Client configurations
│   │   ├── active-client.json     # Current active client
│   │   └── {clientKey}/           # Client-specific folder
│   ├── prompts/                   # Prompt templates (.prompt)
│   ├── knowledge/                 # Knowledge base
│   └── tools/                     # Utility scripts
└── docs/                          # Documentation
```

## Workflow

```
BA (Business Analyst) → ARCHI (Architect) → DEV (Developer) → REVIEWER
     Exigences            Architecture       Implementation    Code Review
```

## Available Agents

| Agent | Pattern | Purpose |
|-------|---------|---------|
| `@ba` | `**/requirements/**,**/specifications/**,**/docs/**` | Business requirements analysis |
| `@architecte` | `**/(docs\|Deployment\|architecture)/**` | Azure architecture design |
| `@dev` | `**/(src\|Functions\|Development)/**,**/*.cs,**/*.py,**/*.sql,**/*.tf` | Code implementation |
| `@reviewer` | `**/(pull_requests)/**,**/*.cs,**/*.py,**/*.sql` | Code review |

## How to Use

### 1. Check Active Client
```bash
cat .github/clients/active-client.json
```

### 2. Invoke an Agent
```bash
@ba "Analyze requirements for [project]"
@architecte "Design architecture for [project]"
@dev "Implement [component]"
@reviewer "Review code for PR #[n]"
```

## Coding Standards

### General
- Use clear, descriptive names
- Document public APIs
- Write tests for all new code
- Follow existing patterns in the codebase

### Azure Specific
- Use Managed Identity over connection strings
- Store secrets in Azure Key Vault
- Apply tags: Environment, Project, Owner, ManagedBy
- Follow Azure Well-Architected Framework

### Languages
- **C#**: PascalCase for public, camelCase for private, async/await patterns
- **Python**: PEP 8, type hints, docstrings
- **SQL**: UPPERCASE keywords, snake_case for objects
- **Terraform**: snake_case, modular structure

## Build & Test

```bash
# .NET projects
dotnet build
dotnet test

# Python projects
pip install -r requirements.txt
pytest

# Terraform
terraform init
terraform validate
terraform plan
```

## Important Notes

- Always load client context before starting work
- Reference architecture documents (TAD, ADRs) when implementing
- Use structured logging with CorrelationId
- No secrets in code - use Key Vault references
