param(
    [Parameter(Mandatory=$true)]
    [string]$Client
)

$root = Split-Path -Parent $PSScriptRoot
$activeClientPath = Join-Path $root ".github\agents-framework\active-client.json"

$activeClientContent = @{
    clientKey = $Client
} | ConvertTo-Json -Depth 3

Set-Content -Path $activeClientPath -Value $activeClientContent -Encoding UTF8
Write-Host "clientKey set to: $Client" -ForegroundColor Green
