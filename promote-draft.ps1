# promote-draft.ps1 - Interactive script to promote draft posts to publication

param(
    [string]$Date = "",
    [switch]$Help
)

# Colors for output
$Colors = @{
    Red = "Red"
    Green = "Green"
    Yellow = "Yellow"
    Blue = "Blue"
    White = "White"
}

# Function to write colored output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Colors[$Color]
}

function Write-Error {
    param([string]$Message)
    Write-ColorOutput "ERROR: $Message" "Red"
}

function Write-Success {
    param([string]$Message)
    Write-ColorOutput "SUCCESS: $Message" "Green"
}

function Write-Warning {
    param([string]$Message)
    Write-ColorOutput "WARNING: $Message" "Yellow"
}

function Write-Info {
    param([string]$Message)
    Write-ColorOutput "INFO: $Message" "Blue"
}

# Function to display help
function Show-Help {
    Write-ColorOutput "promote-draft.ps1 - Promote draft posts to publication" "Blue"
    Write-Host ""
    Write-ColorOutput "USAGE:" "Yellow"
    Write-Host "    .\promote-draft.ps1 [OPTIONS]"
    Write-Host ""
    Write-ColorOutput "OPTIONS:" "Yellow"
    Write-Host "    -Date DATE         Custom publish date (YYYY-MM-DD format)"
    Write-Host "    -Help              Show this help message"
    Write-Host ""
    Write-ColorOutput "DESCRIPTION:" "Yellow"
    Write-Host "    This script lists available drafts in the _drafts/ directory and allows"
    Write-Host "    you to select one for promotion. The selected draft will be moved to"
    Write-Host "    _posts/ with a proper date prefix and its front matter will be updated"
    Write-Host "    to set published: true."
    Write-Host ""
    Write-ColorOutput "EXAMPLES:" "Yellow"
    Write-Host "    .\promote-draft.ps1                    # Promote with today's date"
    Write-Host "    .\promote-draft.ps1 -Date 2025-12-25    # Promote with custom date"
    Write-Host ""
}

# Function to validate date format
function Test-DateFormat {
    param([string]$DateString)
    
    if ($DateString -notmatch '^\d{4}-\d{2}-\d{2}$') {
        return $false
    }
    
    try {
        $date = [datetime]::ParseExact($DateString, "yyyy-MM-dd", $null)
        return $true
    }
    catch {
        return $false
    }
}

# Function to check if file has valid front matter
function Test-FrontMatter {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        return $false
    }
    
    $content = Get-Content $FilePath -Raw
    $lines = $content -split "`n"
    
    if ($lines.Count -eq 0 -or $lines[0] -ne "---") {
        return $false
    }
    
    # Check if file has closing front matter
    for ($i = 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -eq "---") {
            return $true
        }
    }
    
    return $false
}

# Function to extract title from filename or front matter
function Get-PostTitle {
    param([string]$FilePath)
    
    $basename = [System.IO.Path]::GetFileNameWithoutExtension($FilePath)
    
    # Try to extract title from front matter first
    if (Test-FrontMatter $FilePath) {
        $content = Get-Content $FilePath -Raw
        if ($content -match '(?s)^---.*?^title:\s*["'']?([^"'']\n\r]+)["'']?.*?^---') {
            $title = $matches[1].Trim()
            if ($title) {
                return $title
            }
        }
    }
    
    # Fallback to filename
    $title = $basename -replace '^\d{4}-\d{2}-\d{2}-', '' -replace '-', ' '
    return (Get-Culture).TextInfo.ToTitleCase($title.ToLower())
}

# Function to update front matter
function Update-FrontMatter {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    
    if (Test-FrontMatter $FilePath) {
        # Update existing front matter
        $lines = $content -split "`n"
        $newLines = @()
        $inFrontMatter = $false
        $frontMatterEnded = $false
        $publishedAdded = $false
        
        foreach ($line in $lines) {
            if ($line -eq "---") {
                if (-not $inFrontMatter) {
                    $inFrontMatter = $true
                    $newLines += $line
                }
                elseif (-not $frontMatterEnded) {
                    # Add published field before closing if not already added
                    if (-not $publishedAdded) {
                        $newLines += "published: true"
                        $publishedAdded = $true
                    }
                    $frontMatterEnded = $true
                    $newLines += $line
                }
                else {
                    $newLines += $line
                }
            }
            elseif ($inFrontMatter -and -not $frontMatterEnded) {
                if ($line -match '^published:') {
                    $newLines += "published: true"
                    $publishedAdded = $true
                }
                else {
                    $newLines += $line
                }
            }
            else {
                $newLines += $line
            }
        }
        
        $newContent = $newLines -join "`n"
    }
    else {
        # Create new front matter
        $title = Get-PostTitle $FilePath
        $newContent = @"
---
layout: post
title: "$title"
published: true
---

$content
"@
    }
    
    Set-Content $FilePath $newContent -Encoding UTF8
}

