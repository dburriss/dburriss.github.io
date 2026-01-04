## 1. Search Data + Index Generation (build time)
- [x] 1.1 Define the search document schema emitted by the renderer for `/search/docs.json`.
- [x] 1.2 Update F# SiteRenderer to emit `/search/docs.json` as a static JSON artifact in the output directory.
- [x] 1.3 Add a Bun-based build script that reads `/search/docs.json` and builds a FlexSearch index.
- [x] 1.4 Export deterministic static artifacts: `/search/index.json` and `/search/manifest.json`.
- [x] 1.5 Document the `/search/*.json` output format and versioning rules.

## 2. Client Search Runtime
- [x] 2.1 Add a small client-side JS module that lazy-loads the prebuilt index and document store when the search input is focused.
- [x] 2.2 Implement focus-triggered loading with visual feedback (e.g., grey out search button while downloading).
- [x] 2.3 Implement query handling (debounced input), result rendering, and empty-state behavior.
- [x] 2.4 Ensure the client never performs indexing (no `add`/`update` calls; import-only).

## 3. UI Updates
- [x] 3.1 Replace the homepage hero (“Devon Burriss' Blog” + tagline) with a search input and results container.
- [x] 3.2 Add a right-aligned nav badge “DEVON BURRISS” using the existing commented-out nav section.
- [x] 3.3 Add responsive CSS to hide the badge on smaller viewports.

## 4. Pipeline Integration + Validation
- [x] 4.1 Integrate the Bun index generation script into `render.sh` and `render.ps1` to run after dotnet site generation.
- [x] 4.2 Add a deterministic build check (regenerate index twice; ensure identical output).
- [x] 4.3 Validate locally by running `./run.sh --serve` and testing search in a browser.
- [x] 4.4 Run `dotnet fantomas ./src/SiteRenderer/` and `dotnet build ./src/SiteRenderer/SiteRenderer.fsproj` after implementation changes.
