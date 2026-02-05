---
applyTo: "**/Development/**,**/docs/**"
type: knowledge
---

# Knowledge: Microsoft Dataverse

## 📋 Vue d'ensemble

**Microsoft Dataverse** (anciennement Common Data Service) est la plateforme de données sous-jacente à Power Platform, permettant de stocker et gérer des données métier.

## 🎯 Use Cases

- Backend pour Power Apps
- Stockage données Dynamics 365
- Intégration avec Azure (via connecteurs)
- Master Data Management simplifié
- Portails clients/fournisseurs

## 🏗️ Concepts Clés

### Environment

Container isolé avec sa propre base de données.

```
Tenant
├── Environment: Development
│   └── Dataverse Database
├── Environment: Test
│   └── Dataverse Database
└── Environment: Production
    └── Dataverse Database
```

### Tables (Entities)

Équivalent des tables SQL avec métadonnées riches.

| Type | Description |
|------|-------------|
| **Standard** | Tables Microsoft (Account, Contact) |
| **Custom** | Tables créées par l'organisation |
| **Virtual** | Données externes (SharePoint, SQL) |
| **Elastic** | Tables JSON pour données non structurées |

### Columns (Fields)

Types de colonnes supportés:

| Type | Usage |
|------|-------|
| Text | Chaînes de caractères |
| Number | Entiers, décimaux |
| Currency | Montants avec devise |
| DateTime | Dates et heures |
| Lookup | Relation vers autre table |
| Choice | Valeurs prédéfinies (picklist) |
| File/Image | Pièces jointes |
| Formula | Colonnes calculées |

### Relationships

| Type | Cardinalité | Exemple |
|------|-------------|---------|
| One-to-Many | 1:N | Account → Contacts |
| Many-to-One | N:1 | Contact → Account |
| Many-to-Many | N:N | Contact ↔ Product |

## 💻 Exemples

### Web API - Authentification

```csharp
// Managed Identity (recommandé pour Azure)
var credential = new DefaultAzureCredential();
var token = await credential.GetTokenAsync(
    new TokenRequestContext(new[] { "https://orgname.crm.dynamics.com/.default" })
);

var client = new HttpClient
{
    BaseAddress = new Uri("https://orgname.api.crm.dynamics.com/api/data/v9.2/")
};
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token.Token);
```

### Créer un enregistrement (Upsert)

```csharp
var purchaseOrder = new
{
    sbm_name = "PO-2026-001",
    sbm_vendorid = vendorId,
    sbm_amount = 15000.00m,
    sbm_orderdate = DateTime.UtcNow,
    statuscode = 918890002  // Draft
};

var content = new StringContent(
    JsonSerializer.Serialize(purchaseOrder),
    Encoding.UTF8,
    "application/json"
);

// Upsert avec alternate key
var response = await client.PatchAsync(
    $"sbm_purchaseorders(sbm_ponumber='{poNumber}')",
    content
);
```

### Requêter avec OData

```csharp
// Filtrer et sélectionner
var query = "sbm_purchaseorders" +
    "?$select=sbm_name,sbm_amount,sbm_orderdate" +
    "&$filter=statuscode eq 918890002 and sbm_amount gt 10000" +
    "&$orderby=sbm_orderdate desc" +
    "&$top=100";

var response = await client.GetAsync(query);
var data = await response.Content.ReadFromJsonAsync<ODataResponse<PurchaseOrder>>();
```

### Lookup (relation)

```csharp
// Créer avec lookup vers Account
var order = new
{
    sbm_name = "PO-2026-002",
    // Format: entityname@odata.bind
    ["sbm_vendorid@odata.bind"] = $"/accounts({vendorAccountId})"
};
```

### Batch Operations

```csharp
var batchContent = new MultipartContent("mixed", $"batch_{Guid.NewGuid()}");

foreach (var order in orders)
{
    var changeSet = new MultipartContent("mixed", $"changeset_{Guid.NewGuid()}");
    
    var request = new HttpRequestMessage(HttpMethod.Post, "sbm_purchaseorders")
    {
        Content = JsonContent.Create(order)
    };
    request.Headers.Add("Content-ID", order.Id);
    
    changeSet.Add(new HttpMessageContent(request));
    batchContent.Add(changeSet);
}

var batchRequest = new HttpRequestMessage(HttpMethod.Post, "$batch")
{
    Content = batchContent
};

var response = await client.SendAsync(batchRequest);
```

## 🔧 Status Codes Conventions

### Status Reason (statuscode)

Convention SBM pour tables custom:

| Code | Label | Description |
|------|-------|-------------|
| 918890000 | Draft | Brouillon, en création |
| 918890001 | Submitted | Soumis, en attente validation |
| 918890002 | Approved | Approuvé |
| 918890003 | Rejected | Rejeté |
| 918890004 | Cancelled | Annulé |
| 918890005 | Completed | Terminé |

### State (statecode)

| Code | Label |
|------|-------|
| 0 | Active |
| 1 | Inactive |

## ✅ Bonnes Pratiques

### Naming Conventions

- **Publisher prefix**: `sbm_` (ou autre préfixe client)
- **Tables**: `sbm_purchaseorder` (singulier, snake_case après préfixe)
- **Columns**: `sbm_ordernumber`, `sbm_totalamount`

### Performance

- Utiliser `$select` pour limiter les colonnes
- Paginer avec `$top` et `@odata.nextLink`
- Batch pour opérations multiples (max 1000/batch)
- Indexer les colonnes de filtre fréquentes

### Sécurité

- Toujours Managed Identity depuis Azure
- RBAC via Dataverse Security Roles
- Row-level security avec Business Units
- Field-level security pour données sensibles

## 💰 Coûts

| Licence | Inclus | Prix approx |
|---------|--------|-------------|
| Per User | 20K API requests/24h | ~$10-40/user/mois |
| Per App | 6K API requests/24h | ~$5/user/app/mois |
| Capacity | 1TB Database, 20TB File | Inclus + add-ons |

## 📚 Références

- [Dataverse Documentation](https://learn.microsoft.com/power-apps/maker/data-platform/)
- [Web API Reference](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/overview)
- [OData Query Options](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api)
