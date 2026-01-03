# Change: Update blog to tech-minimal theme + dedicated About page

## Why
The current site theme is based on a classic “Clean Blog” aesthetic with photo headers and Bootstrap styling. The request is to shift the site to a “tech minimalist” look with subtle brutalist accents (see `design/tech-min.png`), support both dark and light themes using the Jade Pebble Morning palette, and make the author bio (“About me”) available as a dedicated page rather than a sidebar widget.

## What Changes
- Introduce a new site-wide visual theme inspired by `design/tech-min.png`.
- Implement dark + light themes using the Jade Pebble Morning palette (`https://www.figma.com/color-palettes/jade-pebble-morning/`).
- Add a theme toggle control and persist the reader’s preference.
- Remove Bootstrap and the existing Clean Blog styling and replace it with modern semantic HTML and minimal, modern CSS (optionally using a minimal CSS library such as Skeleton).
- Add a dedicated About page at `/about/` and move the existing “About me” content there.
- Remove the “About me” sidebar widget so the bio appears in one canonical location.
- Add an “About” link to the top navigation.

## Explicitly Out of Scope
- Adding the search bar shown in `design/tech-min.png` (to be handled in a separate change/spec later).

## Impact
- Affected specs:
  - `openspec/specs/site-publishing/spec.md` (relaxes “layout parity” language; theme refresh is intentional)
  - New capabilities proposed in this change: `site-theme`, `about-page`
- Likely affected code/assets during implementation:
  - `src/SiteRenderer/Layouts.fs` (navigation, page/post/index markup, sidebar removal)
  - `css/**` (new theme styles; remove dependency on existing theme styles)
  - `js/**` (remove Bootstrap/Clean Blog JS dependency if no longer needed)
  - New root page content file: `about.md` (permalink `/about/`)
