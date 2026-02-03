#!/usr/bin/env pwsh
<#
.SYNOPSIS
Script de test interactif pour les agents GitHub Copilot

.DESCRIPTION
Permet de tester les agents en simulant des conversations et affichant
les instructions, skills, et comportements attendus.
#>

$ErrorActionPreference = "Stop"

# Couleurs
$colors = @{
    Header = "Cyan"
    Agent = "Green"
    Skill = "Yellow"
    Warning = "Red"
    Info = "Blue"
    Success = "Green"
    Prompt = "Magenta"
}

function Show-Banner {
    Write-Host ""
    Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║       🤖 TEST INTERACTIF - GitHub Copilot Agents          ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

function Show-AgentList {
    Write-Host "📋 Agents disponibles:" -ForegroundColor $colors.Header
    Write-Host ""
    Write-Host "1. 🏗️  Architecte     - Conception et architecture système" -ForegroundColor $colors.Info
    Write-Host "2. 💻 Développeur    - Implémentation et développement" -ForegroundColor $colors.Info
    Write-Host "3. 👀 Reviewer       - Revues de code et qualité" -ForegroundColor $colors.Info
    Write-Host "4. 🚪 Quitter" -ForegroundColor $colors.Info
    Write-Host ""
}

function Read-AgentChoice {
    $choice = Read-Host "Choisissez un agent (1-4)"
    return $choice
}

function Load-AgentFile {
    param([string]$Agent)
    
    $filePath = ".github/agents/$Agent.md"
    if (Test-Path $filePath) {
        return Get-Content $filePath -Raw
    }
    return $null
}

function Load-SkillFiles {
    param([string]$Content)
    
    $skillMatches = [regex]::Matches($Content, '<file>\.github/skills/([^/]+)/SKILL\.md</file>')
    $skills = @()
    
    foreach ($match in $skillMatches) {
        $skillName = $match.Groups[1].Value
        $skillPath = ".github/skills/$skillName/SKILL.md"
        if (Test-Path $skillPath) {
            $skills += @{
                name = $skillName
                content = Get-Content $skillPath -Raw
            }
        }
    }
    
    return $skills
}

function Show-AgentInfo {
    param(
        [string]$Agent,
        [string]$AgentContent,
        [array]$Skills
    )
    
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    
    # Extraire les premières lignes (titre et description)
    $lines = $AgentContent -split "`n"
    $title = $lines[0] -replace "^# ", ""
    
    Write-Host ""
    Write-Host "📌 Agent: $title" -ForegroundColor $colors.Agent
    Write-Host ""
    
    # Afficher les sections principales
    $inIdentity = $false
    $inExpertise = $false
    $inSkills = $false
    
    foreach ($line in $lines) {
        if ($line -match "^## Identité") { $inIdentity = $true; $inExpertise = $false; $inSkills = $false; continue }
        if ($line -match "^## Expertise") { $inIdentity = $false; $inExpertise = $true; $inSkills = $false; continue }
        if ($line -match "^## Compétences") { $inIdentity = $false; $inExpertise = $false; $inSkills = $true; continue }
        if ($line -match "^##") { $inIdentity = $false; $inExpertise = $false; $inSkills = $false; continue }
        
        if ($inIdentity -and $line.Trim() -and -not $line.StartsWith("##")) {
            Write-Host "  $line" -ForegroundColor $colors.Info
        }
        if ($inExpertise -and $line.Trim() -and $line.StartsWith("-")) {
            Write-Host "  $line" -ForegroundColor $colors.Info
        }
    }
    
    # Afficher les skills
    Write-Host ""
    Write-Host "🎯 Skills disponibles:" -ForegroundColor $colors.Skill
    foreach ($skill in $Skills) {
        Write-Host "   • $($skill.name)" -ForegroundColor $colors.Skill
    }
    
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
    Write-Host ""
}

function Show-SkillDetails {
    param([array]$Skills)
    
    Write-Host "🔍 Détails des Skills:" -ForegroundColor $colors.Header
    Write-Host ""
    
    foreach ($skill in $Skills) {
        Write-Host "▶ $($skill.name)" -ForegroundColor $colors.Skill
        
        # Extraire la description
        $lines = $skill.content -split "`n"
        $descLine = $lines | Where-Object { $_ -match "^## Description" }
        
        $desc = @()
        $inDesc = $false
        
        foreach ($line in $lines) {
            if ($line -match "^## Description") { $inDesc = $true; continue }
            if ($line -match "^## " -and $inDesc) { break }
            if ($inDesc -and $line.Trim()) {
                $desc += $line
            }
        }
        
        if ($desc) {
            Write-Host "  $($desc[0])" -ForegroundColor $colors.Info
        }
        Write-Host ""
    }
}

function Show-SkillFullContent {
    param(
        [string]$SkillName,
        [array]$Skills
    )
    
    $skill = $Skills | Where-Object { $_.name -eq $SkillName }
    
    if ($skill) {
        Write-Host ""
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
        Write-Host "📚 Détail du Skill: $SkillName" -ForegroundColor $colors.Header
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $colors.Header
        Write-Host ""
        
        # Afficher les 30 premières lignes
        $lines = $skill.content -split "`n"
        $displayLines = $lines[0..29]
        
        foreach ($line in $displayLines) {
            if ($line -match "^#") {
                Write-Host $line -ForegroundColor $colors.Header
            } elseif ($line -match "^##") {
                Write-Host $line -ForegroundColor $colors.Agent
            } elseif ($line -match "^###") {
                Write-Host $line -ForegroundColor $colors.Skill
            } elseif ($line.Trim().StartsWith("-")) {
                Write-Host $line -ForegroundColor $colors.Info
            } else {
                Write-Host $line
            }
        }
        
        Write-Host ""
        Write-Host "[Voir le fichier complet: .github/skills/$SkillName/SKILL.md]" -ForegroundColor $colors.Warning
    }
}

function Show-TestExamples {
    param([string]$Agent)
    
    $examples = @{
        "architecte" = @(
            "@architecte /diagramme système de paiement",
            "@architecte /tad microservices avec Azure",
            "@architecte /patterns problème: scalabilité",
            "@architecte /adr choisir entre monolithe et microservices"
        )
        "developpeur" = @(
            "@developpeur /implement authentification JWT",
            "@developpeur /refactor cette fonction legacy",
            "@developpeur /test créer tests unitaires",
            "@developpeur /debug TypeError: undefined is not a function"
        )
        "reviewer" = @(
            "@reviewer /review code d'authentification",
            "@reviewer /security audit module paiement",
            "@reviewer /performance analyser ce endpoint API",
            "@reviewer /conventions vérifier ce code"
        )
    }
    
    Write-Host ""
    Write-Host "💡 Exemples d'utilisation de @${Agent}:" -ForegroundColor $colors.Header
    Write-Host ""
    
    if ($examples.ContainsKey($Agent)) {
        foreach ($example in $examples[$Agent]) {
            Write-Host "  > $example" -ForegroundColor $colors.Prompt
        }
    }
    
    Write-Host ""
}

function Interactive-TestMenu {
    param(
        [string]$Agent,
        [array]$Skills
    )
    
    while ($true) {
        Write-Host "Que voulez-vous faire?" -ForegroundColor $colors.Prompt
        Write-Host ""
        Write-Host "1. 📖 Voir les détails des skills" -ForegroundColor $colors.Info
        Write-Host "2. 📚 Voir le contenu d'un skill" -ForegroundColor $colors.Info
        Write-Host "3. 💬 Exemples d'utilisation" -ForegroundColor $colors.Info
        Write-Host "4. ⬅️  Retour à la sélection d'agent" -ForegroundColor $colors.Info
        Write-Host ""
        
        $choice = Read-Host "Votre choix (1-4)"
        
        switch ($choice) {
            "1" {
                Show-SkillDetails -Skills $Skills
            }
            "2" {
                Write-Host ""
                Write-Host "Skills disponibles:" -ForegroundColor $colors.Skill
                for ($i = 0; $i -lt $Skills.Count; $i++) {
                    Write-Host "$($i + 1). $($Skills[$i].name)" -ForegroundColor $colors.Info
                }
                Write-Host ""
                
                $skillChoice = Read-Host "Choisissez un skill (numéro)"
                
                if ($skillChoice -match "^\d+$" -and [int]$skillChoice -gt 0 -and [int]$skillChoice -le $Skills.Count) {
                    $selectedSkill = $Skills[[int]$skillChoice - 1]
                    Show-SkillFullContent -SkillName $selectedSkill.name -Skills $Skills
                }
            }
            "3" {
                Show-TestExamples -Agent $Agent
            }
            "4" {
                return
            }
            default {
                Write-Host "Choix invalide" -ForegroundColor $colors.Warning
            }
        }
        
        Write-Host ""
        Read-Host "Appuyez sur Entrée pour continuer"
        Write-Host ""
        Clear-Host
    }
}

function Main {
    while ($true) {
        Clear-Host
        Show-Banner
        Show-AgentList
        
        $choice = Read-AgentChoice
        
        switch ($choice) {
            "1" {
                $agent = "architecte"
                $agentContent = Load-AgentFile -Agent $agent
                $skills = Load-SkillFiles -Content $agentContent
                
                if ($agentContent) {
                    Clear-Host
                    Show-Banner
                    Show-AgentInfo -Agent $agent -AgentContent $agentContent -Skills $skills
                    Interactive-TestMenu -Agent $agent -Skills $skills
                }
            }
            "2" {
                $agent = "developpeur"
                $agentContent = Load-AgentFile -Agent $agent
                $skills = Load-SkillFiles -Content $agentContent
                
                if ($agentContent) {
                    Clear-Host
                    Show-Banner
                    Show-AgentInfo -Agent $agent -AgentContent $agentContent -Skills $skills
                    Interactive-TestMenu -Agent $agent -Skills $skills
                }
            }
            "3" {
                $agent = "reviewer"
                $agentContent = Load-AgentFile -Agent $agent
                $skills = Load-SkillFiles -Content $agentContent
                
                if ($agentContent) {
                    Clear-Host
                    Show-Banner
                    Show-AgentInfo -Agent $agent -AgentContent $agentContent -Skills $skills
                    Interactive-TestMenu -Agent $agent -Skills $skills
                }
            }
            "4" {
                Write-Host ""
                Write-Host "Au revoir! 👋" -ForegroundColor $colors.Success
                Write-Host ""
                exit 0
            }
            default {
                Write-Host "Choix invalide. Veuillez réessayer." -ForegroundColor $colors.Warning
                Read-Host "Appuyez sur Entrée pour continuer"
            }
        }
    }
}

# Lancer le programme principal
Main
