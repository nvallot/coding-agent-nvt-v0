# Multi-Agent System for Azure Data Integration

A professional GitHub Copilot agent system designed for Azure cloud consulting, specializing in data integration and analytics projects.

## 🎯 Overview

This repository contains a sophisticated multi-agent system that assists with end-to-end Azure data integration projects. It's specifically designed for consulting environments serving multiple clients with a focus on:

- **Cloud Platform**: Microsoft Azure (99% of projects)
- **Primary Languages**: C# (.NET 8+), Python 3.11+
- **Infrastructure**: Terraform, Bicep, ARM Templates
- **Data Stack**: Azure Data Factory, Synapse, Databricks, ADLS Gen2
- **Multi-Client Support**: Isolated configurations per client

## 🤖 The Four Specialized Agents

### @ba - Business Analyst
Analyzes business requirements, creates BRDs, defines user stories, and identifies business risks.

**Use for**: Requirements gathering, BRD creation, business logic clarification

### @archi - Solution Architect
Designs technical architectures, creates TADs, makes technology decisions, and produces architectural diagrams.

**Use for**: Architecture design, technology selection, TAD creation, ADR documentation

### @dev - Developer
Implements solutions in C#, Python, Terraform, and Azure services following best practices and architecture specifications.

**Use for**: Code implementation, data pipelines, IaC templates, unit testing

### @reviewer - Code Reviewer
Reviews code for quality, security, performance, and compliance, ensuring adherence to standards.

**Use for**: Code reviews, security audits, performance optimization, quality assurance

## 🏗️ Architecture

```
.github/
├── agents/                    # Agent definitions (Markdown)
│   ├── business-analyst.md
│   ├── solution-architect.md
│   ├── developer.md
│   └── code-reviewer.md
├── clients/                   # Multi-client support
│   ├── active-client.json     # Currently active client
│   ├── default/               # Default configuration
│   │   ├── CLIENT.md
│   │   └── config/
│   └── template/              # Template for new clients
│       ├── CLIENT.md
│       └── config/
├── config/
│   └── copilot-config.json    # GitHub Copilot configuration
├── instructions/              # Auto-applied coding standards
│   ├── csharp.instructions.md
│   ├── python.instructions.md
│   ├── terraform.md
│   ├── data-integration.md
│   └── [10+ specialized files]
├── knowledge/                 # Knowledge base
│   ├── azure/                 # Azure services & best practices
│   ├── architecture/          # Architecture patterns
│   └── best-practices/        # Industry standards
├── prompts/                   # Reusable prompt templates
│   ├── brd.prompt.md
│   ├── tad.prompt.md
│   └── code-review.prompt.md
├── skills/                    # Specialized capabilities
│   ├── solution-design/
│   ├── diagram-creation/
│   ├── code-implementation/
│   ├── code-review/
│   ├── testing/
│   ├── debugging/
│   └── security-audit/
├── tools/                     # Management scripts
│   └── client-manager.sh      # Client switching tool
└── workflows/                 # CI/CD pipelines
    ├── dotnet-build-deploy.yml
    ├── terraform-deploy.yml
    └── adf-deploy.yml
```

## 🚀 Quick Start

### 1. Installation

Clone this repository into your project's `.github` folder:

```bash
# In your project root
git clone <this-repo> .github/agents-config
cp -r .github/agents-config/* .github/
rm -rf .github/agents-config
```

### 2. Verify Setup

```bash
# Check that GitHub Copilot recognizes the agents
# Open VS Code and type @ in the Copilot chat
# You should see: @ba, @archi, @dev, @reviewer
```

### 3. Configure Your Client

```bash
# Option 1: Use the default configuration
# (Already active out of the box)

# Option 2: Create a new client configuration
./.github/tools/client-manager.sh create my-client-name
# Edit .github/clients/my-client-name/CLIENT.md
./.github/tools/client-manager.sh activate my-client-name
```

### 4. Start Using Agents

Open GitHub Copilot Chat in VS Code and try:

```
@ba Analyze the need to migrate our SQL Server database to Azure SQL
@archi Design an architecture for real-time data synchronization with Azure Event Hubs
@dev Implement a Terraform module for Azure Data Factory with ADLS Gen2
@reviewer Review this data pipeline for security and performance issues
```

## 💡 Usage Examples

### Example 1: New Data Integration Project

```
Step 1: Business Analysis
@ba /analyze "We need to integrate sales data from SAP into Power BI for real-time dashboards"

Step 2: Architecture Design
@archi /design "Real-time ETL pipeline from SAP to Power BI via Azure"

Step 3: Implementation
@dev /implement "Create ADF pipeline with incremental load and data quality checks"

Step 4: Code Review
@reviewer /review "Check the pipeline implementation for best practices"
```

### Example 2: Infrastructure as Code

```
@archi Design Terraform modules for a complete Azure data platform
@dev Create Terraform code following the architecture design
@reviewer /iac-review "Verify security, naming conventions, and cost optimization"
```

### Example 3: Using Prompt Templates

```
# Generate a BRD
#prompt:brd project_name="Customer 360 Analytics Platform"

# Generate a TAD
#prompt:tad architecture_type="Real-time Data Integration"

# Perform code review
#prompt:code-review file_path="./src/DataPipeline.cs"
```

