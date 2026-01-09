# Site Renderer Architecture

## Overview

This document describes the architecture of the SiteRenderer static site generator, with a focus on the **Option 3: Deferred HTML Generation** architecture that was implemented to solve wiki link resolution timing issues.

## Background: The Wiki Link Resolution Problem

The original architecture had a fundamental timing issue:

1. **HTML Generation (early)**: Content was converted to HTML immediately upon loading
2. **Link Resolution (late)**: Wiki link resolution data was built after HTML generation
3. **Result**: Wiki links couldn't be resolved because the HTML was already generated

This led to wiki links like `[[Token]]` being rendered as `<span class="unresolved-link">Token</span>` instead of proper `<a href="/notes/token/">Token</a>` links, even when the target content existed.

## Option 3: Deferred HTML Generation Architecture

The new architecture separates content loading from HTML generation, allowing wiki links to be resolved during rendering.

### Core Principles

1. **Deferred Rendering**: HTML is not generated until all content is loaded and indexed
2. **Resolution Context**: A comprehensive lookup table is built before any HTML generation
3. **Context-Aware Pipeline**: The Markdig pipeline receives resolution data during rendering
4. **Single Pass**: HTML is generated once with full context, avoiding re-processing

### Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│ Phase 1: Raw Content Loading                                │
│ ├─ loadRawPosts: Load markdown + metadata (no HTML)         │
│ ├─ loadRawPages: Load markdown + metadata (no HTML)         │
│ └─ loadRawNotes: Load markdown + metadata (no HTML)         │
├─────────────────────────────────────────────────────────────┤
│ Phase 2: Build Resolution Context                           │
│ └─ buildResolutionContext: Create title → path mappings     │
│     ├─ TitleLookup: normalized title → RawContentItem list  │
│     └─ PathLookup: normalized title → output path           │
├─────────────────────────────────────────────────────────────┤
│ Phase 3: Render with Context                                │
│ └─ renderContentItem: For each raw item                     │
│     ├─ Pass ResolutionContext to Markdig pipeline          │
│     ├─ WikiLinkExtension resolves [[links]] in real-time   │
│     └─ Generate final HTML with resolved links             │
├─────────────────────────────────────────────────────────────┤
│ Phase 4: Site Generation                                    │
│ └─ Standard site generation with rendered content           │
└─────────────────────────────────────────────────────────────┘
```

## Key Components

### Data Models

#### RawContentItem
Stores content before HTML generation:
```fsharp
type RawContentItem = {
    SourcePath: string      // Original file path
    Markdown: string        // Raw markdown content
    Meta: FrontMatter      // Parsed frontmatter
    Kind: string           // "post", "page", or "note"
}
```

#### ResolutionContext
Provides lookup tables for wiki link resolution:
```fsharp
type ResolutionContext = {
    TitleLookup: Map<string, RawContentItem list>  // normalized title → items
    PathLookup: Map<string, string>                // normalized title → URL path
}
```

#### RenderedContentItem
Final rendered content with resolved links:
```fsharp
type RenderedContentItem = {
    SourcePath: string
    OutputPath: string      // e.g., "developer-quest/index.html"
    HtmlContent: string     // Final HTML with resolved wiki links
    ExcerptHtml: string option
    Meta: FrontMatter
    PageMeta: PageMeta
    Kind: string
}
```

### Key Functions

#### buildResolutionContext
Creates lookup tables from all raw content:
- Normalizes titles (lowercase, trim, standardize spaces)
- Maps titles to their corresponding content items
- Maps titles to their output URLs

#### renderContentItem
Renders a single content item with wiki link resolution:
- Creates context-aware Markdig pipeline
- Passes ResolutionContext to WikiLinkExtension
- Generates HTML with resolved links

### Wiki Link Resolution

The `WikiLinkExtension` has been enhanced to accept a `ResolutionContext`:

1. **Parser**: Identifies `[[wiki link]]` patterns in markdown
2. **Renderer**: Uses ResolutionContext to resolve links:
   - Found in context → `<a href="/path/">Title</a>`
   - Not found → `<span class="unresolved-link">Title</span>`

## URL Generation

Content URLs are determined by:

1. **Permalinks (first priority)**: If frontmatter contains `permalink: some-slug`
2. **Content type fallback**:
   - **Posts**: Uses permalink or filename as slug
   - **Pages**: Uses filename (e.g., `about.html`)
   - **Notes**: Uses slugified title (e.g., `/notes/machine-learning/`)

### Important: Posts use permalinks, not date-based URLs

The system prioritizes permalinks from frontmatter over date-based paths extracted from filenames.

## Memory Characteristics

Option 3 holds all content in memory during processing:

- **Small sites (100 items)**: ~3-6MB peak memory
- **Medium sites (1,000 items)**: ~30-60MB peak memory
- **Large sites (10,000 items)**: ~300-600MB peak memory

For most blogs, this is negligible on modern hardware.

## Benefits of Option 3

1. **Improved Wiki Link Resolution**: Links between content now resolve correctly
2. **Single-Pass Generation**: More efficient than re-processing HTML
3. **Clean Architecture**: Clear separation between loading, resolution, and rendering
4. **Extensible**: Easy to add more cross-content features (backlinks, related content, etc.)

## Program Flow (Program.fs)

The main program flow has been updated to use Option 3:

```fsharp
// Phase 1: Load raw content (no HTML generation)
let rawPosts = Renderer.loadRawPosts postsDir
let rawPages = Renderer.loadRawPages sourceDir
let rawNotes = Renderer.loadRawNotes notesDir

// Phase 2: Build resolution context
let allRawContent = rawPosts @ rawPages @ rawNotes
let resolutionContext = Renderer.buildResolutionContext allRawContent

// Phase 3: Render with context
let posts = rawPosts |> List.map (fun raw -> 
    Renderer.renderContentItem raw resolutionContext 
    |> Renderer.renderedContentItemToContentItem)
// ... similar for pages and notes

// Phase 4: Continue with standard site generation
let index = Renderer.buildSiteIndex posts pages notes
// ...
```

## Testing Strategy

The implementation follows Test-Driven Development (TDD):

1. **Integration Tests**: Test complete wiki link resolution pipeline
2. **Unit Tests**: Test individual components (models, resolution, rendering)
3. **Validation Script**: `scripts/validate-site.fsx` ensures generated site integrity

## Migration Notes

Option 3 is fully backward compatible:
- Existing templates work unchanged
- Output structure remains the same
- Only the internal processing pipeline has changed

## Future Enhancements

The deferred rendering architecture enables:
- Bidirectional links (backlinks)
- Tag-based content networks
- Dependency graphs
- Advanced content relationships
- Link validation and reporting