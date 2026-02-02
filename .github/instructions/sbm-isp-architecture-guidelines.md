# SBM ISP Azure Architecture Best Practices

**CRITICAL**: Follow these architectural guidelines when designing or implementing solutions for the SBM Integration Services Platform.

## Core Architecture Principles

### 1. Shared Core Resources

**DO**:
- ✅ Use the existing Core Service Bus namespace for all messaging
- ✅ Route all external APIs through the existing API Management instance
- ✅ Store all secrets in the Core Key Vault
- ✅ Use the Core App Service Plan for Logic Apps and Function Apps
- ✅ Send all telemetry to the Core Application Insights instance
- ✅ Log to the Core Log Analytics workspace

**DON'T**:
- ❌ Create a new Service Bus namespace for your project
- ❌ Deploy a separate API Management instance
- ❌ Create project-specific Key Vaults unless absolutely necessary
- ❌ Deploy standalone App Service Plans
- ❌ Create separate monitoring resources

**Rationale**: Shared resources reduce costs, simplify management, and ensure consistent monitoring across all integrations.

### 2. Resource Group Organization

**Pattern**: One resource group per integration project/flow

**Structure**:
```
RG-ISP-<PROJECT>-<ENV>
  ├── Logic Apps (project-specific)
  ├── Function Apps (project-specific)
  ├── Storage Accounts (if needed)
  └── Project-specific resources
```

**Example**:
```
RG-ISP-IFS-DV
  ├── SBWE1-ISP-DV-LAP-01 (IFS Purchase Order Logic App)
  ├── SBWE1-ISP-DV-FAP-01 (IFS Transformation Function)
  └── sbwe1ispdvsta01 (IFS Storage Account)
```

### 3. Network Architecture

**Requirements**:
- All resources MUST be deployed within the ISP Virtual Network
- Use App Service Environment v3 for compute isolation
- Private endpoints for PaaS services
- Service endpoints for Azure services
- Network Security Groups for traffic control

**Network Topology**:
```
ISP Virtual Network (SBWE1-ISP-<ENV>-VNT-01)
  ├── SUB-01: App Service Environment
  │   ├── Logic Apps
  │   └── Function Apps
  ├── SUB-02: Private Endpoints
  │   ├── Service Bus
  │   ├── Storage
  │   └── Key Vault
  └── SUB-03: Management
      └── DevOps Agents
```

## Component Selection Guidelines

### When to Use Logic Apps

**Use Logic Apps for**:
- Workflow orchestration with multiple steps
- Integration scenarios with built-in connectors
- Event-driven processes
- Low-code/no-code requirements
- Visual workflow representation

**Example Scenarios**:
- Processing Service Bus messages with conditional logic
- Orchestrating multi-step API calls
- File processing workflows
- Data synchronization between systems

**Standard Configuration**:
```json
{
  "name": "SBWE1-ISP-DV-LAP-01",
  "sku": "WorkflowStandard",
  "settings": {
    "FUNCTIONS_WORKER_RUNTIME": "node",
    "AzureWebJobsStorage": "@Microsoft.KeyVault(...)"
  }
}
```

### When to Use Function Apps

**Use Function Apps for**:
- Complex data transformations
- Custom business logic
- High-performance requirements
- Reusable code libraries
- Unit testable code

**Example Scenarios**:
- Complex JSON/XML transformations
- Custom authentication/authorization
- Batch processing
- Data validation and enrichment

**Runtime Selection**:
- **.NET 6**: For enterprise C# applications, complex logic
- **Node.js**: For JavaScript/TypeScript, JSON processing

**Standard Configuration**:
```json
{
  "name": "SBWE1-ISP-DV-FAP-01",
  "runtime": "dotnet",
  "version": "6",
  "settings": {
    "ApplicationInsights": "@Microsoft.KeyVault(...)",
    "ServiceBusConnection": "@Microsoft.KeyVault(...)"
  }
}
```

### Service Bus Topology

**Queues vs Topics**:

