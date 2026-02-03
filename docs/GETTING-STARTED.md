# 🚀 Guide de Démarrage - GitHub Copilot Agents v1

Bienvenue dans le système d'agents GitHub Copilot pour consulting en intégration de données Azure !

## 📋 Prérequis

- Visual Studio Code avec GitHub Copilot activé
- Git installé
- PowerShell 7+ (pour les scripts de gestion)
- Accès à Azure (si vous travaillez sur des projets clients)

## ⚡ Démarrage Rapide (5 minutes)

### 1. Cloner le Repository

```bash
git clone https://github.com/your-org/agent-nvt-v1.git
cd agent-nvt-v1
```

### 2. Ouvrir dans VS Code

```bash
code .
```

### 3. Configurer le Client Actif

Par défaut, le client `default` est actif. Pour changer:

```powershell
# Lister les clients disponibles
.\.github\tools\client-manager.ps1 -List

# Activer un client spécifique
.\.github\tools\client-manager.ps1 -SetActive sbm

# Vérifier le client actif
.\.github\tools\client-manager.ps1 -GetActive
```

### 4. Utiliser votre Premier Agent

Dans VS Code, ouvrez le chat Copilot et tapez:

```
@ba /analyze "Migration des données CRM vers Azure"
```

L'agent Business Analyst va:
1. Charger automatiquement le contexte du client actif
2. Analyser votre demande
3. Produire un cahier des charges structuré

## 🤖 Les 4 Agents Disponibles

### @ba (Business Analyst)

**Spécialité**: Analyse métier, exigences, cahier des charges

**Commandes**:
- `/analyze <sujet>` - Analyser un besoin
- `/requirements <contexte>` - Extraire exigences
- `/mapping <source> <target>` - Data mapping
- `/risks <projet>` - Analyse des risques
- `/stories <epic>` - User stories

**Exemple**:
```
@ba /analyze "Pipeline ETL pour consolidation ventes multi-sources"
```

### @archi (Architecte)

**Spécialité**: Architecture technique, conception système, diagrammes

**Commandes**:
- `/design <sujet>` - Concevoir architecture
- `/diagramme <type> <sujet>` - Créer diagrammes
- `/tad <projet>` - Technical Architecture Document
- `/adr <sujet>` - Architecture Decision Record
- `/cost <architecture>` - Estimation coûts
- `/review <architecture>` - Revue architecture

**Exemple**:
```
@archi /design "Pipeline temps réel avec Event Hubs et Databricks"
```

### @dev (Développeur)

**Spécialité**: Implémentation, code production, tests

**Commandes**:
- `/implement <feature>` - Implémenter fonctionnalité
- `/refactor <code>` - Refactoriser code
- `/test <code>` - Créer tests
- `/debug <error>` - Déboguer problème

**Exemple**:
```
@dev /implement "Pipeline ADF pour ingérer CSV vers ADLS"
```

### @reviewer (Reviewer)

**Spécialité**: Revue de code, qualité, sécurité, performance

**Commandes**:
- `/review <code>` - Revue complète
- `/security <code>` - Audit sécurité
- `/performance <code>` - Analyse performance

**Exemple**:
```
@reviewer /review "Vérifier qualité du notebook de transformation"
```

## 🔄 Workflow Complet

Voici un workflow typique pour un projet complet:

### Étape 1: Analyse Métier (BA)

```
@ba /analyze "Migration Dynamics 365 vers Power Platform"
```

Le BA va produire:
- Cahier des charges fonctionnel
- Table des exigences (RF/RNF)
- Data mapping si applicable
- Analyse des risques

### Étape 2: Conception Architecture (Architecte)

```
@archi /tad "Migration Dynamics 365"
```

L'architecte va produire:
- Technical Architecture Document complet
- Diagrammes (C4, séquence, network)
- Architecture Decision Records
- Estimation des coûts Azure

### Étape 3: Implémentation (Développeur)

```
@dev /implement "Pipeline ADF selon architecture définie"
```

Le développeur va produire:
- Code production (pipelines, notebooks, SQL)
- Tests (unit, integration)
- Documentation technique

### Étape 4: Revue (Reviewer)

```
@reviewer /review "Code implémenté par @dev"
```

Le reviewer va produire:
- Rapport de revue structuré
- Classification: Blocker / Important / Mineur
- Actions correctives

### Étape 5: Itération

Selon les retours du reviewer, retour au développeur pour corrections, puis nouvelle revue.

## 📝 Utiliser les Prompt Files

Les prompt files sont des templates réutilisables pour des tâches récurrentes.

### Exemples:

**Générer un TAD complet**:
```
#file:tad.prompt project_name="Migration CRM" project_description="Migration des données CRM legacy vers Azure Synapse Analytics avec Power BI"
```

