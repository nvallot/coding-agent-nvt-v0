param(
    [Parameter(Mandatory=$true)]
    [string]$Client
)

$root = Split-Path -Parent $PSScriptRoot
$source = Join-Path $PSScriptRoot ("mcp.$Client.json")
$target = Join-Path $PSScriptRoot "mcp.json"

if (-not (Test-Path $source)) {
    $available = Get-ChildItem -Path $PSScriptRoot -Filter "mcp.*.json" | ForEach-Object {
        $_.BaseName -replace '^mcp\.', ''
    }
    $list = if ($available) { $available -join ', ' } else { '(none)' }
    throw "Config not found: $source. Available clients: $list"
}

Copy-Item -Path $source -Destination $target -Force
Write-Host "Active MCP config: $target" -ForegroundColor Green
