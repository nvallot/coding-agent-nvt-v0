# 📚 Exemples d'utilisation des Agents

## Scénario 1: Nouveau projet data complet

### Étape 1️⃣: Recueil d'exigences avec Business Analyst

**Context**: Vous avez un besoin de refondre votre système de rapports financiers

```bash
# Ouvrir un fichier de requirements
# Ex: docs/requirements/financial-reporting-v2.md

@ba "Analyser les exigences pour la refonte du système de rapports financiers.
     
Contexte:
- Client: NADIA
- Système source: SAP ERP v6.0
- Volume: 500 rapports/jour
- SLA: Disponibilité 99.9%, latence < 2h

Analyser:
1. Besoins métier actuels vs futurs
2. Sources de données
3. Transformations nécessaires
4. Livrables attendus
5. Risques identifiés"

# @ba charge automatiquement: .github/instructions/business-analyst.instructions.md
# Grâce au pattern: **/requirements/**,**/specifications/**,**/docs/**
```

**Livrables produits**:
- BRD structuré (Business Requirements Document)
- Data mapping détaillé
- User stories avec acceptance criteria
- Risks & Mitigations

---

### Étape 2️⃣: Conception d'architecture avec Architecte

**Context**: Les exigences sont finalisées, vous devez concevoir l'architecture

```bash
# Ouvrir un fichier d'architecture
# Ex: docs/architecture/financial-reporting-design.md

@archi "Concevoir l'architecture pour le système de rapports financiers.

Exigences (depuis BA):
[Copier le résumé des exigences]

Concevoir:
1. Architecture overview (C4 diagrams)
2. Sélection des services Azure
3. Data flow end-to-end
4. Security & compliance
5. Cost estimation
6. ADRs pour décisions majores
7. Infrastructure as Code (Terraform)"

# @archi charge automatiquement: .github/instructions/architecte.instructions.md
# Grâce au pattern: **/(docs|Deployment|architecture)/**
```

**Livrables produits**:
- Technical Architecture Document (TAD)
- Diagrammes C4 (Context, Container, Component)
- Diagrammes data flow et network
- Architecture Decision Records (ADRs)
- Terraform HCL prêt pour déploiement
- Estimation des coûts Azure détaillée

---

### Étape 3️⃣: Implémentation avec Développeur

**Context**: L'architecture est approuvée, vous devez implémenter

```bash
# Ouvrir un fichier source
# Ex: src/pipelines/financial_reports_pipeline.py

@dev "Implémenter le pipeline d'ingestion pour les rapports financiers.

Architecture (depuis Archi):
[Copier les détails architecturaux]

Implémenter:
1. Azure Data Factory pipelines
2. Databricks notebooks (transformation)
3. Synapse SQL (agrégations)
4. Azure Functions (orchestration)
5. Tests (unit, integration, E2E)
6. Documentation complète"

# @dev charge automatiquement: .github/instructions/developpeur.instructions.md
# Grâce au pattern: **/(src|Functions|Development|*.cs|*.py|*.sql|*.tf)/**
```

**Livrables produits**:
- Code production (pipelines, notebooks, functions)
- Infrastructure as Code complètement testé
- Tests automatisés (couverture >80%)
- Documentation et README
- Scripts de déploiement

---

### Étape 4️⃣: Code review avec Reviewer

**Context**: La PR est prête pour review

```bash
# Ouvrir la PR
# GitHub crée automatiquement un commentaire de review

@reviewer "Faire une revue complète de la PR #234

Critères:
1. Respect des standards de code (naming, structure)
2. Sécurité (secrets, validation, auth)
3. Performance (latency, throughput)
4. Tests (couverture, quality)
5. Compliance (GDPR, SOX, Azure WAF)

Produire un rapport détaillé avec:
- Score global
- Blockers (must fix)
- Important issues (should fix)
- Suggestions (nice to have)
- Security audit
- Performance assessment"

# @reviewer charge automatiquement: .github/instructions/reviewer.instructions.md
# Grâce au pattern: **/(pull_requests|*.cs|*.py|*.sql)/**
# Avec excludeAgent: coding-agent (code-review uniquement)
```

**Livrables produits**:
- Rapport de revue détaillé
- Score par catégorie
- Listes d'action (blockers, warnings, suggestions)
- Security & compliance assessment
- Performance recommendations

---

## Scénario 2: Amélioration d'un composant existant

### 🎯 Ajouter une nouvelle source de données

```bash
# 1. Business Analyst: Analyser l'impact métier
cd docs/requirements
# Fichier: new-data-source-analysis.md
@ba "Analyser l'intégration d'une nouvelle source de données
     - Nom: Système CRM Salesforce
     - Volume: 10K records/jour
     - Latence required: < 30min
     - Fréquence: Temps réel (streaming)"

# 2. Architecte: Concevoir l'ajout architectural
cd docs/architecture
# Fichier: cad-salesforce-integration.md
@archi "Conception de l'intégration Salesforce
        - Impact sur l'architecture existante
        - Nouvelles connexions/pipelines
        - Modif du data model
        - Coûts additionnels"

# 3. Développeur: Implémenter
cd src
# Fichier: connectors/salesforce_ingestion.py
@dev "Implémenter le connecteur Salesforce
      - Authentification OAuth 2.0
      - Pagination et retry logic
      - Validation des données
      - Tests"

# 4. Reviewer: Code review
# PR créée par @dev
@reviewer "Revue de la nouvelle intégration Salesforce"
```

---

## Scénario 3: Refactoring de sécurité

### 🔒 Remplacer les secrets hard-codés par Key Vault

