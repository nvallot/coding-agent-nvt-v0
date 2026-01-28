---
client: "generic"
description: "Client générique sans contraintes spécifiques"
---

# Conventions générales

- Architecture orientée maintenabilité et lisibilité
- Sécurité par défaut
- Documentation obligatoire

## Schéma : Principes d'architecture

```mermaid
flowchart TB
    subgraph Principes["🏗️ Principes Fondamentaux"]
        M[📖 Maintenabilité]
        L[👁️ Lisibilité]
        S[🔒 Sécurité]
        D[📝 Documentation]
    end
    
    M --> Code["Code modulaire"]
    L --> Code
    S --> Défense["Défense en profondeur"]
    D --> Docs["Docs obligatoires"]
    
    style Principes fill:#e1f5fe
    style M fill:#81d4fa
    style L fill:#81d4fa
    style S fill:#ef9a9a
    style D fill:#a5d6a7
```

# Contraintes

- Aucune contrainte réseau imposée
- Aucune plateforme cloud imposée
- Aucun outil imposé

## Schéma : Flexibilité des contraintes

```mermaid
flowchart LR
    subgraph Liberté["🔓 Aucune Contrainte Imposée"]
        N[🌐 Réseau]
        C[☁️ Cloud]
        O[🔧 Outillage]
    end
    
    N --> Choix1["Libre choix"]
    C --> Choix2["Libre choix"]
    O --> Choix3["Libre choix"]
    
    Choix1 & Choix2 & Choix3 --> Projet["📦 Projet"]
    
    style Liberté fill:#fff3e0
    style Projet fill:#c8e6c9
```

# Outillage

- Gestion de code : non imposée
- Gestion des exigences : non imposée

## Schéma : Stack technique recommandée

```mermaid
flowchart TB
    subgraph Stack["🛠️ Stack Recommandée (non imposée)"]
        subgraph Code["Gestion de Code"]
            Git[Git]
            GitHub[GitHub/GitLab]
        end
        
        subgraph Exigences["Gestion des Exigences"]
            Jira[Jira]
            Azure[Azure DevOps]
            Notion[Notion]
        end
        
        subgraph CI["CI/CD"]
            Actions[GitHub Actions]
            Pipelines[Azure Pipelines]
        end
    end
    
    Git --> GitHub
    GitHub --> CI
    
    style Stack fill:#f3e5f5
    style Code fill:#e1bee7
    style Exigences fill:#e1bee7
    style CI fill:#e1bee7
```

# Autorisations

- Lecture/écriture autorisées sur les livrables générés

## Schéma : Matrice des permissions

```mermaid
flowchart LR
    subgraph Permissions["🔐 Permissions"]
        R["📖 Lecture"]
        W["✏️ Écriture"]
    end
    
    subgraph Livrables["📁 Livrables Générés"]
        Docs["📄 Documentation"]
        Code["💻 Code source"]
        Config["⚙️ Configuration"]
        Diag["📊 Diagrammes"]
    end
    
    R --> Livrables
    W --> Livrables
    
    style Permissions fill:#c8e6c9
    style Livrables fill:#bbdefb
    style R fill:#a5d6a7
    style W fill:#a5d6a7
```

# Diagrammes Draw.io

Pour des diagrammes plus complexes, utilisez les fichiers `.drawio` suivants :

| Diagramme | Description | Fichier |
|-----------|-------------|---------|
| Architecture globale | Vue d'ensemble du système | `diagrams/architecture.drawio` |
| Flux de données | Circulation des données | `diagrams/dataflow.drawio` |
| Déploiement | Infrastructure cible | `diagrams/deployment.drawio` |

> 💡 **Note** : Les fichiers `.drawio` peuvent être édités directement dans VS Code avec l'extension [Draw.io Integration](https://marketplace.visualstudio.com/items?itemName=hediet.vscode-drawio)
