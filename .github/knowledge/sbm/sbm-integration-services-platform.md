# SBM Integration Services Platform (ISP) - Knowledge Base (v0.1)

> Source de synthèse. En cas de conflit, knowledge/sbm/ fait foi.

## Overview

The SBM Integration Services Platform (ISP) is an Azure-based integration platform designed for enterprise application integration. It serves as the central hub for connecting various enterprise systems including IFS, NEO, LUCY, Anaplan, and other business applications.

## Core Architecture

### Azure Core Resources

The ISP platform is built on a shared core architecture that includes the following Azure resources:

- **Key Vault**: Centralized secrets management
- **App Configuration**: Configuration management across environments
- **Application Insights**: Application performance monitoring
- **Log Analytics Workspace**: Centralized logging and monitoring
- **Service Bus**: Message queuing and pub/sub messaging
- **Data Factory**: Data integration and ETL orchestration
- **Storage Account**: Blob and file storage
- **App Service Environment V3 (ASE)**: Isolated and dedicated hosting environment
- **App Service Plan**: Compute resources for Logic Apps and Function Apps
- **API Management (APIM)**: API gateway and management
- **Relay**: Hybrid connectivity

### Key Principles

1. **Shared Core Resources**: All ISP projects share the same core infrastructure resources to maintain consistency and reduce overhead
2. **Project-Specific Resource Groups**: Each integration project has its own resource group for project-specific resources
3. **Environment Isolation**: Three environments - DEV, STG (Staging), and PRD (Production)
4. **Virtual Network Integration**: All resources are deployed within an Azure Virtual Network for security

## Development Tools and Process

### Design Tool: BrainBoard

- **Purpose**: Visual design and documentation of integration flows
- **Features**:
  - Technical diagram creation with Azure resources
  - Infrastructure as Code (Terraform) generation
  - Configuration management for Azure resources
  - Template library for common patterns
- **Organization**:
  - One Environment = One Functional Project
  - One Architecture = One Integration Flow

### Development IDEs

- **VS Code** (recommended) with extensions:
  - Azure Account
  - Azure Repos
  - Azure Functions
  - Azure Logic Apps (Standard)
  - NuGet Package Manager
  - HashiCorp Terraform

- **Visual Studio** (2019 or 2022)

### Required SDKs
- Versions selon projet et README des solutions
- Azure Functions Core Tools (version alignée avec le runtime du projet)
- Git (version supportée par l’entreprise)

### Network Access

- VPN connection to SBM network for accessing ISP VNet
- Development VM (SBWE1ISPDVVM01) available for VNet-internal operations
- Access via remote connection through SBM Citrix portal

## Naming Conventions

Référence: knowledge/sbm/naming-conventions.md

### Azure Resources (uppercase with dashes)

Pattern: `SB<REGION>1-ISP-<ENV>-<RESOURCE_TYPE>-<NUMBER>`

Example: `SBWE1-ISP-DV-LAP-01`

### Storage and lowercase resources (no dashes)

Pattern: `sb<region>1isp<env><resource_type><number>`

Example: `sbwe1ispststa01`

### Resource Type Codes

| Resource Type | Code |
|--------------|------|
| API Management | APM |
| App Service Environment | ASE |
| App Service Plan | ASP |
| Application Insight | API |
| Data Factory | DFA |
| Function App | FAP |
| Key Vault | KVA |
| Log Analytics | LAW |
| Logic App | LAP |
| Service Bus | SBN |
| Service Bus Queue | SBQ |
| Service Bus Topic | SBT |
| Service Bus Subscription | SBS |
| Storage Account | STA |
| Virtual Machine | VM |
| Virtual Network | VNT |

### Service Bus Naming

Pattern: `<project_trigram>.<type>.<entity>.<event>`

Examples:
- `ifs.q.shipment.events` (Queue)
- `ccs.t.documenttype` (Topic)
- `ccs.s.documenttype.cis` (Subscription)

### Resource Tags

Required tags for all project-specific resources:
- `env`: Environment (DEV, STG, PRD)
- `flow`: Flow code (LUCY, MDM, IFS, etc.)
- `desc`: Short description (optional)

**Convention**: Tag names in lowercase, values in Upper Camel Case

## Integration Patterns

### Common Architecture Patterns

1. **Event-Driven Integration**
   - Source system emits event to Service Bus
   - Logic App or Function App processes event
   - Target system receives transformed data

2. **API-Based Integration**
   - Requests flow through API Management
   - Authentication and routing handled by APIM
   - Backend Logic Apps or Function Apps process requests

3. **Scheduled Data Synchronization**
   - Data Factory or Function App with timer trigger
   - Extract data from source
   - Transform and load into target system

### Integration Components

- **Logic Apps**: Workflow orchestration, ideal for declarative integrations
- **Function Apps**: Custom code execution, C# or Node.js
- **Service Bus Queues**: Point-to-point messaging
- **Service Bus Topics/Subscriptions**: Publish-subscribe messaging
- **API Management**: API gateway, security, throttling, and monitoring

## API Management Best Practices

### Service Catalog Approach

1. **Service Mapping**: Identify all services offered by business unit
2. **Resource Diagram**: Define business entities and their relationships
3. **API Operations**: Define REST operations (GET, POST, PUT, DELETE)
4. **Generic APIs**: Prefer reusable APIs over project-specific implementations

