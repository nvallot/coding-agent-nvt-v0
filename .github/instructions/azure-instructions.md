# Instructions Azure

## 🎯 Vue d'Ensemble

Instructions spécifiques pour le développement et le déploiement d'applications sur Azure.

## ☁️ Services Azure Recommandés

### Compute

| Service | Usage | Quand l'utiliser |
|---------|-------|------------------|
| **Azure Functions** | Serverless, event-driven | Tâches légères, déclencheurs événementiels |
| **Container Apps** | Conteneurs serverless | Microservices, APIs avec scaling automatique |
| **App Service** | Applications web traditionnelles | Applications monolithiques, prototypage rapide |
| **AKS** | Kubernetes géré | Applications complexes, contrôle total |

### Storage

| Service | Usage | Quand l'utiliser |
|---------|-------|------------------|
| **Blob Storage** | Objets/Fichiers | Documents, images, backups |
| **Cosmos DB** | NoSQL distribué | Applications globales, faible latence |
| **SQL Database** | Relationnel géré | Applications OLTP traditionnelles |
| **Table Storage** | NoSQL simple | Logs, données structurées simples |

### Messaging

| Service | Usage | Quand l'utiliser |
|---------|-------|------------------|
| **Service Bus** | Messaging entreprise | Transactions, ordre garanti, sessions |
| **Event Grid** | Événements pub/sub | Intégrations événementielles |
| **Event Hubs** | Streaming données | Télémétrie, logs à grande échelle |

## 🏗️ Patterns d'Architecture Azure

### 1. Microservices avec Container Apps

```yaml
# Exemple: container-app-architecture.yaml
services:
  - name: api-gateway
    type: ContainerApp
    ingress:
      external: true
      targetPort: 8080
    scale:
      minReplicas: 1
      maxReplicas: 10
  
  - name: user-service
    type: ContainerApp
    ingress:
      external: false
      targetPort: 3000
    scale:
      minReplicas: 2
      maxReplicas: 20

dependencies:
  - type: ServiceBus
    name: messaging
  - type: CosmosDB
    name: user-db
```

### 2. Event-Driven avec Functions

```typescript
// Exemple: Azure Function avec Service Bus
import { app, InvocationContext } from "@azure/functions";

export async function serviceBusQueueTrigger(
  message: unknown,
  context: InvocationContext
): Promise<void> {
  context.log('Service Bus queue function processed message:', message);
  
  // Traiter le message
  await processOrder(message);
  
  // Le message est automatiquement complété
}

app.serviceBusQueue('orderProcessor', {
  connection: 'ServiceBusConnection',
  queueName: 'orders',
  handler: serviceBusQueueTrigger
});
```

### 3. API Management

```typescript
// Configuration APIM
{
  "policies": {
    "inbound": [
      { "rate-limit": { "calls": 100, "renewal-period": 60 } },
      { "validate-jwt": { "header-name": "Authorization" } },
      { "set-backend-service": { "base-url": "https://backend.azurewebsites.net" } }
    ],
    "backend": [],
    "outbound": [
      { "set-header": { "name": "X-Powered-By", "value": "Azure" } }
    ],
    "on-error": [
      { "log-to-eventhub": { "logger-id": "error-logger" } }
    ]
  }
}
```

## 🔐 Sécurité Azure

### Managed Identity

```typescript
// ✅ Bon - Utiliser Managed Identity
import { DefaultAzureCredential } from "@azure/identity";
import { BlobServiceClient } from "@azure/storage-blob";

const credential = new DefaultAzureCredential();
const blobServiceClient = new BlobServiceClient(
  `https://${accountName}.blob.core.windows.net`,
  credential
);

// ❌ Mauvais - Connection string en dur
const blobServiceClient = BlobServiceClient.fromConnectionString(
  "DefaultEndpointsProtocol=https;AccountName=..."
);
```

### Key Vault

```typescript
// ✅ Bon - Secrets dans Key Vault
import { SecretClient } from "@azure/keyvault-secrets";
import { DefaultAzureCredential } from "@azure/identity";

const credential = new DefaultAzureCredential();
const vaultUrl = `https://${keyVaultName}.vault.azure.net`;
const client = new SecretClient(vaultUrl, credential);

