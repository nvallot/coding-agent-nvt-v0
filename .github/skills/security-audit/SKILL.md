# Skill: Security Audit

## Description
Compétence pour identifier et corriger les vulnérabilités et failles de sécurité dans le code.

## Objectif
Fournir une approche systématique pour auditer la sécurité et implémenter des défenses robustes.

## Domaines de Sécurité

### 1. Authentification & Autorisation
- **Authentification**: Vérifier l'identité (login, JWT, OAuth)
- **Autorisation**: Vérifier les permissions (roles, ACL)
- **Risques**: Contournement, escalade de privilèges
- **Validation**: Tests d'accès, revue des rôles

### 2. Injection d'Attaques
- **SQL Injection**: Paramétrer les requêtes
- **Command Injection**: Valider les entrées
- **XSS (Cross-Site Scripting)**: Échapper les sorties
- **NoSQL Injection**: Éviter l'interpolation
- **LDAP Injection**: Filtrer les entrées

### 3. Validation des Entrées
- Valider tous les inputs
- Lister blanche, pas liste noire
- Longueur et format
- Rejeter les données invalides
- Logging des tentatives

### 4. Gestion des Secrets
- Ne jamais stocker en dur
- Utiliser un vault (Azure Key Vault)
- Rotation régulière
- Audit des accès
- Chiffrement en transit et au repos

### 5. Sécurité Web
- **CSRF**: Tokens CSRF
- **CORS**: Configuration stricte
- **Headers**: CSP, X-Frame-Options
- **HTTPS**: TLS/SSL obligatoire
- **Cookies**: Secure, HttpOnly, SameSite

### 6. Gestion des Erreurs
- Ne pas révéler d'infos sensibles
- Logging des erreurs côté serveur
- Messages génériques côté client
- Stack traces en développement seulement
- Monitoring et alertes

### 7. Dépendances
- Auditer les packages
- Mettre à jour régulièrement
- Outils: npm audit, Snyk, Dependabot
- CVE monitoring
- Lockfiles versionnés

### 8. Sécurité Infrastruture
- Firewall et VPN
- Encryption en transit (TLS)
- Encryption au repos
- Chiffrement des DB
- Isolation des réseaux

## Checklist de Sécurité

### Avant le Déploiement
- [ ] Validation des inputs
- [ ] Pas de secrets en dur
- [ ] Authentification activée
- [ ] Logs de sécurité en place
- [ ] Headers sécurité configurés
- [ ] HTTPS obligatoire
- [ ] Rate limiting activé

### Code
- [ ] Pas de vulnérabilités OWASP Top 10
- [ ] Dependencies à jour
- [ ] Pas de hardcoded credentials
- [ ] Error handling propre
- [ ] SQL paramétrisé
- [ ] XSS prevention
- [ ] CSRF tokens

### Infrastructure
- [ ] Firewall configuré
- [ ] VPN pour prod access
- [ ] Chiffrement données
- [ ] Backups sécurisés
- [ ] Logs centralisés
- [ ] Alerting actif
- [ ] Incident response plan

## Vulnérabilités Courantes

### OWASP Top 10
1. Broken Access Control
2. Cryptographic Failures
3. Injection
4. Insecure Design
5. Security Misconfiguration
6. Vulnerable & Outdated Components
7. Authentication Failures
8. Data Integrity Failures
9. Logging & Monitoring Failures
10. SSRF

## Outils d'Audit

### Code Analysis
- **SonarQube**: Code quality et security
- **Semgrep**: Pattern-based scanning
- **OWASP ZAP**: Web application scanner
- **Burp Suite**: Security testing

### Dependency Scanning
- **npm audit**: npm packages
- **Snyk**: Multi-langue
- **Dependabot**: GitHub integration
- **OWASP Dependency-Check**: Java focus

### Infrastructure
- **Trivy**: Container scanning
- **Tenable Nessus**: Vulnerability scanner
- **Qualys**: Cloud security
- **Rapid7 InsightVM**: Vulnerability management

## Par Langage

### JavaScript/TypeScript
- Valider avec Joi ou Zod
- Hasher les passwords (bcrypt)
- JWT pour l'authentification
- CORS strictement configuré

### Python
- SQLAlchemy ORM (prévient SQL injection)
- Werkzeug/Passlib pour passwords
- Flask-Security pour auth
- Paramétrer les queries

### Java
- Prepared Statements
- Spring Security
- OWASP ESAPI
- Maven dependency check

### C#/.NET
- Entity Framework (ORM)
- ASP.NET Identity
- BCrypt.Net pour passwords
- NuGet audit

## Exemples d'Utilisation

### Auditer le code
```
@reviewer /security audit module authentication
```

### Vérifier les vulnérabilités
```
@reviewer /security check for OWASP Top 10
```

### Analyser les dependencies
```
@reviewer /security audit npm dependencies
```

## Critères de Succès
- ✅ Pas de vulnérabilités critiques
- ✅ Secrets sécurisés
- ✅ Authentification/Autorisation testées
- ✅ Inputs validés
- ✅ Logs en place
- ✅ Dependencies à jour
- ✅ Plan d'incident

## Ressources
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CWE Top 25](https://cwe.mitre.org/top25/)
- [OWASP Cheat Sheets](https://cheatsheetseries.owasp.org/)
- [Security.txt](https://securitytxt.org/)
