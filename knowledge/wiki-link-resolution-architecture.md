# Wiki Link Resolution Architecture Analysis

## Summary

Investigation into the wiki link rendering issue revealed that the problem is not literal `[[...]]` brackets appearing in HTML, but rather a **fundamental architectural disconnect** between link resolution logic and the Markdig rendering pipeline.

## Current Architecture Problems

### The Issue
Wiki links that should resolve to existing notes (e.g., `[[Token]]` → `/notes/token/`) are incorrectly rendered as unresolved `<span class="unresolved-link">Token</span>` instead of proper `<a href="/notes/token/">Token</a>` links.

### Root Cause
The current system has a **timing problem** in its architecture:

1. **HTML Generation** (early): `Parsing.markdownToHtml` converts `[[Token]]` → `<span class="unresolved-link">Token</span>`
2. **Link Resolution** (late): `Renderer.resolveWikiLinks` builds resolution lookup **after** HTML is already generated
3. **No Connection**: The resolution data never reaches the Markdig pipeline that renders the HTML

### Evidence
Testing shows 13 wiki link resolution failures across 5 note files:
- `Token` should resolve to `/notes/token/` (directory exists)
- `Sampling Parameters` should resolve to `/notes/sampling-parameters/` (directory exists)
- `Model Weights` should resolve to `/notes/model-weights/` (directory exists)
- `Context` should resolve to `/notes/context/` (directory exists)
- `LLM` should resolve to `/notes/llm/` (directory exists)

All these have matching source files with correct titles but fail to resolve.

## Current System Flow

```
Program.fs main():
┌─────────────────────────────────────────────────────────────┐
│ 1. loadPosts/loadPages/loadNotes                            │
│    └─> loadContentItem                                     │
│        └─> Parsing.markdownToHtml ──┐                     │
│                                      │ HTML generated HERE │
├─────────────────────────────────────────────────────────────┤
│ 2. buildSiteIndex                   │                      │
├─────────────────────────────────────────────────────────────┤
│ 3. resolveWikiLinks ←────────────────┘                     │
│    (builds resolution data - TOO LATE!)                    │
├─────────────────────────────────────────────────────────────┤
│ 4. renderSite                                              │
│    (uses already-generated HTML with unresolved links)     │
└─────────────────────────────────────────────────────────────┘
```

The resolution logic correctly identifies links and targets but has no way to influence the already-generated HTML.

## Proposed Solutions

### Option 1: Two-Pass Approach (Minimal Architectural Changes)
**Complexity**: Low | **Performance**: 2x HTML generation | **Risk**: Low

**Changes Required**:
1. Modify `loadContentItem` to store raw markdown instead of calling `markdownToHtml`
2. After `resolveWikiLinks`, create a second pass that renders HTML with resolution context
3. Modify `Parsing.markdownToHtml` to accept optional resolution lookup

**Pros**:
- Minimal changes to existing architecture
- Clear separation of concerns maintained
- Easy to understand and debug

**Cons**:
- Double HTML generation (performance impact)
- Content items temporarily store raw markdown

**Implementation Estimate**: ~100 lines of changes across 2-3 files

---

### Option 2: Context-Aware Markdig Pipeline (Medium Architectural Changes)
**Complexity**: Medium | **Performance**: Optimal | **Risk**: Medium

**Changes Required**:
1. Create a `WikiLinkContext` that holds resolution lookup
2. Modify `WikiLinkExtension` to accept and use this context during rendering
3. Restructure content loading to build resolution data first, then generate HTML
4. Thread the context through the rendering pipeline

**Pros**:
- Single HTML generation pass
- More elegant architectural solution
- Extensible for future cross-content resolution needs

**Cons**:
- Requires deeper understanding of Markdig extension architecture
- More complex changes to the pipeline setup
- Risk of breaking other Markdig functionality

**Implementation Estimate**: ~200 lines of changes across 4-5 files

---

### Option 3: Deferred HTML Generation (Major Architectural Changes)
**Complexity**: High | **Performance**: Optimal | **Risk**: High

**Changes Required**:
1. Complete restructure of `ContentItem` to separate metadata from rendered content
2. Change all content loading to defer HTML generation
3. Implement HTML rendering as a final pipeline step after all resolution
4. Modify all rendering functions to handle deferred content

**Pros**:
- Most architecturally "correct" solution
- Enables complex cross-content dependencies
- Future-proof for advanced features

**Cons**:
- Major refactoring required
- High risk of introducing bugs
- Significant testing required across all content types

**Implementation Estimate**: ~500+ lines across 10+ files

## Files Involved

Key files that would need modification:
- `src/SiteRenderer/Renderer.fs` - Content loading and resolution logic
- `src/SiteRenderer/Parsing.fs` - HTML generation pipeline
- `src/SiteRenderer/WikiLinkExtension.fs` - Markdig extension (Options 2-3)
- `src/SiteRenderer/Program.fs` - Main flow coordination
- `src/SiteRenderer/Models.fs` - Data structures (Option 3)

## Testing Strategy

Comprehensive test coverage created:
- `test-wiki-links.fsx` - Unit tests for parsing and rendering
- `test-site-validation.fsx` - Integration tests for generated site
- `test-wiki-link-resolution.fsx` - Specific resolution failure detection
- Updated `render.sh --test` to run all validation

These tests clearly expose the current failures and can validate any architectural fix.

## Status

Issue confirmed and analyzed. Awaiting decision on which architectural approach to pursue based on project priorities around complexity, performance, and risk tolerance.