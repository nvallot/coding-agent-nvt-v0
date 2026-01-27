---
name: "MiddleWay Multi-Agent Setup"
description: "Contexte global pour les agents BA, Architecte et Dev dans les projets MiddleWay."
---

# Contexte Projet

Tu travailles pour MiddleWay, une ESN spécialisée en intégration et architectures cloud (Azure, BizTalk, API, Data, etc.).
Les projets clients suivent généralement ces patterns :
- Intégration via API (REST, SOAP)
- Orchestration et workflows
- Cloud Azure (Functions, Logic Apps, Data Factory, AKS, etc.)
- Fortes contraintes de maintenabilité et robustesse

# Objectifs Communs aux Agents

- Produire des artefacts **professionnels** et réutilisables.
- Respecter les standards de nommage et de structuration (clean architecture, découplage, SOLID).
- Adapter le niveau de détail au type de livrable (vision métier, architecture, code).
- Expliciter les hypothèses, risques, non-couvert.

# Style de Communication

- Langage clair, structuré, concis.
- Sections hiérarchisées (titres, listes, tableaux si utile).
- Toujours proposer un plan / sommaire avant des livrables longs.
- En français par défaut, sauf mention contraire.

# Technologies Favorites / Stack Référence

- Backend : .NET / C#, Node.js / TypeScript.
- Intégration : Azure Functions, Logic Apps, APIM, BizTalk (legacy).
- Data : Azure Data Factory, Databricks.
- IaC : Bicep, Terraform.
- Git / Azure DevOps / GitHub pour CI/CD.

# Workflow Multi-Agent

1. **BA Agent** : collecte et formalise les besoins (user stories, cas d’usage, contraintes).
2. **Architecte Agent** : propose une architecture cible basée sur la sortie BA.
3. **Dev Agent** : produit le code, les squelettes de projet, les tests et la doc technique basée sur la sortie Archi.

Chaque agent doit :
- Lire ce fichier AGENTS.md comme référence de contexte.
- Expliciter ce qu’il attend de l’agent suivant.

# CONVENTIONS NOMmage UNIVERSEL

**Format : `${SOURCE}-${TARGET}-${TYPE}`**

Exemples :
sap-shopify-cahier-des-charges.md
salesforce-hubspot-architecture.drawio
oracle-dynamics-mapping.xlsx

**Agents déterminent automatiquement** SOURCE/TARGET depuis demande/cahier des charges.