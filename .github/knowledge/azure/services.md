# Azure Services - Guide de Référence

## 🎯 Catalogue des Services

### Compute Services

#### Azure Functions
**Type**: Serverless FaaS (Function as a Service)

**Cas d'usage**:
- Traitement d'événements
- Tâches planifiées (CRON)
- Webhooks et API légères
- Intégrations avec services externes

**Triggers supportés**:
- HTTP
- Timer (CRON)
- Service Bus Queue/Topic
- Event Grid
- Event Hubs
- Blob Storage
- Cosmos DB
- Queue Storage

**Pricing**:
- Consumption Plan: Pay-per-execution
- Premium Plan: Pre-warmed instances
- Dedicated Plan: App Service Plan

**Limites**:
- Timeout: 5-10 min (Consumption), illimité (Dedicated)
- Memory: Jusqu'à 1.5 GB (Consumption)
- Execution count: Illimité

**Exemple**:
```typescript
import { app, HttpRequest, HttpResponseInit, InvocationContext } from "@azure/functions";

export async function httpTrigger(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
    context.log('HTTP trigger function processed request.');
    
    const name = request.query.get('name') || await request.text() || 'World';

    return { 
        body: `Hello, ${name}!`,
        headers: { 'Content-Type': 'application/json' }
    };
}

app.http('httpTrigger', {
    methods: ['GET', 'POST'],
    authLevel: 'function',
    handler: httpTrigger
});
```

#### Azure Container Apps
**Type**: Serverless Containers

**Cas d'usage**:
- Microservices
- APIs avec auto-scaling
- Background workers
- Applications conteneurisées sans gestion d'infrastructure

**Features**:
- Auto-scaling (0 à N replicas)
- Ingress HTTP/HTTPS
- Blue-green deployments
- Dapr integration
- KEDA scaling rules

**Pricing**:
- Pay per vCPU and memory
- Scale to zero capability

**Configuration**:
```yaml
properties:
  template:
    containers:
      - name: myapp
        image: myregistry.azurecr.io/myapp:v1
        resources:
          cpu: 0.5
          memory: 1Gi
    scale:
      minReplicas: 0
      maxReplicas: 10
      rules:
        - name: http-rule
          http:
            metadata:
              concurrentRequests: '50'
```

#### Azure Kubernetes Service (AKS)
**Type**: Managed Kubernetes

**Cas d'usage**:
- Applications complexes multi-conteneurs
- Orchestration avancée
- Service mesh (Istio, Linkerd)
- Contrôle total de l'infrastructure

**Features**:
- Kubernetes natif
- Auto-scaling (HPA, VPA, Cluster Autoscaler)
- Azure Monitor integration
- Azure AD integration
- Virtual nodes (ACI integration)

**Node Pools**:
- System node pool: Services système
- User node pools: Workloads applicatifs

**Networking**:
- Kubenet (basic)
- Azure CNI (advanced)
- Private clusters

#### Azure App Service
**Type**: PaaS pour applications web

**Cas d'usage**:
- Applications web (.NET, Java, Node.js, Python, PHP)
- APIs RESTful
- Applications mobiles backend
- Applications WordPress

**Features**:
- Auto-scaling
- Deployment slots
- Custom domains et SSL
- Continuous deployment
- Easy Auth

**Tiers**:
- Free/Shared: Dev/test
- Basic: Petit trafic
- Standard: Production
- Premium: Haute disponibilité
- Isolated: Dédié, VNet isolation

### Storage Services

#### Azure Blob Storage
**Type**: Object storage

**Cas d'usage**:
- Stockage de fichiers (images, vidéos, documents)
- Backups
- Data lakes
- Static website hosting

**Tiers**:
- **Hot**: Accès fréquent
- **Cool**: Accès occasionnel (> 30 jours)
- **Cold**: Accès rare (> 90 jours)
- **Archive**: Accès très rare (> 180 jours)

**Features**:
- Versioning
- Soft delete
- Lifecycle management
- Immutable storage
- Change feed

**Exemple**:
```typescript
import { BlobServiceClient } from "@azure/storage-blob";
import { DefaultAzureCredential } from "@azure/identity";

const credential = new DefaultAzureCredential();
const blobServiceClient = new BlobServiceClient(
  `https://${accountName}.blob.core.windows.net`,
  credential
);

const containerClient = blobServiceClient.getContainerClient('mycontainer');
const blockBlobClient = containerClient.getBlockBlobClient('myblob.txt');

await blockBlobClient.uploadData(Buffer.from('Hello, World!'));
```

#### Azure Cosmos DB
**Type**: NoSQL multi-model database

**Cas d'usage**:
- Applications globalement distribuées
- Gaming leaderboards
- IoT data
- Catalogs et inventaires
- Social networks

**APIs supportées**:
- NoSQL (native)
- MongoDB
- Cassandra
- Gremlin (Graph)
- Table

**Consistency Levels**:
1. Strong: Cohérence immédiate
2. Bounded Staleness: Latence contrôlée
3. Session: Cohérence par session
4. Consistent Prefix: Ordre garanti
5. Eventual: Cohérence finale

**Pricing**:
- Provisioned throughput (RU/s)
- Serverless (pay-per-request)
- Autoscale

**Exemple**:
```typescript
import { CosmosClient } from "@azure/cosmos";

const client = new CosmosClient({
  endpoint: "https://myaccount.documents.azure.com:443/",
  key: process.env.COSMOS_KEY
});

