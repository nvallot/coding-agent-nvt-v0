# Cahier des Charges – systemA-systemB

**Version** : 1.0  
**Date** : 27 janvier 2026  
**Client** : client-demo  
**Auteur** : Business Analyst  
**Statut** : Validé pour handoff  

---

## 1. Contexte

### 1.1 Présentation du projet

Ce projet vise à mettre en place un **échange bidirectionnel de données** entre deux systèmes hétérogènes désignés « systemA » et « systemB ».

Les deux systèmes n'ayant pas été conçus pour communiquer nativement, une solution d'intégration est nécessaire pour assurer un flux de données fiable, sécurisé et traçable.

### 1.2 Enjeux métier

- **Interopérabilité** : Permettre aux deux systèmes de partager des données sans intervention manuelle
- **Continuité d'activité** : Garantir la disponibilité des échanges même en cas de défaillance partielle
- **Conformité** : Assurer une traçabilité complète des données échangées pour répondre aux exigences d'audit

### 1.3 Périmètre

| Inclus | Exclu |
|--------|-------|
| Échanges de données systemA ↔ systemB | Refonte des systèmes A ou B |
| Mécanisme de traçabilité | Développement de nouvelles fonctionnalités métier dans A ou B |
| Gestion des erreurs et reprises | Migration de données historiques (sauf si requis) |
| Documentation technique et fonctionnelle | Formation utilisateurs finaux |

---

## 2. Cas d'usage (Use Cases)

### UC-001 : Envoi de données de systemA vers systemB

| Élément | Description |
|---------|-------------|
| **Acteur** | systemA (initiateur) |
| **Précondition** | systemA dispose de données à transmettre ; systemB est accessible |
| **Scénario nominal** | 1. systemA prépare les données<br>2. Validation du format<br>3. Transmission vers systemB<br>4. Accusé de réception<br>5. Journalisation |
| **Scénario alternatif** | Si systemB indisponible → mise en file d'attente + retry |
| **Postcondition** | Données disponibles dans systemB ; échange tracé |

### UC-002 : Envoi de données de systemB vers systemA

| Élément | Description |
|---------|-------------|
| **Acteur** | systemB (initiateur) |
| **Précondition** | systemB dispose de données à transmettre ; systemA est accessible |
| **Scénario nominal** | 1. systemB prépare les données<br>2. Validation du format<br>3. Transmission vers systemA<br>4. Accusé de réception<br>5. Journalisation |
| **Scénario alternatif** | Si systemA indisponible → mise en file d'attente + retry |
| **Postcondition** | Données disponibles dans systemA ; échange tracé |

### UC-003 : Consultation de l'historique des échanges

| Élément | Description |
|---------|-------------|
| **Acteur** | Administrateur / Opérateur |
| **Précondition** | Accès autorisé au registre des échanges |
| **Scénario nominal** | 1. Connexion au système<br>2. Recherche par critères (date, statut, ID)<br>3. Affichage des résultats<br>4. Détail d'un échange |
| **Postcondition** | Information consultée ; action tracée |

### UC-004 : Rejeu d'un échange échoué

| Élément | Description |
|---------|-------------|
| **Acteur** | Administrateur / Opérateur |
| **Précondition** | Échange en statut « Échec » identifié |
| **Scénario nominal** | 1. Sélection de l'échange<br>2. Analyse de l'erreur<br>3. Correction éventuelle<br>4. Déclenchement du rejeu<br>5. Suivi du nouvel échange |
| **Postcondition** | Échange rejoué avec nouveau statut ; traçabilité conservée |

---

## 3. Exigences Fonctionnelles (RF)

| ID | Exigence | Priorité | Critère d'acceptation |
|----|----------|----------|----------------------|
| RF-001 | Le système doit permettre l'envoi de données de systemA vers systemB | Haute | Données reçues intégralement et correctement dans systemB |
| RF-002 | Le système doit permettre l'envoi de données de systemB vers systemA | Haute | Données reçues intégralement et correctement dans systemA |
| RF-003 | Chaque échange doit être horodaté et identifié de manière unique | Haute | Identifiant unique généré (UUID) et timestamp UTC enregistré |
| RF-004 | Les échanges doivent être journalisés dans un registre centralisé | Haute | Logs accessibles et consultables par les administrateurs |
| RF-005 | Le système doit notifier les parties prenantes en cas d'échec d'échange | Moyenne | Notification envoyée dans un délai < 5 minutes après échec |
| RF-006 | Le système doit permettre la consultation de l'historique des échanges | Moyenne | Interface ou API de consultation disponible |
| RF-007 | Le système doit supporter le rejeu manuel d'un échange échoué | Moyenne | Fonctionnalité de rejeu disponible avec traçabilité |
| RF-008 | Le système doit valider le format des données avant transmission | Haute | Rejet des données non conformes avec message d'erreur explicite |
| RF-009 | Le système doit générer un accusé de réception pour chaque échange réussi | Haute | Accusé horodaté retourné au système émetteur |
| RF-010 | Le système doit gérer une file d'attente en cas d'indisponibilité du destinataire | Haute | Messages conservés et transmis dès disponibilité |

