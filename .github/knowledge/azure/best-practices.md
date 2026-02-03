# Azure Best Practices

## 🛡️ Sécurité

### 1. Identity & Access Management

#### Managed Identity
**Toujours utiliser Managed Identity au lieu de secrets**

```typescript
// ✅ BON
import { DefaultAzureCredential } from "@azure/identity";

const credential = new DefaultAzureCredential();
const client = new BlobServiceClient(
  `https://${accountName}.blob.core.windows.net`,
  credential
);

// ❌ MAUVAIS
const connectionString = "DefaultEndpointsProtocol=https;AccountName=...";
const client = BlobServiceClient.fromConnectionString(connectionString);
```

#### Least Privilege
- Attribuer uniquement les permissions nécessaires
- Utiliser des rôles Azure built-in quand possible
- Créer des rôles custom pour besoins spécifiques

```bicep
// Rôle Storage Blob Data Reader (lecture uniquement)
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(subscription().id, managedIdentity.id, 'StorageBlobDataReader')
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1')
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}
```

### 2. Network Security

#### Private Endpoints
```bicep
resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-05-01' = {
  name: 'pe-${storageAccount.name}'
  location: location
  properties: {
    subnet: {
      id: subnet.id
    }
    privateLinkServiceConnections: [
      {
        name: 'connection'
        properties: {
          privateLinkServiceId: storageAccount.id
          groupIds: ['blob']
        }
      }
    ]
  }
}

