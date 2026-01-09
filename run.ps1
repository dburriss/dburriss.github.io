#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds and optionally serves the blog site using the F# SiteRenderer.

.DESCRIPTION
    This script builds the F# SiteRenderer project, generates the static site,
    and optionally serves it locally for preview.
    By default, it cleans generated content before building.

.PARAMETER Serve
    If specified, serves the generated site locally after building.

.PARAMETER Watch
    If specified, runs the project in watch mode (dotnet watch run).

.PARAMETER Debug
    If specified, builds in Debug configuration instead of Release.

.PARAMETER Port
    Port number for the local server (default: 8080).

.PARAMETER SkipClean
    If specified, skips cleaning before building.

.EXAMPLE
    ./run.ps1
    Cleans, builds the site in Release mode.

.EXAMPLE
    ./run.ps1 -Serve
    Cleans, builds and serves the site locally.

.EXAMPLE
    ./run.ps1 -Watch
    Runs the site generation in watch mode.

.EXAMPLE
    ./run.ps1 -Serve -Debug -Port 5000
    Cleans, builds in Debug mode and serves on port 5000.

.EXAMPLE
    ./run.ps1 -SkipClean
    Builds without cleaning first.
#>

[CmdletBinding()]
param(
    [switch]$Serve,
    [switch]$Watch,
    [switch]$Debug,
    [int]$Port = 8080,
    [switch]$SkipClean
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Project = Join-Path $ScriptDir "src/SiteRenderer/SiteRenderer.fsproj"
$OutputDir = Join-Path $ScriptDir "_site"

if ($Serve -and $Watch) {
    Write-Error "Cannot use -Serve and -Watch together."
    exit 1
}

if ($Watch) {
    Write-Host "Starting SiteRenderer in watch mode..." -ForegroundColor Cyan
    dotnet watch --project $Project run -- --source $ScriptDir --output $OutputDir
    return
}

# Clean before building (unless skipped)
if (-not $SkipClean) {
    Write-Host "Cleaning previous build..." -ForegroundColor Cyan
    & "$ScriptDir/clean.ps1"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Clean failed"
        exit 1
    }
    Write-Host ""
}

$Configuration = if ($Debug) { "Debug" } else { "Release" }

Write-Host "Building SiteRenderer ($Configuration)..." -ForegroundColor Cyan
dotnet build $Project -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
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
    Write-Error "Search index build failed"
    exit 1
}

Write-Host "Site generated at $OutputDir" -ForegroundColor Green

if ($Serve) {
    Write-Host "Serving site at http://localhost:$Port..." -ForegroundColor Cyan
    dotnet serve -o -d $OutputDir -p $Port
}