```bash
# Ouvrir un fichier avec secrets
cd src/Functions
# Fichier: config.cs

@dev "Refactorer pour utiliser Azure Key Vault au lieu des secrets hard-codés.
      
      Actuellement:
      ```csharp
      var connectionString = 'Server=prod.sql.azure.com;Password=MySecret123';
      ```
      
      Requis:
      1. Utiliser Managed Identity
      2. Récupérer secrets depuis Key Vault
      3. Gérer la rotation de secrets
      4. Ajouter les tests"

# Produit:
# - Code utilisant DefaultAzureCredential
# - SecretClient pour accès Key Vault
# - Tests pour la récupération de secrets
# - Documentation de setup
```

---

## Scénario 4: Optimisation de performance

### ⚡ Améliorer la latence d'un pipeline

```bash
# Fichier avec la query lente
cd src/SQL
# Fichier: load_gold_layer.sql

@dev "Optimiser la performance de ce script SQL.
      
      Problème actuel:
      - Execution time: 45 minutes
      - Volume: 10M rows
      - Target: < 15 minutes
      
      Analyser:
      1. Execution plan
      2. Indexes manquants
      3. Query rewrites
      4. Partitioning strategy
      
      Produire:
      - Script optimisé
      - Index creation
      - Before/after timing
      - Monitoring setup"
```

---

## Scénario 5: Documentation d'une décision majeure

### 📋 ADR: Choix entre Azure Data Factory vs Synapse Pipelines

```bash
# Ouvrir/créer le fichier ADR
cd docs/architecture/adrs
# Fichier: adr-002-orchestration-choice.md

@archi "Créer un ADR pour la sélection d'un outil d'orchestration.

Contexte:
- Pipeline data avec 50+ transformations
- Source ERP, target data warehouse
- Latency < 2h required

Options à évaluer:
1. Azure Data Factory
2. Synapse Pipelines
3. Databricks Workflows

Pour chaque option:
- Pros/Cons
- Coûts
- Performance estimée
- Limites connues

Décision recommandée avec justification
Conséquences et mitigation"
```

---

## Scénario 6: Audit de sécurité d'un composant

### 🔐 Vérifier la sécurité d'une Azure Function

```bash
# Fichier de la fonction
cd src/Functions
# Fichier: ProcessVendor.cs

@reviewer "Faire un audit de sécurité complet de cette Azure Function.

Vérifier:
1. Authentication & Authorization
2. Input validation (injection attacks)
3. Secret management (no hard-coded)
4. Encryption (data in transit/at-rest)
5. Error handling (no info leakage)
6. Logging (audit trail)
7. Compliance (GDPR, SOX)
8. Dependency vulnerabilities

Produire un rapport détaillé avec:
- Score de sécurité
- Blockers
- Recommandations
- Ressources pour remediation"
```

---

## 🎯 Pattern Selection Reference

### Quand utiliser @ba (Business Analyst)

Travaillez dans ces dossiers:
```
docs/requirements/
docs/specifications/
docs/stakeholder-analysis/
docs/data-mapping/
docs/user-stories/
```

### Quand utiliser @archi (Architecte)

Travaillez dans ces dossiers:
```
docs/architecture/
Deployment/
Deployment/Terraform/
docs/diagrams/
docs/adrs/
```

### Quand utiliser @dev (Développeur)

Travaillez avec ces fichiers:
```
src/**/*.py
src/**/*.cs
src/**/*.sql
src/**/*.tf
Functions/
Development/
tests/
infrastructure/
```

### Quand utiliser @reviewer

C'est automatique pour les PR!
```
Pull requests
*.cs files
*.py files
*.sql files
```

---

## 💡 Tips & Tricks

### 1️⃣ Charger le contexte client automatiquement

```bash
@ba "Client: [clientKey]. Analyser les exigences pour..."
# Ajoute le contexte du client en début de prompt
```

### 2️⃣ Référencer un livreable précédent

```bash
@dev "Implémenter selon l'architecture dans:
      - docs/architecture/tad-orders.md
      - Deployment/Terraform/
      - docs/adrs/adr-*.md"
```

### 3️⃣ Spécifier des contraintes

```bash
@dev "Implémenter avec:
      - Naming: {client}-{component}-{env}
      - Logging: JSON structured logs
      - Testing: >80% coverage
      - Documentation: README + docstrings"
```

### 4️⃣ Demander un livreable spécifique

```bash
@archi "Produit uniquement:
        1. C4 Context diagram (Mermaid)
        2. 3 ADRs pour décisions clés
        3. Terraform main.tf structure
        
        Format: Markdown avec code blocks"
```

---

## 🔍 Vérifier l'application des instructions

```bash
# Pour vérifier qu'une instruction s'applique:

# 1. Ouvrir un fichier matching le pattern
cd src/Functions
touch ProcessOrder.cs

# 2. Ouvrir le chat Copilot
# Les instructions devraient être chargées automatiquement

# 3. Vérifier dans le chat qu'il reconnaît l'agent
# Exemple message Copilot:
# "📋 I'm using Developer agent instructions for Azure Functions"
```

---

## 📞 Support & Questions

### Instruction ne s'applique pas?
1. ✅ Vérifier le chemin du fichier vs `applyTo` pattern
2. ✅ Fermer et rouvrir le chat Copilot
3. ✅ Recharger le repository dans GitHub Copilot
4. ✅ Consulter `.github/instructions/README.md`

### Besoin de modifier les instructions?
1. Éditer `.github/instructions/{agent}.instructions.md`
2. Commit et push
3. Les changements s'appliquent immédiatement

### Créer un nouvel agent?
1. Créer `.github/agents/{agent-name}.md`
2. Créer `.github/instructions/{agent-name}.instructions.md`
3. Ajouter le frontmatter avec `applyTo`
4. Documenter dans `AGENTS.md`

---

**Exemples Copilot Version**: 1.0.0  
**Last Updated**: 2026-02-04