resource privateDnsZoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-05-01' = {
  parent: privateEndpoint
  name: 'default'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'config'
        properties: {
          privateDnsZoneId: privateDnsZone.id
        }
      }
    ]
  }
}
```

#### Network Security Groups
```bicep
resource nsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: 'nsg-${environment}'
  location: location
  properties: {
    securityRules: [
      {
        name: 'AllowHTTPS'
        properties: {
          priority: 100
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: 'Internet'
          destinationAddressPrefix: '*'
        }
      }
      {
        name: 'DenyAll'
        properties: {
          priority: 4096
          direction: 'Inbound'
          access: 'Deny'
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}
```

### 3. Data Protection

#### Encryption at Rest
```bicep
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: accountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    encryption: {
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
      }
      keySource: 'Microsoft.Keyvault'
      keyvaultproperties: {
        keyname: 'storage-encryption-key'
        keyvaulturi: keyVault.properties.vaultUri
      }
    }
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}
```

#### Azure Backup
```bicep
resource recoveryServicesVault 'Microsoft.RecoveryServices/vaults@2023-01-01' = {
  name: 'rsv-${environment}'
  location: location
  sku: {
    name: 'RS0'
    tier: 'Standard'
  }
  properties: {}
}

resource backupPolicy 'Microsoft.RecoveryServices/vaults/backupPolicies@2023-01-01' = {
  parent: recoveryServicesVault
  name: 'DailyBackupPolicy'
  properties: {
    backupManagementType: 'AzureIaasVM'
    schedulePolicy: {
      schedulePolicyType: 'SimpleSchedulePolicy'
      scheduleRunFrequency: 'Daily'
      scheduleRunTimes: ['2024-01-01T02:00:00Z']
    }
    retentionPolicy: {
      retentionPolicyType: 'LongTermRetentionPolicy'
      dailySchedule: {
        retentionTimes: ['2024-01-01T02:00:00Z']
        retentionDuration: {
          count: 30
          durationType: 'Days'
        }
      }
    }
  }
}
```

## 🚀 Performance

### 1. Caching

#### Azure Cache for Redis
```typescript
import { createClient } from 'redis';

const client = createClient({
  url: `rediss://${redisCacheName}.redis.cache.windows.net:6380`,
  password: process.env.REDIS_PASSWORD
});

await client.connect();

// Cache-aside pattern
async function getUserById(userId: string): Promise<User> {
  const cacheKey = `user:${userId}`;
  
  // Try cache first
  const cached = await client.get(cacheKey);
  if (cached) {
    return JSON.parse(cached);
  }
  
  // Fetch from database
  const user = await database.users.findById(userId);
  
  // Store in cache (5 minutes TTL)
  await client.setEx(cacheKey, 300, JSON.stringify(user));
  
  return user;
}
```

#### CDN
```bicep
resource cdn 'Microsoft.Cdn/profiles@2023-05-01' = {
  name: 'cdn-${appName}'
  location: 'global'
  sku: {
    name: 'Standard_Microsoft'
  }
}

resource cdnEndpoint 'Microsoft.Cdn/profiles/endpoints@2023-05-01' = {
  parent: cdn
  name: 'cdn-${appName}-${uniqueString(resourceGroup().id)}'
  location: 'global'
  properties: {
    originHostHeader: '${storageAccount.name}.blob.core.windows.net'
    origins: [
      {
        name: 'origin1'
        properties: {
          hostName: '${storageAccount.name}.blob.core.windows.net'
          httpsPort: 443
          originHostHeader: '${storageAccount.name}.blob.core.windows.net'
        }
      }
    ]
    deliveryPolicy: {
      rules: [
        {
          name: 'CacheStaticAssets'
          order: 1
          conditions: [
            {
              name: 'UrlFileExtension'
              parameters: {
                operator: 'Equal'
                matchValues: ['jpg', 'png', 'css', 'js', 'woff2']
              }
            }
          ]
          actions: [
            {
              name: 'CacheExpiration'
              parameters: {
                cacheBehavior: 'SetIfMissing'
                cacheType: 'All'
                cacheDuration: '7.00:00:00'
              }
            }
          ]
        }
      ]
    }
  }
}
```

### 2. Database Optimization

#### Connection Pooling
```typescript
import { ConnectionPool } from 'mssql';

const config = {
  server: process.env.SQL_SERVER,
  database: process.env.SQL_DATABASE,
  authentication: {
    type: 'azure-active-directory-default'
  },
  pool: {
    max: 10,
    min: 2,
    idleTimeoutMillis: 30000
  },
  options: {
    encrypt: true,
    trustServerCertificate: false
  }
};

const pool = new ConnectionPool(config);
await pool.connect();

// Reuse connection pool
export { pool };
```

#### Query Optimization
```sql
-- ✅ BON: Index approprié
CREATE INDEX IX_Orders_CustomerId_OrderDate 
ON Orders(CustomerId, OrderDate)
INCLUDE (TotalAmount);

SELECT OrderId, OrderDate, TotalAmount
FROM Orders
WHERE CustomerId = @CustomerId
  AND OrderDate >= @StartDate
ORDER BY OrderDate DESC;

-- ❌ MAUVAIS: Scan complet
SELECT *
FROM Orders
WHERE YEAR(OrderDate) = 2024;  -- Fonction sur colonne indexée
```

### 3. Async Operations

```typescript
// ✅ BON: Opérations parallèles
async function processOrder(orderId: string) {
  const [order, customer, inventory] = await Promise.all([
    orderRepository.findById(orderId),
    customerRepository.findById(customerId),
    inventoryService.checkAvailability(productIds)
  ]);
  
  // Process...
}

// ❌ MAUVAIS: Opérations séquentielles
async function processOrder(orderId: string) {
  const order = await orderRepository.findById(orderId);
  const customer = await customerRepository.findById(order.customerId);
  const inventory = await inventoryService.checkAvailability(order.productIds);
  
  // Process...
}
```

## 📊 Monitoring & Observability

### 1. Application Insights Telemetry

```typescript
import { ApplicationInsights } from '@azure/monitor-opentelemetry';
import { trace, context } from '@opentelemetry/api';

// Setup
ApplicationInsights.setup()
  .setAutoCollectRequests(true)
  .setAutoCollectPerformance(true)
  .setAutoCollectExceptions(true)
  .setAutoCollectDependencies(true)
  .setAutoCollectConsole(true)
  .start();

const tracer = trace.getTracer('my-service');

// Custom telemetry
export async function processOrder(order: Order) {
  return await tracer.startActiveSpan('process-order', async (span) => {
    try {
      span.setAttribute('order.id', order.id);
      span.setAttribute('order.total', order.total);
      
      const result = await orderService.process(order);
      
      span.setStatus({ code: SpanStatusCode.OK });
      return result;
      
    } catch (error) {
      span.recordException(error);
      span.setStatus({
        code: SpanStatusCode.ERROR,
        message: error.message
      });
      throw error;
      
    } finally {
      span.end();
    }
  });
}
```

### 2. Structured Logging

```typescript
import { Logger } from 'winston';

const logger = new Logger({
  format: format.combine(
    format.timestamp(),
    format.errors({ stack: true }),
    format.json()
  ),
  transports: [
    new transports.Console(),
    new transports.ApplicationInsights({
      instrumentationKey: process.env.APPINSIGHTS_INSTRUMENTATIONKEY
    })
  ]
});

// Usage
logger.info('Order processed', {
  orderId: order.id,
  customerId: order.customerId,
  amount: order.total,
  processingTime: duration,
  status: 'completed'
});

logger.error('Payment failed', {
  orderId: order.id,
  errorCode: error.code,
  errorMessage: error.message,
  stack: error.stack
});
```

### 3. Health Checks

```typescript
import { HealthCheckService } from '@nestjs/terminus';

@Controller('health')
export class HealthController {
  constructor(
    private health: HealthCheckService,
    private db: TypeOrmHealthIndicator,
    private http: HttpHealthIndicator
  ) {}
  
  @Get()
  check() {
    return this.health.check([
      // Database
      () => this.db.pingCheck('database'),
      
      // External APIs
      () => this.http.pingCheck('payment-api', 'https://api.payment.com/health'),
      
      // Custom checks
      async () => {
        const queueDepth = await this.getQueueDepth();
        const isHealthy = queueDepth < 1000;
        
        return {
          queue: {
            status: isHealthy ? 'up' : 'down',
            depth: queueDepth
          }
        };
      }
    ]);
  }
}
```

### 4. Alertes

```bicep
resource metricAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-high-cpu'
  location: 'global'
  properties: {
    description: 'Alert when CPU exceeds 80%'
    severity: 2
    enabled: true
    scopes: [
      containerApp.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'HighCPU'
          metricName: 'CpuPercentage'
          operator: 'GreaterThan'
          threshold: 80
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}
```

## 💰 Cost Optimization

### 1. Resource Tagging

```bicep
var commonTags = {
  Environment: environment
  Application: appName
  CostCenter: 'IT-Engineering'
  Owner: 'team@example.com'
  ManagedBy: 'Bicep'
  Project: projectName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: accountName
  location: location
  tags: commonTags
  // ...
}
```

### 2. Budget Alerts

```bicep
resource budget 'Microsoft.Consumption/budgets@2023-05-01' = {
  name: 'budget-${environment}'
  properties: {
    category: 'Cost'
    amount: 1000
    timeGrain: 'Monthly'
    timePeriod: {
      startDate: '2024-01-01'
    }
    filter: {
      tags: {
        name: 'Environment'
        values: [environment]
      }
    }
    notifications: {
      Actual_80_Percent: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 80
        contactEmails: ['team@example.com']
        contactRoles: ['Owner', 'Contributor']
      }
      Forecasted_100_Percent: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 100
        thresholdType: 'Forecasted'
        contactEmails: ['team@example.com']
      }
    }
  }
}
```

### 3. Auto-Shutdown

```bicep
resource autoShutdown 'Microsoft.DevTestLab/schedules@2018-09-15' = {
  name: 'shutdown-computevm-${vm.name}'
  location: location
  properties: {
    status: 'Enabled'
    taskType: 'ComputeVmShutdownTask'
    dailyRecurrence: {
      time: '1900'
    }
    timeZoneId: 'UTC'
    targetResourceId: vm.id
    notificationSettings: {
      status: 'Enabled'
      timeInMinutes: 30
      emailRecipient: 'team@example.com'
    }
  }
}
```

## 🔄 CI/CD Best Practices

### 1. Multi-Stage Pipeline

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop

stages:
  - stage: Build
    jobs:
      - job: BuildApp
        steps:
          - task: Docker@2
            inputs:
              command: buildAndPush
              repository: $(containerRegistry)/$(imageName)
              tags: |
                $(Build.BuildId)
                latest
          
          - task: PublishPipelineArtifact@1
            inputs:
              targetPath: '$(Build.ArtifactStagingDirectory)'
              artifact: 'bicep'

  - stage: DeployDev
    dependsOn: Build
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/develop')
    jobs:
      - deployment: DeployDev
        environment: development
        strategy:
          runOnce:
            deploy:
              steps:
                - template: templates/deploy.yml
                  parameters:
                    environment: dev
                    resourceGroup: rg-dev

  - stage: DeployProd
    dependsOn: Build
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/main')
    jobs:
      - deployment: DeployProd
        environment: production
        strategy:
          runOnce:
            deploy:
              steps:
                - template: templates/deploy.yml
                  parameters:
                    environment: prod
                    resourceGroup: rg-prod
```

