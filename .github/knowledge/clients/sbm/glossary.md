---
type: knowledge
clientKey: sbm
---

# SBM Glossary

> **Chargement**: Manuel par agents quand `clientKey=sbm` dans `active-client.json`

## 📋 Purpose

This glossary contains SBM-specific terminology, abbreviations, and business concepts. Reference this file when working on SBM projects to ensure consistent naming and understanding.

## 🏢 Organization

| Term | Full Name | Description |
|------|-----------|-------------|
| **SBM** | SBM Offshore | Leader mondial des systèmes flottants de production offshore |
| **ISP** | Information Systems Platform | Plateforme interne de systèmes d'information |

## 💻 Systems & Applications

| Acronym | Full Name | Description |
|---------|-----------|-------------|
| **Lucy** | Lucy HR System | Système de gestion RH (employés, compétences, absences) |
| **PKM** | Package Master | Système de gestion des packages de travail |
| **ERP** | Enterprise Resource Planning | Dynamics NAV/Business Central |
| **MDM** | Master Data Management | Référentiel maître pour les données |
| **NAV** | Microsoft Dynamics NAV | ERP legacy (migration vers BC en cours) |
| **BC** | Business Central | Microsoft Dynamics 365 Business Central (nouvel ERP) |

## 🌐 Infrastructure

| Term | Description |
|------|-------------|
| **VNet** | Virtual Network - Réseau isolé Azure pour SBM |
| **NSG** | Network Security Group - Règles firewall |
| **CMK** | Customer-Managed Keys - Clés de chiffrement gérées par SBM |
| **PE** | Private Endpoint - Point d'accès privé aux services Azure |

### VNet Naming Convention

```
sbm-{project}-{env}-vnet-{region}-{instance}

Exemples:
- sbm-isp-prd-vnet-weu-001  (Production West Europe)
- sbm-isp-dev-vnet-weu-001  (Development)
```

### Subnet Naming

```
snet-{purpose}-{env}

Exemples:
- snet-data-prd        (Data services)
- snet-compute-prd     (Compute/Functions)
- snet-integration-prd (Integration services)
```

## 📊 Data Domains

| Domain | Description | Source Systems | Cadence |
|--------|-------------|----------------|---------|
| **Finance** | Comptabilité, budgets, reporting financier | ERP (NAV/BC) | Daily 6h UTC |
| **Procurement** | Achats, fournisseurs, PO, contracts | ERP + ISP | Near real-time |
| **Projects** | Projets offshore, packages engineering | PKM, ISP | Daily |
| **HR** | Employés, compétences, certifications | Lucy | Weekly |
| **Operations** | Production, maintenance, safety | ISP | Real-time |

## 🔢 Identifiers (MDM)

| ID Type | Format | Example | Description |
|---------|--------|---------|-------------|
| **MDM Vendor ID** | `VND-XXXXXX` | `VND-001234` | Identifiant unique fournisseur |
| **MDM Customer ID** | `CUS-XXXXXX` | `CUS-000012` | Identifiant unique client |
| **MDM Employee ID** | Entra Object ID | GUID | Basé sur Azure AD/Entra ID |
| **Project Number** | `PRJ-XXXX-YYYY` | `PRJ-2026-0042` | Numéro projet (année-séquence) |
| **PO Number** | `PO-XXXXXXXX` | `PO-20260145` | Numéro commande d'achat |
| **Work Package** | `WP-XXX-XXXX` | `WP-ENG-0023` | Package de travail (domaine-séquence) |

## 🔄 CorrelationId Format

Format obligatoire pour traçabilité:

```
SBM-{DOMAIN}-{YYYYMMDD}-{UUID}

Exemples:
- SBM-PROCUREMENT-20260203-a1b2c3d4-e5f6-7890-abcd-ef1234567890
- SBM-FINANCE-20260203-b2c3d4e5-f6a7-8901-bcde-f12345678901
- SBM-HR-20260203-c3d4e5f6-a7b8-9012-cdef-123456789012
```

**Domains valides**: `PROCUREMENT`, `FINANCE`, `HR`, `PROJECTS`, `OPERATIONS`, `SYSTEM`

## 🏷️ Naming Convention

### General Pattern

```
{company}-{project}-{env}-{resource}-{region}-{instance}

sbm-{project}-{env}-{type}-{region}-{nnn}
```

### Resource Prefixes

| Resource | Prefix | Example |
|----------|--------|---------|
| Resource Group | `rg` | `sbm-isp-prd-rg-weu-001` |
| Storage Account | `st` | `sbmispprdst weu001` (no hyphens) |
| Data Factory | `adf` | `sbm-isp-prd-adf-weu-001` |
| Databricks | `dbw` | `sbm-isp-prd-dbw-weu-001` |
| Function App | `func` | `sbm-isp-prd-func-weu-001` |
| Key Vault | `kv` | `sbm-isp-prd-kv-weu-001` |
| Event Hub | `evh` | `sbm-isp-prd-evh-weu-001` |
| Service Bus | `sb` | `sbm-isp-prd-sb-weu-001` |
| SQL Database | `sql` | `sbm-isp-prd-sql-weu-001` |
| Synapse | `syn` | `sbm-isp-prd-syn-weu-001` |
| Purview | `pview` | `sbm-isp-prd-pview-weu-001` |

### Environments

| Code | Name | Region(s) | Purpose |
|------|------|-----------|---------|
| `dev` | Development | West Europe | Development & experimentation |
| `tst` | Test/QA | West Europe | Testing & QA |
| `stg` | Staging | WEU + NEU | Pre-production validation |
| `prd` | Production | WEU + NEU | Production (geo-redundant) |

### Regions

| Code | Region | Role |
|------|--------|------|
| `weu` | West Europe | Primary |
| `neu` | North Europe | Secondary (DR) |

## 📈 SLA & KPIs

| Metric | Target | Description |
|--------|--------|-------------|
| **Availability** | 99.95% | Max 4h downtime/year |
| **Batch Latency** | < 5 min | Time after file reception |
| **Stream Latency** | < 1 min | End-to-end processing |
| **RTO** | 2 hours | Recovery Time Objective |
| **RPO** | 1 hour | Recovery Point Objective |
| **Data Quality** | > 99.9% | Quality score target |
| **Pipeline Success** | > 99.5% | Success rate |
| **MTTR** | < 1 hour | Mean Time To Repair |

## 📚 External References

| System | Documentation |
|--------|---------------|
| Lucy API | `https://lucy.sbm.internal/api/docs` |
| PKM API | `https://pkm.sbm.internal/api/docs` |
| MDM Portal | `https://mdm.sbm.internal` |

---

## 🚧 TODO: Terms to Add

> These terms need clarification or documentation from the client:

- [ ] **FPSO**: Floating Production Storage and Offloading - définition exacte SBM
- [ ] **SPS**: à clarifier
- [ ] **FEED**: Front-End Engineering Design - process exact
- [ ] **AFE**: Authorization for Expenditure - format et workflow
- [ ] **EPCI**: Engineering, Procurement, Construction, Installation - phases
- [ ] **RFQ**: Request for Quote - process Supplier Portal
- [ ] **GBS**: à clarifier (Global Business Services?)
- [ ] **WBS**: Work Breakdown Structure - niveaux hiérarchiques
- [ ] **OBS**: Organization Breakdown Structure - structure exacte
- [ ] **EPC**: Engineering, Procurement, Construction - différence avec EPCI

---

**Version**: 1.0.0  
**Last Updated**: 2026-02-05  
**Owner**: Data Integration Team