const database = client.database("mydb");
const container = database.container("items");

const { resource } = await container.items.create({
  id: "1",
  category: "electronics",
  name: "Laptop",
  price: 999.99
});
```

#### Azure SQL Database
**Type**: Managed relational database

**Cas d'usage**:
- Applications OLTP
- Data warehousing (Hyperscale)
- Applications LOB

**Tiers**:
- **DTU-based**: Basic, Standard, Premium
- **vCore-based**: General Purpose, Business Critical, Hyperscale

**Features**:
- Automatic backups
- Point-in-time restore
- Geo-replication
- Elastic pools
- Advanced security (TDE, Always Encrypted)

**Serverless**:
- Auto-pause pendant inactivité
- Auto-scaling compute
- Pay per second

### Messaging Services

#### Azure Service Bus
**Type**: Enterprise messaging

**Cas d'usage**:
- Intégrations d'entreprise
- Transactions distribuées
- Message ordering
- Message sessions
- Duplicate detection

**Queues vs Topics**:
- **Queue**: Point-to-point (1 consommateur)
- **Topic**: Pub/sub (N consommateurs via subscriptions)

**Features**:
- FIFO garantie
- Transactions
- Dead-letter queue
- Message deferral
- Scheduled messages

**Exemple**:
```typescript
import { ServiceBusClient } from "@azure/service-bus";

const client = new ServiceBusClient(connectionString);
const sender = client.createSender("myqueue");

await sender.sendMessages({
  body: { orderId: "123", amount: 99.99 },
  messageId: "order-123",
  sessionId: "user-456"
});

await sender.close();
await client.close();
```

#### Azure Event Grid
**Type**: Event routing service

**Cas d'usage**:
- Réactions à événements Azure (blob created, VM deleted)
- Événements custom d'applications
- Intégrations serverless
- Event-driven architectures

**Event Sources**:
- Azure services (Storage, Functions, etc.)
- Custom topics
- Partner events (Auth0, SAP, etc.)

**Event Handlers**:
- Azure Functions
- Logic Apps
- Event Hubs
- Service Bus
- Webhooks

**Schema**:
```json
{
  "id": "unique-event-id",
  "eventType": "Microsoft.Storage.BlobCreated",
  "subject": "/blobServices/default/containers/mycontainer/blobs/myfile.txt",
  "eventTime": "2024-01-15T12:00:00Z",
  "data": {
    "api": "PutBlob",
    "url": "https://myaccount.blob.core.windows.net/mycontainer/myfile.txt"
  }
}
```

#### Azure Event Hubs
**Type**: Big data streaming

**Cas d'usage**:
- Ingestion de télémétrie
- Logs streaming
- IoT data
- Clickstream analysis

**Features**:
- Millions d'événements/seconde
- Retention jusqu'à 90 jours
- Capture vers Blob/Data Lake
- Apache Kafka compatible

**Throughput Units**:
- 1 TU = 1 MB/s ingress, 2 MB/s egress
- Auto-inflate disponible

### Monitoring & Security

#### Application Insights
**Type**: APM (Application Performance Monitoring)

**Télémétrie collectée**:
- Requests
- Dependencies
- Exceptions
- Page views
- Custom events/metrics
- Traces (logs)

**Features**:
- Distributed tracing
- Live metrics
- Availability tests
- Smart detection (anomalies)
- Application Map

#### Azure Key Vault
**Type**: Secrets management

**Objets stockés**:
- **Secrets**: Chaînes sensibles (passwords, connection strings)
- **Keys**: Clés cryptographiques (RSA, EC)
- **Certificates**: Certificats X.509

**Features**:
- RBAC et Access Policies
- Soft delete et purge protection
- HSM-backed keys
- Key rotation
- Audit logging

**Exemple**:
```typescript
import { SecretClient } from "@azure/keyvault-secrets";
import { DefaultAzureCredential } from "@azure/identity";

const credential = new DefaultAzureCredential();
const client = new SecretClient(
  `https://${keyVaultName}.vault.azure.net`,
  credential
);

const secret = await client.getSecret("database-password");
console.log(secret.value);
```

## 🎯 Matrice de Décision

| Besoin | Service Recommandé | Alternative |
|--------|-------------------|-------------|
| API simple serverless | Functions | Container Apps |
| Microservices | Container Apps | AKS |
| Applications complexes | AKS | App Service |
| Stockage fichiers | Blob Storage | Files (SMB) |
| Database NoSQL | Cosmos DB | Table Storage |
| Database SQL | SQL Database | PostgreSQL |
| Queue simple | Queue Storage | Service Bus |
| Messaging entreprise | Service Bus | Event Hubs |
| Event routing | Event Grid | Service Bus Topics |
| Streaming big data | Event Hubs | Kafka on AKS |
| Secrets | Key Vault | App Configuration |
| Monitoring | Application Insights | Datadog/New Relic |

## 💰 Optimisation des Coûts

### Compute
- Utiliser Consumption/Serverless pour charges variables
- Reserved Instances pour charges stables
- Spot Instances pour workloads interruptibles

### Storage
- Lifecycle policies pour archivage automatique
- Tiers appropriés (Hot/Cool/Archive)
- Supprimer versions anciennes

### Database
- Serverless pour dev/test
- Elastic pools pour mutualisation
- Auto-pause pour SQL Database

### Networking
- Azure Front Door pour CDN global
- Private endpoints pour trafic interne
- Limiter les transferts inter-régions