| Use Queue When | Use Topic When |
|----------------|----------------|
| Single consumer | Multiple consumers |
| Point-to-point | Publish-subscribe |
| Order guaranteed | Fan-out scenarios |
| Simple routing | Content-based routing |

**Queue Example**: Purchase Order Processing
```
ifs.q.purchaseorder.created
  └── Consumed by: IFS PO Processor Logic App
```

**Topic Example**: Employee Updates
```
lucy.t.employee.updated
  ├── lucy.s.employee.payroll (Payroll subscription)
  ├── lucy.s.employee.benefits (Benefits subscription)
  └── lucy.s.employee.reporting (Reporting subscription)
```

**Configuration Best Practices**:
```json
{
  "queueName": "ifs.q.purchaseorder.created",
  "maxDeliveryCount": 10,
  "lockDuration": "PT5M",
  "enableDeadLettering": true,
  "duplicateDetection": true,
  "duplicateDetectionWindow": "PT10M"
}
```

## API Management Patterns

### API Organization

**Pattern**: Group APIs by business domain

```
APIM (SBWE1-ISP-<ENV>-APM-01)
  ├── IFS APIs
  │   ├── /ifs/purchaseorders
  │   ├── /ifs/shipments
  │   └── /ifs/documents
  ├── LUCY APIs
  │   ├── /lucy/employees
  │   └── /lucy/departments
  └── Common APIs
      ├── /health
      └── /metadata
```

### API Design Principles

1. **RESTful Design**
   - Use proper HTTP verbs (GET, POST, PUT, DELETE)
   - Use nouns for resources, not verbs
   - Version your APIs (/v1/, /v2/)

2. **Security**
   - OAuth 2.0 for external APIs
   - Managed Identity for internal services
   - API keys for partners/suppliers
   - Rate limiting on all APIs

3. **Standard Policies**
```xml
<policies>
    <inbound>
        <base />
        <!-- Authentication -->
        <validate-jwt header-name="Authorization" 
                      failed-validation-httpcode="401">
            <!-- JWT validation config -->
        </validate-jwt>
        
        <!-- Rate limiting -->
        <rate-limit calls="100" renewal-period="60" />
        
        <!-- CORS -->
        <cors allow-credentials="false">
            <allowed-origins>
                <origin>https://*.sbmoffshore.com</origin>
            </allowed-origins>
        </cors>
    </inbound>
    
    <backend>
        <base />
    </backend>
    
    <outbound>
        <base />
        <!-- Remove internal headers -->
        <set-header name="X-Powered-By" exists-action="delete" />
    </outbound>
    
    <on-error>
        <base />
        <!-- Custom error response -->
    </on-error>
</policies>
```

## Error Handling and Resilience

### Retry Policies

**Exponential Backoff Pattern**:
```csharp
// Function App
services.AddHttpClient("ExternalAPI")
    .AddTransientHttpErrorPolicy(policy => 
        policy.WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) => {
                logger.LogWarning(
                    "Retry {RetryCount} after {Delay}s", 
                    retryCount, timespan.TotalSeconds);
            }));
```

**Logic Apps**:
- Configure retry policies on HTTP actions
- Use exponential intervals: 5s, 10s, 20s, 40s
- Maximum 4 retries
- Handle 408, 429, 5xx status codes

### Circuit Breaker Pattern

Implement circuit breakers for external dependencies:

```csharp
services.AddHttpClient("UnreliableAPI")
    .AddPolicyHandler(Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)));
```

### Dead Letter Handling

**Service Bus Dead Letter Queue Strategy**:

1. **Automatic Dead Lettering**
   - After max delivery count (10)
   - On message expiration
   - On filter evaluation errors

2. **Manual Dead Lettering**
   - Invalid message format
   - Business rule violations
   - Unrecoverable errors

3. **Dead Letter Processing**
```
Create monitoring Logic App:
  ├── Trigger: Check dead letter queue every 5 minutes
  ├── Action: Read dead letter messages
  ├── Action: Log to Application Insights
  ├── Action: Send alert if count > threshold
  └── Action: Move to poison message storage if needed
```

## Monitoring and Logging