# Function to list available drafts
function Get-DraftList {
    $draftsDir = "_drafts"
    
    if (-not (Test-Path $draftsDir)) {
        Write-Error "Drafts directory '$draftsDir' not found"
        return @()
    }
    
    $draftFiles = Get-ChildItem -Path $draftsDir -Filter "*.md" -File
    
    if ($draftFiles.Count -eq 0) {
        Write-Warning "No draft files found in '$draftsDir'"
        return @()
    }
    
    Write-Info "Available drafts:"
    for ($i = 0; $i -lt $draftFiles.Count; $i++) {
        $title = Get-PostTitle $draftFiles[$i].FullName
        $hasFm = if (Test-FrontMatter $draftFiles[$i].FullName) { "✓" } else { "✗" }
        Write-Host "  $([string]($i + 1).PadLeft(2)) ) $title $hasFm"
    }
    
    return $draftFiles
}

# Function to select draft
function Select-Draft {
    param([array]$DraftFiles)
    
    while ($true) {
        Write-Host -NoNewline "Select draft to promote (1-$($DraftFiles.Count)): "
        $selection = Read-Host
        
        if ($selection -match '^\d+$') {
            $index = [int]$selection - 1
            if ($index -ge 0 -and $index -lt $DraftFiles.Count) {
                return $DraftFiles[$index]
            }
        }
        
        Write-Error "Invalid selection. Please enter a number between 1 and $($DraftFiles.Count)"
    }
}

# Function to check for filename conflicts
function Test-FileConflict {
    param([string]$TargetPath)
    
    if (Test-Path $TargetPath) {
        Write-Warning "Target file already exists: $TargetPath"
        Write-Host -NoNewline "Overwrite? (y/N): "
        $response = Read-Host
        return $response -match '^[Yy]$'
    }
    
    return $true
}

# Main function
function Main {
    # Show help if requested
    if ($Help) {
        Show-Help
        return
    }
    
    # Validate custom date if provided
    $publishDate = $Date
    if ($publishDate) {
        if (-not (Test-DateFormat $publishDate)) {
            Write-Error "Invalid date format: $publishDate. Use YYYY-MM-DD format."
            return
        }
    }
    else {
        $publishDate = Get-Date -Format "yyyy-MM-dd"
    }
    
    Write-Info "Using publish date: $publishDate"
    
    # List and select draft
    $draftFiles = Get-DraftList
    if ($draftFiles.Count -eq 0) {
        return
    }
    
    $selectedDraft = Select-Draft $draftFiles
    $draftFilename = $selectedDraft.Name
    $draftBasename = $selectedDraft.BaseName
    
    Write-Info "Selected draft: $($selectedDraft.FullName)"
    
    # Validate draft has front matter
    if (-not (Test-FrontMatter $selectedDraft.FullName)) {
        Write-Warning "Draft does not have valid front matter. A new front matter will be created."
    }
    
    # Generate target filename
    $slug = $draftBasename -replace '^\d{4}-\d{2}-\d{2}-', ''
    $targetFilename = "$publishDate-$slug.md"
    $targetPath = "_posts\$targetFilename"
    
    # Check for conflicts
    if (-not (Test-FileConflict $targetPath)) {
        Write-Info "Promotion cancelled."
        return
    }
    
    # Create _posts directory if it doesn't exist
    if (-not (Test-Path "_posts")) {
        New-Item -ItemType Directory -Path "_posts" | Out-Null
    }
    
    # Copy and update the draft
    Write-Info "Promoting draft to: $targetPath"
    Copy-Item $selectedDraft.FullName $targetPath
    Update-FrontMatter $targetPath
    
    # Verify the update
    $content = Get-Content $targetPath -Raw
    if ($content -match '^published:\s*true') {
        # Remove the original draft since promotion was successful
        Remove-Item $selectedDraft.FullName
        Write-Success "Draft successfully promoted!"
        Write-Info "File: $targetPath"
        Write-Info "Original draft removed."
    }
    else {
        Write-Error "Failed to update front matter. Please check the file manually."
    }
}

# Run main function
Main