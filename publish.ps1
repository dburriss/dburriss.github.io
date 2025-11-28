#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publishes the generated site to the master branch for GitHub Pages hosting.

.DESCRIPTION
    This script commits the contents of _site (a git submodule) and force-pushes
    to the master branch. It can be run locally for testing or called by CI.

.PARAMETER DryRun
    If specified, shows what would be committed and pushed without making changes.

.PARAMETER Message
    Custom commit message (default: "Deploy site [skip ci]").

.EXAMPLE
    ./publish.ps1
    Publishes the site to master branch.

.EXAMPLE
    ./publish.ps1 -DryRun
    Shows what would be published without making changes.
#>

[CmdletBinding()]
param(
    [switch]$DryRun,
    [string]$Message = "Deploy site [skip ci]"
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SiteDir = Join-Path $ScriptDir "_site"

# Verify _site directory exists
if (-not (Test-Path $SiteDir)) {
    Write-Error "_site directory not found. Run ./run.ps1 first to generate the site."
    exit 1
}

# Check if _site has a .git (submodule pointer)
# The renderer wipes _site, so we may need to reinitialize the submodule
$GitFile = Join-Path $SiteDir ".git"
if (-not (Test-Path $GitFile)) {
    Write-Host "Reinitializing _site submodule..." -ForegroundColor Cyan
    Push-Location $ScriptDir
    git submodule update --init _site
    Pop-Location
}

Push-Location $SiteDir
try {
    # Verify _site is a git repository (submodule)
    $gitCheck = git rev-parse --git-dir 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "_site is not a git repository. Run 'git submodule update --init' first."
        exit 1
    }

    # Configure git user if running in CI
    if ($env:GITHUB_ACTIONS) {
        $actor = if ($env:GITHUB_ACTOR) { $env:GITHUB_ACTOR } else { "github-actions" }
        git config user.name $actor
        git config user.email "$actor@users.noreply.github.com"
        
        # Configure authentication for pushing to external repo
        # Requires DEPLOY_TOKEN secret with repo scope for dburriss/dburriss.github.io
        if ($env:DEPLOY_TOKEN) {
            git remote set-url origin "https://x-access-token:$($env:DEPLOY_TOKEN)@github.com/dburriss/dburriss.github.io.git"
        }
    }

    # Ensure we're on master branch for the commit
    # Submodule may be in detached HEAD state, so we need to checkout or create master
    $null = git checkout master 2>&1
    if ($LASTEXITCODE -ne 0) {
        $null = git checkout -B master origin/master 2>&1
        if ($LASTEXITCODE -ne 0) {
            $null = git checkout -b master 2>&1
        }
    }

    if ($DryRun) {
        Write-Host "=== DRY RUN ===" -ForegroundColor Yellow
        Write-Host "Would commit the following changes:" -ForegroundColor Cyan
        git status --short
        Write-Host ""
        Write-Host "Commit message: $Message" -ForegroundColor Cyan
        Write-Host "Would force-push to origin/master" -ForegroundColor Cyan
    }
    else {
        Write-Host "Staging changes..." -ForegroundColor Cyan
        git add .

        # Check if there are changes to commit
        $status = git status --porcelain
        if ([string]::IsNullOrWhiteSpace($status)) {
            Write-Host "No changes to publish." -ForegroundColor Yellow
            exit 0
        }

        Write-Host "Committing changes..." -ForegroundColor Cyan
        git commit -m $Message

        Write-Host "Pushing to master branch..." -ForegroundColor Cyan
        git push origin master -f

        Write-Host "Site published successfully!" -ForegroundColor Green
    }
}
finally {
    Pop-Location
}
