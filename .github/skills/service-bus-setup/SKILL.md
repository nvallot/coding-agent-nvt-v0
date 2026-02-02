# Skill: Azure Service Bus Setup

**Type**: Messaging Configuration  
**Niveau**: Intermédiaire  
**Prérequis**: Azure CLI, namespace Service Bus créé

## Description

Cette skill détaille la configuration d'Azure Service Bus : création de queues, topics, subscriptions, et gestion des access policies. Adapter les noms selon `knowledge/<client>/naming-conventions.md`.

## Prérequis

- Service Bus Namespace créé (format: `sb-<client>-<project>-<env>-<instance>`)
- Permissions sur le namespace (Azure Service Bus Data Owner ou Contributor)
- Azure CLI authentifié

Voir `knowledge/<client>/naming-conventions.md` pour les conventions spécifiques.

## Étapes

### 1. Créer une Queue

```powershell
az servicebus queue create `
  --resource-group <resource-group-name> `
  --namespace-name <namespace-name> `
  --name <queue-name> `
  --max-size 1024 `
  --default-message-time-to-live P14D
```

Options recommandées :
- `--enable-dead-lettering-on-message-expiration true`
- `--max-delivery-count 10`

### 2. Créer un Topic et une Subscription

```powershell
# Topic
az servicebus topic create `
  --resource-group <resource-group-name> `
  --namespace-name <namespace-name> `
  --name <topic-name> `
  --max-size 1024

# Subscription
az servicebus topic subscription create `
  --resource-group <resource-group-name> `
  --namespace-name <namespace-name> `
  --topic-name <topic-name> `
  --name <subscription-name>
```

### 3. Configurer une Shared Access Policy

```powershell
az servicebus queue authorization-rule create `
  --resource-group <resource-group-name> `
  --namespace-name <namespace-name> `
  --queue-name <queue-name> `
  --name <policy-name> `
  --rights Send Listen
```

### 4. Récupérer la Connection String

```powershell
az servicebus namespace authorization-rule keys list `
  --resource-group <resource-group-name> `
  --namespace-name <namespace-name> `
  --name RootManageSharedAccessKey `
  --query primaryConnectionString -o tsv
```

Stocker dans Azure Key Vault du client.

### 5. Tester avec Service Bus Explorer

Utiliser l'outil Service Bus Explorer dans le portail Azure pour envoyer/recevoir des messages de test.

## Naming Conventions

Utiliser les conventions du client (voir `knowledge/<client>/naming-conventions.md`).

Exemple générique :

| Ressource | Format |
|-----------|--------|
| Namespace | `sb-<client>-<project>-<env>-<instance>` |
| Queue | `queue-<domain>` |
| Topic | `topic-<domain>` |
| Subscription | `sub-<consumer>` |

## Références

- Naming conventions: `knowledge/<client>/naming-conventions.md`
- [Service Bus best practices (Azure)](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-performance-improvements)
- Client architecture guidelines: `clients/<client>/instructions/`

## Related Skills

- `bicep-deployment`: Provisionner le namespace Service Bus
- `azure-function-deployment`: Configurer Function avec Service Bus trigger
