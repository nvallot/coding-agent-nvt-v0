---
applyTo: "**/docs/**,**/architecture/**,**/diagrams/**,**/Deployment/**"
---

# 🎨 Standards Draw.io pour Architectures Azure

## 📋 Vue d'ensemble

Ce document définit les standards visuels pour les diagrammes d'architecture produits avec Draw.io.
L'objectif est de garantir **lisibilité, professionnalisme et cohérence** entre tous les projets.

## 🖼️ Activation des Shapes Azure Natives

### Configuration Draw.io
1. Ouvrir Draw.io
2. Menu **More Shapes...** (en bas du panneau gauche)
3. Section **Networking** → Cocher **Azure**
4. Section **Networking** → Cocher **Azure 2**
5. Cliquer **Apply**

Les icônes officielles Microsoft Azure seront disponibles dans le panneau gauche.

## 🎨 Zones et Couleurs

### Définition des Zones

Chaque diagramme doit clairement délimiter les zones suivantes avec des **rectangles englobants** :

| Zone | Nom | Couleur Fond | Couleur Bordure | Exemple |
|------|-----|--------------|-----------------|---------|
| **On-Premise** | Systèmes internes legacy | `#FFF2CC` (Jaune pâle) | `#D6B656` (Or) | NADIA, SAP, SQL Server |
| **Azure Cloud** | Services Azure managés | `#DAE8FC` (Bleu pâle) | `#6C8EBF` (Bleu) | ISP, Functions, Service Bus |
| **External** | Systèmes tiers externes | `#D5E8D4` (Vert pâle) | `#82B366` (Vert) | Lucy, Dataverse, APIs tierces |
| **Monitoring** | Observabilité | `#F5F5F5` (Gris clair) | `#666666` (Gris) | App Insights, Dashboard |

### Style des Rectangles de Zone
```
- rounded=1
- arcSize=10
- strokeWidth=2
- dashed=0 (bordure pleine)
- fontStyle=1 (gras pour le titre de zone)
- fontSize=14 pour le titre
```

### Configurations de Zones Flexibles

**Configuration 1: Full Azure (100% Cloud)**
```
┌─────────────────────────────────────────────────────────┐
│ 🟦 Azure Cloud                                          │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐       │
│  │ Ingestion   │ │ Processing  │ │  Storage    │       │
│  └─────────────┘ └─────────────┘ └─────────────┘       │
│                                                         │
│  ┌─────────────────────────────────────────────┐       │
│  │ 🟩 External Services (sous-zone)            │       │
│  │   Dataverse, APIs tierces...                │       │
│  └─────────────────────────────────────────────┘       │
└─────────────────────────────────────────────────────────┘
```

**Configuration 2: Hybrid (On-Prem + Azure)**
```
┌──────────────────┐     ┌──────────────────────────────┐
│ 🟨 On-Premise    │────→│ 🟦 Azure Cloud               │
│                  │     │                              │
│  ERP, Legacy DB  │     │  Functions, Storage, etc.    │
│                  │     │                              │
│                  │     │  ┌─────────────────────┐    │
│                  │     │  │ 🟩 External         │    │
│                  │     │  └─────────────────────┘    │
└──────────────────┘     └──────────────────────────────┘
```

**Configuration 3: Multi-Zone (Complexe)**
```
┌──────────────┐   ┌────────────────────────────┐   ┌────────────────┐
│ 🟨 On-Premise │──→│ 🟦 Azure Cloud (ISP)      │──→│ 🟩 External    │
│              │   │                            │   │                │
│  ERP, NAV    │   │  ┌──────────────────────┐ │   │  Lucy, CRM     │
│              │   │  │ Internal Services    │ │   │  Power Platform│
│              │   │  │ (Functions, Storage) │ │   │                │
│              │   │  └──────────────────────┘ │   │                │
│              │   │                            │   │                │
│              │   │  ⬇                        │   │                │
│              │   │  ┌──────────────────────┐ │   │                │
│              │   │  │ ⬜ Monitoring         │ │   │                │
│              │   │  │ App Insights, Logs   │ │   │                │
│              │   │  └──────────────────────┘ │   │                │
└──────────────┘   └────────────────────────────┘   └────────────────┘
```

