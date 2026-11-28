# Builds Intento.sln for x64 with the .NET SDK.
# Usage: .\build.ps1 [-Configuration Debug|Release]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
Set-Location -Path $PSScriptRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host 'ERROR: The .NET SDK was not found. Install the .NET 10 SDK from https://dotnet.microsoft.com/download' -ForegroundColor Red
    exit 1
}

Write-Host "Using .NET SDK $(dotnet --version)"
Write-Host "Building Intento.sln ($Configuration|x64)..." -ForegroundColor Cyan

dotnet build Intento.sln -c $Configuration -p:Platform=x64
if ($LASTEXITCODE -ne 0) {
    Write-Host "BUILD FAILED (exit code $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host 'BUILD SUCCEEDED.' -ForegroundColor Green
Write-Host "App output: src\Intento.App\bin\x64\$Configuration\"
exit 0
