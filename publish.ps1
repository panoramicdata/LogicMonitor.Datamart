#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds and publishes the LogicMonitor.Datamart NuGet package.

.DESCRIPTION
    This script cleans, restores, builds, packs, and publishes the NuGet package to NuGet.org.
    It uses Nerdbank.GitVersioning for versioning.

.PARAMETER ApiKey
    The NuGet API key to use for publishing. If not provided, the script will look for a nuget-key.txt file in the solution root.

.PARAMETER SkipTests
    Skip running tests before publishing.

.PARAMETER Configuration
    The build configuration to use (Debug or Release). Default is Release.

.PARAMETER Source
    The NuGet source to publish to. Default is https://api.nuget.org/v3/index.json

.EXAMPLE
    .\publish.ps1

.EXAMPLE
    .\publish.ps1 -ApiKey "your-api-key-here"

.EXAMPLE
    .\publish.ps1 -SkipTests -Configuration Release
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$ApiKey,

    [Parameter(Mandatory = $false)]
    [switch]$SkipTests,

    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [string]$Source = "https://api.nuget.org/v3/index.json"
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Get the script directory (solution root)
$SolutionRoot = $PSScriptRoot
$ProjectPath = Join-Path $SolutionRoot "LogicMonitor.Datamart" "LogicMonitor.Datamart.csproj"
$OutputPath = Join-Path $SolutionRoot "artifacts"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "LogicMonitor.Datamart NuGet Publisher" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if project file exists
if (-not (Test-Path $ProjectPath)) {
    Write-Error "Project file not found at: $ProjectPath"
    exit 1
}

# Get API key from file if not provided
if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    $ApiKeyFile = Join-Path $SolutionRoot "nuget-key.txt"
    if (Test-Path $ApiKeyFile) {
        Write-Host "Reading API key from nuget-key.txt..." -ForegroundColor Yellow
        $ApiKey = (Get-Content $ApiKeyFile -Raw).Trim()

        if ([string]::IsNullOrWhiteSpace($ApiKey)) {
     Write-Error "API key file exists but is empty: $ApiKeyFile"
     exit 1
      }
    }
    else {
      Write-Error "No API key provided and nuget-key.txt file not found in solution root."
        Write-Host "Please either:" -ForegroundColor Yellow
        Write-Host "  1. Create a nuget-key.txt file in the solution root with your NuGet API key" -ForegroundColor Yellow
      Write-Host "  2. Pass the API key using the -ApiKey parameter" -ForegroundColor Yellow
        exit 1
    }
}

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Green
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Green
dotnet restore $ProjectPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to restore dependencies"
    exit $LASTEXITCODE
}

# Run tests unless skipped
if (-not $SkipTests) {
    Write-Host "Running tests..." -ForegroundColor Green
    $TestProjectPath = Join-Path $SolutionRoot "LogicMonitor.Datamart.Test" "LogicMonitor.Datamart.Test.csproj"

    if (Test-Path $TestProjectPath) {
 dotnet test $TestProjectPath --configuration $Configuration --no-restore
        if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed. Aborting publish."
            exit $LASTEXITCODE
        }
    }
    else {
        Write-Warning "Test project not found, skipping tests."
    }
}
else {
    Write-Host "Skipping tests..." -ForegroundColor Yellow
}

# Build the project
Write-Host "Building project in $Configuration configuration..." -ForegroundColor Green
dotnet build $ProjectPath --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

# Pack the NuGet package
Write-Host "Packing NuGet package..." -ForegroundColor Green
dotnet pack $ProjectPath --configuration $Configuration --no-build --output $OutputPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Pack failed"
    exit $LASTEXITCODE
}

# Find the generated .nupkg file
$NuGetPackages = Get-ChildItem -Path $OutputPath -Filter "*.nupkg" -Exclude "*.symbols.nupkg"
if ($NuGetPackages.Count -eq 0) {
 Write-Error "No NuGet package found in $OutputPath"
    exit 1
}

$PackageFile = $NuGetPackages[0]
Write-Host ""
Write-Host "Package created: $($PackageFile.Name)" -ForegroundColor Cyan
Write-Host "Package path: $($PackageFile.FullName)" -ForegroundColor Gray
Write-Host ""

# Publish to NuGet
Write-Host "Publishing to NuGet.org..." -ForegroundColor Green
dotnet nuget push $PackageFile.FullName --api-key $ApiKey --source $Source --skip-duplicate
if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Successfully published $($PackageFile.Name)!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Package URL: https://www.nuget.org/packages/LogicMonitor.Datamart" -ForegroundColor Gray
