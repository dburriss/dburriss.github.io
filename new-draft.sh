#!/bin/bash

# new-draft.sh - Interactive script to create new blog draft posts

echo "Creating a new blog draft..."

# Prompt for title
read -p "Enter the post title: " title

# Generate slug from title
slug=$(echo "$title" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9 -]//g' | sed 's/[[:space:]]\+/-/g' | sed 's/^-//;s/-$//')

# Prompt for subtitle
read -p "Enter the subtitle (optional): " subtitle

# Prompt for category
read -p "Enter the category (e.g., Programming, Tools): " category

# Prompt for tags (comma separated)
read -p "Enter tags (comma separated, e.g., F#, .NET, Architecture): " tags_input
# Split tags by comma and trim spaces
tags=$(echo "$tags_input" | sed 's/,/\n/g' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//' | paste -sd ',' -)

# Prompt for header image
read -p "Enter header image path (optional, default: img/backgrounds/bulb-bg.jpg): " header_img
if [ -z "$header_img" ]; then
    header_img="img/backgrounds/bulb-bg.jpg"
fi

# Generate filename with date prefix
date_prefix=$(date +%Y-%m-%d)
filename="$date_prefix-$slug.md"
filepath="_drafts/$filename"

# Generate YAML front matter
cat > "$filepath" << EOF
---
layout: post
title: "$title"
subtitle: "$subtitle"
permalink: $slug
author: "Devon Burriss"
category: $category
tags: [$tags]
comments: true
excerpt_separator: <!--more-->
header-img: "$header_img"
published: false
---

Opening paragraph.

<!--more-->

Article here.

EOF

echo "Draft created: $filepath"

# Open in editor if not disabled
if [ "$1" != "--no-editor" ]; then
    read -p "Open file in editor? (y/n): " open_editor
    if [ "$open_editor" = "y" ] || [ "$open_editor" = "Y" ]; then
        # Try VS Code first, then nano, then vi
        if command -v code >/dev/null 2>&1; then
            code "$filepath"
        elif command -v nano >/dev/null 2>&1; then
            nano "$filepath"
        elif command -v vi >/dev/null 2>&1; then
            vi "$filepath"
        else
            echo "No suitable editor found. File created at $filepath"
        fi
    fi
fi
