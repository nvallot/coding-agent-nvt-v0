#!/usr/bin/env pwsh
<#
.SYNOPSIS
Workflow interactif pour orchestrer les agents GitHub Copilot

.DESCRIPTION
Permet de créer un workflow complet ou partiel en orchestrant
les agents (Architecte -> Développeur -> Reviewer) avec validation
à chaque étape.
#>

$ErrorActionPreference = "Stop"

# Configuration
$WorkflowHistory = @()
$CurrentStep = 0

# Couleurs
$colors = @{
    Header = "Cyan"
    Agent = "Green"
    Step = "Yellow"
    Warning = "Red"
    Info = "Blue"
    Success = "Green"
    Prompt = "Magenta"
    Command = "DarkYellow"
}

function Show-Banner {
    Clear-Host
    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║     🔄 WORKFLOW ORCHESTRATOR - GitHub Copilot Agents         ║" -ForegroundColor Cyan
    Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

function Show-MainMenu {
    Write-Host "🎯 Choisissez votre workflow:" -ForegroundColor $colors.Header
    Write-Host ""
    Write-Host "Workflows Complets:" -ForegroundColor $colors.Info
    Write-Host "  1. 🏗️➡️💻➡️👀 Workflow Complet (Architecture → Dev → Review)" -ForegroundColor $colors.Step
    Write-Host "  2. 🏗️➡️💻    Architecture + Développement" -ForegroundColor $colors.Step
    Write-Host "  3. 💻➡️👀    Développement + Review" -ForegroundColor $colors.Step
    Write-Host ""
    Write-Host "Workflows Partiels:" -ForegroundColor $colors.Info
    Write-Host "  4. 🏗️         Architecture uniquement" -ForegroundColor $colors.Step
    Write-Host "  5. 💻         Développement uniquement" -ForegroundColor $colors.Step
    Write-Host "  6. 👀         Review uniquement" -ForegroundColor $colors.Step
    Write-Host ""
    Write-Host "  7. 📋         Voir l'historique du workflow" -ForegroundColor $colors.Info
    Write-Host "  8. 🚪         Quitter" -ForegroundColor $colors.Warning
    Write-Host ""
}

function Get-FileContext {
    Write-Host ""
    Write-Host "📂 Contexte de travail" -ForegroundColor $colors.Header
    Write-Host ""
    
    $useFile = Read-Host "Travailler sur un fichier spécifique? (o/n)"
    
    if ($useFile -eq "o" -or $useFile -eq "O") {
        $filePath = Read-Host "Chemin du fichier (relatif ou absolu)"
        
        if ($filePath -and (Test-Path $filePath)) {
            return @{
                hasFile = $true
                path = $filePath
                name = Split-Path -Leaf $filePath
            }
        } else {
            Write-Host "⚠️  Fichier non trouvé. Continuation sans fichier." -ForegroundColor $colors.Warning
            Start-Sleep -Seconds 2
            return @{ hasFile = $false }
        }
    }
    
    return @{ hasFile = $false }
}

function Get-UserRequirement {
    param([string]$StepName)
    
    Write-Host ""
    Write-Host "📝 Décrivez ce que vous souhaitez:" -ForegroundColor $colors.Prompt
    $requirement = Read-Host "> "
    
    return $requirement
}

function Show-AgentPrompt {
    param(
        [string]$Agent,
        [string]$Requirement,
        [hashtable]$FileContext
    )
    
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    Write-Host "🤖 Prompt à exécuter dans VS Code:" -ForegroundColor $colors.Agent
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    Write-Host ""
    
    $prompt = "@${Agent} ${Requirement}"
    
    if ($FileContext.hasFile) {
        Write-Host "📄 Fichier: $($FileContext.name)" -ForegroundColor $colors.Info
        Write-Host ""
    }
    
    Write-Host $prompt -ForegroundColor $colors.Command
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    
    # Copier dans le presse-papier si possible
    try {
        Set-Clipboard -Value $prompt
        Write-Host "✅ Prompt copié dans le presse-papier!" -ForegroundColor $colors.Success
    } catch {
        Write-Host "⚠️  Impossible de copier dans le presse-papier. Copiez manuellement." -ForegroundColor $colors.Warning
    }
    
    Write-Host ""
    Write-Host "👉 Collez ce prompt dans VS Code Copilot Chat" -ForegroundColor $colors.Info
    Write-Host ""
    
    return $prompt
}

function Get-AgentSuggestions {
    param([string]$Agent)
    
    $suggestions = @{
        "architecte" = @(
            "/diagramme Créer un diagramme d'architecture",
            "/tad Générer un document d'architecture technique",
            "/patterns Suggérer des patterns pour",
            "/adr Créer un Architecture Decision Record pour",
            "Concevoir l'architecture de",
            "Quels patterns recommandes-tu pour"
        )
        "developpeur" = @(
            "/implement Implémenter la fonctionnalité",
            "/refactor Refactoriser le code",
            "/test Créer des tests pour",
            "/debug Déboguer le problème",
            "Créer une API pour",
            "Optimiser les performances de"
        )
        "reviewer" = @(
            "/review Revoir le code",
            "/security Auditer la sécurité de",
            "/performance Analyser les performances de",
            "/conventions Vérifier les conventions sur",
            "Quels problèmes vois-tu dans",
            "Comment améliorer"
        )
    }
    
    Write-Host ""
    Write-Host "💡 Suggestions de commandes pour `@$Agent`:" -ForegroundColor $colors.Info
    Write-Host ""
    
    if ($suggestions.ContainsKey($Agent)) {
        $idx = 1
        foreach ($suggestion in $suggestions[$Agent]) {
            Write-Host "  $idx. $suggestion" -ForegroundColor $colors.Step
            $idx++
        }
    }
    
    Write-Host ""
}

function Wait-ForUserValidation {
    param([string]$StepName)
    
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    Write-Host "✋ Validation de l'étape: $StepName" -ForegroundColor $colors.Prompt
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    Write-Host ""
    Write-Host "Le résultat de l'agent vous convient-il?" -ForegroundColor $colors.Prompt
    Write-Host ""
    Write-Host "1. ✅ Oui, passer à l'étape suivante" -ForegroundColor $colors.Success
    Write-Host "2. 🔄 Non, réitérer avec des ajustements" -ForegroundColor $colors.Warning
    Write-Host "3. ❌ Annuler le workflow" -ForegroundColor $colors.Warning
    Write-Host ""
    
    $choice = Read-Host "Votre choix (1-3)"
    
    return $choice
}

function Request-Adjustments {
    Write-Host ""
    Write-Host "🔧 Que souhaitez-vous ajuster?" -ForegroundColor $colors.Prompt
    $adjustments = Read-Host "> "
    
    return $adjustments
}

function Execute-WorkflowStep {
    param(
        [string]$Agent,
        [string]$StepName,
        [hashtable]$FileContext,
        [string]$PreviousOutput = ""
    )
    
    $stepSuccess = $false
    $iteration = 1
    
    while (-not $stepSuccess) {
        Clear-Host
        Show-Banner
        
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
        Write-Host "📍 Étape: $StepName (Itération $iteration)" -ForegroundColor $colors.Step
        Write-Host "🤖 Agent: @${Agent}" -ForegroundColor $colors.Agent
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
        
        if ($iteration -eq 1) {
            Get-AgentSuggestions -Agent $Agent
            $requirement = Get-UserRequirement -StepName $StepName
        } else {
            Write-Host ""
            Write-Host "🔄 Ajustements demandés:" -ForegroundColor $colors.Warning
            Write-Host $adjustmentText -ForegroundColor $colors.Info
            Write-Host ""
            $requirement = $adjustmentText
        }
        
        if ($PreviousOutput) {
            Write-Host ""
            Write-Host "📋 Contexte de l'étape précédente disponible" -ForegroundColor $colors.Info
            Write-Host ""
        }
        
        $prompt = Show-AgentPrompt -Agent $Agent -Requirement $requirement -FileContext $FileContext
        
        # Enregistrer dans l'historique
        $script:WorkflowHistory += @{
            step = $StepName
            agent = $Agent
            iteration = $iteration
            prompt = $prompt
            timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        }
        
        Write-Host "⏳ Exécutez le prompt dans VS Code et revenez ici une fois terminé..." -ForegroundColor $colors.Info
        Write-Host ""
        Read-Host "Appuyez sur Entrée quand vous avez le résultat"
        
        $validation = Wait-ForUserValidation -StepName $StepName
        
        switch ($validation) {
            "1" {
                $stepSuccess = $true
                Write-Host ""
                Write-Host "✅ Étape validée!" -ForegroundColor $colors.Success
                Write-Host ""
                Start-Sleep -Seconds 1
            }
            "2" {
                $adjustmentText = Request-Adjustments
                $iteration++
            }
            "3" {
                Write-Host ""
                Write-Host "❌ Workflow annulé" -ForegroundColor $colors.Warning
                return $false
            }
            default {
                Write-Host "Choix invalide, considéré comme validation." -ForegroundColor $colors.Warning
                $stepSuccess = $true
            }
        }
    }
    
    return $true
}

function Show-WorkflowHistory {
    Clear-Host
    Show-Banner
    
    Write-Host "📋 Historique du Workflow" -ForegroundColor $colors.Header
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    Write-Host ""
    
    if ($script:WorkflowHistory.Count -eq 0) {
        Write-Host "Aucun historique disponible." -ForegroundColor $colors.Info
    } else {
        foreach ($entry in $script:WorkflowHistory) {
            Write-Host "[$($entry.timestamp)] " -NoNewline -ForegroundColor $colors.Info
            Write-Host "$($entry.step) " -NoNewline -ForegroundColor $colors.Step
            Write-Host "(@$($entry.agent), itération $($entry.iteration))" -ForegroundColor $colors.Agent
            Write-Host "  Prompt: $($entry.prompt)" -ForegroundColor $colors.Command
            Write-Host ""
        }
    }
    
    Write-Host ""
    Read-Host "Appuyez sur Entrée pour continuer"
}

function Execute-ArchitectureStep {
    param([hashtable]$FileContext)
    
    return Execute-WorkflowStep -Agent "architecte" -StepName "Architecture" -FileContext $FileContext
}

function Execute-DevelopmentStep {
    param([hashtable]$FileContext, [string]$PreviousOutput = "")
    
    return Execute-WorkflowStep -Agent "developpeur" -StepName "Développement" -FileContext $FileContext -PreviousOutput $PreviousOutput
}

function Execute-ReviewStep {
    param([hashtable]$FileContext, [string]$PreviousOutput = "")
    
    return Execute-WorkflowStep -Agent "reviewer" -StepName "Code Review" -FileContext $FileContext -PreviousOutput $PreviousOutput
}

function Execute-CompleteWorkflow {
    Clear-Host
    Show-Banner
    
    Write-Host "🚀 Démarrage du workflow complet" -ForegroundColor $colors.Success
    Write-Host ""
    
    $fileContext = Get-FileContext
    
    # Étape 1: Architecture
    $success = Execute-ArchitectureStep -FileContext $fileContext
    if (-not $success) { return }
    
    # Étape 2: Développement
    $success = Execute-DevelopmentStep -FileContext $fileContext -PreviousOutput "architecture-done"
    if (-not $success) { return }
    
    # Étape 3: Review
    $success = Execute-ReviewStep -FileContext $fileContext -PreviousOutput "development-done"
    if (-not $success) { return }
    
    # Workflow terminé
    Clear-Host
    Show-Banner
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Success
    Write-Host "🎉 Workflow Complet Terminé!" -ForegroundColor $colors.Success
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Success
    Write-Host ""
    Write-Host "✅ Architecture: Validée" -ForegroundColor $colors.Success
    Write-Host "✅ Développement: Validé" -ForegroundColor $colors.Success
    Write-Host "✅ Code Review: Validé" -ForegroundColor $colors.Success
    Write-Host ""
    Read-Host "Appuyez sur Entrée pour revenir au menu"
}

function Execute-ArchDevWorkflow {
    Clear-Host
    Show-Banner
    
    Write-Host "🚀 Workflow Architecture + Développement" -ForegroundColor $colors.Success
    Write-Host ""
    
    $fileContext = Get-FileContext
    
    $success = Execute-ArchitectureStep -FileContext $fileContext
    if (-not $success) { return }
    
    $success = Execute-DevelopmentStep -FileContext $fileContext -PreviousOutput "architecture-done"
    if (-not $success) { return }
    
    Clear-Host
    Show-Banner
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Success
    Write-Host "🎉 Workflow Architecture + Dev Terminé!" -ForegroundColor $colors.Success
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Success
    Write-Host ""
    Read-Host "Appuyez sur Entrée pour revenir au menu"
}

function Execute-DevReviewWorkflow {
    Clear-Host
    Show-Banner
    
    Write-Host "🚀 Workflow Développement + Review" -ForegroundColor $colors.Success
    Write-Host ""
    
    $fileContext = Get-FileContext
    
    $success = Execute-DevelopmentStep -FileContext $fileContext
    if (-not $success) { return }
    
    $success = Execute-ReviewStep -FileContext $fileContext -PreviousOutput "development-done"
    if (-not $success) { return }
    
    Clear-Host
    Show-Banner
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Success
    Write-Host "🎉 Workflow Dev + Review Terminé!" -ForegroundColor $colors.Success
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Success
    Write-Host ""
    Read-Host "Appuyez sur Entrée pour revenir au menu"
}

function Main {
    while ($true) {
        Clear-Host
        Show-Banner
        Show-MainMenu
        
        $choice = Read-Host "Votre choix (1-8)"
        
        switch ($choice) {
            "1" { Execute-CompleteWorkflow }
            "2" { Execute-ArchDevWorkflow }
            "3" { Execute-DevReviewWorkflow }
            "4" {
                $fileContext = Get-FileContext
                Execute-ArchitectureStep -FileContext $fileContext
                Read-Host "Appuyez sur Entrée pour continuer"
            }
            "5" {
                $fileContext = Get-FileContext
                Execute-DevelopmentStep -FileContext $fileContext
                Read-Host "Appuyez sur Entrée pour continuer"
            }
            "6" {
                $fileContext = Get-FileContext
                Execute-ReviewStep -FileContext $fileContext
                Read-Host "Appuyez sur Entrée pour continuer"
            }
            "7" { Show-WorkflowHistory }
            "8" {
                Write-Host ""
                Write-Host "👋 Au revoir!" -ForegroundColor $colors.Success
                Write-Host ""
                exit 0
            }
            default {
                Write-Host "Choix invalide" -ForegroundColor $colors.Warning
                Start-Sleep -Seconds 2
            }
        }
    }
}

# Lancer le programme
Main