### Sous-zone: Internal Services

Pour les zones Azure avec beaucoup de composants, utiliser une **sous-zone "Internal Services"** :

```
┌─────────────────────────────────────────────────────┐
│ 🟦 Azure Cloud - ISP                                │
│                                                     │
│  ┌───────────────────────────────────────────────┐ │
│  │ Internal Services                              │ │
│  │                                                │ │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐    │ │
│  │  │ Function │  │ Function │  │ Storage  │    │ │
│  │  │ App 1    │  │ App 2    │  │ Account  │    │ │
│  │  └──────────┘  └──────────┘  └──────────┘    │ │
│  │                                                │ │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐    │ │
│  │  │ Service  │  │ Key      │  │ SQL      │    │ │
│  │  │ Bus      │  │ Vault    │  │ Database │    │ │
│  │  └──────────┘  └──────────┘  └──────────┘    │ │
│  │                                                │ │
│  └───────────────────────────────────────────────┘ │
│                                                     │
└─────────────────────────────────────────────────────┘
```

**Layout matriciel (2xN)** :
- Composants organisés en **grille 2 colonnes**
- Plus facile à équilibrer visuellement
- Évite les lignes trop longues

### Titre de Zone
Placer le titre **en haut à gauche** de chaque zone avec :
- Police: **Bold**
- Taille: 14pt
- Format: `{NOM_ZONE}` (ex: "Integration Services Platform (ISP)")

## 🔢 Numérotation des Flux

### Convention
Utiliser des **cercles blancs numérotés** pour indiquer l'ordre des opérations :

| Numéro | Caractère | Description |
|--------|-----------|-------------|
| 1 | ❶ | Première étape |
| 2 | ❷ | Deuxième étape |
| 3 | ❸ | Troisième étape |
| 4 | ❹ | Quatrième étape |
| 5 | ❺ | Cinquième étape |
| 6 | ❻ | Sixième étape |
| 7 | ❼ | Septième étape |
| 8 | ❽ | Huitième étape |
| 9 | ❾ | Neuvième étape |
| 10 | ❿ | Dixième étape |

### Style des Numéros
- Forme: Cercle (`ellipse`)
- Fond: `#FFFFFF` (blanc)
- Bordure: `#000000` (noir)
- Taille: 24x24 px
- Police: Bold, 12pt
- Placer **sur ou près de la flèche** correspondante

### Labels de Flux
Chaque flèche numérotée doit avoir un label descriptif court :
```
❶ Get last execution date
❷ Get productCode List
❸ Get data PO list
❹ Mapping and Transformation
❺ Send Purchase Order message
❻ Get Lucy PersonalExternalId
❼ Insert data in staging table
```

## 📦 Composants Azure

### Utilisation des Shapes Natives
Toujours utiliser les **shapes Azure officielles** de Draw.io, jamais de rectangles génériques.

### Composants Courants

| Service | Shape Draw.io | Catégorie |
|---------|---------------|-----------|
| Azure Function | `Azure Function App` | Compute |
| Service Bus | `Azure Service Bus` | Messaging |
| Table Storage | `Azure Table Storage` | Storage |
| API Management | `Azure API Management` | Networking |
| Dataverse | `Dataverse` ou `Common Data Service` | Data |
| SQL Database | `Azure SQL Database` | Data |
| Key Vault | `Azure Key Vault` | Security |
| App Insights | `Application Insights` | Monitoring |
| Event Hub | `Azure Event Hubs` | Messaging |

### Taille Standard des Icônes
- Taille recommandée: **60x60 px** à **80x80 px**
- Garder une taille uniforme dans tout le diagramme

## 🏷️ Naming Convention

### Format des Labels
Chaque composant Azure doit afficher son **nom complet** selon la convention de nommage du projet :

```
{SERVICE_TYPE}
{NAMING_CONVENTION}
{FUNCTION_NAME}
```

Exemple :
```
Azure Function
SBWE1-ISP-{ENV}-FAP-65
RetrievePurchaseOrderSupplier
```

