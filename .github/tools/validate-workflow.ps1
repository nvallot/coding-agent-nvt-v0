<#
.SYNOPSIS
    Valide les fichiers requis pour chaque phase du workflow agent.

.DESCRIPTION
    Ce script vérifie l'existence des fichiers obligatoires pour le workflow
    BA → Architecte → Développeur → Reviewer.
    
    Fichiers vérifiés:
    - 00-context.md      (Initial)
    - 01-requirements.md (BA)
    - 02-architecture.md (Architecte)
    - 03-implementation.md (Développeur)
    - 04-review.md       (Reviewer)
    - HANDOFF.md         (Tous)

.PARAMETER DocsPath
    Chemin vers le dossier docs du projet (ex: C:\Users\...\NADIA\docs)

.PARAMETER Flux
    Nom du flux à valider (ex: nadia-to-dataverse)

.PARAMETER Phase
    Phase à valider. Options: context, ba, archi, dev, review, all
    - context: Vérifie 00-context.md
    - ba: Vérifie jusqu'à 01-requirements.md
    - archi: Vérifie jusqu'à 02-architecture.md
    - dev: Vérifie jusqu'à 03-implementation.md
    - review: Vérifie jusqu'à 04-review.md
    - all: Vérifie tous les fichiers

.EXAMPLE
    .\validate-workflow.ps1 -DocsPath "C:\repos\NADIA\docs" -Flux "nadia-to-dataverse" -Phase "dev"
    
.EXAMPLE
    .\validate-workflow.ps1 -DocsPath "C:\repos\NADIA\docs" -Flux "nadia-to-dataverse" -Phase "all"

.NOTES
    Version: 1.0.0
    Date: 2026-02-05
    Author: GitHub Copilot Agent System
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$DocsPath,

    [Parameter(Mandatory = $true)]
    [string]$Flux,

    [Parameter(Mandatory = $false)]
    [ValidateSet("context", "ba", "archi", "dev", "review", "all")]
    [string]$Phase = "all"
)

# Configuration des fichiers par phase
$WorkflowFiles = @{
    "context" = @("00-context.md")
    "ba"      = @("00-context.md", "01-requirements.md")
    "archi"   = @("00-context.md", "01-requirements.md", "02-architecture.md")
    "dev"     = @("00-context.md", "01-requirements.md", "02-architecture.md", "03-implementation.md")
    "review"  = @("00-context.md", "01-requirements.md", "02-architecture.md", "03-implementation.md", "04-review.md")
    "all"     = @("00-context.md", "01-requirements.md", "02-architecture.md", "03-implementation.md", "04-review.md")
}

$PhaseNames = @{
    "00-context.md"        = "Initial Context"
    "01-requirements.md"   = "Business Analyst (@ba)"
    "02-architecture.md"   = "Architecte (@architecte)"
    "03-implementation.md" = "Développeur (@dev)"
    "04-review.md"         = "Reviewer (@reviewer)"
    "HANDOFF.md"           = "Handoff Tracker"
}

# Couleurs pour l'affichage
function Write-Status {
    param([string]$Message, [string]$Status)
    
    switch ($Status) {
        "OK"      { Write-Host "  ✅ " -NoNewline -ForegroundColor Green; Write-Host $Message }
        "MISSING" { Write-Host "  ❌ " -NoNewline -ForegroundColor Red; Write-Host $Message }
        "WARNING" { Write-Host "  ⚠️ " -NoNewline -ForegroundColor Yellow; Write-Host $Message }
        "INFO"    { Write-Host "  ℹ️ " -NoNewline -ForegroundColor Cyan; Write-Host $Message }
    }
}

# Affichage du header
Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║         WORKFLOW VALIDATION - Agent System                 ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""
Write-Host "📁 Docs Path : $DocsPath" -ForegroundColor Cyan
Write-Host "🔄 Flux      : $Flux" -ForegroundColor Cyan
Write-Host "📋 Phase     : $Phase" -ForegroundColor Cyan
Write-Host ""

