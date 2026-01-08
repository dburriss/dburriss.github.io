# Current Issue: Wiki Link Validation Severity and Orphaned Notes

Audience: Agents working in this codebase

Summary
- The renderer resolves wiki-style links and reports issues, but severity is not configurable yet.
- Orphaned notes are detected and listed, but there is no configurable enforcement.

What exists today (code)
- Link extraction: Parsing.extractWikiLinks returns (title, displayText option) from [[Title]] and [[Title|Display]].
- Link resolution: Renderer.resolveWikiLinks builds a LinkGraph and accumulates warnings:
  - Unresolved wiki links → "Unresolved wiki link [[Title]] in <page-url>"
  - Ambiguous wiki links → resolves to notes over posts if a single note match exists; otherwise warns "Ambiguous wiki link [[Title]] in <page-url> (matches N items)"
- Warnings output: Program.fs prints "Link resolution warnings:" followed by each warning.
- Orphaned notes: Renderer.detectOrphanedNotes lists notes with no inbound links (and not explicitly excluded by Published=false).

What the spec requires
- Site Publishing spec (openspec/specs/site-publishing/spec.md):
  - Requirement: Build-time validation for notes links
    - Scenario: Unresolved wiki links reported with context
    - Scenario: Configurable severity for wiki link issues (warnings vs errors)
  - Requirement: Orphaned notes detection in publishing output
    - Scenario: Orphaned notes summary after build

Gap analysis
- Severity configuration for wiki link issues is not implemented.
  - Code always treats link issues as warnings; build does not fail based on unresolved/ambiguous links.
- Orphaned notes detection is present (summary output), but there’s no configuration to treat this as an error or to gate publishing.

Suggested improvement approach
- Add configuration knob(s) to _config.yml or CLI to control severity:
  - links.unresolvedSeverity: warning|error
  - links.ambiguousSeverity: warning|error
  - notes.orphanSeverity: warning|error
- Propagate config into Program.run and Renderer.resolveWikiLinks:
  - Collect issues with type and severity.
  - If any issue has severity=error, exit with non-zero code to fail CI.
- Extend warnings to include file path and context (if available):
  - SourcePath exists on ContentItem; include it in messages.
- Update OpenSpec change to reflect implementation and validate via openspec validate --strict.

Useful references
- Site Publishing spec: openspec/specs/site-publishing/spec.md (contains scenarios for link validation and orphan detection).
- Link code paths:
  - src/SiteRenderer/Parsing.fs → extractWikiLinks, markdown pipeline
  - src/SiteRenderer/Renderer.fs → resolveWikiLinks, detectOrphanedNotes, LinkGraph
  - src/SiteRenderer/Program.fs → prints warnings and orphaned notes

Testing considerations
- Add xUnit tests in src/SiteRenderer.Tests for severity behavior:
  - When config sets unresolvedSeverity=error, unresolved links cause non-zero exit.
  - When ambiguousSeverity=error, ambiguous links cause non-zero exit.
  - Verify warning-only mode prints but does not fail.