### Style des Labels
- Police: Regular, 10pt
- Alignement: Centré sous l'icône
- Multi-lignes autorisées pour clarté

## 📍 Layout et Positionnement

### Direction du Flux
- Flux principal: **Gauche → Droite**
- Flux secondaires: **Haut → Bas**
- Retours/Erreurs: Flèches pointillées

### Espacement (OBLIGATOIRE)

| Élément | Espacement Minimum | Recommandé |
|---------|-------------------|------------|
| Entre composants (horizontal) | **40 px** | 50-60 px |
| Entre composants (vertical) | **30 px** | 40-50 px |
| Entre zones | **80 px** | 100 px |
| Marge interne des zones | **20 px** | 30 px |
| Labels sous icônes | **10 px** | 15 px |

### Grille et Alignement (OBLIGATOIRE)
- **Activer la grille Draw.io**: View → Grid
- **Taille de grille**: 20 px
- **Snap to Grid**: Toujours activé
- **Aligner sur grille**: Tous les composants doivent être alignés

### 🚫 Règles Anti-Chevauchement (CRITIQUE)

**⚠️ Le chevauchement de composants est INTERDIT**

Pour éviter tout chevauchement :

1. **Calcul de position**: Avant de placer un composant, vérifier l'espace disponible
2. **Décalage automatique**: Si collision détectée, décaler de +60px horizontal ou +50px vertical
3. **Vérification des labels**: Les labels ne doivent jamais chevaucher les composants voisins

**Formule de placement**:
```
Position_X = Zone_Margin + (Colonne * (Icon_Width + Horizontal_Gap))
Position_Y = Zone_Margin + Header_Height + (Ligne * (Icon_Height + Vertical_Gap + Label_Height))

Où:
- Zone_Margin = 20px
- Header_Height = 40px (pour le titre de zone)
- Icon_Width/Height = 60-80px
- Horizontal_Gap = 40px minimum
- Vertical_Gap = 30px minimum
- Label_Height = 30px (estimé pour 2 lignes)
```

**Validation avant export**:
- [ ] Aucun composant ne chevauche un autre
- [ ] Aucun label ne chevauche un composant
- [ ] Aucune flèche ne passe à travers un composant
- [ ] Espacement minimum respecté partout

### Règles de Lisibilité
- Maximum **8 composants par zone**
- Si plus de 8, créer des sous-zones ou simplifier
- Éviter les croisements de flèches
- Aligner les composants sur une grille invisible
- **Utiliser un layout matriciel** (lignes et colonnes) pour les zones denses

## 📐 Flèches et Connecteurs

### Style Standard
| Type | Style | Usage |
|------|-------|-------|
| Flux principal | Pleine, pointe fermée | Données en transit normal |
| Flux asynchrone | Pleine + symbole ⚡ | Messages Service Bus, Events |
| Flux optionnel | Pointillée | Chemins conditionnels |
| Flux erreur | Rouge, pointillée | Gestion d'erreurs |

### Épaisseur
- Flux principal: **2px**
- Flux secondaire: **1px**

## 📝 Légende Obligatoire

Chaque diagramme doit inclure une **légende en bas** avec :

### Contenu Minimum
1. **Zones** : Signification des couleurs
2. **Numérotation** : Explication des étapes
3. **Flèches** : Types de flux
4. **Date** : Date de création/mise à jour
5. **Version** : Numéro de version du diagramme

### Exemple de Légende
```
┌─────────────────────────────────────────────────────┐
│ LÉGENDE                                             │
├─────────────────────────────────────────────────────┤
│ 🟨 On-Premise    🟦 Azure Cloud    🟩 External     │
│ ❶❷❸ Ordre des opérations                          │
│ ──→ Flux synchrone    ⚡→ Flux asynchrone          │
│ Version: 1.0 | Date: 2026-02-05                    │
└─────────────────────────────────────────────────────┘
```

## 📁 Dossier de Sortie

### Structure
Tous les diagrammes Draw.io doivent être sauvegardés dans :
```
draw.io/architectures/{PROJECT}/
```

