# new-draft.ps1 - Interactive script to create new blog draft posts

param(
    [switch]$NoEditor
)

Write-Host "Creating a new blog draft..." -ForegroundColor Green

# Prompt for title
$title = Read-Host "Enter the post title"

# Generate slug from title
$slug = $title.ToLower() -replace '[^a-z0-9\s-]', '' -replace '\s+', '-' -replace '-+', '-'

# Prompt for subtitle
$subtitle = Read-Host "Enter the subtitle (optional)"
if ([string]::IsNullOrWhiteSpace($subtitle)) {
    $subtitle = ""
}

# Prompt for category
$category = Read-Host "Enter the category (e.g., Programming, Tools)"

# Prompt for tags (comma separated)
$tagsInput = Read-Host "Enter tags (comma separated, e.g., F#, .NET, Architecture)"
$tags = $tagsInput -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }

# Prompt for header image
$headerImg = Read-Host "Enter header image path (optional, default: img/backgrounds/bulb-bg.jpg)"
if ([string]::IsNullOrWhiteSpace($headerImg)) {
    $headerImg = "img/backgrounds/bulb-bg.jpg"
}

# Generate filename with date prefix
$date = Get-Date -Format "yyyy-MM-dd"
$filename = "$date-$slug.md"
$filePath = "_drafts/$filename"

# Generate YAML front matter
$frontMatter = @"
---
layout: post
title: "$title"
subtitle: "$subtitle"
permalink: $slug
author: "Devon Burriss"
category: $category
tags: [$($tags -join ', ')]
comments: true
excerpt_separator: <!--more-->
header-img: "$headerImg"
published: false
---

Opening paragraph.

<!--more-->

Article here.

"@

# Write the file
$frontMatter | Out-File -FilePath $filePath -Encoding UTF8

Write-Host "Draft created: $filePath" -ForegroundColor Green

# Open in editor if not disabled
if (-not $NoEditor) {
    $openEditor = Read-Host "Open file in editor? (y/n)"
    if ($openEditor -eq 'y' -or $openEditor -eq 'Y') {
        # Try VS Code first, then notepad
        if (Get-Command code -ErrorAction SilentlyContinue) {
            Start-Process code $filePath
        } elseif (Get-Command notepad -ErrorAction SilentlyContinue) {
            Start-Process notepad $filePath
        } else {
            Write-Host "No suitable editor found. File created at $filePath" -ForegroundColor Yellow
        }
    }
}
