# Default Client Configuration

## Client Overview

This is the default client configuration used when no specific client is active. It contains baseline standards and practices for Azure data integration projects.

## Project Context

- **Client Name**: Default / Internal
- **Industry**: Not specified
- **Project Type**: Azure Data Integration & Analytics
- **Timeline**: Ongoing
- **Team Size**: Variable

## Technical Stack

### Cloud Platform
- **Primary**: Microsoft Azure (99% of projects)
- **Regions**: West Europe (primary), North Europe (secondary)

### Programming Languages
- **Primary**: C# (.NET 8+)
- **Secondary**: Python 3.11+
- **Scripting**: PowerShell 7+

### Data Integration
- **Orchestration**: Azure Data Factory, Synapse Pipelines
- **Processing**: Azure Databricks, Synapse Spark
- **Storage**: Azure Data Lake Storage Gen2, Blob Storage
- **Streaming**: Event Hubs, Stream Analytics
- **Governance**: Microsoft Purview

### Infrastructure as Code
- **Preferred**: Terraform
- **Alternative**: Bicep, ARM Templates
- **State Management**: Azure Storage Account with state locking

### Development Tools
- **IDE**: Visual Studio 2022, VS Code
- **Version Control**: Git (GitHub or Azure DevOps)
- **CI/CD**: GitHub Actions or Azure DevOps Pipelines

## Azure Services Usage

### Compute
- Azure Functions (event-driven processing)
- Container Apps (microservices)
- App Service (web applications)

### Data & Analytics
- Azure Data Factory (ETL/ELT orchestration)
- Azure Databricks (advanced analytics, ML)
- Synapse Analytics (data warehousing)
- Azure SQL Database (relational data)

### Integration & Messaging
- Service Bus (enterprise messaging)
- Event Grid (event routing)
- Event Hubs (streaming ingestion)

### Security & Identity
- Azure AD / Entra ID (identity)
- Managed Identity (service authentication)
- Key Vault (secrets management)

### Monitoring & Management
- Application Insights (APM)
- Log Analytics (centralized logging)
- Azure Monitor (metrics and alerts)

## Coding Standards

### General Principles
- Follow SOLID principles
- Write self-documenting code
- Implement comprehensive error handling
- Use dependency injection
- Write unit tests (>80% coverage)

### C# Standards
- Follow Microsoft C# Coding Conventions
- Use nullable reference types
- Async/await for I/O operations
- XML documentation comments for public APIs
- Use latest C# language features appropriately

### Python Standards
- Follow PEP 8 style guide
- Type hints for all function signatures
- Docstrings for all public functions
- Use virtual environments
- pytest for testing

### Terraform Standards
- Follow Azure CAF naming conventions
- Use modules for reusability
- Remote state in Azure Storage
- Tag all resources (Owner, Environment, CostCenter, Application)
- Variables with descriptions and validation

## Data Pipeline Standards

### Azure Data Factory / Synapse
- **Parameterization**: All configurations parameterized
- **Naming**: `pl_[source]_to_[destination]_[frequency]`
- **Retry Logic**: 3 retries with 30s interval
- **Logging**: run_id, source, destination, row_counts, duration
- **Authentication**: Managed Identity only
- **Idempotency**: Design for safe retries

### Databricks
- Use Delta Lake format
- Implement checkpointing for streaming
- Partition large tables appropriately
- Use cluster pools for cost optimization
- Store notebooks in Git

## Security Requirements

### Authentication & Authorization
- Use Azure AD / Entra ID for user authentication
- Use Managed Identity for service-to-service authentication
- Implement RBAC for resource access
- Never store credentials in code or config files

### Data Protection
- Encrypt sensitive data at rest and in transit
- Use Private Endpoints for data services
- Implement column-level encryption for PII
- Follow GDPR requirements for data handling

### Network Security
- Use Virtual Networks and subnets
- Implement Network Security Groups
- Use Private Endpoints for storage accounts
- Enable Azure Firewall for centralized security

## CI/CD Pipeline

### Build Pipeline
1. Code checkout
2. Restore dependencies
3. Build (compilation)
4. Run unit tests
5. Code quality analysis (SonarQube)
6. Security scanning
7. Publish artifacts

### Deployment Pipeline
1. Infrastructure deployment (Terraform)
2. Application deployment
3. Database migrations
4. Integration tests
5. Smoke tests
6. Health checks

### Environments
- **Development**: Auto-deploy from `develop` branch
- **Staging**: Auto-deploy from `staging` branch
- **Production**: Manual approval required

## Naming Conventions

### Azure Resources
Follow Azure Cloud Adoption Framework (CAF) naming conventions:
- Resource Group: `rg-[project]-[environment]-[region]-[instance]`
- Storage Account: `st[project][environment][region][instance]`
- Key Vault: `kv-[project]-[environment]-[region]-[instance]`
- Data Factory: `adf-[project]-[environment]-[region]-[instance]`

### Code
- **C# Classes**: PascalCase (e.g., `OrderService`)
- **C# Methods**: PascalCase (e.g., `GetOrderById`)
- **C# Private Fields**: _camelCase (e.g., `_orderRepository`)
- **Python Functions**: snake_case (e.g., `get_order_by_id`)
- **Python Classes**: PascalCase (e.g., `OrderService`)
- **Constants**: UPPER_CASE (e.g., `MAX_RETRIES`)

## Documentation Requirements

### Code Documentation
- XML comments for all public APIs (C#)
- Docstrings for all public functions (Python)
- README.md in each repository
- Architecture Decision Records (ADR) for major decisions

### Project Documentation
- Business Requirements Document (BRD)
- Technical Architecture Document (TAD)
- API Documentation (OpenAPI/Swagger)
- Deployment Guide
- Runbooks for operations

## Quality Standards

### Code Quality
- No critical SonarQube issues
- Code coverage > 80%
- Cyclomatic complexity < 10
- No code duplication > 5%

### Testing
- Unit tests for all business logic
- Integration tests for critical paths
- End-to-end tests for key scenarios
- Performance tests for high-load components

### Performance
- API response time < 200ms (p95)
- Page load time < 2 seconds
- Database queries optimized (no N+1)
- Efficient data pipeline execution

## Compliance & Governance

### Data Governance
- Data catalog in Microsoft Purview
- Data lineage tracking
- Data classification (public, internal, confidential, restricted)
- Data retention policies

### Regulatory Compliance
- GDPR compliance for EU data
- Industry-specific regulations as applicable
- Regular security audits
- Incident response procedures

## Support & Escalation

### Issue Priority
- **P0 - Critical**: Production down, data loss, security breach
- **P1 - High**: Major functionality impaired
- **P2 - Medium**: Minor functionality impaired
- **P3 - Low**: Cosmetic issues, feature requests

### Escalation Path
1. Development Team
2. Tech Lead
3. Solution Architect
4. Client Engagement Manager

## Change Management

### Change Types
- **Standard**: Pre-approved, low risk (e.g., config changes)
- **Normal**: Requires CAB approval (e.g., code deployments)
- **Emergency**: Fast-track approval for critical fixes

### Change Process
1. Submit change request
2. Impact assessment
3. Approval (CAB for normal changes)
4. Implementation
5. Validation
6. Documentation
7. Review

## Notes

This is the default configuration. Create client-specific configurations in `.github/clients/[client-name]/` to override these defaults for specific client projects.

To switch to a client-specific configuration, use the client manager script:
```bash
./github/tools/client-manager.sh activate [client-name]
```

Or update `.github/clients/active-client.json` manually.