### Application Insights Strategy

**Telemetry Types**:

1. **Traces** (Logs)
```csharp
logger.LogInformation("Processing order {OrderId}", orderId);
logger.LogWarning("Retry attempt {RetryCount} for {OrderId}", retryCount, orderId);
logger.LogError(ex, "Failed to process order {OrderId}", orderId);
```

2. **Custom Events**
```csharp
telemetryClient.TrackEvent("OrderProcessed", 
    new Dictionary<string, string> {
        { "OrderId", orderId },
        { "CustomerId", customerId },
        { "Status", "Success" }
    },
    new Dictionary<string, double> {
        { "ProcessingTime", processingTime.TotalMilliseconds }
    });
```

3. **Dependencies**
```csharp
// Automatically tracked by Application Insights SDK
// HTTP calls, SQL queries, Service Bus messages
```

4. **Metrics**
```csharp
telemetryClient.GetMetric("OrdersProcessedPerMinute").TrackValue(count);
```

### Log Analytics Queries

**Standard Queries**:

1. **Flow Execution Tracking**
```kql
traces
| where customDimensions.flow == "IFS"
| where customDimensions.orderId == "PO12345"
| project timestamp, message, severityLevel
| order by timestamp asc
```

2. **Error Analysis**
```kql
exceptions
| where cloud_RoleName startswith "SBWE1-ISP"
| summarize count() by problemId, outerMessage
| order by count_ desc
| take 10
```

3. **Performance Monitoring**
```kql
customEvents
| where name == "OrderProcessed"
| extend processingTime = todouble(customMeasurements.ProcessingTime)
| summarize avg(processingTime), percentile(processingTime, 95) 
  by bin(timestamp, 1h)
```

### Diagnostic Settings

**Required Configuration for All Resources**:

```json
{
  "diagnosticSettings": {
    "name": "ISP-Diagnostics",
    "workspaceId": "<Core-Log-Analytics-Workspace-ID>",
    "logs": [
      {
        "category": "FunctionAppLogs",
        "enabled": true,
        "retentionPolicy": {
          "enabled": true,
          "days": 90
        }
      }
    ],
    "metrics": [
      {
        "category": "AllMetrics",
        "enabled": true,
        "retentionPolicy": {
          "enabled": true,
          "days": 30
        }
      }
    ]
  }
}
```

## Security Best Practices

### Managed Identities

**Always Use Managed Identities**:
- ✅ System-assigned for single-resource access
- ✅ User-assigned for shared access patterns
- ❌ Never use connection strings in code
- ❌ Never use service principal credentials

**Example Configuration**:
```hcl
# Terraform
resource "azurerm_logic_app_standard" "main" {
  name = "SBWE1-ISP-DV-LAP-01"
  
  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_role_assignment" "servicebus" {
  scope                = azurerm_servicebus_namespace.core.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = azurerm_logic_app_standard.main.identity[0].principal_id
}
```

### Key Vault Integration

**Secrets Management**:
```json
{
  "secrets": {
    "IFS-API-Key": {
      "value": "@Microsoft.KeyVault(SecretUri=https://sbwe1ispdvkva01.vault.azure.net/secrets/IFS-API-Key/)",
      "rotationPolicy": "Every 90 days"
    }
  }
}
```

**Access Policies**:
- Grant minimal permissions (Get Secrets only)
- Use managed identities for access
- Enable soft delete and purge protection
- Audit all access via Log Analytics

## Data Security

### Data Classification

| Classification | Description | Encryption | Access |
|----------------|-------------|------------|---------|
| Public | Non-sensitive | TLS in transit | Anyone |
| Internal | Business data | TLS + at rest | Employees |
| Confidential | PII, Financial | TLS + at rest + envelope | Need-to-know |
| Restricted | Critical secrets | TLS + at rest + HSM | Minimal access |

### Encryption Requirements

**In Transit**:
- TLS 1.2 minimum for all connections
- Disable older protocols (SSL 3.0, TLS 1.0, TLS 1.1)

**At Rest**:
- Azure Storage: Service-managed keys (default)
- Confidential data: Customer-managed keys in Key Vault

