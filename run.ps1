#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds and optionally serves the blog site using the F# SiteRenderer.

.DESCRIPTION
    This script builds the F# SiteRenderer project, generates the static site,
    and optionally serves it locally for preview.

.PARAMETER Serve
    If specified, serves the generated site locally after building.

.PARAMETER Watch
    If specified, runs the project in watch mode (dotnet watch run).

.PARAMETER Debug
    If specified, builds in Debug configuration instead of Release.

.PARAMETER Port
    Port number for the local server (default: 8080).

.EXAMPLE
    ./build.ps1
    Builds the site in Release mode.

.EXAMPLE
    ./build.ps1 -Serve
    Builds and serves the site locally.

.EXAMPLE
    ./build.ps1 -Watch
    Runs the site generation in watch mode.

.EXAMPLE
    ./build.ps1 -Serve -Debug -Port 5000
    Builds in Debug mode and serves on port 5000.
#>

[CmdletBinding()]
param(
    [switch]$Serve,
    [switch]$Watch,
    [switch]$Debug,
    [int]$Port = 8080
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Project = Join-Path $ScriptDir "src/SiteRenderer/SiteRenderer.fsproj"
$OutputDir = Join-Path $ScriptDir "_site"

if ($Watch) {
    Write-Host "Starting SiteRenderer in watch mode..." -ForegroundColor Cyan
    dotnet watch --project $Project run -- --source $ScriptDir --output $OutputDir
    return
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

Write-Host "Site generated at $OutputDir" -ForegroundColor Green

if ($Serve) {
    Write-Host "Serving site at http://localhost:$Port..." -ForegroundColor Cyan
    dotnet serve -o -d $OutputDir -p $Port
}
