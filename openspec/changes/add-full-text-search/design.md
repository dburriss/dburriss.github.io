## Context
The blog is rendered by an F# SiteRenderer into static HTML and assets that are published to GitHub Pages. There is no backend.

The current homepage includes a hero section (`Layouts.fs` `home-hero`) that displays the site title and description, and the top navigation (`site-header`) is sticky and built via Giraffe.ViewEngine.

## Goals / Non-Goals
- Goals:
  - Provide fast, client-side full-text search over posts.
  - Keep GitHub Pages compatibility (static assets only).
  - Ensure FlexSearch indexing happens at build time, never in the browser.
  - Make search index generation deterministic and reproducible.
  - Implement the requested UI adjustments (home hero replaced by search input; nav badge).
- Non-Goals:
  - No server-side search or API.
  - No runtime crawling/indexing of HTML by the browser.
  - No heavy UI framework.

## Decision: Build-time indexing with Bun + FlexSearch (Option 1)
### Decision
Use the existing render output as the source of “search documents”, then run a Bun script that builds and exports a FlexSearch index into a static artifact that the browser loads and queries.

### Rationale
- Keeps runtime JS minimal (load index + query).
- Avoids large CPU work on low-powered devices.
- Works on GitHub Pages (static files only).
- Determinism is easier to enforce during build.

### Proposed data flow
1. F# SiteRenderer emits `/search/docs.json` (deterministic document feed) into the output directory.
2. A Bun script reads `/search/docs.json` and builds a FlexSearch index.
3. The Bun script exports deterministic artifacts:
   - `/search/index.json` (FlexSearch export payload)
   - `/search/manifest.json` (schemaVersion + counts)
4. The Bun script is integrated into `./render.sh` and `./render.ps1`, running after the dotnet site generation.
5. Client JS lazy-loads `/search/index.json` and `/search/docs.json` only when the search input is focused, provides visual feedback (e.g., greying out a search button) while downloading, then imports the index and executes queries.

## Determinism Strategy
- The renderer emits documents in a stable order (e.g., by canonical URL).
- The Bun script uses a pinned FlexSearch version and fixed index options.
- Export uses a stable JSON serialization strategy (sorted keys, stable array ordering, consistent newline/EOL).
- Avoid timestamps, random IDs, or environment-specific absolute paths.

## UI Notes
- Homepage: search replaces `home-hero` (title/tagline removed) and renders a search input + results container.
- Navigation: add a right-aligned badge "DEVON BURRISS" using the existing commented-out `site-title` anchor location in `navNode`.
- Responsiveness: hide badge below a defined viewport width (media query).

## Risks / Trade-offs
- Index size may grow as content grows; we mitigate by indexing only relevant fields (title + plain-text body) and by excluding code blocks or excessive markup.
- Determinism may be impacted by document normalization choices; mitigate by defining canonical text extraction rules.

## Migration Plan
- Introduce the search artifacts as additive static files; no URL changes.
- Deploy with search hidden behind the new homepage UI; no impact on existing pages except the home hero.
- Rollback by removing the build step and restoring the previous home hero markup.
