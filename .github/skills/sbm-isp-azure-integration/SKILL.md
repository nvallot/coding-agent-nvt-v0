# Skill: SBM Integration Services Platform (ISP) - Azure Integration Architecture

## Description

Expert in designing, developing, and deploying Azure-based enterprise integration solutions following SBM Integration Services Platform standards. This skill covers the complete lifecycle of integration projects from architecture design in BrainBoard to deployment via Azure DevOps CI/CD pipelines.

## Core Competencies

### Architecture Design
- Design integration flows using BrainBoard
- Define Azure resource topology (Logic Apps, Function Apps, Service Bus, APIM)
- Create reusable templates for common integration patterns
- Generate Infrastructure as Code (Terraform) from BrainBoard designs

### Azure Services Expertise
- **Logic Apps (Standard)**: Workflow orchestration, connectors, error handling
- **Function Apps**: C# and Node.js serverless functions for custom logic
- **Service Bus**: Message queuing, topics/subscriptions, dead-letter handling
- **API Management**: API gateway, policies, security, rate limiting
- **Key Vault**: Secrets, certificates, and key management
- **App Configuration**: Centralized configuration management
- **Application Insights**: Monitoring, telemetry, and custom logging
- **App Service Environment v3**: Isolated hosting environment

### Integration Patterns
- Event-driven architecture with Service Bus
- API-first design with APIM
- Request-response patterns
- Asynchronous messaging
- Publish-subscribe patterns
- Batch processing with Data Factory
- Scheduled synchronization

### Development Practices
- Follow SBM naming conventions for all resources
- Implement comprehensive logging and monitoring
- Use managed identities for authentication
- Configure diagnostic settings for all resources
- Apply proper tagging (env, flow, desc)
- Environment-agnostic Terraform configurations

### CI/CD Pipeline Management
- Azure DevOps repository organization
- Build pipelines for validation and compilation
- Release pipelines with environment promotion
- Terraform deployment automation
- Variable substitution across environments
- Approval gates for production

## When to Use This Skill

Apply this skill when the user needs help with:
- Designing new integration flows for SBM ISP
- Developing Logic Apps or Function Apps for integration scenarios
- Creating BrainBoard architectures following ISP standards
- Setting up Service Bus queues, topics, or subscriptions
- Configuring API Management policies
- Implementing monitoring and logging solutions
- Troubleshooting integration issues
- Following ISP naming conventions and best practices
- Deploying integration solutions via Azure DevOps
- Integrating systems like IFS, NEO, LUCY, Anaplan, etc.

## Usage Guidelines

### Architecture Design Phase

When designing a new integration flow:

1. **Understand Requirements**
   - Identify source and target systems
   - Define data flow and transformation needs
   - Determine integration pattern (event-driven, API, scheduled)

2. **BrainBoard Design**
   - Use ISP templates as starting point
   - Apply naming conventions: `SBWE1-ISP-<ENV>-<TYPE>-<NUM>`
   - Reference Core resources via Terraform data sources
   - Add required tags: env, flow, desc

3. **Resource Selection**
   - Use Logic Apps for workflow orchestration
   - Use Function Apps for complex transformations or custom logic
   - Use Service Bus queues for point-to-point messaging
   - Use Service Bus topics for pub-sub patterns
   - Route external calls through APIM

### Development Phase

When implementing integration code:

1. **Logic Apps Development**
   - Use VS Code with Azure Logic Apps extension
   - Implement error handling with try-catch scopes
   - Add custom logging steps for traceability
   - Configure retry policies for external calls
   - Store connection strings in Key Vault

2. **Function Apps Development**
   - Use .NET 6 or Node.js runtime
   - Implement dependency injection
   - Use Application Insights SDK for custom telemetry
   - Handle exceptions gracefully
   - Return appropriate HTTP status codes

3. **Service Bus Configuration**
   - Follow naming: `<project>.q.<entity>.<event>` for queues
   - Follow naming: `<project>.t.<entity>` for topics
   - Configure dead-letter queues
   - Set appropriate message TTL
   - Implement duplicate detection if needed

### Testing Phase

Before deployment:

1. **Local Testing**
   - Connect to DEV environment via VPN or Dev VM
   - Test Logic Apps locally with Azurite
   - Debug Function Apps locally
   - Validate message processing

2. **DEV Environment Testing**
   - Deploy to DEV via CI/CD
   - Perform end-to-end testing
   - Validate monitoring and logging
   - Test error scenarios

### Deployment Phase

When deploying to STG/PRD:

