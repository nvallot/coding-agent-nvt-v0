# SBM Integration Services Platform - Naming Conventions

**CRITICAL**: All Azure resources and configurations for SBM Integration Services Platform MUST follow these naming conventions strictly.

## Azure Resource Naming

### Standard Resources (with dashes and uppercase)

**Pattern**: `SB<REGION>1-ISP-<ENV>-<RESOURCE_TYPE>-<NUMBER>`

**Components**:
- `SB`: SBM prefix
- `<REGION>`: Azure region code (e.g., WE for West Europe)
- `1`: Instance number
- `ISP`: Integration Services Platform
- `<ENV>`: Environment code
  - `DV`: Development
  - `ST`: Staging
  - `PR`: Production
- `<RESOURCE_TYPE>`: Resource type code (see table below)
- `<NUMBER>`: Sequential number (01, 02, etc.)

**Examples**:
```
SBWE1-ISP-DV-LAP-01    # Logic App in Development
SBWE1-ISP-ST-FAP-02    # Function App in Staging
SBWE1-ISP-PR-SBQ-05    # Service Bus Queue in Production
SBWE1-ISP-DV-KVA-01    # Key Vault in Development
```

### Lowercase Resources (no dashes)

**Pattern**: `sb<region>1isp<env><resource_type><number>`

Used for resources that require lowercase and no special characters (Storage Accounts, etc.)

**Examples**:
```
sbwe1ispdvsta01    # Storage Account in Development
sbwe1ispststa02    # Storage Account in Staging
```

## Resource Type Codes

| Resource Type | Code | Example |
|--------------|------|---------|
| API Management | APM | SBWE1-ISP-DV-APM-01 |
| App Service Environment | ASE | SBWE1-ISP-DV-ASE-01 |
| App Service Plan | ASP | SBWE1-ISP-DV-ASP-01 |
| Application Insights | API | SBWE1-ISP-DV-API-01 |
| Data Factory | DFA | SBWE1-ISP-DV-DFA-01 |
| Function App | FAP | SBWE1-ISP-DV-FAP-01 |
| Key Vault | KVA | SBWE1-ISP-DV-KVA-01 |
| Log Analytics Workspace | LAW | SBWE1-ISP-DV-LAW-01 |
| Logic App | LAP | SBWE1-ISP-DV-LAP-01 |
| On-Premise Data Gateway | ODG | SBWE1-ISP-DV-ODG-01 |
| Private Endpoint | NIC | SBWE1-ISP-DV-NIC-01 |
| Relay | REL | SBWE1-ISP-DV-REL-01 |
| Service Bus Namespace | SBN | SBWE1-ISP-DV-SBN-01 |
| Service Bus Queue | SBQ | SBWE1-ISP-DV-SBQ-01 |
| Service Bus Topic | SBT | SBWE1-ISP-DV-SBT-01 |
| Service Bus Subscription | SBS | SBWE1-ISP-DV-SBS-01 |
| Storage Account | STA | sbwe1ispdvsta01 |
| Subnet | SUB | SBWE1-ISP-DV-SUB-01 |
| Virtual Machine | VM | SBWE1-ISP-DV-VM-01 |
| Virtual Network | VNT | SBWE1-ISP-DV-VNT-01 |

## Service Bus Naming Conventions

### Pattern
`<project_trigram>.<object_type>.<business_entity>.<event>`

**Components**:
- `<project_trigram>`: 3-4 letter project code (lowercase)
- `<object_type>`: q (queue), t (topic), s (subscription)
- `<business_entity>`: Business entity name (lowercase)
- `<event>`: Event type or action (lowercase, optional)

### Examples

**Queues**:
```
ifs.q.shipment.events           # IFS shipment events queue
lucy.q.employee.created         # LUCY employee creation queue
neo.q.document.archived         # NEO document archive queue
```

**Topics**:
```
ccs.t.documenttype              # CCS document type topic
ifs.t.purchaseorder             # IFS purchase order topic
mdm.t.masterdata                # MDM master data topic
```

**Subscriptions**:
```
ccs.s.documenttype.cis          # CCS document type subscription for CIS
ifs.s.purchaseorder.neo         # IFS PO subscription for NEO
lucy.s.employee.payroll         # LUCY employee subscription for payroll
```

## Resource Tagging

**MANDATORY**: All project-specific resources MUST include these tags.

### Required Tags

| Tag Name | Description | Format | Example |
|----------|-------------|--------|---------|
| `env` | Environment | Uppercase | `DEV`, `STG`, `PRD` |
| `flow` | Flow/Project code | Uppercase | `LUCY`, `IFS`, `NEO` |
| `desc` | Resource description | Upper Camel Case | `ProcessOrders`, `SyncEmployees` |

### Tag Conventions
- **Tag names**: Always lowercase
- **Tag values**: Upper Camel Case (e.g., `ProcessOrders`, `GetCustomer`)
- **Environment values**: All uppercase (DEV, STG, PRD)

