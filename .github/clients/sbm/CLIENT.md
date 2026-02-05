# Client: SBM Offshore

## 📋 Contexte

**SBM Offshore** - Leader mondial des systèmes flottants de production offshore pour l'industrie pétrolière et gazière.

**Projet**: Intégration de données ERP/ISP vers Power Platform et Analytics sur Azure.

## 🎯 Priorités

1. **Sécurité Maximale**: Industrie pétrolière, données sensibles
2. **Conformité**: Standards offshore internationaux
3. **Disponibilité**: 99.95% minimum (opérations critiques)
4. **Gouvernance Stricte**: Traçabilité complète, audit logs

## 🔐 Sécurité Renforcée

- **Managed Identity**: Obligatoire pour toute ressource
- **Private Endpoints**: Tous les services exposés
- **Network Isolation**: VNet avec NSG strictes
- **Key Vault**: Chiffrement clés CMK (Customer-Managed Keys)
- **Data Classification**: Confidential, Internal, Public
- **Zero Trust**: Principe appliqué partout

## 🏗️ Architecture

### Stack Technique

**Ingestion**:
- Azure Data Factory (orchestration)
- Event Hubs (streaming temps réel)

**Storage**:
- ADLS Gen2 (Data Lake) - 3 layers (Bronze/Silver/Gold)
- Azure SQL Database (metadata, config)

**Processing**:
- Azure Databricks (transformations Spark)
- Azure Synapse Analytics (data warehouse)

**Governance**:
- Microsoft Purview (catalogue, lineage)
- Azure Policy (compliance)

**Monitoring**:
- Application Insights (APM)
- Log Analytics (centralisé)
- Azure Sentinel (SIEM - sécurité)

### Naming Convention (SBM Standard)

```
{company}-{project}-{env}-{resource}-{region}-{instance}

Exemples:
- sbm-isp-prd-rg-weu-001
- sbm-isp-prd-st-weu-001
- sbm-isp-prd-adf-weu-001
- sbm-isp-prd-dbw-weu-001
```

**Préfixes obligatoires**:
- `sbm-` pour toutes les ressources
- `isp-` pour projet ISP (Information Systems Platform)

### Environnements

- `dev` - Development (West Europe)
- `tst` - Test/QA (West Europe)
- `stg` - Staging (West Europe + North Europe)
- `prd` - Production (West Europe + North Europe - Geo-redundant)

## 📊 Data Specifics

### Sources de Données

1. **ERP (Dynamics NAV/BC)**
   - Données financières, achats, projets
   - Batch quotidien (6h00 UTC)
   - Format: CSV, 1-10 GB

2. **ISP (Internal Systems Platform)**
   - Données opérationnelles
   - Near real-time (Event Hubs)
   - Format: JSON, avro

3. **Lucy (HR System)**
   - Données RH, employés
   - Batch hebdomadaire
   - Format: Excel, API REST

### Data Domains

- **Finance**: Comptabilité, budgets
- **Procurement**: Achats, fournisseurs, PO
- **Projects**: Projets, packages, engineering
- **HR**: Employés, compétences, absences
- **Operations**: Production, maintenance

### Data Quality Rules

**Critères stricts**:
- Taux erreur max: 0.1%
- Données incomplètes rejetées
- Validation schéma obligatoire
- Alerte si volume ±20% de la normale

## 🔖 Conventions Spécifiques

### Data Mapping

Toujours utiliser les MDM IDs (Master Data Management):
- **MDM Vendor ID**: Identifiant unique fournisseur
- **MDM Customer ID**: Identifiant unique client
- **MDM Employee ID**: Entra ID ObjectId
- **Project Number**: Format `PRJ-XXXX-YYYY`

### CorrelationId

Format obligatoire:
```
SBM-{domain}-{date}-{uuid}

Exemple:
SBM-PROCUREMENT-20260203-a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

### Logging

Structured JSON avec champs obligatoires:
```json
{
  "timestamp": "2026-02-03T10:30:00Z",
  "correlationId": "SBM-PROCUREMENT-20260203-...",
  "level": "INFO",
  "domain": "procurement",
  "message": "Pipeline completed",
  "metadata": {
    "rowsProcessed": 10000,
    "duration": "00:05:30",
    "status": "success"
  }
}
```

## 🧪 Testing

### Niveaux Obligatoires

1. **Unit Tests**: 90% minimum (vs 80% standard)
2. **Integration Tests**: Tous les composants critiques
3. **E2E Tests**: Scénarios métier complets
4. **Data Quality Tests**: Sur chaque layer
5. **Performance Tests**: Load testing avant prod
6. **Security Tests**: Pen testing annuel

## ⚙️ CI/CD

### Pipeline Spécifique

```
GitHub (source) → GitHub Actions → Self-hosted runners (dans Azure)
                                  → Tests automatisés
                                  → Security scanning (SonarQube, Snyk)
                                  → Manual approval (2 personnes min pour prod)
                                  → Deployment (Blue/Green)
                                  → Smoke tests
                                  → Monitoring validation
```

**Approval Gates Production**:
- Approbation Data Lead
- Approbation Security Team
- Approbation Business Owner

## 📚 Sources de Vérité

1. `.github/clients/sbm/CLIENT.md` (ce fichier - profil client)
2. `.github/instructions/clients/sbm/` (instructions SBM - auto-chargées)
3. `.github/knowledge/clients/sbm/` (knowledge SBM - glossary, etc.)
4. `.github/instructions/` (standards génériques)

## 📋 SLA & KPIs

### SLA

- **Disponibilité**: 99.95% (max 4h downtime/an)
- **Latence Batch**: < 5 min après réception fichier
- **Latence Stream**: < 1 min end-to-end
- **RTO** (Recovery Time Objective): 2 heures
- **RPO** (Recovery Point Objective): 1 heure

### KPIs

- **Data Quality Score**: > 99.9%
- **Pipeline Success Rate**: > 99.5%
- **Processing Time**: < 30 min pour batch quotidien
- **Incident Response Time**: < 15 min
- **MTTR** (Mean Time To Repair): < 1 heure

## 🚨 Incidents & Support

### Contacts

- **Data Lead**: [email]
- **Security Team**: [email]
- **DevOps On-Call**: [Teams channel]
- **Business Owner**: [email]

### Escalation

1. L1: Data Engineering Team (response: 15 min)
2. L2: Senior Data Engineers (response: 30 min)
3. L3: Architecture & Security (response: 1h)
4. L4: Management escalation

## 📝 Notes Importantes

⚠️ **Sensibilité des Données**: Toutes les données projet contiennent des infos commerciales sensibles. Traiter avec précaution.

⚠️ **Conformité**: Respecter GDPR + standards industrie offshore.

⚠️ **Performance Critique**: Les pipelines alimentent des tableaux de bord décisionnels. Toute panne a un impact business.

⚠️ **Multi-région**: Production en West Europe (primaire) + North Europe (secondaire) pour disaster recovery.

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-03  
**Client**: SBM Offshore  
**Projet**: ISP Data Integration
