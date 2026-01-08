# WikiLink Extension (Markdig)

Audience: Agents working in this codebase

Purpose
- Provide a custom Markdig inline parser and HTML renderer for wiki-style links [[Title]] and [[Title|DisplayText]].

Implementation summary
- File: src/SiteRenderer/WikiLinkExtension.fs
- Types and modules:
  - WikiLinkInline(title: string, displayText: string option)
    - Title, DisplayText, ResolvedUrl: string option
    - Label computed for backward compatibility (DisplayText or Title)
  - WikiLinkParser: InlineParser that looks for [[...]] and splits on the first '|'
    - Sets processor.Inline to WikiLinkInline and advances the slice
  - WikiLinkHtmlRenderer: HtmlObjectRenderer<WikiLinkInline>
    - ResolvedUrl = Some url → render <a href="url">displayText</a>
    - ResolvedUrl = None → render <span class="unresolved-link">displayText</span>
  - WikiLinkExtension: IMarkdownExtension
    - Setup(MarkdownPipelineBuilder): insert WikiLinkParser at position 0 if not already present
    - Setup(MarkdownPipeline, HtmlRenderer): insert WikiLinkHtmlRenderer at position 0 if not present
  - WikiLinkExtensions module: MarkdownPipelineBuilder.UseWikiLinks() convenience method

Pipeline usage
- Parsing.markdownPipeline configures Markdig with UseWikiLinks() alongside advanced extensions, pipe tables and YAML front matter.

Display behavior
- Uses DisplayText when provided. If absent, falls back to Title.

Testing coverage
- src/SiteRenderer.Tests/Tests.fs validates:
  - Extension registration
  - Parsing correctness for pipe syntax and fallbacks
  - Html rendering for resolved and unresolved links

References (fetched)
- Markdig general usage and extension model: https://github.com/xoofx/markdig
- Giraffe.ViewEngine for HTML composition elsewhere in the renderer: https://giraffe.wiki/view-engine