### Examples

```json
{
  "tags": {
    "env": "DEV",
    "flow": "IFS",
    "desc": "ProcessPurchaseOrders"
  }
}
```

```json
{
  "tags": {
    "env": "PRD",
    "flow": "LUCY",
    "desc": "SyncEmployeeData"
  }
}
```

## Configuration Keys

### App Configuration Naming

**Pattern**: `<Project>:<Component>:<Setting>`

**Examples**:
```
ISP:IFS:ApiUrl
ISP:NEO:ApiKey
ISP:ServiceBus:ConnectionString
ISP:LUCY:BatchSize
```

## Variable Naming in Terraform

### Global Variables (provided by templates)
- `env`: Environment (dev, stg, prd)
- `prefixLow`: Lowercase prefix (sbwe1ispdv)
- `prefixUp`: Uppercase prefix (SBWE1-ISP-DV)
- `rgCoreName`: Core resource group name

### Custom Variables
- Use lowercase with underscores
- Be descriptive
- Examples: `logic_app_name`, `function_app_sku`, `storage_replication_type`

## Domain Names

### Pattern
`<resource_name>.isp.sbmoffshore.com`

**Examples**:
```
sbwe1-isp-dv-lap-01.isp.sbmoffshore.com
sbwe1-isp-pr-fap-02.isp.sbmoffshore.com
```

### Certificate
- Wildcard certificate: `*.isp.sbmoffshore.com`
- Stored in Key Vault

## Function and Method Naming

### Function App Functions
- Use PascalCase
- Verb-Noun pattern
- Examples: `ProcessOrder`, `GetEmployee`, `SyncMasterData`

### Logic App Actions
- Use descriptive names with spaces
- Examples: "Get Order Details", "Transform Employee Data", "Send to Service Bus"

## File and Folder Structure

### Azure DevOps Repository Structure
```
<project-name>/
├── terraform/
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
├── src/
│   ├── function-apps/
│   │   └── <FAP-NAME>/
│   └── logic-apps/
│       └── <LAP-NAME>/
├── docs/
└── pipelines/
    ├── build.yml
    └── release.yml
```

### BrainBoard Resource Naming
- Use only the suffix of Azure resource name
- Examples: `LAP-01`, `FAP-02-ProcessOrders`, `SBQ-ShipmentEvents`

## Examples by Project

### IFS Integration Project

```
# Resource Group
RG-ISP-IFS-DV

# Resources
SBWE1-ISP-DV-LAP-01     # Main Logic App
SBWE1-ISP-DV-FAP-01     # Helper Function App
sbwe1ispdvsta01         # Storage Account

# Service Bus
ifs.q.purchaseorder.created
ifs.t.shipment.events
ifs.s.shipment.neo

# Tags
{
  "env": "DEV",
  "flow": "IFS",
  "desc": "POProcessing"
}
```

### LUCY Integration Project

```
# Resource Group
RG-ISP-LUCY-DV

# Resources
SBWE1-ISP-DV-LAP-05     # Main Logic App
SBWE1-ISP-DV-FAP-03     # Transformation Function
sbwe1ispdvsta02         # Storage Account

# Service Bus
lucy.q.employee.sync
lucy.t.payroll.events
lucy.s.payroll.ifs

# Tags
{
  "env": "DEV",
  "flow": "LUCY",
  "desc": "EmployeeSync"
}
```

## Validation Checklist

Before creating any resource, verify:

- [ ] Resource name follows pattern: `SBWE1-ISP-<ENV>-<TYPE>-<NUM>`
- [ ] Environment code is correct (DV, ST, or PR)
- [ ] Resource type code matches the table
- [ ] Number is sequential and unique
- [ ] Service Bus entities follow `<project>.<type>.<entity>` pattern
- [ ] All required tags are present (env, flow, desc)
- [ ] Tag names are lowercase
- [ ] Tag values are properly cased
- [ ] Configuration keys follow `Project:Component:Setting` pattern

## Common Mistakes to Avoid

❌ **WRONG**:
```
IFS-LAP-01                    # Missing region and ISP prefix
SBWE1-ISP-DEV-LAP-01         # Wrong env code (DEV instead of DV)
sbwe1-isp-dv-lap-01          # Should be uppercase
SBWE1_ISP_DV_LAP_01          # Underscores instead of dashes
IFS-Q-PURCHASEORDER          # Not following Service Bus pattern
```

✅ **CORRECT**:
```
SBWE1-ISP-DV-LAP-01          # Correct format
sbwe1ispdvsta01              # Correct for storage
ifs.q.purchaseorder.created  # Correct Service Bus naming
```

## Reference

For any questions or clarifications on naming conventions, refer to:
- ISP Wiki: [Best Practices](/Development/Best-practices)
- BrainBoard templates
- MiddleWay ISP team

**Last Updated**: Based on ISP Wiki documentation
**Applies To**: All ISP projects and integrations
