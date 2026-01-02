## Context
Adopting the topics taxonomy replaces legacy categories/tags in UI and workflow. `_config.yml` already defines a topic catalog (IDs, names, descriptions, legacy mappings). The F# SiteRenderer currently renders category/tag pages and shows category/tag widgets.

## Goals / Non-Goals
- Goals: use topics for navigation and labeling; add topic pages; require topics/keywords in authoring; preserve legacy links via meta-refresh
- Non-Goals: bulk rewrite of existing Markdown front matter; removing legacy fields from files; server-side 301 management (static site only)

## Decisions
- Use meta `http-equiv="refresh"` for legacy category/tag pages to avoid server config; zero-JS, simple static redirect.
- Topics overview page at `/topics/` lists all topics with descriptions pulled from `_config.yml`.
- Per-topic pages at `/topics/{id}/` list posts with matching `topics` IDs.
- Hover tooltip via HTML `title` attributes for minimal implementation; can enhance with JS later if needed.
- `keywords` continue to mirror legacy `category` and `tags` and remain required for search/related-posts, even as UI switches to topics.
- Provide a cleanup script to remove legacy `category` and `tags` keys; idempotent and safe with valid YAML preservation.

## Risks / Trade-offs
- Meta refresh is less ideal than HTTP 301; acceptable for static hosting with GitHub Pages.
- Posts without topics will not appear on topic pages; ensure scripts enforce required fields.
- Legacy category/tag widgets removed may impact readers used to that navigation; topics overview mitigates.

## Migration Plan
1. Implement UI changes with topics and add required pages.
2. Create redirect pages for legacy taxonomy.
3. Update scripts to enforce topics/keywords when creating and promoting drafts.
4. Validate locally; update CI documentation.

## Open Questions
- Do we need additional mapping beyond `_config.yml` for specific tags to topics? (assume no; use provided `legacy_tags`.)
- Should `keywords` be auto-derived by scripts from legacy metadata for existing drafts? (out of scope for this change)
