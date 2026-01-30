param(
    [Parameter(Mandatory=$true)]
    [string]$Profile
)

$root = Split-Path -Parent $PSScriptRoot
$codeCmd = "code"
$activeClientPath = Join-Path $root ".github\agents-framework\active-client.json"
$userDataRoot = Join-Path $root ".vscode\.user-data"
$userDataDir = Join-Path $userDataRoot $Profile

$activeClientContent = @{
    clientKey = $Profile
} | ConvertTo-Json -Depth 3

Set-Content -Path $activeClientPath -Value $activeClientContent -Encoding UTF8

New-Item -Path $userDataDir -ItemType Directory -Force | Out-Null

& $codeCmd $root --user-data-dir $userDataDir --profile $Profile