## 📋 Instructions (Auto-Applied)

Instructions are automatically applied based on file type:

| File Type | Instruction File | Applied To |
|-----------|-----------------|-----------|
| C# | `csharp.instructions.md` | `**/*.cs` |
| Python | `python.instructions.md` | `**/*.py` |
| Terraform | `terraform.md` | `**/*.tf` |
| Data Pipelines | `data-integration.md` | `{data,etl,pipelines}/**` |

These enforce:
- Coding standards and conventions
- Security best practices
- Performance optimization guidelines
- Azure-specific patterns

## 🏢 Multi-Client Management

### Listing Clients

```bash
./.github/tools/client-manager.sh list
```

### Switching Clients

```bash
./.github/tools/client-manager.sh activate contoso
```

### Creating New Clients

```bash
# Create from template
./.github/tools/client-manager.sh create fabrikam

# Edit the configuration
nano .github/clients/fabrikam/CLIENT.md

# Activate
./.github/tools/client-manager.sh activate fabrikam
```

### Client-Specific Configurations

Each client can override:
- Azure services used
- Naming conventions
- Tagging standards
- Security requirements
- Compliance needs (GDPR, HIPAA, etc.)
- CI/CD workflows

## 🔧 CI/CD Pipelines

### Included Workflows

1. **`.github/workflows/dotnet-build-deploy.yml`**
   - Build, test, and deploy .NET applications
   - Code quality analysis
   - Security scanning
   - Multi-environment deployment (dev/staging/prod)

2. **`.github/workflows/terraform-deploy.yml`**
   - Terraform validation and formatting
   - Security scanning (tfsec, Checkov)
   - Cost estimation (Infracost)
   - Multi-environment IaC deployment

3. **`.github/workflows/adf-deploy.yml`**
   - Azure Data Factory artifact validation
   - Naming convention checks
   - Multi-environment pipeline deployment
   - Trigger management

### Required Secrets

Configure these in your GitHub repository settings:

```
# Azure Credentials
AZURE_CREDENTIALS_DEV
AZURE_CREDENTIALS_STAGING
AZURE_CREDENTIALS_PROD

# Terraform State
TF_STATE_RG_DEV
TF_STATE_SA_DEV
TF_STATE_RG_STAGING
TF_STATE_SA_STAGING
TF_STATE_RG_PROD
TF_STATE_SA_PROD

# Azure Data Factory
ADF_RG_DEV
ADF_NAME_DEV
ADF_RG_STAGING
ADF_NAME_STAGING
ADF_RG_PROD
ADF_NAME_PROD

# Optional: Cost estimation
INFRACOST_API_KEY
```

## 📚 Documentation

- **[START-HERE.md](docs/START-HERE.md)** - First steps and orientation
- **[GETTING-STARTED.md](docs/GETTING-STARTED.md)** - Detailed setup guide
- **[CLIENT-MANAGEMENT.md](docs/CLIENT-MANAGEMENT.md)** - Managing multiple clients
- **[AGENT-USAGE.md](docs/AGENT-USAGE.md)** - How to use each agent effectively
- **[TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)** - Common issues and solutions

## 🔒 Security & Compliance

### Security Features
- Managed Identity for Azure authentication (no credentials in code)
- Azure Key Vault integration for secrets
- Private Endpoints for data services
- Network security groups and firewalls
- Security scanning in CI/CD pipelines

### Compliance Support
- GDPR compliance patterns
- HIPAA-compliant architectures
- SOC 2 controls
- Audit logging
- Data classification

## 🎓 Best Practices

### Azure Data Integration
- Use Managed Identity for authentication
- Implement idempotent pipelines
- Prefer incremental loads with watermarks
- Enable diagnostic logging to Log Analytics
- Implement data quality checks
- Use parameterization (no hard-coded values)

### Infrastructure as Code
- Use Terraform modules for reusability
- Store state in Azure Storage with locking
- Tag all resources consistently
- Follow Azure CAF naming conventions
- Use workspaces or tfvars for environments

### Code Quality
- >80% unit test coverage
- SOLID principles
- Comprehensive error handling
- Structured logging with correlation IDs
- No secrets in code or configuration

## 🤝 Contributing

This is an internal consulting tool. To contribute:

1. Test changes in a feature branch
2. Create a pull request
3. Get review from the team
4. Update documentation
5. Merge to main

## 📝 License

Internal use only - Proprietary

## 🆘 Support

- **Documentation**: Check `/docs` folder
- **Issues**: Create GitHub issue
- **Questions**: Contact the architecture team

## 🔄 Version History

- **v1.0.0** (2026-02-05) - Initial release
  - Four specialized agents (BA, Archi, Dev, Reviewer)
  - Multi-client support
  - Complete Azure knowledge base
  - Seven specialized skills
  - Comprehensive instructions (11 files)
  - Three CI/CD workflows
  - Client management tooling

---

**Built for**: Azure Data Integration Consulting  
**Focus**: Data Factory, Synapse, Databricks, ADLS Gen2  
**Languages**: C#, Python, Terraform  
**Maintained by**: Architecture Team
