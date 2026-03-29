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
$ProjectPath = Join-Path -Path $SolutionRoot -ChildPath "LogicMonitor.Datamart" -AdditionalChildPath "LogicMonitor.Datamart.csproj"
$OutputPath = Join-Path -Path $SolutionRoot -ChildPath "artifacts"

Write-Output "========================================"
Write-Output "LogicMonitor.Datamart NuGet Publisher"
Write-Output "========================================"
Write-Output ""

# Check if project file exists
if (-not (Test-Path $ProjectPath)) {
    Write-Error "Project file not found at: $ProjectPath"
    exit 1
}

# Get API key from file if not provided
if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    $ApiKeyFile = Join-Path $SolutionRoot "nuget-key.txt"
    if (Test-Path $ApiKeyFile) {
        Write-Output "Reading API key from nuget-key.txt..."
   $ApiKey = (Get-Content $ApiKeyFile -Raw).Trim()
 
        if ([string]::IsNullOrWhiteSpace($ApiKey)) {
         Write-Error "API key file exists but is empty: $ApiKeyFile"
      exit 1
}
    }
    else {
        Write-Error "No API key provided and nuget-key.txt file not found in solution root."
        Write-Output "Please either:"
        Write-Output "  1. Create a nuget-key.txt file in the solution root with your NuGet API key"
   Write-Output "  2. Pass the API key using the -ApiKey parameter"
        exit 1
    }
}

# Clean previous builds
Write-Output "Cleaning previous builds..."
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}

dotnet clean $ProjectPath --configuration $Configuration --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Clean command returned non-zero exit code, but continuing..."
}

# Restore dependencies
Write-Output "Restoring dependencies..."
dotnet restore $ProjectPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to restore dependencies"
    exit $LASTEXITCODE
}

# Run tests unless skipped
if (-not $SkipTests) {
    Write-Output "Running tests..."
    $TestProjectPath = Join-Path -Path $SolutionRoot -ChildPath "LogicMonitor.Datamart.Test" -AdditionalChildPath "LogicMonitor.Datamart.Test.csproj"
    
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
    Write-Output "Skipping tests..."
}

# Build the project
Write-Output "Building project in $Configuration configuration..."
dotnet build $ProjectPath --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
  exit $LASTEXITCODE
}

# Pack the NuGet package
Write-Output "Packing NuGet package..."
# Use /p:PackageOutputPath to ensure package goes to our output folder
dotnet pack $ProjectPath --configuration $Configuration --no-build --output $OutputPath /p:PackageOutputPath=$OutputPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Pack failed"
    exit $LASTEXITCODE
}

# Find the generated .nupkg file
$NuGetPackages = Get-ChildItem -Path $OutputPath -Filter "*.nupkg" -Exclude "*.symbols.nupkg" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
if ($null -eq $NuGetPackages -or $NuGetPackages.Count -eq 0) {
    # Try looking in the bin folder as fallback
    $BinOutputPath = Join-Path -Path $SolutionRoot -ChildPath "LogicMonitor.Datamart" -AdditionalChildPath "bin", $Configuration
    Write-Output "No package found in $OutputPath, checking $BinOutputPath..."
    $NuGetPackages = Get-ChildItem -Path $BinOutputPath -Filter "*.nupkg" -Exclude "*.symbols.nupkg" -Recurse -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
    
 if ($null -eq $NuGetPackages -or $NuGetPackages.Count -eq 0) {
        Write-Error "No NuGet package found in $OutputPath or $BinOutputPath"
        exit 1
    }
}

$PackageFile = $NuGetPackages[0]
Write-Output ""
Write-Output "Package created: $($PackageFile.Name)"
Write-Output "Package path: $($PackageFile.FullName)"
Write-Output ""

# Publish to NuGet
Write-Output "Publishing to NuGet.org..."
dotnet nuget push $PackageFile.FullName --api-key $ApiKey --source $Source --skip-duplicate
if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit $LASTEXITCODE
}

Write-Output ""
Write-Output "========================================"
Write-Output "Successfully published $($PackageFile.Name)!"
Write-Output "========================================"
Write-Output ""
Write-Output "Package URL: https://www.nuget.org/packages/LogicMonitor.Datamart"
