---
applyTo: "**/docs/**,**/architecture/**"
type: knowledge
---

# Knowledge: Data Modeling Reference

## 📋 Vue d'ensemble

Concepts et types de modèles de données pour architectures data.

## 🏗️ Types de Modèles

### Modèle Conceptuel

**Niveau**: Business, entités métier  
**Audience**: Business stakeholders, analystes  
**Outils**: Draw.io, Lucidchart, Visio

```
┌─────────────┐       ┌─────────────┐
│   Client    │──────<│  Commande   │
└─────────────┘  passe└─────────────┘
                           │
                           │ contient
                           ▼
                      ┌─────────────┐
                      │   Produit   │
                      └─────────────┘
```

**Caractéristiques**:
- Pas de types de données
- Pas de contraintes techniques
- Relations en langage métier
- Indépendant de la technologie

### Modèle Logique

**Niveau**: Design, attributs et relations  
**Audience**: Data architects, développeurs  
**Outils**: ERwin, PowerDesigner, dbdiagram.io

```
Customer                    Order
├── CustomerID (PK)         ├── OrderID (PK)
├── Name                    ├── CustomerID (FK)
├── Email                   ├── OrderDate
└── CreatedAt               ├── TotalAmount
                            └── Status

OrderLine
├── OrderLineID (PK)
├── OrderID (FK)
├── ProductID (FK)
├── Quantity
└── UnitPrice
```

**Caractéristiques**:
- Attributs avec types logiques
- Clés primaires et étrangères
- Contraintes (NOT NULL, UNIQUE)
- Indépendant du SGBD

### Modèle Physique

**Niveau**: Implémentation, optimisation  
**Audience**: DBA, développeurs  
**Outils**: SSMS, Azure Data Studio, specific DB tools

```sql
CREATE TABLE dbo.Orders (
    OrderID         BIGINT IDENTITY(1,1) PRIMARY KEY,
    CustomerID      BIGINT NOT NULL,
    OrderDate       DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    TotalAmount     DECIMAL(18,2) NOT NULL,
    Status          TINYINT NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    ModifiedAt      DATETIME2(0) NULL,
    
    CONSTRAINT FK_Orders_Customers 
        FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(CustomerID),
    
    INDEX IX_Orders_CustomerID (CustomerID),
    INDEX IX_Orders_OrderDate (OrderDate DESC)
);
```

**Caractéristiques**:
- Types de données spécifiques au SGBD
- Indexes, partitions
- Contraintes physiques
- Optimisations (compression, clustering)

## 📊 Patterns de Modélisation

### Star Schema (Dimensional)

```
                    ┌─────────────────┐
                    │   Dim_Customer  │
                    └────────┬────────┘
                             │
┌─────────────┐    ┌─────────┴─────────┐    ┌─────────────┐
│  Dim_Date   │────│    Fact_Sales     │────│ Dim_Product │
└─────────────┘    └───────────────────┘    └─────────────┘
                             │
                    ┌────────┴────────┐
                    │   Dim_Store     │
                    └─────────────────┘
```

**Use case**: Reporting, analytics, BI  
**Avantages**: Queries simples, performance  
**Inconvénients**: Redondance, maintenance dimensions

### Snowflake Schema

```
                    ┌───────────┐
                    │Dim_Country│
                    └─────┬─────┘
                          │
┌─────────────┐    ┌──────┴──────┐    ┌─────────────┐
│  Dim_Date   │    │ Dim_Customer│    │Dim_Category │
└─────────────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                   │
       └──────────────────┼───────────────────┘
                    ┌─────┴─────┐
                    │Fact_Sales │
                    └───────────┘
```

**Use case**: Économie de stockage, normalisation  
**Avantages**: Moins de redondance  
**Inconvénients**: Queries plus complexes, jointures

### Data Vault

```
┌─────────────────┐
│   Hub_Customer  │
│   - CustomerBK  │
│   - LoadDate    │
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
┌───┴───┐ ┌───┴───────────┐
│ Sat_  │ │ Link_Customer │
│Customer│ │    _Order    │
│ Details│ └───────┬──────┘
└────────┘         │
              ┌────┴────┐
              │Hub_Order│
              └─────────┘
```

**Composants**:
- **Hub**: Business keys (identifiants métier)
- **Link**: Relations entre hubs
- **Satellite**: Attributs descriptifs avec historique

**Use case**: Data Lake, historique complet, auditabilité

## 📈 Tables de Rétention

| Layer | Type données | Rétention | Storage Tier | Access |
|-------|-------------|-----------|--------------|--------|
| Bronze | Raw, tous formats | 90 jours | Hot/Cool | Fréquent |
| Silver | Cleaned, Parquet | 2 ans | Cool | Modéré |
| Gold | Aggregated | 5 ans | Cool | Analytics |
| Archive | Compliance | 7+ ans | Archive | Rare |

### Politique par domaine

| Domaine | Bronze | Silver | Gold |
|---------|--------|--------|------|
| Finance | 90j | 7 ans | 10 ans |
| RH | 30j | 2 ans | 5 ans |
| Operations | 7j | 90j | 1 an |
| Logs | 30j | 90j | N/A |

## 🔐 Classification des Données

| Niveau | Description | Exemples | Contrôles |
|--------|-------------|----------|-----------|
| **Public** | Information publique | Catalogue produits | Aucun |
| **Internal** | Usage interne | Rapports, KPIs | Auth employé |
| **Confidential** | Business sensitive | Contrats, pricing | Need-to-know |
| **Restricted** | Highly sensitive | PII, financier | Encryption, audit |

## 📚 Références

- [Data Modeling Fundamentals](https://learn.microsoft.com/azure/synapse-analytics/sql-data-warehouse/sql-data-warehouse-tables-overview)
- [Dimensional Modeling](https://www.kimballgroup.com/data-warehouse-business-intelligence-resources/kimball-techniques/dimensional-modeling-techniques/)
- [Data Vault](https://datavaultalliance.com/news/dv-modeling/)
