# [Client Name] - Client Configuration Template

## Client Overview

- **Client Name**: [Full client name]
- **Industry**: [e.g., Finance, Healthcare, Retail, Manufacturing]
- **Project Name**: [Project or engagement name]
- **Start Date**: [YYYY-MM-DD]
- **Expected End Date**: [YYYY-MM-DD or "Ongoing"]
- **Team Size**: [Number of team members]
- **Project Type**: [e.g., Data Migration, Analytics Platform, Real-time Integration]

## Business Context

### Business Objectives
- [Primary business objective 1]
- [Primary business objective 2]
- [Primary business objective 3]

### Success Criteria
- [Measurable success criterion 1]
- [Measurable success criterion 2]
- [Measurable success criterion 3]

### Key Stakeholders
- **Business Owner**: [Name, Title]
- **Technical Lead**: [Name, Title]
- **Project Manager**: [Name, Title]
- **End Users**: [Description of user base]

## Technical Stack

### Cloud Platform
- **Primary Cloud**: [Azure / AWS / GCP / Multi-cloud]
- **Azure Subscription ID**: [XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX]
- **Azure Regions**: [Primary region, Secondary region]

### Programming Languages
- **Primary**: [e.g., C# .NET 8]
- **Secondary**: [e.g., Python 3.11]
- **Scripting**: [e.g., PowerShell 7]

### Data Integration Stack
- **Orchestration**: [e.g., Azure Data Factory, Synapse Pipelines, Airflow]
- **Processing**: [e.g., Databricks, Synapse Spark, Azure Functions]
- **Storage**: [e.g., ADLS Gen2, Synapse SQL Pool, Cosmos DB]
- **Streaming**: [e.g., Event Hubs, Kafka, Stream Analytics]

### Infrastructure as Code
- **Tool**: [Terraform / Bicep / ARM Templates / Pulumi]
- **State Backend**: [Azure Storage / Terraform Cloud]
- **Version**: [Terraform 1.6+ / Bicep 0.24+]

### CI/CD Platform
- **Platform**: [GitHub Actions / Azure DevOps / GitLab CI]
- **Repository**: [Organization/repo-name]
- **Branch Strategy**: [Git Flow / Trunk-based / GitHub Flow]

## Client-Specific Azure Services

List the specific Azure services this client uses:

### Data Services
- [ ] Azure Data Factory
- [ ] Azure Synapse Analytics
- [ ] Azure Databricks
- [ ] Azure SQL Database
- [ ] Cosmos DB
- [ ] ADLS Gen2
- [ ] Blob Storage
- [ ] Microsoft Fabric

### Compute Services
- [ ] Azure Functions
- [ ] Container Apps
- [ ] App Service
- [ ] AKS (Kubernetes)
- [ ] Virtual Machines

### Integration Services
- [ ] Service Bus
- [ ] Event Grid
- [ ] Event Hubs
- [ ] Logic Apps
- [ ] API Management

### Security & Identity
- [ ] Azure AD / Entra ID
- [ ] Managed Identity
- [ ] Key Vault
- [ ] Private Link / Private Endpoints

### Monitoring & Management
- [ ] Application Insights
- [ ] Log Analytics
- [ ] Azure Monitor
- [ ] Microsoft Purview

## Client-Specific Standards

### Naming Conventions
Override default naming if client has specific requirements:
- Resource Groups: [pattern]
- Storage Accounts: [pattern]
- Data Factory: [pattern]
- Key Vault: [pattern]
- [Other resources]: [pattern]

### Tagging Standards
Required tags for all resources:
- `Environment`: [dev, test, staging, prod]
- `CostCenter`: [Client's cost center code]
- `Owner`: [Team or person responsible]
- `Application`: [Application name]
- [Client-specific tag]: [value]

### Coding Standards Overrides
Any deviations from default standards:
- [Standard 1 override]
- [Standard 2 override]

## Security & Compliance

### Compliance Requirements
- [ ] GDPR (EU data protection)
- [ ] HIPAA (healthcare)
- [ ] SOC 2 (service organization controls)
- [ ] PCI DSS (payment card industry)
- [ ] [Other industry-specific compliance]

### Data Classification
- **Public**: [Definition and handling]
- **Internal**: [Definition and handling]
- **Confidential**: [Definition and handling]
- **Restricted**: [Definition and handling]

### Access Control
- **Authentication**: [Azure AD / SSO provider / MFA requirements]
- **Authorization**: [RBAC groups / custom roles]
- **Network Access**: [VPN / Private Link / IP restrictions]

### Sensitive Data
Types of sensitive data handled:
- [ ] PII (Personally Identifiable Information)
- [ ] PHI (Protected Health Information)
- [ ] Financial data (credit cards, bank accounts)
- [ ] Intellectual property
- [ ] [Other sensitive data types]

## Data Pipeline Requirements

### Data Sources
1. **[Source System 1]**
   - Type: [SQL Server / Oracle / SAP / API / Files]
   - Connection Method: [Private Link / VPN / Public endpoint]
   - Data Volume: [GB/TB per day]
   - Update Frequency: [Real-time / Hourly / Daily / Weekly]
   
2. **[Source System 2]**
   - Type: [...]
   - Connection Method: [...]
   - Data Volume: [...]
   - Update Frequency: [...]

### Data Destinations
1. **[Destination System 1]**
   - Type: [Synapse SQL Pool / ADLS Gen2 / Power BI]
   - Purpose: [Analytics / Reporting / ML]
   - SLA: [Latency requirements]

### Data Quality Rules
- [Rule 1: e.g., No null values in customer_id]
- [Rule 2: e.g., Date formats must be ISO 8601]
- [Rule 3: e.g., Email addresses must be valid]

## Performance Requirements

### SLAs
- **Pipeline Completion**: [e.g., Daily pipelines complete by 8 AM]
- **API Response Time**: [e.g., < 200ms p95]
- **Data Freshness**: [e.g., < 15 minutes for real-time]
- **Uptime**: [e.g., 99.9%]

### Scalability
- **Concurrent Users**: [e.g., 10,000]
- **Data Volume Growth**: [e.g., 20% year-over-year]
- **Peak Load**: [e.g., 10x normal during month-end]

## CI/CD Configuration

### Environments
1. **Development**
   - Branch: `develop` or feature branches
   - Deployment: Automatic on commit
   - Testing: Unit tests only
   
2. **Staging**
   - Branch: `staging`
   - Deployment: Automatic on merge to staging
   - Testing: Integration and E2E tests
   
3. **Production**
   - Branch: `main` with tag
   - Deployment: Manual approval required
   - Testing: Smoke tests post-deployment

### Approval Process
- **Staging**: [Auto / Tech Lead approval]
- **Production**: [Client approval required / Change Advisory Board]

## Monitoring & Alerting

### Critical Alerts
Configure alerts for:
- [ ] Pipeline failures
- [ ] API errors (>5xx)
- [ ] Data quality violations
- [ ] Performance degradation
- [ ] Security events
- [ ] Resource quota limits

### Alert Recipients
- **P0 (Critical)**: [Pager / On-call team]
- **P1 (High)**: [Email / Teams channel]
- **P2 (Medium)**: [Email / Daily digest]

### Dashboards
- Pipeline execution dashboard: [URL]
- Application performance: [URL]
- Cost monitoring: [URL]

## Documentation Requirements

### Required Documents
- [ ] Business Requirements Document (BRD)
- [ ] Technical Architecture Document (TAD)
- [ ] API Documentation (OpenAPI spec)
- [ ] Data Dictionary
- [ ] Deployment Runbook
- [ ] Disaster Recovery Plan
- [ ] User Guides
- [Client-specific document]

### Document Location
- **Repository**: [GitHub repo URL]
- **Confluence/SharePoint**: [URL if applicable]
- **Client Portal**: [URL if applicable]

## Communication

### Regular Meetings
- **Daily Standup**: [Time, attendees]
- **Sprint Planning**: [Frequency, attendees]
- **Sprint Review**: [Frequency, attendees]
- **Stakeholder Updates**: [Frequency, format]

### Communication Channels
- **Primary**: [Slack / Teams / Email]
- **Urgent Issues**: [Phone / Pager]
- **Status Updates**: [Email / Dashboard]

### Escalation Path
1. Development Team
2. [Tech Lead Name]
3. [Client Technical Lead Name]
4. [Client Business Owner Name]

## Known Constraints & Limitations

### Technical Constraints
- [e.g., Must use client's existing SQL Server 2016]
- [e.g., Cannot use Public IP addresses]
- [e.g., Limited to specific Azure regions]

### Business Constraints
- [e.g., Budget limited to $X per month]
- [e.g., Must go live by specific date]
- [e.g., Limited access to source systems]

### Resource Constraints
- [e.g., Only 2 developers available]
- [e.g., SME availability 10 hours/week]

## Client-Specific Notes

### Cultural / Process Notes
- [e.g., Client requires formal change requests for all changes]
- [e.g., All communication must go through Project Manager]
- [e.g., Weekly status reports in specific format]

### Lessons Learned
Document learnings from this engagement:
- [Lesson 1]
- [Lesson 2]
- [Lesson 3]

### Success Stories
- [Achievement 1]
- [Achievement 2]

## Contact Information

### Client Contacts
- **Business Owner**: [Name, email, phone]
- **Technical Lead**: [Name, email, phone]
- **Project Manager**: [Name, email, phone]

### Our Team
- **Project Lead**: [Name, email, phone]
- **Solution Architect**: [Name, email, phone]
- **Lead Developer**: [Name, email, phone]

## Configuration Files

Client-specific configurations are stored in:
- **Config**: `.github/clients/[client-name]/config/`
- **Secrets**: Azure Key Vault: `kv-[client]-[env]`
- **IaC Variables**: `terraform/[client]/variables.tfvars`

## Activation

To activate this client configuration:

```bash
./github/tools/client-manager.sh activate [client-name]
```

Or manually update `.github/clients/active-client.json`:
```json
{
  "activeClient": "[client-name]",
  "lastUpdated": "[ISO 8601 timestamp]"
}
```

---

**Last Updated**: [YYYY-MM-DD]  
**Updated By**: [Name]  
**Version**: 1.0