1. **CI/CD Configuration**
   - Commit code to Azure DevOps Repos
   - Configure build pipeline
   - Set up release pipeline with approval gates
   - Define environment-specific variables

2. **Terraform Deployment**
   - Validate Terraform syntax
   - Review generated plan
   - Apply with proper variable values
   - Verify resource creation

3. **Post-Deployment**
   - Validate diagnostic settings
   - Check Application Insights
   - Review Log Analytics queries
   - Perform smoke testing

## Best Practices

### Naming and Organization

```
# Azure Resources
SBWE1-ISP-DV-LAP-01      # Logic App in DEV
SBWE1-ISP-ST-FAP-02      # Function App in STG
SBWE1-ISP-PR-SBQ-ORDERS  # Service Bus Queue in PRD

# Service Bus
ifs.q.purchaseorder.created
lucy.t.employee.events
neo.s.documents.archive
```

### Logging Strategy

```csharp
// Function App logging
logger.LogInformation("Processing order {OrderId}", orderId);
logger.LogError(ex, "Failed to process order {OrderId}", orderId);

// Custom telemetry
telemetryClient.TrackEvent("OrderProcessed", 
    new Dictionary<string, string> { 
        { "OrderId", orderId },
        { "Status", "Success" }
    });
```

### Configuration Management

```json
// App Configuration keys
{
  "ISP:IFS:ApiUrl": "https://ifs-api.sbm.com",
  "ISP:NEO:ApiKey": "@Microsoft.KeyVault(SecretUri=...)",
  "ISP:ServiceBus:QueueName": "ifs.q.orders.events"
}
```

### Error Handling

```csharp
// Retry policy for external calls
services.AddHttpClient("IFS")
    .AddTransientHttpErrorPolicy(p => 
        p.WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## Common Scenarios

### Scenario 1: Event-Driven Integration

**Requirement**: When an order is created in IFS, send details to external system

**Solution**:
1. IFS emits event to Service Bus topic `ifs.t.order.events`
2. Logic App subscribes to topic
3. Logic App retrieves order details via APIM
4. Logic App transforms data
5. Logic App sends to target system via APIM
6. Log success/failure to Application Insights

### Scenario 2: API-Based Data Retrieval

**Requirement**: External system needs to retrieve employee data

**Solution**:
1. Define API in APIM: `GET /api/employees/{id}`
2. APIM policy validates JWT token
3. APIM routes to Function App backend
4. Function App queries source system
5. Function App transforms response
6. APIM returns data to caller
7. All requests logged to Application Insights

### Scenario 3: Scheduled Data Synchronization

**Requirement**: Sync master data from System A to System B daily

**Solution**:
1. Create Logic App with recurrence trigger (daily)
2. Logic App queries System A via APIM
3. Logic App transforms data
4. Logic App sends batches to Service Bus queue
5. Function App processes queue messages
6. Function App updates System B via APIM
7. Monitor via Application Insights dashboard

## Reference Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Core Resources                       │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐             │
│  │   Key    │  │   App    │  │  Service │             │
│  │  Vault   │  │  Config  │  │   Bus    │             │
│  └──────────┘  └──────────┘  └──────────┘             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐             │
│  │   App    │  │   APIM   │  │   Log    │             │
│  │ Insights │  │          │  │Analytics │             │
│  └──────────┘  └──────────┘  └──────────┘             │
└─────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              Project Resource Group                      │
│  ┌──────────────┐         ┌──────────────┐             │
│  │  Logic App   │────────▶│  Function    │             │
│  │    LAP-01    │         │   App FAP-01 │             │
│  └──────────────┘         └──────────────┘             │
│         │                         │                      │
│         └─────────┬───────────────┘                      │
│                   ▼                                      │
│         ┌──────────────────┐                            │
│         │   Service Bus    │                            │
│         │ Queue/Topic/Sub  │                            │
│         └──────────────────┘                            │
└─────────────────────────────────────────────────────────┘
```

## Related Skills

- `azure-logic-apps-development`: Deep dive into Logic Apps workflows
- `azure-functions-dotnet`: C# Function Apps development
- `azure-service-bus-messaging`: Service Bus messaging patterns
- `azure-api-management`: APIM policy and API design
- `terraform-azure-iac`: Infrastructure as Code with Terraform

## Keywords

Azure Integration, Enterprise Integration, Logic Apps, Function Apps, Service Bus, API Management, BrainBoard, Terraform, Azure DevOps, CI/CD, Event-Driven Architecture, Microservices, IFS Integration, NEO Integration, ISP Platform, SBM
