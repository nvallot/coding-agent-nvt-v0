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

### Espacement
- Entre composants dans une zone: **40-60 px**
- Entre zones: **80-100 px**
- Marge interne des zones: **20 px**

### Règles de Lisibilité
- Maximum **8 composants par zone**
- Si plus de 8, créer des sous-zones ou simplifier
- Éviter les croisements de flèches
- Aligner les composants sur une grille invisible

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

## 📚 Ressources

- [Azure Architecture Icons](https://learn.microsoft.com/azure/architecture/icons/)
- [C4 Model](https://c4model.com/)
- [Draw.io Azure Shapes](https://www.diagrams.net/blog/azure-diagrams)
