#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates the site content using the F# SiteRenderer.

.DESCRIPTION
    This script runs the F# SiteRenderer to generate the static site content to the _site directory
    and runs validation tests by default. By default, it cleans generated content before rendering.

.PARAMETER Debug
    If specified, runs in Debug configuration instead of Release.

.PARAMETER SkipTests
    If specified, skips validation tests after generation.

.PARAMETER SkipClean
    If specified, skips cleaning before rendering.

.EXAMPLE
    ./render.ps1
    Cleans, generates the site, and runs tests.

.EXAMPLE
    ./render.ps1 -Debug
    Cleans, generates the site in Debug mode, and runs tests.

.EXAMPLE
    ./render.ps1 -SkipTests
    Cleans and generates the site without running tests.

.EXAMPLE
    ./render.ps1 -SkipClean
    Generates the site without cleaning first.
#>

[CmdletBinding()]
param(
    [switch]$Debug,
    [switch]$SkipTests,
    [switch]$SkipClean
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Project = Join-Path $ScriptDir "src/SiteRenderer/SiteRenderer.fsproj"
$OutputDir = Join-Path $ScriptDir "_site"

$Configuration = if ($Debug) { "Debug" } else { "Release" }

# Clean before rendering (unless skipped)
if (-not $SkipClean) {
    Write-Host "Cleaning previous build..." -ForegroundColor Cyan
    & "$ScriptDir/clean.ps1"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Clean failed"
        exit 1
    }
    Write-Host ""
}

Write-Host "Generating site..." -ForegroundColor Cyan
dotnet run --project $Project -c $Configuration --no-build -- --source $ScriptDir --output $OutputDir
if ($LASTEXITCODE -ne 0) {
    Write-Error "Site generation failed"
    exit 1
}

Write-Host "Building search index..." -ForegroundColor Cyan
bun run scripts/build-search-index.ts
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Search index build failed, but continuing..."
}

Write-Host "Site generated at $OutputDir" -ForegroundColor Green

# Run tests by default (unless skipped)
if (-not $SkipTests) {
    Write-Host ""
    Write-Host "Running validation tests..." -ForegroundColor Cyan
    Write-Host "==========================" -ForegroundColor Cyan
    
    Write-Host "Running unit tests..." -ForegroundColor Yellow
    dotnet test src/SiteRenderer.Tests/
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Unit tests failed"
        exit 1
    }
    
    Write-Host "Running site validation..." -ForegroundColor Yellow  
    dotnet fsi scripts/validate-site.fsx
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Site validation failed"
        exit 1
    }
    
    Write-Host "✅ All tests passed!" -ForegroundColor Green
}