# Construction du chemin complet
$WorkflowPath = Join-Path -Path $DocsPath -ChildPath "workflows\$Flux"

# Vérification du dossier
if (-not (Test-Path $WorkflowPath)) {
    Write-Host "❌ ERREUR: Le dossier du workflow n'existe pas!" -ForegroundColor Red
    Write-Host "   Chemin: $WorkflowPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Créez d'abord le dossier et le fichier 00-context.md" -ForegroundColor Yellow
    exit 1
}

Write-Host "📂 Dossier workflow: $WorkflowPath" -ForegroundColor Gray
Write-Host ""

# Validation des fichiers
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Gray
Write-Host " VALIDATION DES FICHIERS (Phase: $Phase)" -ForegroundColor White
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Gray
Write-Host ""

$FilesToCheck = $WorkflowFiles[$Phase]
$MissingFiles = @()
$ExistingFiles = @()

foreach ($File in $FilesToCheck) {
    $FilePath = Join-Path -Path $WorkflowPath -ChildPath $File
    $PhaseName = $PhaseNames[$File]
    
    if (Test-Path $FilePath) {
        $FileInfo = Get-Item $FilePath
        $Size = "{0:N0}" -f $FileInfo.Length
        $LastModified = $FileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
        Write-Status "$File ($PhaseName)" "OK"
        Write-Host "      Taille: $Size bytes | Modifié: $LastModified" -ForegroundColor Gray
        $ExistingFiles += $File
    }
    else {
        Write-Status "$File ($PhaseName)" "MISSING"
        $MissingFiles += $File
    }
}

# Vérification du HANDOFF.md (toujours requis)
Write-Host ""
$HandoffPath = Join-Path -Path $WorkflowPath -ChildPath "HANDOFF.md"
if (Test-Path $HandoffPath) {
    $FileInfo = Get-Item $HandoffPath
    $LastModified = $FileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
    Write-Status "HANDOFF.md (Tracker)" "OK"
    Write-Host "      Modifié: $LastModified" -ForegroundColor Gray
}
else {
    Write-Status "HANDOFF.md (Tracker)" "WARNING"
    Write-Host "      Le fichier HANDOFF.md devrait exister pour tracker les handoffs" -ForegroundColor Yellow
}

# Résumé
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Gray
Write-Host " RÉSUMÉ" -ForegroundColor White
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Gray
Write-Host ""

if ($MissingFiles.Count -eq 0) {
    Write-Host "🎉 SUCCÈS: Tous les fichiers requis pour la phase '$Phase' sont présents!" -ForegroundColor Green
    Write-Host ""
    
    # Suggestion de la prochaine étape
    $NextPhase = switch ($Phase) {
        "context" { "ba"; "@ba pour créer 01-requirements.md" }
        "ba"      { "archi"; "@architecte pour créer 02-architecture.md" }
        "archi"   { "dev"; "@dev pour créer 03-implementation.md" }
        "dev"     { "review"; "@reviewer pour créer 04-review.md" }
        "review"  { "complete"; "Workflow terminé! Prêt pour merge." }
        "all"     { "complete"; "Workflow complet!" }
        default   { "unknown"; "" }
    }
    
    if ($NextPhase -ne "complete" -and $NextPhase -ne "unknown") {
        Write-Host "👉 Prochaine étape: Invoquer $($NextPhase[1])" -ForegroundColor Cyan
    }
    elseif ($NextPhase -eq "complete") {
        Write-Host "✅ $($NextPhase[1])" -ForegroundColor Green
    }
    
    exit 0
}
else {
    Write-Host "❌ ÉCHEC: $($MissingFiles.Count) fichier(s) manquant(s):" -ForegroundColor Red
    Write-Host ""
    
    foreach ($Missing in $MissingFiles) {
        $PhaseName = $PhaseNames[$Missing]
        Write-Host "   • $Missing ($PhaseName)" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "💡 Action requise: Exécuter l'agent correspondant pour créer les fichiers manquants." -ForegroundColor Yellow
    
    exit 1
}