## Performance Optimization

### Logic Apps

**Best Practices**:
- Use concurrency control to prevent throttling
- Implement chunking for large datasets
- Use batch processing for high-volume scenarios
- Cache reference data

**Configuration**:
```json
{
  "concurrency": {
    "runs": 25,
    "repetitions": 1
  },
  "splitOn": "@triggerBody()?['items']",
  "batchSize": 100
}
```

### Function Apps

**Best Practices**:
- Use async/await for I/O operations
- Implement connection pooling
- Cache configuration
- Use Durable Functions for long-running workflows

**Example**:
```csharp
private static readonly HttpClient httpClient = new HttpClient();

public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function)] HttpRequest req,
    ILogger log)
{
    // Use static HttpClient for connection pooling
    var response = await httpClient.GetAsync("https://api.example.com");
    return new OkObjectResult(await response.Content.ReadAsStringAsync());
}
```

### Service Bus

**Optimization**:
- Use batch operations where possible
- Implement prefetching for high-throughput
- Configure appropriate session settings
- Use partitioning for high-volume queues

## Deployment Best Practices

### Infrastructure as Code

**Terraform Structure**:
```
project-terraform/
├── main.tf              # Main resource definitions
├── variables.tf         # Input variables
├── outputs.tf           # Output values
├── providers.tf         # Provider configuration
├── backend.tf           # State backend configuration
└── modules/
    ├── logic-app/
    ├── function-app/
    └── service-bus/
```

**Environment Variables**:
```hcl
variable "env" {
  type = string
  validation {
    condition     = contains(["dv", "st", "pr"], var.env)
    error_message = "Environment must be dv, st, or pr"
  }
}
```

### CI/CD Pipeline

**Build Stage**:
1. Validate Terraform syntax
2. Run security scans
3. Compile Function Apps
4. Run unit tests
5. Package Logic Apps

**Release Stage**:
```yaml
stages:
  - stage: DEV
    jobs:
      - deployment: Deploy
        environment: ISP-DEV
        strategy:
          runOnce:
            deploy:
              steps:
                - task: TerraformCLI@0
                  inputs:
                    command: apply
                    workingDirectory: $(Pipeline.Workspace)/terraform
                    environmentServiceName: ISP-DEV
                    
  - stage: STG
    dependsOn: DEV
    condition: succeeded()
    jobs:
      - deployment: Deploy
        environment: ISP-STG
        strategy:
          runOnce:
            preDeploy:
              steps:
                - task: ManualValidation@0
            deploy:
              steps:
                - task: TerraformCLI@0
```

## Compliance and Governance

### Azure Policy

**Required Policies**:
- Enforce required tags (env, flow)
- Restrict allowed resource locations
- Require diagnostic settings
- Enforce managed identities
- Require network integration

### Cost Management

**Cost Optimization**:
- Use shared App Service Plans
- Right-size Function App plans
- Implement auto-scaling
- Monitor and optimize Storage costs
- Review and clean up unused resources

**Tagging for Cost Tracking**:
```json
{
  "tags": {
    "env": "DEV",
    "flow": "IFS",
    "costCenter": "IT-Integration",
    "owner": "john.doe@sbm.com"
  }
}
```

## Documentation Requirements

### Mandatory Documentation

For every integration flow:
1. **High-Level Design (HLD)**
   - Business purpose
   - Data flow diagram
   - System interactions
   - SLAs and requirements

2. **BrainBoard Architecture**
   - Visual design
   - Resource configuration
   - Terraform generation

3. **Runbook**
   - Deployment steps
   - Troubleshooting guide
   - Contact information

4. **API Documentation**
   - OpenAPI/Swagger spec
   - Authentication requirements
   - Sample requests/responses

---

**Compliance**: All solutions MUST follow these guidelines unless explicitly approved by the ISP Architecture Board.

**Review**: These practices are reviewed quarterly and updated based on lessons learned and Azure platform updates.

**Contact**: For clarifications or exceptions, contact the ISP Architecture team.