### Fichiers Requis
Pour chaque projet, créer :
| Fichier | Contenu |
|---------|---------|
| `{PROJECT}-c4-context.drawio` | Diagramme C4 Context |
| `{PROJECT}-c4-container.drawio` | Diagramme C4 Container |
| `{PROJECT}-data-flow.drawio` | Flux de données end-to-end |
| `README.md` | Description des diagrammes |

### Export
Exporter également en :
- **PNG** : Pour documentation (300 DPI)
- **SVG** : Pour intégration web
- **PDF** : Pour partage client

## ✅ Checklist Qualité

Avant de valider un diagramme, vérifier :

- [ ] Toutes les zones sont délimitées avec les couleurs correctes
- [ ] Les shapes Azure natives sont utilisées
- [ ] Chaque composant a son nom complet (naming convention)
- [ ] Les flux sont numérotés (❶❷❸...)
- [ ] Les flèches ont des labels descriptifs
- [ ] La légende est présente et complète
- [ ] Maximum 8 composants par zone
- [ ] Pas de croisement de flèches
- [ ] Le fichier est sauvegardé dans le bon dossier
- [ ] Les exports PNG/SVG sont générés

## 🎨 Icônes Azure Officielles (SVG)

### Emplacement
Les icônes Azure officielles sont disponibles localement :
```
.github/templates/Azure_Public_Service_Icons/Icons/
```

### Index de Référence
Consulter **`.github/templates/azure-icons-index.md`** pour la liste complète des chemins.

### Icônes les Plus Utilisées

| Service | Chemin |
|---------|--------|
| **Function Apps** | `compute/10029-icon-service-Function-Apps.svg` |
| **Service Bus** | `integration/10836-icon-service-Azure-Service-Bus.svg` |
| **Storage Account** | `storage/10086-icon-service-Storage-Accounts.svg` |
| **Table Storage** | `general/10841-icon-service-Table.svg` |
| **Blob Storage** | `general/10780-icon-service-Blob-Block.svg` |
| **Key Vault** | `security/10245-icon-service-Key-Vaults.svg` |
| **App Insights** | `monitor/00012-icon-service-Application-Insights.svg` |
| **Virtual Network** | `networking/10061-icon-service-Virtual-Networks.svg` |
| **Resource Group** | `general/10007-icon-service-Resource-Groups.svg` |
| **Data Factory** | `integration/10126-icon-service-Data-Factories.svg` |
| **Logic Apps** | `integration/02631-icon-service-Logic-Apps.svg` |
| **SQL Database** | `databases/10130-icon-service-SQL-Database.svg` |

### Import dans Draw.io
1. **File** → **Import from** → **Device**
2. Sélectionner le fichier `.svg`
3. Redimensionner à **60x60 px** ou **80x80 px**

### ⚠️ OBLIGATOIRE
Utiliser les icônes SVG officielles Microsoft pour tous les composants Azure.
Ne **jamais** utiliser de shapes génériques (rectangles, cercles) pour représenter des services Azure.

## 📁 Dossier de Sortie (OBLIGATOIRE)

### Structure Standard
Les diagrammes Draw.io doivent être sauvegardés dans :
```
{docsPath}/workflows/{flux}/diagrams/
```

### Fichiers Requis
| Fichier | Contenu |
|---------|---------|
| `{flux}-c4-container.drawio` | Diagramme C4 Container (OBLIGATOIRE) |
| `{flux}-c4-container.png` | Export PNG 300 DPI (OBLIGATOIRE) |
| `{flux}-data-flow.drawio` | Flux de données (si complexe) |

### Exports Obligatoires
- **PNG** : 300 DPI minimum pour documentation
- Format de nommage : `{flux}-{type}.png`

## 📚 Ressources

- [Azure Architecture Icons](https://learn.microsoft.com/azure/architecture/icons/)
- [C4 Model](https://c4model.com/)
- [Draw.io Azure Shapes](https://www.diagrams.net/blog/azure-diagrams)
- [Index icônes local](.github/templates/azure-icons-index.md)
