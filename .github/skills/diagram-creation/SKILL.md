# Skill: Diagram Creation

## 🎯 Objectif

Créer des diagrammes d'architecture professionnels et clairs pour documenter et communiquer les solutions techniques.

## 📋 Types de Diagrammes

### 1. C4 Model

#### Level 1: Context Diagram
Montre le système dans son contexte avec acteurs et systèmes externes.

```mermaid
C4Context
    title System Context - Data Platform
    
    Person(analyst, "Data Analyst", "Analyse les données")
    Person(engineer, "Data Engineer", "Maintient la plateforme")
    
    System(platform, "Data Platform", "Plateforme analytics Azure")
    
    System_Ext(erp, "ERP", "Système source")
    System_Ext(crm, "CRM", "Données clients")
    System_Ext(powerbi, "Power BI", "Visualisation")
    
    Rel(analyst, platform, "Consomme données", "HTTPS")
    Rel(engineer, platform, "Maintient", "Azure Portal")
    Rel(platform, erp, "Ingère données", "API/Files")
    Rel(platform, crm, "Ingère données", "API")
    Rel(platform, powerbi, "Alimente", "Direct Query")
```

#### Level 2: Container Diagram
Détaille les conteneurs applicatifs principaux.

```mermaid
C4Container
    title Container Diagram - Data Platform
    
    System_Boundary(platform, "Data Platform") {
        Container(adf, "Azure Data Factory", "Orchestration", "Pipelines ETL")
        ContainerDb(adls, "ADLS Gen2", "Data Lake", "Bronze/Silver/Gold")
        Container(databricks, "Databricks", "Processing", "Spark")
        ContainerDb(synapse, "Synapse SQL", "DWH", "Analytics")
        Container(function, "Functions", "Triggers", "Event-driven")
    }
    
    System_Ext(source, "Source Systems", "ERP, CRM")
    System_Ext(powerbi, "Power BI", "BI Tool")
    
    Rel(source, adf, "Fichiers/API", "HTTPS")
    Rel(adf, adls, "Écrit", "ABFS")
    Rel(adf, databricks, "Trigger", "REST API")
    Rel(databricks, adls, "Lit/Écrit", "ABFS")
    Rel(databricks, synapse, "Charge", "JDBC")
    Rel(adls, function, "Trigger", "Event Grid")
    Rel(synapse, powerbi, "Requêtes", "TDS")
```

### 2. Sequence Diagram
Montre le flux d'exécution temporel.

```mermaid
sequenceDiagram
    participant User
    participant ADF as Azure Data Factory
    participant ADLS as ADLS Gen2
    participant DBW as Databricks
    participant SQL as Synapse SQL
    
    User->>ADF: Trigger Pipeline (Manual/Schedule)
    activate ADF
    
    ADF->>ADLS: Copy files to Bronze
    activate ADLS
    ADLS-->>ADF: Success
    deactivate ADLS
    
    ADF->>DBW: Execute Notebook
    activate DBW
    DBW->>ADLS: Read Bronze
    DBW->>ADLS: Write Silver (cleaned)
    DBW->>ADLS: Write Gold (aggregated)
    DBW->>SQL: Load dimensions
    DBW->>SQL: Load facts
    DBW-->>ADF: Success
    deactivate DBW
    
    ADF->>User: Pipeline Completed
    deactivate ADF
```

### 3. Network Diagram
Architecture réseau et sécurité.

```mermaid
graph TB
    subgraph "Internet"
        Users[👥 Users]
        Sources[📦 Source Systems]
    end
    
    subgraph "Azure VNet: data-vnet (10.0.0.0/16)"
        subgraph "Subnet: ingestion (10.0.1.0/24)"
            ADF[🔧 Data Factory<br/>Private Endpoint]
        end
        
        subgraph "Subnet: storage (10.0.2.0/24)"
            ADLS[💾 ADLS Gen2<br/>Private Endpoint]
        end
        
        subgraph "Subnet: compute (10.0.3.0/24)"
            DBW[⚡ Databricks<br/>Private Endpoint]
        end
        
        subgraph "Subnet: management (10.0.254.0/24)"
            Bastion[🔐 Bastion]
        end
    end
    
    subgraph "Shared Services"
        KV[🔑 Key Vault]
        LA[📊 Log Analytics]
        FW[🛡️ Azure Firewall]
    end
    
    Sources -->|HTTPS| FW
    FW --> ADF
    Users -->|HTTPS| FW
    FW --> Bastion
    
    ADF -->|Private Link| ADLS
    ADF -->|Private Link| DBW
    DBW -->|Private Link| ADLS
    
    ADF -.->|Secrets| KV
    DBW -.->|Secrets| KV
    ADF -.->|Logs| LA
    DBW -.->|Logs| LA
    
    style ADF fill:#0078D4
    style ADLS fill:#0078D4
    style DBW fill:#FF6C37
    style KV fill:#FFB900
    style FW fill:#F25022
```

### 4. Data Flow Diagram
Flux de données end-to-end.

```mermaid
graph LR
    subgraph "Sources"
        ERP[(ERP<br/>CSV)]
        CRM[(CRM<br/>API)]
        IOT[(IoT<br/>Stream)]
    end
    
    subgraph "Ingestion"
        ADF[Azure Data Factory]
        EH[Event Hubs]
    end
    
    subgraph "Bronze Layer"
        B1[(Raw ERP)]
        B2[(Raw CRM)]
        B3[(Raw IoT)]
    end
    
    subgraph "Silver Layer"
        S1[(Cleaned Sales)]
        S2[(Cleaned Customers)]
        S3[(Cleaned Events)]
    end
    
    subgraph "Gold Layer"
        G1[(Customer 360)]
        G2[(Sales Analytics)]
        G3[(IoT Dashboard)]
    end
    
    subgraph "Consumption"
        PBI[Power BI]
        API[REST API]
        ML[ML Models]
    end
    
    ERP -->|Batch| ADF
    CRM -->|API| ADF
    IOT -->|Stream| EH
    
    ADF --> B1
    ADF --> B2
    EH --> B3
    
    B1 --> S1
    B2 --> S2
    B3 --> S3
    
    S1 & S2 --> G1
    S1 --> G2
    S3 --> G3
    
    G1 --> PBI
    G2 --> PBI
    G3 --> API
    G1 --> ML
    
    style B1 fill:#CD853F
    style B2 fill:#CD853F
    style B3 fill:#CD853F
    style S1 fill:#C0C0C0
    style S2 fill:#C0C0C0
    style S3 fill:#C0C0C0
    style G1 fill:#FFD700
    style G2 fill:#FFD700
    style G3 fill:#FFD700
```

## 🛠️ Outils

### Mermaid (Intégré)
Pour diagrammes simples et rapides, directement dans Markdown.

### DrawIO / Diagrams.net
Pour architectures complexes nécessitant plus de personnalisation.

### PlantUML
Alternative pour diagrammes UML plus traditionnels.

## ✅ Bonnes Pratiques

1. **Clarté**: Un diagramme = Un objectif
2. **Légende**: Toujours expliquer les symboles
3. **Couleurs**: Utiliser pour grouper logiquement
4. **Niveaux**: Respecter les niveaux d'abstraction (C4)
5. **Mise à jour**: Maintenir synchronisé avec code
6. **Annotations**: Ajouter notes pour décisions importantes

## 📝 Templates

Voir `.github/prompts/diagram.prompt` pour templates réutilisables.

---

**Version**: 1.0.0  
**Type**: Skill  
**Agents**: @archi (principal), @ba (context), @dev (implementation)