---

## 4. Exigences Non-Fonctionnelles (RNF)

### 4.1 Disponibilité

| ID | Exigence | Critère |
|----|----------|---------|
| RNF-001 | Haute disponibilité du service d'échange | Taux de disponibilité ≥ 99,5% |
| RNF-002 | Temps de reprise après incident (RTO) | ≤ 15 minutes |
| RNF-003 | Perte de données maximale acceptée (RPO) | 0 transaction perdue |

### 4.2 Performance

| ID | Exigence | Critère |
|----|----------|---------|
| RNF-004 | Temps de traitement d'un échange unitaire | < 5 secondes (P95) |
| RNF-005 | Capacité de traitement en pic | À définir selon volumétrie réelle |
| RNF-006 | Temps de réponse de l'interface de consultation | < 2 secondes |

### 4.3 Sécurité (contraintes client-demo)

| ID | Exigence | Critère |
|----|----------|---------|
| RNF-007 | Revue sécurité obligatoire avant mise en production | Validation formelle par équipe sécurité |
| RNF-008 | Chiffrement des données en transit | TLS 1.2 minimum |
| RNF-009 | Authentification des systèmes | Mécanisme d'authentification mutuelle |
| RNF-010 | Environnements cloisonnés (DEV, UAT, PROD) | Isolation réseau et données |
| RNF-011 | Principe du moindre privilège | Accès restreints selon rôles |

### 4.4 Traçabilité

| ID | Exigence | Critère |
|----|----------|---------|
| RNF-012 | Conservation des logs d'échange | Durée à définir selon politique client |
| RNF-013 | Imputabilité des actions | Identité du système initiateur tracée |
| RNF-014 | Intégrité des logs | Logs non modifiables après écriture |

### 4.5 Maintenabilité

| ID | Exigence | Critère |
|----|----------|---------|
| RNF-015 | Documentation renforcée | Documentation technique et opérationnelle complète |
| RNF-016 | Monitoring et alerting | Tableaux de bord de supervision disponibles |
| RNF-017 | Évolutivité | Architecture permettant l'ajout de nouveaux systèmes |

---

## 5. Hypothèses

| ID | Hypothèse | Impact si invalide |
|----|-----------|-------------------|
| H-001 | Les deux systèmes (A et B) disposent d'API ou d'interfaces exploitables pour l'intégration | Nécessité de développer des connecteurs spécifiques |
| H-002 | Les formats de données échangés peuvent être définis et documentés | Retard sur la phase de conception |
| H-003 | Les équipes techniques des deux systèmes sont disponibles pour les ateliers de spécification | Risque de spécifications incomplètes |
| H-004 | Les environnements de test sont accessibles pour les développements | Impossibilité de valider avant production |
| H-005 | La volumétrie des échanges est raisonnable (< 10 000 transactions/jour) | Révision de l'architecture nécessaire |
| H-006 | Aucune contrainte réglementaire spécifique (RGPD, secteur bancaire, santé) n'est applicable | Ajout de mesures de conformité |
| H-007 | Les systèmes A et B peuvent fonctionner de manière asynchrone | Impact sur le design des flux |

---

## 6. Risques

| ID | Risque | Probabilité | Impact | Mitigation proposée |
|----|--------|-------------|--------|---------------------|
| R-001 | Incompatibilité des formats de données entre A et B | Moyenne | Élevé | Définir un format pivot d'échange canonique |
| R-002 | Indisponibilité temporaire d'un des deux systèmes | Moyenne | Élevé | Mécanisme de file d'attente avec retry automatique |
| R-003 | Performance insuffisante en cas de pic de charge | Moyenne | Moyen | Dimensionnement et tests de charge préalables |
| R-004 | Échec de la revue sécurité | Faible | Élevé | Intégration des exigences sécurité dès la conception |
| R-005 | Dépendance à des compétences non disponibles | Moyenne | Moyen | Identification précoce des ressources |
| R-006 | Évolution des systèmes A ou B pendant le projet | Faible | Moyen | Gel des versions pendant l'intégration |
| R-007 | Perte de messages en cas de défaillance | Faible | Critique | Mécanisme de persistance garantie |

---

## 7. Éléments Non-Couverts

Les éléments suivants sont **explicitement hors périmètre** de ce cahier des charges :

