# 🔧 Client Manager

Script PowerShell pour gérer les clients dans le système d'agents.

## 📋 Commandes

### Lister les clients disponibles

```powershell
.\client-manager.ps1 -List
```

### Activer un client

```powershell
.\client-manager.ps1 -SetActive "sbm"
```

### Obtenir le client actif

```powershell
.\client-manager.ps1 -GetActive
```

### Créer un nouveau client

```powershell
.\client-manager.ps1 -Create "nouveau-client"
```

## 💻 Code

```powershell
# client-manager.ps1
param(
    [switch]$List,
    [string]$SetActive,
    [switch]$GetActive,
    [string]$Create
)

$clientsPath = ".\.github\clients"
$activeClientFile = "$clientsPath\active-client.json"

function List-Clients {
    Write-Host "📋 Clients disponibles:" -ForegroundColor Cyan
    $clients = Get-ChildItem -Path $clientsPath -Directory | Where-Object { $_.Name -ne "active-client.json" }
    foreach ($client in $clients) {
        $active = ""
        if (Test-Path $activeClientFile) {
            $activeJson = Get-Content $activeClientFile | ConvertFrom-Json
            if ($activeJson.clientKey -eq $client.Name) {
                $active = " ✅ (actif)"
            }
        }
        Write-Host "  - $($client.Name)$active" -ForegroundColor Green
    }
}

function Set-ActiveClient {
    param([string]$clientKey)
    
    $clientPath = "$clientsPath\$clientKey"
    if (!(Test-Path $clientPath)) {
        Write-Host "❌ Client '$clientKey' n'existe pas!" -ForegroundColor Red
        return
    }
    
    $activeJson = @{
        clientKey = $clientKey
        loadedAt = (Get-Date).ToUniversalTime().ToString("o")
        version = "1.0.0"
    } | ConvertTo-Json
    
    $activeJson | Set-Content -Path $activeClientFile
    Write-Host "✅ Client actif: $clientKey" -ForegroundColor Green
}

function Get-ActiveClient {
    if (!(Test-Path $activeClientFile)) {
        Write-Host "⚠️ Aucun client actif" -ForegroundColor Yellow
        return
    }
    
    $activeJson = Get-Content $activeClientFile | ConvertFrom-Json
    Write-Host "📌 Client actif: $($activeJson.clientKey)" -ForegroundColor Cyan
    Write-Host "   Chargé le: $($activeJson.loadedAt)" -ForegroundColor Gray
}

function Create-NewClient {
    param([string]$clientKey)
    
    $clientPath = "$clientsPath\$clientKey"
    if (Test-Path $clientPath) {
        Write-Host "❌ Client '$clientKey' existe déjà!" -ForegroundColor Red
        return
    }
    
    # Créer structure
    New-Item -ItemType Directory -Path $clientPath -Force | Out-Null
    New-Item -ItemType Directory -Path "$clientPath\instructions" -Force | Out-Null
    New-Item -ItemType Directory -Path "$clientPath\knowledge" -Force | Out-Null
    New-Item -ItemType Directory -Path "$clientPath\config" -Force | Out-Null
    New-Item -ItemType Directory -Path "$clientPath\data" -Force | Out-Null
    
    # Copier template depuis default
    if (Test-Path "$clientsPath\default\CLIENT.md") {
        Copy-Item "$clientsPath\default\CLIENT.md" "$clientPath\CLIENT.md"
    }
    
    Write-Host "✅ Client '$clientKey' créé avec succès!" -ForegroundColor Green
    Write-Host "📝 Éditez $clientPath\CLIENT.md pour personnaliser" -ForegroundColor Cyan
}

# Main
if ($List) {
    List-Clients
}
elseif ($SetActive) {
    Set-ActiveClient -clientKey $SetActive
}
elseif ($GetActive) {
    Get-ActiveClient
}
elseif ($Create) {
    Create-NewClient -clientKey $Create
}
else {
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\client-manager.ps1 -List"
    Write-Host "  .\client-manager.ps1 -SetActive <client-key>"
    Write-Host "  .\client-manager.ps1 -GetActive"
    Write-Host "  .\client-manager.ps1 -Create <client-key>"
}
```

## 📝 Exemples

```powershell
# Lister tous les clients
.\client-manager.ps1 -List

# Activer le client SBM
.\client-manager.ps1 -SetActive sbm

# Vérifier le client actif
.\client-manager.ps1 -GetActive

# Créer un nouveau client
.\client-manager.ps1 -Create contoso
```
