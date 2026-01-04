# Notes Documentation

## Overview

Notes are a first-class content type in this blog, designed for shorter, evolving, concept-centric content that complements the chronological blog posts.

## Creating a Note

1. Create a Markdown file in the `_notes/` directory
2. Add YAML front matter with required and optional fields
3. Write your content using standard Markdown

## Front Matter

### Required Fields
- `title`: The title of the note (used for URL generation and linking)

### Optional Fields
- `published`: Boolean (default: true). Set to `false` to exclude from search/navigation but still build the page
- `keywords`: List of keywords for search and metadata
- `topics`: List of topic IDs (must match topics defined in `_config.yml`)
- `status`: String indicating the note's state (e.g., "draft", "active", "archived")

### Example

```yaml
---
title: My Note Title
published: true
keywords: [software, architecture]
topics: [software-development]
status: draft
---
```

## Wiki-Style Links

Notes support wiki-style links using double square brackets: `[[Note Title]]`

### Features
- **Bidirectional linking**: Links are tracked in both directions
- **Case-insensitive matching**: `[[my note]]` matches "My Note"
- **Cross-linking**: Wiki links work between notes and posts
- **Backlinks**: Each note automatically shows which other notes link to it

### Example

```markdown
This note references [[Another Note]] and [[Some Post Title]].
```

### Link Resolution
- Links are resolved at build time by matching against note/post titles
- **Ignored patterns**: Certain patterns like `[[:space:]]` (bash regex character classes) are automatically ignored and not treated as wiki links
- **Warnings** are generated for:
  - Unresolved links (no matching note/post found)
  - Ambiguous links (multiple notes/posts with the same title)
- Standard Markdown links `[text](url)` continue to work as normal

## Backlinks

Each note page automatically includes a "Linked from" section at the bottom, showing all notes and posts that link to it.

## Topics Integration

Notes can be tagged with topics just like posts:
- Notes appear on topic pages alongside posts
- Topic pages separate posts and notes into distinct sections
- Notes are sorted alphabetically on topic pages (unlike date-sorted posts)

## Search

Published notes (`published: true` or not specified) are automatically included in the site search index with the same ranking as posts.

## URLs

Notes use a deterministic URL structure:
- Source file: `_notes/my-note-title.md`
- Generated URL: `/notes/my-note-title/`
- URL is derived from the filename using slugification (lowercase, hyphens, no special characters)

## Status Badge

If a note has a `status` field, it displays as a badge:
- On the note page itself
- On the notes index page

## Feeds

Notes are **excluded** from RSS/Atom feeds by default. Only blog posts appear in feeds.

## Validation

The build process validates notes and provides feedback:

### Link Warnings
- Unresolved wiki links are reported with the source note
- Ambiguous links (multiple targets) are flagged

### Orphaned Notes
- Notes with no inbound links are reported
- This helps identify isolated content that might need more connections
- Published notes only (unpublished notes are not checked)

## Best Practices

1. **Use descriptive titles**: Wiki links match on titles, so make them clear and unique
2. **Link liberally**: Notes are designed for interconnected knowledge
3. **Use topics**: Tag notes with relevant topics for discoverability
4. **Review orphans**: Periodically check the build output for orphaned notes
5. **Keywords matter**: Add relevant keywords for better search results
6. **Status tracking**: Use the status field to indicate work-in-progress or archived notes
