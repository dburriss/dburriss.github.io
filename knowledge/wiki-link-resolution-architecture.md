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

#### Option 1 Flow Diagram
```
Program.fs main():
┌─────────────────────────────────────────────────────────────┐
│ 1. loadPosts/loadPages/loadNotes                            │
│    └─> loadContentItem (MODIFIED)                          │
│        └─> Store RAW MARKDOWN ──┐                          │
│                                  │ NO HTML generation yet   │
├─────────────────────────────────────────────────────────────┤
│ 2. buildSiteIndex                │                          │
├─────────────────────────────────────────────────────────────┤
│ 3. resolveWikiLinks              │                          │
│    (builds resolution lookup)    │                          │
│                                  │                          │
├─────────────────────────────────────────────────────────────┤
│ 4. SECOND PASS: generateHtml ←───┘                          │
│    ├─> For each ContentItem:                               │
│    │   └─> Parsing.markdownToHtml(content, resolutionMap)  │
│    │       └─> WikiLinkExtension uses resolutionMap ──┐    │
│    │                                                   │    │
│    └─> Update ContentItem.html ←─────────────────────────┘    │
├─────────────────────────────────────────────────────────────┤
│ 5. renderSite                                              │
│    (uses HTML with RESOLVED links)                         │
└─────────────────────────────────────────────────────────────┘

Performance Impact: 2x HTML generation
Timing Fix: Resolution data available BEFORE HTML generation (pass 2)
Key Change: loadContentItem stores markdown, HTML generated after resolution
```

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

#### Option 2 Flow Diagram
```
Program.fs main():
┌─────────────────────────────────────────────────────────────┐
│ 1. PRE-SCAN: extractMetadata(posts/pages/notes)             │
│    └─> Build content index (titles, paths, slugs)          │
│        └─> Create WikiLinkContext ──┐                      │
│                                      │                      │
├─────────────────────────────────────────────────────────────┤
│ 2. loadPosts/loadPages/loadNotes     │                      │
│    └─> loadContentItem              │                      │
│        └─> Parsing.markdownToHtml(content, wikiContext) ───┘
│            └─> WikiLinkExtension.Setup(wikiContext)         │
│                └─> REAL-TIME resolution during parsing      │
│                    ├─> [[Token]] → check wikiContext       │
│                    ├─> Found: emit <a href="/notes/token/"> │
│                    └─> Not found: emit <span unresolved>    │
├─────────────────────────────────────────────────────────────┤
│ 3. buildSiteIndex                                          │
├─────────────────────────────────────────────────────────────┤
│ 4. renderSite                                              │
│    (uses HTML with ALREADY RESOLVED links)                 │
└─────────────────────────────────────────────────────────────┘

Performance Impact: Single HTML generation + fast metadata pre-scan
Timing Fix: WikiLinkContext available DURING HTML generation
Key Change: Extension has resolution context at render time
Decision Points:
  ┌─> [[Token]] found in context → <a href="/notes/token/">Token</a>
  └─> [[Unknown]] not found → <span class="unresolved-link">Unknown</span>
```

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

#### Option 3 Flow Diagram
```
Program.fs main():
┌─────────────────────────────────────────────────────────────┐
│ 1. loadPosts/loadPages/loadNotes (RESTRUCTURED)             │
│    └─> loadContentItem                                     │
│        ├─> Parse frontmatter                               │
│        ├─> Store RAW MARKDOWN                              │
│        └─> Build ContentItem { metadata, rawMarkdown }    │
│                                                            │
├─────────────────────────────────────────────────────────────┤
│ 2. buildSiteIndex                                          │
│    └─> Index all metadata (titles, paths, relationships)   │
├─────────────────────────────────────────────────────────────┤
│ 3. buildResolutionContext                                  │
│    ├─> Analyze all cross-references                        │
│    ├─> Build comprehensive lookup tables                   │
│    └─> Handle complex dependencies                         │
│        ├─> Circular references                            │
│        ├─> Multi-hop relationships                        │
│        └─> Tag-based connections                          │
├─────────────────────────────────────────────────────────────┤
│ 4. FINAL RENDER PHASE:                                     │
│    └─> For each ContentItem:                              │
│        ├─> Parsing.markdownToHtml(rawMarkdown, fullContext)│
│        │   └─> All extensions have complete context       │
│        │       ├─> WikiLinkExtension resolves [[links]]   │
│        │       ├─> TagExtension resolves #tags            │
│        │       └─> BacklinkExtension generates reverse    │
│        └─> Store final HTML in ContentItem                │
├─────────────────────────────────────────────────────────────┤
│ 5. renderSite                                              │
│    (uses HTML with FULLY RESOLVED everything)              │
└─────────────────────────────────────────────────────────────┘

Performance Impact: Single HTML generation + comprehensive context building
Timing Fix: ALL resolution data available BEFORE any HTML generation
Key Change: Complete separation of content loading and rendering phases

Data Flow Evolution:
┌─ Phase 1 ─┐  ┌─ Phase 2 ─┐  ┌─ Phase 3 ─┐  ┌─ Phase 4 ─┐
│ Raw Data  │→ │ Metadata  │→ │ Context   │→ │ Final HTML │
│ Loading   │  │ Indexing  │  │ Building  │  │ Generation │
└───────────┘  └───────────┘  └───────────┘  └────────────┘

Future Capabilities Enabled:
- Complex cross-content dependencies
- Bidirectional link resolution  
- Tag-based content networks
- Advanced content relationship analysis
```

