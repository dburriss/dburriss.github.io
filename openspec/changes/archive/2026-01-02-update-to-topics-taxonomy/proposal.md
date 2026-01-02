# Change: Adopt Topics Taxonomy and Replace Categories/Tags

## Why
The current categories/tags taxonomy is inconsistent and decays over time. Topics provide a stable, human-centered classification aligned with the knowledge guidance, improving browsing, consistency, and UI clarity.

## What Changes
- Visually replace categories and tags with topics across layouts and components. **BREAKING**
- Add topic landing pages: one per topic, plus a topics overview page with descriptions.
- Show topic descriptions on hover for topic labels in UI.
- Generate legacy category/tag pages as meta `http-equiv="refresh"` redirects to mapped topic pages; fallback to topics overview when unmapped.
- Require `topics` and `keywords` in front matter for new drafts and promotions; do not require legacy `category`/`tags` fields.
- Provide a cleanup script to remove legacy `category` and `tags` from front matter (idempotent).

## Impact
- Affected specs: `specs/site-publishing/spec.md`, `specs/topics/spec.md`, `specs/draft-promotion/spec.md`
- Affected code: `src/SiteRenderer/Layouts.fs`, `src/SiteRenderer/Renderer.fs`, `src/SiteRenderer/Parsing.fs`, `run.ps1|sh`, `new-draft.ps1|sh`, `promote-draft.ps1|sh`
- Configuration: `_config.yml` topics catalog and legacy mappings