### 2. Blue-Green Deployment

```bicep
resource containerAppProduction 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'ca-api-prod'
  properties: {
    configuration: {
      ingress: {
        traffic: [
          {
            revisionName: 'ca-api-prod--${newRevision}'
            weight: 100
            label: 'green'
          }
          {
            revisionName: 'ca-api-prod--${oldRevision}'
            weight: 0
            label: 'blue'
          }
        ]
      }
    }
  }
}
```

### 3. Infrastructure Validation

```yaml
- task: AzureCLI@2
  displayName: 'Validate Bicep'
  inputs:
    azureSubscription: $(serviceConnection)
    scriptType: bash
    scriptLocation: inlineScript
    inlineScript: |
      # Validate syntax
      az bicep build --file main.bicep
      
      # What-if analysis
      az deployment group what-if \
        --resource-group $(resourceGroup) \
        --template-file main.bicep \
        --parameters @parameters.$(environment).json
      
      # Security scan
      az security assessment list \
        --resource-group $(resourceGroup) \
        --query "[?status.code=='Unhealthy']"
```

## 📋 Checklist Complète

### Avant Déploiement
- [ ] Managed Identity configurée
- [ ] Secrets dans Key Vault
- [ ] Private Endpoints si nécessaire
- [ ] NSG configurés
- [ ] Encryption at rest activé
- [ ] TLS 1.2 minimum
- [ ] Tags sur toutes les ressources
- [ ] Budget alerts configurés

### Monitoring
- [ ] Application Insights configuré
- [ ] Structured logging
- [ ] Health checks
- [ ] Alertes critiques définies
- [ ] Dashboard monitoring

### Performance
- [ ] Caching strategy
- [ ] CDN pour assets statiques
- [ ] Connection pooling
- [ ] Async operations
- [ ] Database indexes

### Haute Disponibilité
- [ ] Multi-région si critique
- [ ] Auto-scaling configuré
- [ ] Backup automatique
- [ ] Disaster recovery plan
- [ ] Health probes

### Sécurité
- [ ] Vulnerability scanning
- [ ] Dependency updates
- [ ] Security headers
- [ ] WAF si exposition publique
- [ ] DDoS protection
