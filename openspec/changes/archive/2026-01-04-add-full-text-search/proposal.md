# Change: Add full-text search to the static site

## Why
Readers currently need external tools (browser search, site: queries) to find older posts. A small, client-side full-text search improves discoverability without adding a backend, preserving GitHub Pages hosting.

## What Changes
- Add client-side full-text search powered by FlexSearch.
- Generate the FlexSearch index at build time (Bun), not in the browser.
- Extend the render/build pipeline to emit deterministic search documents and a prebuilt index as static artifacts.
- Update the homepage header to replace the current title/tagline hero with a search input.
- Add a right-aligned navigation badge “DEVON BURRISS” that is hidden on smaller viewports.

## Constraints
- Client-side only; no server/API.
- No `npm` usage (Bun is allowed for build-time scripting).
- The browser MUST NOT perform document indexing.
- Index generation MUST be deterministic and reproducible.

## Impact
- Affected specs:
  - `openspec/specs/site-theme/spec.md` (homepage header + navigation layout; remove prior “no search UI” constraint)
  - `openspec/specs/site-publishing/spec.md` (pipeline adds search artifact generation)
  - New capability: `site-search` (index format, deterministic build, runtime search behavior)
- Affected areas (implementation stage):
  - `src/SiteRenderer/**` (emit search documents; inject scripts/UI)
  - `render.sh` / CI workflow (add Bun-based indexing step)
  - `css/site.css`, `js/**` (search UI + runtime query)

## Open Questions
- What fields should be indexed: title-only, body-only, or both with different weights?
- Should results be limited to posts only, or include pages (About, Topics, Reading)?