**Brainstorming sur un sujet**:
```
#file:brainstorm.prompt topic="Optimisation des coûts Azure Data Platform"
```

**Estimation de coûts**:
```
#file:cost-estimation.prompt architecture="ADF + ADLS + Databricks + Synapse"
```

## 👥 Gestion Multi-Client

### Structure Client

Chaque client a sa propre structure:

```
.github/clients/[client-name]/
├── CLIENT.md              # Contexte et priorités
├── instructions/          # Instructions spécifiques
│   ├── naming.md
│   ├── architecture.md
│   └── security.md
├── knowledge/            # Base de connaissance
│   ├── apis/
│   ├── schemas/
│   └── mapping/
├── config/               # Configuration
│   └── azure-resources.json
└── data/                 # Données de référence
```

### Créer un Nouveau Client

```powershell
# Créer la structure
.\.github\tools\client-manager.ps1 -Create nouveau-client

# Éditer le fichier CLIENT.md
code .\.github\clients\nouveau-client\CLIENT.md

# Activer le client
.\.github\tools\client-manager.ps1 -SetActive nouveau-client
```

### Basculer entre Clients

```powershell
# Client par défaut (projets génériques)
.\.github\tools\client-manager.ps1 -SetActive default

# Client SBM (projets SBM Offshore)
.\.github\tools\client-manager.ps1 -SetActive sbm

# Votre nouveau client
.\.github\tools\client-manager.ps1 -SetActive nouveau-client
```

**Important**: Les agents chargent automatiquement le contexte du client actif !

## 🎯 Bonnes Pratiques

### 1. Toujours Définir le Client Actif

Avant de commencer à travailler:
```powershell
.\.github\tools\client-manager.ps1 -GetActive
```

### 2. Commencer par le BA

Pour tout nouveau projet, commencez toujours par une analyse métier:
```
@ba /analyze "Description du besoin"
```

### 3. Utiliser les Handoffs

Les agents peuvent se passer la main:
```
@ba /analyze "..." 
→ BA termine avec: "Handoff vers @archi"
→ @archi prend le relais automatiquement
```

### 4. Itérer

N'hésitez pas à faire plusieurs allers-retours:
```
@dev implémente → @reviewer révise → @dev corrige → @reviewer valide
```

### 5. Documenter les Décisions

Utilisez les ADRs pour les décisions importantes:
```
@archi /adr "Choix entre ADF et Synapse Pipelines"
```

## 🔍 Structure du Projet

```
agent-nvt-v1/
├── .github/
│   ├── agents/              # 4 agents (BA, Archi, Dev, Reviewer)
│   ├── clients/             # Espaces clients multi-tenant
│   ├── instructions/        # Instructions globales
│   ├── skills/              # Compétences spécialisées
│   ├── knowledge/           # Base de connaissance globale
│   ├── prompts/             # Templates réutilisables
│   ├── config/              # Configuration système
│   └── tools/               # Scripts de gestion
├── docs/                    # Documentation
├── examples/                # Exemples d'utilisation
└── README.md                # Vue d'ensemble
```

## 🆘 Résolution de Problèmes

### Les agents ne chargent pas le contexte client

Vérifiez que le client est bien actif:
```powershell
.\.github\tools\client-manager.ps1 -GetActive
```

### Je ne vois pas mes agents dans Copilot

1. Assurez-vous que GitHub Copilot est activé
2. Rechargez VS Code (Ctrl+Shift+P → "Reload Window")
3. Vérifiez que le dossier `.github/agents/` contient les fichiers `.md`

### Les commandes `/` ne fonctionnent pas

Les commandes sont définies dans chaque agent. Vérifiez le fichier de l'agent correspondant dans `.github/agents/`.

## 📚 Ressources Supplémentaires

- [Architecture Détaillée](ARCHITECTURE.md)
- [Gestion des Clients](CLIENT-MANAGEMENT.md)
- [Utilisation des Agents](AGENT-USAGE.md)
- [Diagramme Architecture](../.attachments/architecture.drawio)

## 🤝 Support

Pour toute question ou problème:

1. Consultez la documentation dans `docs/`
2. Vérifiez les exemples dans `examples/`
3. Contactez l'équipe: [votre-email]

## 🎉 Prêt à Démarrer !

Vous êtes maintenant prêt à utiliser les agents GitHub Copilot pour vos projets d'intégration de données Azure.

**Premier pas suggéré**:

```powershell
# 1. Activer le client par défaut
.\.github\tools\client-manager.ps1 -SetActive default

# 2. Ouvrir VS Code
code .

# 3. Demander au BA d'analyser un besoin
@ba /analyze "Pipeline ETL pour consolidation données ventes"
```

Bonne consultation ! 🚀

---

**Version**: 1.0.0  
**Dernière mise à jour**: 2026-02-03
