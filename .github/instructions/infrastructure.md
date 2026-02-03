---
applyTo: "infrastructure/**"
---

# Instructions Infrastructure

Quand tu travailles dans le dossier infrastructure:

- Infrastructure as Code (Bicep ou Terraform)
- Variables par environnement
- Modules réutilisables
- Documentation des ressources

```bicep
// Module réutilisable
@description('The location for all resources')
param location string = resourceGroup().location

@description('The environment name')
@allowed(['dev', 'staging', 'prod'])
param environment string

@description('The application name')
param appName string

var resourcePrefix = '${appName}-${environment}'

// Resources definitions...
```