| Catégorie | Élément exclu |
|-----------|---------------|
| **Technique** | Spécifications techniques détaillées des systèmes A et B |
| **Technique** | Choix technologiques (middleware, protocoles, langages) |
| **Données** | Définition précise des structures de données (schémas, formats) |
| **Données** | Migration de données historiques |
| **Opérationnel** | Procédures d'exploitation et de supervision détaillées |
| **Opérationnel** | Formation des utilisateurs finaux |
| **Financier** | Chiffrage budgétaire et planning détaillé |
| **Déploiement** | Stratégie de déploiement et de migration |

Ces éléments seront traités dans les phases suivantes par l'Architecte Solution et l'équipe de développement.

---

## 8. Handoff vers le Solution Architect

### 8.1 Livrables transmis

| Livrable | Statut | Référence |
|----------|--------|-----------|
| Cahier des charges fonctionnel | ✅ Complet | Ce document |
| Cas d'usage (Use Cases) | ✅ Complet | Section 2 |
| Table des exigences RF | ✅ Complet | Section 3 (RF-001 à RF-010) |
| Table des exigences RNF | ✅ Complet | Section 4 (RNF-001 à RNF-017) |
| Hypothèses documentées | ✅ Complet | Section 5 (H-001 à H-007) |
| Risques identifiés | ✅ Complet | Section 6 (R-001 à R-007) |

### 8.2 Attentes pour le Solution Architect

L'Architecte Solution devra produire les livrables suivants (cf. `artefacts.contract.md`) :

1. **Architecture textuelle** (`systemA-systemB-architecture.md`) incluant :
   - Composants d'intégration et leurs responsabilités
   - Flux de données détaillés (A→B et B→A)
   - Mécanismes de haute disponibilité répondant à RNF-001/002/003
   - Stratégie de reprise sur erreur (RF-007, RF-010)
   - Patterns d'intégration retenus

2. **Diagrammes logiques** (`systemA-systemB-architecture.drawio`) :
   - Vue composants
   - Vue flux de données
   - Vue déploiement

3. **Justification des choix** répondant aux exigences RNF

### 8.3 Questions ouvertes pour le Solution Architect

| ID | Question | Contexte |
|----|----------|----------|
| Q-001 | Quel pattern d'intégration est le plus adapté (EIP, API Gateway, ESB, Event-Driven) ? | Contraintes de haute disponibilité et bidirectionnalité |
| Q-002 | Comment garantir le RPO = 0 (RNF-003) en cas de défaillance ? | Criticité de la non-perte de données |
| Q-003 | Quel mécanisme de monitoring proposer pour satisfaire RNF-016 ? | Exigence de traçabilité et documentation renforcée |
| Q-004 | Faut-il un format pivot canonique entre A et B ? | Risque R-001 d'incompatibilité |
| Q-005 | Quelle stratégie de retry et de dead-letter queue ? | Exigence RF-010 de file d'attente |
| Q-006 | Comment assurer l'évolutivité vers d'autres systèmes (RNF-017) ? | Vision long terme |

### 8.4 Contraintes à respecter

- ✅ Revue sécurité obligatoire (contrainte client-demo)
- ✅ Documentation renforcée (contrainte client-demo)
- ✅ Environnements cloisonnés DEV/UAT/PROD (contrainte client-demo)

---

## Annexes

### A. Glossaire

| Terme | Définition |
|-------|------------|
| systemA | Premier système source/cible de l'échange |
| systemB | Second système source/cible de l'échange |
| RTO | Recovery Time Objective – temps maximal d'interruption acceptable |
| RPO | Recovery Point Objective – perte de données maximale acceptable |
| RF | Exigence Fonctionnelle (Requirement Functional) |
| RNF | Exigence Non-Fonctionnelle (Requirement Non-Functional) |
| EIP | Enterprise Integration Patterns |
| Dead-letter queue | File pour messages non traitables |

### B. Matrice de traçabilité

| Contrainte projet | Exigences associées |
|-------------------|---------------------|
| Haute disponibilité | RNF-001, RNF-002, RNF-003, RF-010 |
| Traçabilité des échanges | RF-003, RF-004, RF-006, RNF-012, RNF-013, RNF-014 |
| Reprise sur erreur | RF-007, RF-010, RNF-002 |

### C. Documents de référence

| Document | Description |
|----------|-------------|
| `AGENTS.base.md` | Framework multi-agent – règles globales |
| `agents/flow.contract.md` | Contrat de flux inter-agents |
| `agents/artefacts.contract.md` | Contrat des livrables par rôle |
| `clients/client-demo/CLIENT.md` | Contraintes spécifiques client |
| `projects/systemA-systemB/PROJECT.md` | Description du projet |

---

*Document généré conformément au framework AGENTS.base.md v1.0*  
*Prêt pour handoff vers Solution Architect*
