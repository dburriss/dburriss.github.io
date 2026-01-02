## Context
The site is rendered by the F# `SiteRenderer` using Giraffe.ViewEngine templates (`src/SiteRenderer/Layouts.fs`) and bundled static assets (`css/**`, `js/**`, `img/**`). The existing theme is “Clean Blog”-like, with photo headers and Bootstrap-styled components.

## Goals / Non-Goals
- Goals:
  - Adopt a “tech minimalist” look with subtle brutalist accents, inspired by `design/tech-min.png`.
  - Keep existing content and URLs stable (posts, topics, feeds).
  - Make the author bio a dedicated About page.
- Non-Goals:
  - Implement search UI or search behavior.
  - Rework content taxonomy or routing.

## Decisions
- Decision: Implement as a theme + layout refresh (CSS + markup updates) rather than a renderer re-architecture.
  - Rationale: Minimizes risk to rendering pipeline while achieving the desired look.
- Decision: Remove Bootstrap and the existing “Clean Blog” styling.
  - Rationale: The new design explicitly avoids Bootstrap and requires modern, minimal HTML/CSS.
- Decision: Use modern semantic HTML plus minimal custom CSS; optionally layer a minimal CSS library (e.g., Skeleton) for base typography/reset.
  - Rationale: Keeps the CSS small and understandable while providing a reasonable baseline.
- Decision: Implement dark + light themes using CSS variables, `prefers-color-scheme`, and a manual theme toggle.
  - Rationale: Provides a good default while still allowing an explicit reader preference.
- Decision: Move “About me” into a single canonical page generated from `about.md` at `/about/`.
  - Rationale: Makes the content linkable and avoids duplicated/competing bio content.

## Design Notes (derived from `design/tech-min.png`)
- Dark background with subtle gradient and high-contrast text.
- Sections separated by crisp horizontal rules.
- “Topics” presented as bordered buttons.
- “Recent Posts” presented as a clean list with dates aligned right.

## Theme Tokens (Jade Pebble Morning)
Palette source: `https://www.figma.com/color-palettes/jade-pebble-morning/`
- `#404E3B` (deep olive)
- `#6C8480` (muted teal)
- `#7B9669` (sage)
- `#BAC8B1` (pale green)
- `#E6E6E6` (near-white)

Proposed mapping:
- Dark theme:
  - Background: `#404E3B`
  - Text: `#E6E6E6`
  - Accents/borders: `#6C8480`, `#7B9669`, `#BAC8B1`
- Light theme:
  - Background: `#E6E6E6`
  - Text: `#404E3B`
  - Accents/borders: `#6C8480`, `#7B9669`, `#BAC8B1`

## Risks / Trade-offs
- Visual regressions across older content (code blocks, blockquotes, tables) when switching to a darker theme.
- Subtle dependency on legacy “Clean Blog” classes in templates and JS.

## Migration Plan
- Phase 1: Add About page and navigation link; remove sidebar bio.
- Phase 2: Introduce theme CSS and update templates for the new structure.
- Phase 3: Visual QA in multiple viewports; iterate on small CSS fixes.

## Open Questions
- None.