### API Operations
- **POST**: Send information (create)
- **GET**: Retrieve information (read)
- **PUT**: Update existing information
- **DELETE**: Remove information

## Monitoring and Observability

### Logging Requirements

1. **Diagnostic Settings**: Enable for all resources (Logic Apps, Function Apps)
   - Link to Log Analytics workspace of the environment

2. **Application Insights Loggers**: Configure for APIM and other resources

3. **Custom Logging**: Implement logs at key flow steps
   - Track run progress
   - Log business events
   - Error handling and troubleshooting

## CI/CD Process

### Source Control
- Azure DevOps Repos for all source code
- One repository per integration project or domain

### Build Pipelines
- Automated builds on commit
- Infrastructure as Code validation (Terraform)
- Function App compilation and packaging

### Release Pipelines
- Automated deployment to DEV
- Approval gates for STG and PRD
- Environment-specific parameter substitution
- No direct manual changes to STG/PRD environments

### Terraform Deployment
- BrainBoard generates Terraform scripts
- Environment-agnostic configuration
- Variable-driven deployments
- Use of data sources to reference existing resources

## Integration Projects

### Active Projects

The platform supports numerous integration projects including:

- **LUCY**: Large ERP integration project
- **IFS**: ERP system integration
- **NEO**: Document management integration
- **Anaplan**: Planning and analytics integration
- **MDM**: Master Data Management
- **CCS**: Content management integration
- **NADIA**: Data analytics integration
- **Shipment**: Logistics management
- **HR Payroll**: Human resources integration
- **AKS**: Kubernetes-based services

### Example: NEO to IFS Integration

**Use Case**: Purchase Order (PO) lifecycle management

**Blocks**:
1. **PO Metadata Synchronization**: Auto-refresh PO metadata from NEO to IFS
2. **PO PDF Generation**: Copy generated PO PDF to centralized CIS repository
3. **RFQ Process**: Send Request for Quotation with NEO documents to Supplier Portal
4. **PO Transmittal**: Send PO transmittal packages to suppliers
5. **NEO Transmittal**: Send approved document packages from NEO to suppliers

**Architecture Components**:
- IFS (Source/Target ERP system)
- NEO (Document Management)
- Azure API Management (Gateway)
- Azure Functions (Processing logic)
- Service Bus (Event messaging)
- Supplier Portal (External system)

## Development Best Practices

### Logic Apps
- Add `FUNCTIONS_WORKER_RUNTIME` configuration key with value `node`
- Configure custom domain: `<LAP_name>.isp.sbmoffshore.com`
- Use wildcard certificate from Key Vault (*.isp.sbmoffshore.com)
- Implement logging steps for run progress tracking

### Function Apps
- Update host file on development VM with ASE endpoints
- Configure Application Insights for monitoring
- Use dependency injection for testability
- Implement retry policies for external calls

### BrainBoard Best Practices
- Resource labels match end of Azure resource name (e.g., LAP-21, FAP-01-HK)
- Minimize variables, prefer Terraform data sources
- Use global variables: `env`, `prefixLow`, `prefixUp`, `rgCoreName`
- Ensure environment-agnostic Terraform scripts

## Security

### Network Security
- Private endpoints for PaaS services
- Virtual Network integration for compute resources
- Network Security Groups (NSGs) for traffic control
- Service endpoints for secure Azure service access

### Identity and Access
- Managed identities for Azure resource authentication
- Key Vault for secrets management
- Azure AD authentication for APIs
- Role-Based Access Control (RBAC)

### Deployment Security
- No direct portal access for developers in STG/PRD
- All changes deployed via CI/CD pipelines
- Approval gates for production deployments
- Audit logs for all deployments

## Troubleshooting and Support

### Common Issues

1. **Connectivity Issues**: Verify VPN connection and NSG rules
2. **Authentication Failures**: Check managed identity assignments and Key Vault access policies
3. **Performance Issues**: Review Application Insights and Log Analytics
4. **Deployment Failures**: Validate Terraform syntax and Azure permissions

### Monitoring Tools

- **Application Insights**: Real-time application monitoring
- **Log Analytics**: Query and analyze logs
- **Azure Monitor**: Alerting and dashboards
- **Service Bus Explorer**: Message inspection

### Support Contacts

- **MiddleWay Team**: Platform architecture and infrastructure
- **Project Teams**: Application-specific integrations
- **Azure DevOps**: https://dev.azure.com/sbm-offshore/Integration%20Services%20Platform

## Resources

### Documentation
- Wiki: https://dev.azure.com/sbm-offshore/Integration%20Services%20Platform/_wiki
- BrainBoard: https://app.brainboard.co/

### Key Wiki Pages
- Architecture Design
- Core Architecture
- Developer Guide
- Best Practices
- Azure Environments
- APIM Service Catalog

## Environment Information

### Subscriptions
- **DEV**: Development subscription with SBWE1ISPDVVM01 VM
- **STG**: Staging/Pre-production
- **PRD**: Production

### Resource Groups
- Core resources: Shared across projects
- Project-specific: One per integration flow

### Regions
- Primary: West Europe (WE)
- Naming prefix: SBWE1
