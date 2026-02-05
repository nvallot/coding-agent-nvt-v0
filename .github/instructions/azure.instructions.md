# Instructions Azure (C# / Python)

## Intended Agents

- `@dev` (Developer) - Primary user for implementation
- `@archi` (Solution Architect) - For service selection and design
- `@reviewer` (Code Reviewer) - For review purposes

---

## 🎯 Vue d'Ensemble

Instructions spécifiques pour le développement et le déploiement d'applications sur Azure avec C# et Python.

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

```csharp
// Exemple: Azure Function C# avec Service Bus
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;
    private readonly IOrderService _orderService;

    public OrderProcessor(ILogger<OrderProcessor> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [Function("ProcessOrder")]
    public async Task Run(
        [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")] 
        ServiceBusReceivedMessage message,
        CancellationToken ct)
    {
        _logger.LogInformation("Processing order: {MessageId}", message.MessageId);
        
        var order = message.Body.ToObjectFromJson<OrderMessage>();
        await _orderService.ProcessAsync(order, ct);
        
        _logger.LogInformation("Order processed: {OrderId}", order.Id);
    }
}
```

```python
# Exemple: Azure Function Python avec Service Bus
import azure.functions as func
import logging
import json

app = func.FunctionApp()

@app.service_bus_queue_trigger(arg_name="msg", queue_name="orders", connection="ServiceBusConnection")
async def process_order(msg: func.ServiceBusMessage):
    logging.info(f"Processing order: {msg.message_id}")
    
    order = json.loads(msg.get_body().decode('utf-8'))
    await process_order_async(order)
    
    logging.info(f"Order processed: {order['id']}")
```

### 3. API Management

```csharp
// Configuration APIM via Bicep/ARM - voir section IaC
// Policies APIM en XML
/*
<policies>
    <inbound>
        <rate-limit calls="100" renewal-period="60" />
        <validate-jwt header-name="Authorization" />
        <set-backend-service base-url="https://backend.azurewebsites.net" />
    </inbound>
    <backend>
        <forward-request />
    </backend>
    <outbound>
        <set-header name="X-Powered-By" exists-action="override">
            <value>Azure</value>
        </set-header>
    </outbound>
    <on-error>
        <log-to-eventhub logger-id="error-logger" />
    </on-error>
</policies>
*/
```

## 🔐 Sécurité Azure

### Managed Identity

```csharp
// ✅ Bon - Utiliser Managed Identity (C#)
using Azure.Identity;
using Azure.Storage.Blobs;

var credential = new DefaultAzureCredential();
var blobServiceClient = new BlobServiceClient(
    new Uri($"https://{accountName}.blob.core.windows.net"),
    credential);

// ❌ Mauvais - Connection string en dur
// var blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=...");
```

```python
# ✅ Bon - Utiliser Managed Identity (Python)
from azure.identity import DefaultAzureCredential
from azure.storage.blob import BlobServiceClient

credential = DefaultAzureCredential()
blob_service_client = BlobServiceClient(
    account_url=f"https://{account_name}.blob.core.windows.net",
    credential=credential
)

# ❌ Mauvais - Connection string en dur
# blob_service_client = BlobServiceClient.from_connection_string("DefaultEndpointsProtocol=https;...")
```

### Key Vault

```csharp
// ✅ Bon - Secrets dans Key Vault (C#)
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var credential = new DefaultAzureCredential();
var vaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");
var client = new SecretClient(vaultUri, credential);

KeyVaultSecret secret = await client.GetSecretAsync("database-password");
var connectionString = BuildConnectionString(secret.Value);
```

```python
# ✅ Bon - Secrets dans Key Vault (Python)
from azure.identity import DefaultAzureCredential
from azure.keyvault.secrets import SecretClient

credential = DefaultAzureCredential()
vault_url = f"https://{key_vault_name}.vault.azure.net"
client = SecretClient(vault_url=vault_url, credential=credential)

secret = client.get_secret("database-password")
connection_string = build_connection_string(secret.value)
```

## 📊 Monitoring et Observabilité

### Application Insights

```csharp
// Configuration Application Insights (C# - Program.cs)
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// Ajouter Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Logs personnalisés
builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

var app = builder.Build();

// Utilisation dans un service
public class OrderService
{
    private readonly TelemetryClient _telemetry;
    private readonly ILogger<OrderService> _logger;

    public OrderService(TelemetryClient telemetry, ILogger<OrderService> logger)
    {
        _telemetry = telemetry;
        _logger = logger;
    }

    public async Task ProcessOrderAsync(Order order)
    {
        // Event tracking
        _telemetry.TrackEvent("OrderPlaced", new Dictionary<string, string>
        {
            { "OrderId", order.Id.ToString() },
            { "Amount", order.Total.ToString() }
        });

        // Metric tracking
        _telemetry.TrackMetric("OrderProcessingTime", duration.TotalMilliseconds);

        // Structured logging
        _logger.LogInformation("Order processed: {OrderId}, Amount: {Amount}", order.Id, order.Total);
    }
}
```

```python
# Configuration Application Insights (Python)
from opencensus.ext.azure.log_exporter import AzureLogHandler
from opencensus.ext.azure import metrics_exporter
import logging

# Setup logging avec Application Insights
logger = logging.getLogger(__name__)
logger.addHandler(AzureLogHandler(connection_string='InstrumentationKey=...'))

# Custom events et metrics
def process_order(order: Order):
    # Structured logging
    logger.info(
        "Order processed",
        extra={
            "custom_dimensions": {
                "order_id": str(order.id),
                "amount": order.total
            }
        }
    )
```

### Structured Logging

```csharp
// ✅ Bon - Logs structurés (C#)
_logger.LogInformation("Order processed: {OrderId}, {UserId}, {Amount}, {ProcessingTime}",
    order.Id, order.UserId, order.Total, duration);

// ❌ Mauvais - Logs non structurés
// Console.WriteLine($"Order {order.Id} processed for user {order.UserId}");
```

```python
# ✅ Bon - Logs structurés (Python)
logger.info(
    "Order processed",
    extra={
        "order_id": str(order.id),
        "user_id": str(order.user_id),
        "amount": order.total,
        "processing_time": duration,
        "timestamp": datetime.utcnow().isoformat()
    }
)

# ❌ Mauvais - Logs non structurés
# print(f"Order {order.id} processed for user {order.user_id}")
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
