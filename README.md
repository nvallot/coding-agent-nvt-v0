# Architecture des Custom Agents GitHub Copilot

Ce projet implémente l'architecture complète des Custom Agents GitHub Copilot telle que décrite dans le diagramme d'architecture.

## 📁 Structure du Projet

```
copilot-agents-architecture/
├── .github/
│   ├── agents/
│   │   ├── architecte.md
│   │   ├── developpeur.md
│   │   └── reviewer.md
│   ├── instructions/
│   │   ├── conventions.md
│   │   ├── azure-instructions.md
│   │   ├── path-based-instructions.md
│   │   ├── backend.md
│   │   ├── data-integration.md
│   │   ├── frontend.md
│   │   ├── infrastructure.md
│   │   ├── terraform.md
│   │   ├── tests.md
│   │   ├── docs.md
│   │   └── workflows.md
│   ├── knowledge/
│   │   ├── azure/
│   │   │   ├── services.md
│   │   │   └── best-practices.md
│   │   └── architecture/
│   │       └── patterns.md
│   ├── skills/
│   │   ├── diagram-creation/
│   │   │   └── SKILL.md
│   │   ├── solution-design/
│   │   │   └── SKILL.md
│   │   ├── code-review/
│   │   │   └── SKILL.md
│   │   ├── code-implementation/
│   │   │   └── SKILL.md
│   │   ├── debugging/
│   │   │   └── SKILL.md
│   │   ├── testing/
│   │   │   └── SKILL.md
│   │   └── security-audit/
│   │       └── SKILL.md
│   ├── prompts/
│   │   ├── brainstorm.prompt
│   │   ├── tad.prompt
│   │   └── cost-estimation.prompt
│   ├── copilot-config.json
│   └── copilot-instructions.md
├── docs/
│   ├── architecture-diagram.mermaid
│   └── architecture-diagram.png
└── README.md
```

## 🚀 Composants Principaux

### 1. **Agents** (`.github/agents/`)
Définissent les personnalités et rôles des agents:
- `architecte.md`: Agent pour l'architecture système
- `developpeur.md`: Agent pour le développement
- `reviewer.md`: Agent pour les revues de code

### 2. **Instructions** (`.github/instructions/`)
Instructions path-based chargées selon le contexte:
- `conventions.md`: Conventions de code
- `azure-instructions.md`: Instructions spécifiques Azure
- `path-based-instructions.md`: Instructions conditionnelles
- `backend.md`, `frontend.md`, `infrastructure.md`, etc.: Instructions par domaine

### 3. **Knowledge** (`.github/knowledge/`)
Base de connaissances statique organisée par thème:
- `azure/`: Documentation Azure
- `architecture/`: Patterns d'architecture

### 4. **Skills** (`.github/skills/`)
Compétences actives définissant le "comment faire":
- `diagram-creation/`: Création de diagrammes
- `solution-design/`: Conception de solutions
- `code-review/`: Revue de code
- `code-implementation/`, `debugging/`, `testing/`: Skills de développement

### 5. **Prompt Files** (`.github/prompts/`)
Templates réutilisables pour tâches récurrentes:
- `brainstorm.prompt`: Session de brainstorming
- `tad.prompt`: Document d'architecture technique
- `cost-estimation.prompt`: Estimation des coûts

## 📝 Composition du System Prompt

Le System Prompt final est composé dans cet ordre:

1. **Base GitHub Copilot** (non modifiable)
2. **Agent Instructions** (agents/\*.md)
3. **Path-based Instructions** (si workspace match)
4. **Knowledge Chunks** (via RAG)
5. **Workspace Context** (fichiers ouverts)
6. **Tools Available** (liste des outils)

## 🔧 Configuration

Le fichier `copilot-config.json` définit:
- Les modèles LLM utilisés
- Les paramètres (température, max_tokens)
- Les serveurs MCP
- Les variables d'environnement

## 💡 Utilisation

### Déclencher un Agent
```
@architecte /diagramme
```

### Utiliser un Prompt File
```
/brainstorm [votre question]
```

### Charger une Skill
Les skills sont automatiquement chargées selon les instructions de l'agent.

## 📊 Hiérarchie des Concepts

**Prompt File** → utilise → **Agent** → référence → **Skills** → accède → **Knowledge**

## 🎯 Points Clés

- **Séparation**: Instructions vs Skills vs Knowledge
- **Flexibilité**: Chargement statique + dynamique
- **Réutilisabilité**: Prompt Files pour tâches récurrentes
- **Contexte intelligent**: Path-based + RAG
- **Extensibilité**: Multiple modèles et tools (MCP)

## 📚 Différences Importantes

### Instructions vs Knowledge
- **Instructions**: "Tu es un architecte..."
- **Knowledge**: "Voici des infos Azure..."

### Knowledge vs Skills
- **Knowledge**: "Quoi" (informations)
- **Skills**: "Comment" (méthodes)
