param(
    [string]$Profile
)

$root = Split-Path -Parent $PSScriptRoot

if (-not $Profile -or $Profile.Trim().Length -eq 0) {
    $Profile = $env:VSCODE_PROFILE
}
if (-not $Profile -or $Profile.Trim().Length -eq 0) {
    $Profile = $env:VSCODE_PROFILE_NAME
}
if (-not $Profile -or $Profile.Trim().Length -eq 0) {
    $Profile = Read-Host "Profile name (GroupeSEB | sbm | middleway | seb)"
}

$profileName = $Profile.Trim()

$clientKey = $null
$mcpClient = $null

switch -Regex ($profileName) {
    '^(?i)GroupeSEB$' { $clientKey = 'seb'; $mcpClient = 'seb' }
    '^(?i)seb$' { $clientKey = 'seb'; $mcpClient = 'seb' }
    '^(?i)sbm$' { $clientKey = 'sbm'; $mcpClient = 'sbm' }
    '^(?i)middleway$' { $clientKey = 'middleway'; $mcpClient = 'middleway' }
    default { throw "Unknown profile: $profileName. Expected: GroupeSEB | sbm | middleway | seb." }
}

$activeClientPath = Join-Path $root ".github\agents-framework\active-client.json"
$activeClientContent = @{
    clientKey = $clientKey
} | ConvertTo-Json -Depth 3

Set-Content -Path $activeClientPath -Value $activeClientContent -Encoding UTF8
Write-Host "clientKey set to: $clientKey" -ForegroundColor Green

$source = Join-Path $PSScriptRoot ("mcp.$mcpClient.json")
$target = Join-Path $PSScriptRoot "mcp.json"

if (-not (Test-Path $source)) {
    $available = Get-ChildItem -Path $PSScriptRoot -Filter "mcp.*.json" | ForEach-Object {
        $_.BaseName -replace '^mcp\.', ''
    }
    $list = if ($available) { $available -join ', ' } else { '(none)' }
    throw "Config not found: $source. Available clients: $list"
}

Copy-Item -Path $source -Destination $target -Force
Write-Host "Active MCP config: $target (from $source)" -ForegroundColor Green