const secret = await client.getSecret("database-password");
const connectionString = buildConnectionString(secret.value);
```

## 📊 Monitoring et Observabilité

### Application Insights

```typescript
import { ApplicationInsights } from '@azure/monitor-opentelemetry';

// Configuration
ApplicationInsights.setup()
  .setAutoCollectRequests(true)
  .setAutoCollectPerformance(true)
  .setAutoCollectExceptions(true)
  .setAutoCollectDependencies(true)
  .start();

// Logs personnalisés
const client = ApplicationInsights.defaultClient;
client.trackEvent({ name: "OrderPlaced", properties: { orderId, amount } });
client.trackMetric({ name: "OrderProcessingTime", value: duration });

// Distributed tracing
client.trackDependency({
  dependencyTypeName: "HTTP",
  name: "GET /api/users",
  data: "https://api.example.com/users",
  duration: 245,
  resultCode: 200,
  success: true
});
```

### Structured Logging

```typescript
// ✅ Bon - Logs structurés
logger.info('Order processed', {
  orderId: order.id,
  userId: order.userId,
  amount: order.total,
  processingTime: duration,
  timestamp: new Date().toISOString()
});

// ❌ Mauvais - Logs non structurés
console.log(`Order ${order.id} processed for user ${order.userId}`);
```

## 🚀 Déploiement

### Infrastructure as Code (Bicep)

```bicep
// main.bicep
param location string = resourceGroup().location
param environment string = 'dev'

// Container Apps Environment
resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: 'cae-${environment}'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

// Container App
resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'ca-api-${environment}'
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
      }
      secrets: [
        {
          name: 'container-registry-password'
          value: containerRegistry.listCredentials().passwords[0].value
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'api'
          image: 'myregistry.azurecr.io/api:latest'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 10
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '50'
              }
            }
          }
        ]
      }
    }
  }
}
```

### CI/CD avec GitHub Actions

```yaml
# .github/workflows/deploy-azure.yml
name: Deploy to Azure

on:
  push:
    branches: [main]

env:
  AZURE_CONTAINER_REGISTRY: myregistry.azurecr.io
  CONTAINER_APP_NAME: ca-api-prod
  RESOURCE_GROUP: rg-prod

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Build and Push Docker Image
        run: |
          az acr build \
            --registry ${{ env.AZURE_CONTAINER_REGISTRY }} \
            --image api:${{ github.sha }} \
            --image api:latest \
            .
      
      - name: Deploy to Container Apps
        run: |
          az containerapp update \
            --name ${{ env.CONTAINER_APP_NAME }} \
            --resource-group ${{ env.RESOURCE_GROUP }} \
            --image ${{ env.AZURE_CONTAINER_REGISTRY }}/api:${{ github.sha }}
```

## 💰 Optimisation des Coûts

### Bonnes Pratiques

1. **Compute**
   - Utiliser serverless (Functions, Container Apps) pour charges variables
   - Activer auto-scaling avec limites appropriées
   - Arrêter les environnements non-prod la nuit

2. **Storage**
   - Utiliser les tiers appropriés (Hot/Cool/Archive)
   - Activer lifecycle management
   - Supprimer les anciennes versions de blobs

3. **Database**
   - Choisir le bon SKU (Basic/Standard/Premium)
   - Utiliser reserved capacity pour charges prévisibles
   - Activer auto-pause pour SQL Database serverless

4. **Monitoring**
   - Limiter la rétention des logs
   - Utiliser sampling pour Application Insights
   - Archiver les vieux logs dans Blob Storage

## 📋 Checklist Déploiement Azure

### Sécurité
- [ ] Managed Identity activée
- [ ] Secrets dans Key Vault
- [ ] Network Security Groups configurés
- [ ] HTTPS/TLS activé
- [ ] Azure AD authentication configurée

### Monitoring
- [ ] Application Insights configuré
- [ ] Alertes définies
- [ ] Dashboards créés
- [ ] Structured logging implémenté

### Haute Disponibilité
- [ ] Multi-région si critique
- [ ] Auto-scaling configuré
- [ ] Health checks définis
- [ ] Backup/Recovery plan

### Performance
- [ ] CDN pour contenus statiques
- [ ] Caching activé (Redis)
- [ ] Connection pooling
- [ ] Query optimization

### Coûts
- [ ] Tags pour cost tracking
- [ ] Budget alerts configurés
- [ ] Reserved capacity évalué
- [ ] Unused resources supprimés