## Files Involved

Key files that would need modification:
- `src/SiteRenderer/Renderer.fs` - Content loading and resolution logic
- `src/SiteRenderer/Parsing.fs` - HTML generation pipeline
- `src/SiteRenderer/WikiLinkExtension.fs` - Markdig extension (Options 2-3)
- `src/SiteRenderer/Program.fs` - Main flow coordination
- `src/SiteRenderer/Models.fs` - Data structures (Option 3)

## Testing Strategy

The testing architecture has been cleaned up and consolidated:

### Current Testing Structure
- **`src/SiteRenderer.Tests/Tests.fs`** - xUnit unit tests for wiki link parsing and rendering components
  - **⚠️ Implementation Coupling**: These tests are strongly coupled to the current implementation
  - **TDD Opportunity**: Consider deleting these tests and following a TDD approach during architectural changes
  - **Testing Philosophy**: The new architecture will create an in-memory pipeline where input/output testing can be more effective than testing internal implementation details
- **`scripts/validate-site.fsx`** - Comprehensive site validation script covering:
  - Wiki link resolution validation
  - Asset inclusion validation (css, js, img, fonts, tkd, CNAME, sitemap.xml)
  - Content count validation (posts vs generated pages)
  - Site structure integrity checks

### Test Integration
The `render.sh` script includes automatic test execution:
- **Unit tests**: `dotnet test src/SiteRenderer.Tests/` - Tests parsing logic in isolation
- **Site validation**: `dotnet fsi scripts/validate-site.fsx` - Tests generated site integrity
- **Options**: Use `--skip-tests` to bypass validation during development

### Test Coverage
This testing setup clearly exposes the current wiki link resolution failures:
- 13 resolution failures across 5 note files
- Links that should resolve to existing notes but render as unresolved spans
- Comprehensive validation ensures any architectural fix maintains site integrity

### Testing Strategy for Architectural Changes

The current unit tests are tightly coupled to implementation details (testing specific classes like `WikiLinkInline`, `WikiLinkExtension`, etc.) which may become obstacles during refactoring. 

**TDD Approach Recommendation**:
1. **Delete existing unit tests** if they impede architectural changes
2. **Start with failing integration tests** from `scripts/validate-site.fsx` (which clearly show current resolution failures)
3. **Build testable pipeline architecture** that enables clean input/output testing
4. **Write new tests** that focus on behavior rather than implementation:
   - Input: Markdown content + metadata
   - Output: Correctly resolved HTML
   - Benefits: Tests the pipeline end-to-end without coupling to internal structure

**Benefits of New Testing Architecture**:
- **In-memory pipeline**: All content loaded before processing enables comprehensive test scenarios  
- **Input/Output clarity**: Test "given this content, expect this HTML" rather than "given this class, expect this method call"
- **Architectural flexibility**: Tests survive refactoring of internal implementation details
- **Better coverage**: Can test complex cross-content scenarios and resolution edge cases

The site validation tests serve as both failure detection and regression prevention for any architectural changes.

## Memory Usage Analysis

### Memory Consumption by Option

Each architecture option has different memory characteristics that should be considered:

#### Option 1: Two-Pass Approach
**Memory Profile**: Similar to current system with temporary spike

```
Memory Usage During Processing:
┌─────────────────────────────────────────────────────────────┐
│ Phase 1: Load raw markdown (temporary storage)             │  ▲
│ Peak: Current content + all raw markdown                   │  │ Memory
├─────────────────────────────────────────────────────────────┤  │
│ Phase 2: Generate HTML (with resolution)                   │  │
│ Peak: Raw markdown + generated HTML + resolution maps      │  │
└─────────────────────────────────────────────────────────────┘  ▼

Estimated Peak Memory:
- Small site (100 notes): ~2-4MB (manageable)
- Medium site (1,000 notes): ~20-40MB (acceptable) 
- Large site (10,000 notes): ~200-400MB (caution)
```

