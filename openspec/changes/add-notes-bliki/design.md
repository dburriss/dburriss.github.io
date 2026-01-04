## Problem statement
The existing blog is optimized for dated, long-form posts. There is no first-class place for shorter, iterative, concept-centric notes that evolve over time or act as a personal knowledge base. External options (GitHub Wiki, separate repos, third-party tools) fragment navigation and search, and do not benefit from the same build-time guarantees as the main site.

## Design goals and constraints
- Maintain a single URL space, theme, and navigation across posts and notes.
- Reuse the existing F# renderer, Markdig configuration, and FlexSearch-based search pipeline.
- Prefer build-time guarantees for link correctness, backlink generation, and search inclusion.
- Keep authoring friction low: simple Markdown files plus minimal front matter.
- Avoid server-side components, external CMSs, and additional npm dependencies (Bun-only for JS tooling).
- Preserve existing post URLs and feeds; notes must be additive, not disruptive.

## Content model changes (types, metadata)
- Introduce a `Note` content type distinct from `Post`.
- Store notes under a dedicated source directory (e.g. `notes/` at repo root).
- Define minimal YAML front matter for notes:
  - `title` (required)
  - `published` (boolean; controls search/navigation/listing visibility but not whether the note is built)
  - `keywords` (optional but recommended; mirrors topics and key concepts)
  - `topics` (optional list of topic IDs; reuses existing `topics` catalog)
  - `status` (optional; e.g. `draft`, `active`, `archived`)
- Represent notes in the renderer’s domain model with:
  - Stable, deterministic `slug` and `url` fields.
  - Parsed `topics` list using the existing topic catalog.
  - A `kind`/discriminator to distinguish notes from posts inside shared code paths.

## Markdown/linking extensions
- Extend the Markdown pipeline to support wiki-style links using `[[Page Name]]` syntax.
- Interpret wiki links as:
  - Prefer note targets first (by matching note titles), then posts if unambiguous.
  - Support explicit overrides later (e.g. syntax for "post only" or "note only"), but not required for v1.
- Resolve wiki links at build time to concrete URLs and record link relationships for validation/backlinks.
- Preserve standard Markdown links (`[text](url)`) and treat them as-is unless they also match internal note/post URLs.

## Rendering and layout implications
- Notes share the same global layout shell (header, nav, footer, theme) as posts.
- Individual note pages:
  - Render note title and topics/keywords metadata similarly to posts, but without a published date by default.
  - Include a "Linked from"/backlinks section derived from build-time link analysis.
  - Render a per-note Table of Contents based on headings, implemented using a reusable component that can later be enabled for posts without structural changes.
  - Optionally show a status badge (e.g. `Draft`, `Thinking`, `Archived`).
- Notes index/landing:
  - Provide at least one entry point from navigation (e.g. `/notes/`) listing notes by concept (e.g. grouped or sorted by title/topic) rather than by date, and extend topic pages (`/topics/{id}/`) to include a topic-centric list of notes alongside posts.
- The design should not preclude future visualization layers (e.g. graph views) built on top of the same link metadata.

## Backlink and link-resolution algorithm (high level)
- During the rendering phase:
  - Discover all notes and posts and assign deterministic IDs/URLs.
  - Parse Markdown for each document with the wiki-link extension enabled.
  - For each wiki link:
    - Normalize the label (e.g. trim/normalize spaces; case-insensitive match).
    - Look up matching notes and posts by title/slug.
    - If exactly one target exists, resolve to that URL and record an outbound link.
    - If multiple targets exist, mark as ambiguous and record a validation warning.
    - If no targets exist, mark as unresolved and record a validation warning or error (configurable).
  - After processing all documents, invert outbound links to compute backlinks per note.
- Persist backlink/link metadata in memory for rendering and (optionally) in an artifact for future tooling.

## Search indexing changes
- Extend `docs.json` generation so notes with `published: true` are emitted alongside posts.
- For each note document, include at minimum:
  - `id`, `url`, `title`, `content/body`, and optionally `headings`, `topics`, and `keywords`.
- Ensure notes are indexed with the same FlexSearch configuration, preserving deterministic index generation.
- Confirm that notes are searchable by title, headings, and body content, and that they appear in results intermixed with posts (single index, shared ranking).

## URL and routing rules
- Notes live under a dedicated URL prefix: `/notes/`.
- Source files reside in a dedicated folder (e.g. `notes/`), but URLs are derived from slugs, not file extensions.
- Slug generation for notes MUST be deterministic and stable over time:
  - Derived from the file name or title using the same slugification rules as posts.
  - Independent of note status.
- Pretty URLs are required:
  - A note at slug `foo-bar` renders to `/notes/foo-bar/` (`notes/foo-bar/index.html` in `_site/`).
- Existing post URLs remain unchanged; there is no overlap between `/notes/...` and `/YYYY/...` post URLs.

## Build-time validation and failure modes
- Unresolved wiki links:
  - Default behavior: fail the build when unresolved internal wiki links are detected, with a clear summary.
  - Allow a configuration option or override to downgrade certain cases to warnings if needed.
- Ambiguous wiki links (multiple targets with the same label):
  - Treat as warnings with explicit diagnostics; links may be left unlinked or resolved via a deterministic tie-breaker (TBD).
- Orphaned notes:
  - Detect notes that have neither inbound links nor explicit navigation entries.
  - Report orphans in a build-time summary (non-fatal by default).
- Front matter validation:
  - Treat missing required fields (e.g. `title`) as build failures.
  - Treat optional fields (`topics`, `keywords`, `status`) as warning-only when missing, but ensure they do not break rendering.

## Migration and backward compatibility
- Existing posts and pages remain unchanged; there is no requirement to convert posts into notes.
- Search and navigation remain functional even when no notes exist (empty `notes/` directory).
- The initial implementation can be introduced behind a feature flag or configuration switch that enables note discovery.
- No changes to existing RSS/Atom feeds in v1; notes are excluded from feeds by default.

## Open questions and deferred features
- How should the system behave when a wiki label matches both a note and a post (priority rules vs explicit syntax)?
- Future work (explicitly deferred):
  - Graph visualization of notes and links.
  - Per-note change history or last-updated metadata surfaced in the UI.
