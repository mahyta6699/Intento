# Publishes Intento as a self-contained x64 app into dist\Intento-win-x64 (plus a .zip).
# The output bundles the .NET runtime and the Windows App SDK, so end users install nothing.
# Unused framework code is trimmed and only English resources are kept to reduce size.
# Only this build machine needs the .NET 10 SDK; if it is missing it is downloaded into .dotnet\ (no admin needed).
# Usage: .\build.ps1 [-Configuration Release|Debug]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
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
$outDir = Join-Path $PSScriptRoot 'dist\Intento-win-x64'
if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force }  # drop files from older, larger builds
Write-Host "Publishing Intento ($Configuration|x64, self-contained). NuGet packages are downloaded on the first run..." -ForegroundColor Cyan

& $dotnet publish src\Intento.App\Intento.App.csproj -c $Configuration -p:Platform=x64 -r win-x64 `
    --self-contained true -p:WindowsAppSDKSelfContained=true -p:WindowsPackageType=None `
    -p:PublishTrimmed=true -p:TrimMode=partial -p:SatelliteResourceLanguages=en `
    -p:DebugType=None -p:DebugSymbols=false -o $outDir
if ($LASTEXITCODE -ne 0) {
    Write-Host "BUILD FAILED (exit code $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

$zip = Join-Path $PSScriptRoot 'dist\Intento-win-x64.zip'
Compress-Archive -Path (Join-Path $outDir '*') -DestinationPath $zip -Force

Write-Host 'BUILD SUCCEEDED.' -ForegroundColor Green
Write-Host "App folder: $outDir  (run Intento.App.exe)"
$sizeMB = [math]::Round(((Get-ChildItem $outDir -Recurse -File | Measure-Object Length -Sum).Sum) / 1MB, 1)
$zipMB = [math]::Round((Get-Item $zip).Length / 1MB, 1)
Write-Host "Zip to share: $zip"
Write-Host "Size: $sizeMB MB unpacked, $zipMB MB zipped"
exit 0