**Memory Characteristics**:
- **Temporary spike**: During HTML generation phase
- **Quick release**: Raw markdown freed after HTML generation
- **Predictable**: Similar pattern to current architecture

#### Option 2: Context-Aware Pipeline  
**Memory Profile**: Minimal overhead, streaming architecture maintained

```
Memory Usage During Processing:
┌─────────────────────────────────────────────────────────────┐
│ Pre-scan: Extract metadata (lightweight)                   │  ▲
│ Peak: Metadata index + WikiLinkContext                     │  │ Memory
├─────────────────────────────────────────────────────────────┤  │
│ Streaming: Process one item at a time                      │  │
│ Peak per item: Single content + context + HTML             │  │
└─────────────────────────────────────────────────────────────┘  ▼

Estimated Peak Memory:
- Any size site: ~1-5MB for context + largest single item
- Metadata index: ~0.5KB per content item
- WikiLinkContext: ~0.1KB per resolvable link
```

**Memory Characteristics**:
- **Flat profile**: No significant spikes
- **Context overhead**: Small, proportional to content count
- **Streaming**: Individual items processed and released
- **Scalable**: Memory usage doesn't grow with total content size

#### Option 3: Deferred HTML Generation
**Memory Profile**: Largest memory footprint, all content in memory

```
Memory Usage During Processing:
┌─────────────────────────────────────────────────────────────┐
│ Phase 1: Load ALL raw markdown                             │  ▲
├─────────────────────────────────────────────────────────────┤  │
│ Phase 2: Build complete indexes                            │  │ Memory
├─────────────────────────────────────────────────────────────┤  │
│ Phase 3: Build resolution context                          │  │
├─────────────────────────────────────────────────────────────┤  │
│ Phase 4: Generate ALL HTML (PEAK MEMORY)                   │  │
│ Peak: All markdown + all HTML + all indexes                │  │
└─────────────────────────────────────────────────────────────┘  ▼

Estimated Peak Memory:
- Small site (100 notes): ~3-6MB
- Medium site (1,000 notes): ~30-60MB  
- Large site (10,000 notes): ~300-600MB
- Very large site (50,000 notes): ~1.5-3GB (problematic)
```

**Memory Characteristics**:
- **Highest peak**: All content simultaneously in memory
- **Sustained usage**: Memory held throughout entire pipeline
- **Resolution complexity**: Additional overhead for cross-references
- **Risk**: May hit memory limits on large sites or constrained environments

### Memory Usage Breakdown

**Typical content sizes**:
- Raw Markdown: 1-5KB per note/post
- Generated HTML: 2-10KB per item (2-3x markdown size)
- Metadata: 0.5KB per item (title, path, frontmatter)
- Resolution maps: 0.1KB per resolvable link
- Indexes: 0.2KB per item

### Memory Recommendations

**Choose Option 1 if**:
- Medium-sized sites (100-2,000 items)
- Acceptable temporary memory spikes
- Want minimal architectural changes

**Choose Option 2 if**:
- Any size site with memory constraints
- Prefer consistent memory usage
- Want optimal performance without risk

**Choose Option 3 if**:
- Small-medium sites (under 5,000 items)
- Have sufficient memory headroom (>1GB available)
- Need advanced cross-content features
- Running on modern development/server hardware

### Mitigation Strategies for Option 3

If choosing Option 3 for a large site, consider these optimizations:

1. **Chunked Processing**: Process content in batches within each phase
2. **Streaming Generation**: Generate and write HTML immediately rather than accumulating
3. **Memory Monitoring**: Add runtime checks to prevent OOM conditions
4. **Lazy Loading**: Load content on-demand during HTML generation
5. **Disk Caching**: Use temporary files for intermediate data

### Memory Testing

Add memory profiling to the test suite:
```fsharp
// test-memory-usage.fsx - Memory profiling during processing
let measureMemoryUsage() =
    let beforeMb = GC.GetTotalMemory(true) / 1024L / 1024L
    // Run processing...
    let afterMb = GC.GetTotalMemory(true) / 1024L / 1024L
    printfn $"Memory used: {afterMb - beforeMb}MB"
```

## Status

Issue confirmed and analyzed. Awaiting decision on which architectural approach to pursue based on project priorities around complexity, performance, memory usage, and risk tolerance.
