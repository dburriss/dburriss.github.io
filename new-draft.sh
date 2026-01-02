#!/bin/bash

# new-draft.sh - Interactive script to create new blog draft posts
#
# DESCRIPTION:
#   Creates a new Markdown draft in `_drafts/` with required `topics` and
#   `keywords` front matter. Legacy `category` and `tags` are not required.
#
# USAGE:
#   ./new-draft.sh                # Guided prompts
#   ./new-draft.sh --no-editor    # Skip opening in editor
#
# NOTES:
#   - `topics` must be one or more topic IDs as configured in _config.yml
#   - `keywords` must be one or more words/phrases used for search/related posts

set -e

echo "Creating a new blog draft..."

# Prompt for title
read -p "Enter the post title: " title

# Generate slug from title
slug=$(echo "$title" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9 -]//g' | sed 's/[[:space:]]\+/-/g' | sed 's/^-//;s/-$//')

# Prompt for subtitle
read -p "Enter the subtitle (optional): " subtitle

# Prompt for topics (comma separated IDs)
read -p "Enter topics (IDs, comma separated, e.g., software-design, tooling-automation): " topics_input
# Normalize: split by comma and trim spaces
topics=$(echo "$topics_input" | sed 's/,/\n/g' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//' | paste -sd ',' -)
if [ -z "$topics" ]; then
  echo "ERROR: topics are required (one or more topic IDs)."
  exit 1
fi

# Prompt for keywords (comma separated)
read -p "Enter keywords (comma separated, e.g., Architecture, DDD, F#, Testing): " keywords_input
keywords=$(echo "$keywords_input" | sed 's/,/\n/g' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//' | paste -sd ',' -)
if [ -z "$keywords" ]; then
  echo "ERROR: keywords are required (one or more values)."
  exit 1
fi

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
topics: [$topics]
keywords: [$keywords]
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
