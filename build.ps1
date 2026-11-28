# Builds Intento.sln for x64 with the .NET 10 SDK.
# If no .NET 10 SDK is installed, it is downloaded into .dotnet\ next to this script (no admin needed).
# Usage: .\build.ps1 [-Configuration Debug|Release]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
Set-Location -Path $PSScriptRoot
$localDotnet = Join-Path $PSScriptRoot '.dotnet'

function Test-Sdk10([string]$exe) {
    if (-not $exe) { return $false }
    try { return [bool](& $exe --list-sdks 2>$null | Where-Object { $_ -match '^10\.' }) } catch { return $false }
}

# 1. Prefer a local copy, then any dotnet on PATH that has a 10.x SDK.
$dotnet = Join-Path $localDotnet 'dotnet.exe'
if (-not (Test-Sdk10 $dotnet)) {
    $cmd = Get-Command dotnet -ErrorAction SilentlyContinue
    $dotnet = if ($cmd -and (Test-Sdk10 $cmd.Source)) { $cmd.Source } else { $null }
}

# 2. Otherwise download the .NET 10 SDK with Microsoft's official install script.
if (-not $dotnet) {
    Write-Host '.NET 10 SDK not found. Downloading it into .dotnet\ (this can take a few minutes)...' -ForegroundColor Yellow
    try {
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        $installer = Join-Path $env:TEMP 'dotnet-install.ps1'
        Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile $installer -UseBasicParsing
        & $installer -Channel 10.0 -InstallDir $localDotnet -NoPath
    } catch {
        Write-Host "ERROR: Could not download the .NET 10 SDK: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host 'Install it manually from https://dotnet.microsoft.com/download/dotnet/10.0 and run this again.' -ForegroundColor Red
        exit 1
    }
    $dotnet = Join-Path $localDotnet 'dotnet.exe'
    if (-not (Test-Sdk10 $dotnet)) {
        Write-Host 'ERROR: The .NET 10 SDK install did not complete.' -ForegroundColor Red
        exit 1
    }
}

# Make the chosen SDK the one MSBuild and child processes use.
$env:DOTNET_ROOT = Split-Path $dotnet
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

Write-Host "Using .NET SDK $(& $dotnet --version) from $env:DOTNET_ROOT"
Write-Host "Building Intento.sln ($Configuration|x64). NuGet packages are downloaded on the first build..." -ForegroundColor Cyan

& $dotnet build Intento.sln -c $Configuration -p:Platform=x64
if ($LASTEXITCODE -ne 0) {
    Write-Host "BUILD FAILED (exit code $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host 'BUILD SUCCEEDED.' -ForegroundColor Green
Write-Host "App output: src\Intento.App\bin\x64\$Configuration\"
exit 0
