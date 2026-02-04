# 🚀 QUICK START: Pour Les Nouveaux Utilisateurs

**Bienvenue!** Ce guide te guide les premières étapes pour utiliser les agents.

⏱️ **Temps de lecture**: 5 minutes

---

## 1️⃣ Comprendre la Structure (2 min)

**Question**: Comment tout fonctionne?  
**Réponse**: Lire `.github/AGENTS-FLOW-DIAGRAM.md`

Ce fichier contient des **diagrammes visuels** qui expliquent:
- Comment les agents se déclenchent
- Quel agent fait quoi
- Où aller pour chaque info

**Résumé rapide**:
```
BA (Business Analyst)    → Exigences métier
    ↓
Architecte              → Architecture Azure
    ↓
Développeur            → Code + Tests
    ↓
Reviewer               → Revue de code
```

---

## 2️⃣ Identifier le Client (1 min)

**Fichier**: `.github/clients/active-client.json`

```json
{
  "clientKey": "nadia",
  "name": "NADIA"
}
```

**Ça veut dire**: Le client actif est **NADIA**.

**Pourquoi?** Chaque client a ses propres conventions (nommage, architecture, sécurité).

---

## 3️⃣ Charger le Contexte Client (1 min)

**Fichier**: `.github/clients/nadia/`

Chaque client a:
```
clients/nadia/
├── CLIENT.md              ← Contexte métier
├── instructions/
│   ├── naming.md         ← Conventions nommage
│   ├── architecture.md   ← Patterns client
│   └── security.md       ← Standards sécurité
└── knowledge/            ← Knowledge base client
```

**Qu'est-ce que je dois faire?**  
→ Lire `.github/clients/nadia/CLIENT.md`

---

## 4️⃣ Utiliser Les Agents (1 min)

### Pour un Business Analyst
```bash
@ba "Analyser les exigences pour [projet]"
```
L'agent chargera automatiquement:
1. Client NADIA
2. Contexte client
3. Instructions BA

### Pour un Architecte
```bash
@architecte "Concevoir l'architecture pour [projet]"
```
L'agent chargera:
1. Client NADIA  
2. Architecture patterns NADIA
3. Conventions NADIA
4. Instructions Architecte

### Pour un Développeur
```bash
@dev "Implémenter [composant]"
```
L'agent chargera:
1. Client NADIA
2. Conventions de nommage NADIA
3. TAD (architecture)
4. Instructions développeur

### Pour un Reviewer
```bash
@reviewer "Revue PR #123"
```
L'agent chargera:
1. TAD (architecture)
2. Standards de qualité
3. Instructions reviewer

---

## 5️⃣ Où Chercher L'Info (Bonus!)

### Je suis nouveau, comment je commence?
→ Lire: `.github/README.md`

### Je comprends pas comment les agents marchent
→ Lire: `.github/AGENTS-FLOW-DIAGRAM.md`

### Je dois implémenter du code
→ Lire: `.github/instructions/agents/developpeur.md`

### Je dois faire une revue de code
→ Lire: `.github/instructions/agents/reviewer.md`

### Je dois concevoir une architecture
→ Lire: `.github/instructions/agents/architecte.md`

### Je dois analyser les exigences
→ Lire: `.github/instructions/agents/business-analyst.md`

### Je dois comprendre les patterns Azure
→ Lire: `.github/instructions/domains/azure-patterns.md`

### Je dois implémenter Terraform
→ Lire: `.github/instructions/domains/iac-terraform.md`

### Je dois écrire des tests
→ Lire: `.github/instructions/domains/testing.md`

### Je dois comprendre la data architecture
→ Lire: `.github/instructions/domains/data-architecture.md`

### Format des livrables (BRD, TAD, ADR)
→ Lire: `.github/instructions/contracts/artefacts.md`

### Convention nommage client
→ Lire: `.github/clients/{key}/instructions/naming.md`

---

## 📋 Fichiers Essentiels (À Signeter)

| Fichier | Qui | Quand |
|---------|-----|-------|
| `.github/README.md` | Tous | En arrivant |
| `.github/AGENTS-FLOW-DIAGRAM.md` | Tous | Pour comprendre |
| `.github/REFACTORISATION-SUMMARY.md` | PM/Leads | Aperçu v2.0 |
| `.github/instructions/README.md` | Agents | Avant de bosser |
| `.github/clients/active-client.json` | Tous | Identifier client |
| `.github/clients/{key}/CLIENT.md` | Tous | Contexte |

---

## 🎯 Workflow Simplifié

```
START
  │
  ├─ Lire: .github/README.md (what is this?)
  │
  ├─ Lire: .github/clients/active-client.json (quel client?)
  │
  ├─ Lire: .github/clients/{key}/CLIENT.md (contexte)
  │
  ├─ Appeler: @agent "description"
  │
  └─ Agent charge automatiquement:
       .github/instructions/agents/{agent}.md
       .github/instructions/domains/*.md
       .github/clients/{key}/instructions/*.md
       .github/instructions/base/*.md
```

---

## ❓ Questions Fréquentes

### Q: Où est la configuration des agents?
**A**: `.github/config/copilot-config.json`

### Q: Comment j'ajoute un nouveau client?
**A**: 
1. Créer `.github/clients/new-client/`
2. Ajouter `CLIENT.md`
3. Ajouter `instructions/` (naming, architecture, security)
4. Mettre à jour `.github/clients/active-client.json`

### Q: Peut-je personnaliser les instructions?
**A**: OUI! Mais:
- ✅ Client-specific → `.github/clients/{key}/instructions/`
- ❌ Pas dans `.github/instructions/` (réservé common)

### Q: Les agents sont trop lents?
**A**: Vérifier:
1. Client chargé correctement? (`.github/clients/active-client.json`)
2. Instructions trouvées? (check frontmatter `applyTo`)
3. Pas de fichiers énormes? (chaque <500 lignes)

### Q: J'ai une question spécifique à NADIA?
**A**: Lire `.github/clients/nadia/instructions/`

---

## 🚨 Ne Pas Oublier

1. **Client d'abord**: Toujours charger `.github/clients/{key}/CLIENT.md`
2. **Conventions client**: Avant de coder, lire conventions nommage
3. **Pas de redondance**: Referencer l'existant, ne pas copier
4. **Modularité**: Chaque fichier <500 lignes
5. **Client-safe**: Aucun client-specific dans `.github/instructions/base` ou `.github/instructions/agents/`

---

## ✅ Checklist Avant de Commencer

- [ ] Lire `.github/README.md`
- [ ] Lire `.github/AGENTS-FLOW-DIAGRAM.md`
- [ ] Vérifier client actif: `.github/clients/active-client.json`
- [ ] Charger contexte client: `.github/clients/{key}/CLIENT.md`
- [ ] Charger instructions agent: `.github/instructions/agents/{agent}.md`
- [ ] Prêt à travailler! 🚀

---

## 📞 Besoin D'Aide?

| Problème | Solution |
|----------|----------|
| Je suis perdu | Lire `.github/README.md` |
| Je comprends pas les agents | Lire `.github/AGENTS-FLOW-DIAGRAM.md` |
| Je sais pas quoi faire ensuite | Lire `.github/instructions/agents/{agent}.md` |
| Je dois chercher une info | Lire `.github/instructions/domains/*.md` |
| Client-specific manquant | Créer `.github/clients/{key}/instructions/` |

---

**Bonne chance!** 🎉

Pour des questions détaillées → Lire la documentation complète dans `.github/instructions/`

