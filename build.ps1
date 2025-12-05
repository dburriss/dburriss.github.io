#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the F# SiteRenderer solution.

.DESCRIPTION
    This script builds the F# SiteRenderer project in the specified configuration.

.PARAMETER Debug
    If specified, builds in Debug configuration instead of Release.

.EXAMPLE
    ./build.ps1
    Builds the SiteRenderer in Release mode.

.EXAMPLE
    ./build.ps1 -Debug
    Builds the SiteRenderer in Debug mode.
#>

[CmdletBinding()]
param(
    [switch]$Debug
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Project = Join-Path $ScriptDir "src/SiteRenderer/SiteRenderer.fsproj"

$Configuration = if ($Debug) { "Debug" } else { "Release" }

Write-Host "Building SiteRenderer ($Configuration)..." -ForegroundColor Cyan
dotnet build $Project -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Green