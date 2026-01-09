#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Clean generated site content and build artifacts.

.DESCRIPTION
    Wrapper script that calls the F# clean-site.fsx script to remove all
    generated content from _site directory while preserving the .git submodule,
    and runs dotnet clean on the solution.

.PARAMETER DryRun
    Show what would be deleted without actually deleting.

.EXAMPLE
    ./clean.ps1
    Clean all generated content.

.EXAMPLE
    ./clean.ps1 -DryRun
    Preview what would be deleted.
#>

[CmdletBinding()]
param(
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Convert switch to string for F# script
$DryRunParam = if ($DryRun) { "true" } else { "false" }

# Call the F# script with the dry-run parameter
& dotnet fsi (Join-Path $ScriptDir "scripts/clean-site.fsx") $DryRunParam

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}