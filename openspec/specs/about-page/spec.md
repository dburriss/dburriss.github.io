# about-page Specification

## Purpose
TBD - created by archiving change update-tech-minimal-theme. Update Purpose after archive.
## Requirements
### Requirement: Dedicated About page
The site SHALL provide a dedicated About page at `/about/` that contains the author bio content previously rendered in the “About me” sidebar widget, including an avatar image.

#### Scenario: About page is generated from Markdown with pretty URL
- **WHEN** the repository contains a root `about.md` page with `permalink: /about/`
- **THEN** the renderer generates an `about/index.html` output page
- **AND** the rendered page includes the author bio text and avatar image

### Requirement: About is reachable from navigation
The site SHALL include an “About” link in the top navigation that routes to `/about/`.

#### Scenario: About link is visible in nav
- **WHEN** a reader views any primary page (homepage, posts, topics)
- **THEN** the top navigation contains an “About” link
- **AND** the link target is `/about/`

### Requirement: About widget removed from sidebar
The site SHALL NOT render an “About me” sidebar widget.

#### Scenario: Sidebar no longer contains about widget
- **WHEN** a reader views the homepage or a page layout with a sidebar
- **THEN** the sidebar does not include an “About me” section

