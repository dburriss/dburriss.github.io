#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates the site content using the F# SiteRenderer.

.DESCRIPTION
    This script runs the F# SiteRenderer to generate the static site content to the _site directory.

.PARAMETER Debug
    If specified, runs in Debug configuration instead of Release.

.PARAMETER Test
    If specified, runs wiki link validation tests after generation.

.PARAMETER TestOnly
    If specified, only runs tests and skips site generation.

.EXAMPLE
    ./render.ps1
    Generates the site in Release mode.

.EXAMPLE
    ./render.ps1 -Debug
    Generates the site in Debug mode.

.EXAMPLE
    ./render.ps1 -Test
    Generates the site and runs validation tests.
#>

[CmdletBinding()]
param(
    [switch]$Debug,
    [switch]$Test,
    [switch]$TestOnly
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Project = Join-Path $ScriptDir "src/SiteRenderer/SiteRenderer.fsproj"
$OutputDir = Join-Path $ScriptDir "_site"

$Configuration = if ($Debug) { "Debug" } else { "Release" }

# Run tests only if requested
if ($TestOnly) {
    Write-Host "Running wiki link tests only..." -ForegroundColor Cyan
    Write-Host "================================" -ForegroundColor Cyan
    
    Write-Host "Running unit tests..." -ForegroundColor Yellow
    dotnet fsi test-wiki-links.fsx
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Unit tests failed"
        exit 1
    }
    
    Write-Host "Running site validation tests..." -ForegroundColor Yellow  
    dotnet fsi test-site-validation.fsx
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Site validation tests failed"
        exit 1
    }
    
    Write-Host "✅ All wiki link tests passed!" -ForegroundColor Green
    exit 0
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

# Run tests if requested
if ($Test) {
    Write-Host ""
    Write-Host "Running wiki link validation tests..." -ForegroundColor Cyan
    Write-Host "=====================================" -ForegroundColor Cyan
    
    dotnet fsi test-site-validation.fsx
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Wiki link validation failed"
        exit 1
    }
    
    Write-Host "✅ Wiki link validation passed!" -ForegroundColor Green
}