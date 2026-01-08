# Links Implementation (Wiki-style links and LinkGraph)

Audience: Agents working in this codebase

Overview
- The site supports wiki-style links in markdown: [[Title]] and [[Title|DisplayText]].
- A custom Markdig extension parses inline wiki links; the renderer resolves targets and builds a link graph with outbound and inbound links.

Key components
- Markdown pipeline: Parsing.markdownPipeline = MarkdownPipelineBuilder().UseAdvancedExtensions().UsePipeTables().UseYamlFrontMatter().UseWikiLinks().Build()
- Extension: src/SiteRenderer/WikiLinkExtension.fs
  - Inline type: WikiLinkInline(title, displayText option) with Title, DisplayText, ResolvedUrl, and Label (back-compat)
  - Parser: detects [[...]], splits first pipe into title and display text
  - Html renderer: renders <a href> when ResolvedUrl is Some url, otherwise <span class="unresolved-link">display</span>
  - Pipeline registration: MarkdownPipelineBuilder.UseWikiLinks()
- Extraction for resolution: src/SiteRenderer/Parsing.fs → extractWikiLinks(markdown) → (title, displayText option) list
- Resolution logic and LinkGraph: src/SiteRenderer/Renderer.fs → resolveWikiLinks(index)
  - Normalization: titles normalized with Parsing.normalizeWikiLabel
  - Lookup map: normalized title → all ContentItem matches across posts and notes
  - Resolution outcomes:
    - Unique match → resolved with target URL
    - Multiple matches → if exactly one note match exists, prefer that and add a warning about ambiguity; else mark unresolved and warn
    - No match → unresolved and warn
  - LinkGraph metadata:
    - OutboundLinks: list of WikiLink { SourceUrl; TargetTitle; TargetDisplayText; ResolvedUrl; IsResolved }
    - InboundLinks: list of source URLs (computed by inverting outbound links)
- Backlinks display: noted in RenderNote via Layouts.noteDocument with backlinks and all content

Behavior details
- Display text vs title: The HTML uses DisplayText when provided (after the pipe), else falls back to Title.
- Unresolved links: Rendered as <span class="unresolved-link">...</span> and recorded in warnings.
- Ambiguity handling: Prefer notes over posts when a single note match exists; otherwise unresolved with warning including count.
- Ignored patterns: extractWikiLinks filters common bracket patterns like [[:space:]] to avoid false positives.

Where to look in code
- src/SiteRenderer/WikiLinkExtension.fs (inline type, parser, renderer, pipeline extension)
- src/SiteRenderer/Parsing.fs (markdown pipeline, extractWikiLinks, normalization)
- src/SiteRenderer/Renderer.fs (resolveWikiLinks, LinkGraph, detectOrphanedNotes)
- src/SiteRenderer/Models.fs (WikiLink, LinkMetadata)

Testing
- src/SiteRenderer.Tests/Tests.fs has xUnit tests covering:
  - Extension registration/build
  - Parsing of [[Title|Display]] and fallbacks
  - Html rendering for resolved/unresolved links
  - Parsing.extractWikiLinks edge cases (multiple pipes, ignore patterns)

Future improvements
- Configurable severity for link issues (warning vs error) per spec.
- Include source file path and line/section context in warning messages.
