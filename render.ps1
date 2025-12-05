#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates the site content using the F# SiteRenderer.

.DESCRIPTION
    This script runs the F# SiteRenderer to generate the static site content to the _site directory.

.PARAMETER Debug
    If specified, runs in Debug configuration instead of Release.

.EXAMPLE
    ./render.ps1
    Generates the site in Release mode.

.EXAMPLE
    ./render.ps1 -Debug
    Generates the site in Debug mode.
#>

[CmdletBinding()]
param(
    [switch]$Debug
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Project = Join-Path $ScriptDir "src/SiteRenderer/SiteRenderer.fsproj"
$OutputDir = Join-Path $ScriptDir "_site"

$Configuration = if ($Debug) { "Debug" } else { "Release" }

Write-Host "Generating site..." -ForegroundColor Cyan
dotnet run --project $Project -c $Configuration --no-build -- --source $ScriptDir --output $OutputDir
if ($LASTEXITCODE -ne 0) {
    Write-Error "Site generation failed"
    exit 1
}

Write-Host "Site generated at $OutputDir" -ForegroundColor Green