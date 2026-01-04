# Change: Add bliki-style notes alongside posts

## Why
A single chronological blog stream makes it hard to maintain evolving, concept-centric knowledge. A first-class "Notes" space enables shorter, iterative, wiki-like entries that stay tightly integrated with the existing theme, navigation, and search.

## What Changes
- Add a first-class "Notes" content type rendered to `/notes/...` URLs.
- Support minimal YAML front matter for notes (title, keywords, topics, optional status).
- Extend Markdown to support wiki-style `[[Note Title]]` links with deterministic slug resolution.
- Generate backlinks and link-resolution metadata at build time, including unresolved-link warnings and orphan detection.
- Integrate notes into the existing docs/search pipeline so notes are searchable but excluded from feeds.
- Update navigation and layouts so notes share the same visual theme while remaining concept-oriented (not date-ordered).

## Impact
- Affected specs (via deltas under this change):
  - `site-publishing` (renderer pipeline, docs.json shaping, feeds)
  - `site-search` (search document coverage and fields for notes)
  - New capability: `notes` (content model, linking, URLs, validation)
- Affected implementation areas (apply stage, not part of this proposal):
  - `src/SiteRenderer/Models.fs`, `Parsing.fs`, `Renderer.fs`, `Search.fs`, `Layouts.fs`
  - Markdown/Markdig configuration for link extensions
  - Search index build script (`scripts/build-search-index.ts`) and search UI as needed for notes metadata display
  - Navigation/layout components and any topics-related UI that should surface notes

## Open Questions
- Should notes ever appear in any existing feed (e.g., an opt-in "recent notes" feed) or only via search and navigation?
- How strongly should the system enforce front matter completeness for notes (warnings vs. hard failures) in CI?

## Clarifications
- Notes SHALL use a `published` flag consistent with posts; notes with `published: false` still build and can be reached via direct URLs but are excluded from search, navigation, and topic listings.
