#!/usr/bin/env pwsh
<#
.SYNOPSIS
Valide la structure et les références des agents Copilot

.DESCRIPTION
Vérifie que:
- Tous les fichiers agents existent
- Toutes les références aux skills existent
- La syntaxe JSON est valide
- Les chemins de fichiers sont corrects
#>

$ErrorCount = 0
$WarningCount = 0
$SuccessCount = 0

Write-Host "🔍 Validation de l'architecture des Agents GitHub Copilot..." -ForegroundColor Cyan
Write-Host ""

# Test 1: Vérifier les fichiers de config
Write-Host "Test 1️⃣ : Configuration principale" -ForegroundColor Blue
$configPath = ".github/copilot-config.json"
if (Test-Path $configPath) {
    try {
        $config = Get-Content $configPath | ConvertFrom-Json
        Write-Host "✅ copilot-config.json valide ($(($config.agents).Count) agents)" -ForegroundColor Green
        $SuccessCount++
    } catch {
        Write-Host "❌ copilot-config.json - Erreur JSON: $_" -ForegroundColor Red
        $ErrorCount++
    }
} else {
    Write-Host "❌ copilot-config.json non trouvé" -ForegroundColor Red
    $ErrorCount++
}
Write-Host ""

# Test 2: Vérifier les fichiers agents
Write-Host "Test 2️⃣ : Fichiers Agents" -ForegroundColor Blue
$agents = @("architecte", "developpeur", "reviewer")
foreach ($agent in $agents) {
    $agentFile = ".github/agents/$agent.md"
    if (Test-Path $agentFile) {
        $content = Get-Content $agentFile -Raw
        
        # Vérifier si les skills sont référencés
        $skillMatches = [regex]::Matches($content, '<file>\.github/skills/([^/]+)/SKILL\.md</file>')
        if ($skillMatches.Count -gt 0) {
            Write-Host "✅ $agent.md (${skillMatches.Count} skills référencés)" -ForegroundColor Green
            $SuccessCount++
        } else {
            Write-Host "⚠️  $agent.md - Aucun skill référencé" -ForegroundColor Yellow
            $WarningCount++
        }
    } else {
        Write-Host "❌ $agent.md non trouvé" -ForegroundColor Red
        $ErrorCount++
    }
}
Write-Host ""

# Test 3: Vérifier que tous les skills référencés existent
Write-Host "Test 3️⃣ : Existence des Skills" -ForegroundColor Blue
$skillsReferenced = @()
foreach ($agent in $agents) {
    $agentFile = ".github/agents/$agent.md"
    if (Test-Path $agentFile) {
        $content = Get-Content $agentFile -Raw
        $matches = [regex]::Matches($content, '\.github/skills/([^/]+)/SKILL\.md')
        foreach ($match in $matches) {
            if ($skillsReferenced -notcontains $match.Groups[1].Value) {
                $skillsReferenced += $match.Groups[1].Value
            }
        }
    }
}

foreach ($skill in $skillsReferenced) {
    $skillPath = ".github/skills/$skill/SKILL.md"
    if (Test-Path $skillPath) {
        Write-Host "✅ Skill: $skill" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "❌ Skill: $skill - INTROUVABLE" -ForegroundColor Red
        $ErrorCount++
    }
}
Write-Host ""

# Test 4: Vérifier les instructions
Write-Host "Test 4️⃣ : Fichiers d'Instructions" -ForegroundColor Blue
$instructionFiles = Get-ChildItem -Path ".github/instructions" -Filter "*.md" -ErrorAction SilentlyContinue
if ($instructionFiles) {
    Write-Host "✅ Instructions trouvées: $($instructionFiles.Count) fichiers" -ForegroundColor Green
    $SuccessCount++
} else {
    Write-Host "⚠️  Aucun fichier d'instructions trouvé" -ForegroundColor Yellow
    $WarningCount++
}
Write-Host ""

# Test 5: Vérifier la knowledge base
Write-Host "Test 5️⃣ : Knowledge Base" -ForegroundColor Blue
$knowledgeFiles = Get-ChildItem -Path ".github/knowledge" -Filter "*.md" -Recurse -ErrorAction SilentlyContinue
if ($knowledgeFiles) {
    Write-Host "✅ Knowledge trouvée: $($knowledgeFiles.Count) fichiers" -ForegroundColor Green
    $SuccessCount++
} else {
    Write-Host "⚠️  Aucun fichier knowledge trouvé" -ForegroundColor Yellow
    $WarningCount++
}
Write-Host ""

# Résumé
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📊 RÉSUMÉ DE VALIDATION" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "✅ Succès: $SuccessCount" -ForegroundColor Green
Write-Host "⚠️  Avertissements: $WarningCount" -ForegroundColor Yellow
Write-Host "❌ Erreurs: $ErrorCount" -ForegroundColor Red

if ($ErrorCount -eq 0 -and $WarningCount -eq 0) {
    Write-Host ""
    Write-Host "✨ Tous les tests sont passés avec succès!" -ForegroundColor Green
    exit 0
} elseif ($ErrorCount -gt 0) {
    Write-Host ""
    Write-Host "⚠️  Des erreurs ont été détectées. Veuillez les corriger." -ForegroundColor Red
    exit 1
} else {
    Write-Host ""
    Write-Host "⚠️  Quelques avertissements détectés." -ForegroundColor Yellow
    exit 0
}
