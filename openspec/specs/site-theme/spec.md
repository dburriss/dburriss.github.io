# site-theme Specification

## Purpose
TBD - created by archiving change update-tech-minimal-theme. Update Purpose after archive.
## Requirements
### Requirement: Tech-minimal theme baseline
The site SHALL adopt a “tech minimalist” visual theme with subtle brutalist accents, inspired by `design/tech-min.png`.

#### Scenario: Theme establishes tech-minimal visual language
- **WHEN** a reader visits the homepage
- **THEN** sections are visually separated using simple borders and/or horizontal rules
- **AND** typography and spacing are minimal and consistent

### Requirement: Consistent styling across primary views
The site SHALL apply the tech-minimal theme consistently across homepage, post pages, page content, topics pages, and archive/pagination views.

#### Scenario: Shared components are visually consistent
- **WHEN** a reader navigates between homepage, a post page, and a topics page
- **THEN** the navigation, typography, and divider styles are consistent

### Requirement: Jade Pebble Morning palette for light and dark themes
The site SHALL use the Jade Pebble Morning palette (`https://www.figma.com/color-palettes/jade-pebble-morning/`) to define both light and dark themes.

#### Scenario: Light theme uses palette
- **WHEN** a reader uses a light color scheme preference
- **THEN** the site background is light and derived from the palette
- **AND** text and accents are derived from the palette

#### Scenario: Dark theme uses palette
- **WHEN** a reader uses a dark color scheme preference
- **THEN** the site background is dark and derived from the palette
- **AND** text and accents are derived from the palette

### Requirement: Modern HTML and minimal CSS (no Bootstrap)
The site MUST NOT depend on Bootstrap or the existing “Clean Blog” styling.

#### Scenario: Rendered pages do not include Bootstrap or legacy theme assets
- **WHEN** a reader loads any site page
- **THEN** the HTML does not reference `bootstrap.css`/`bootstrap.min.css` or `bootstrap.js`/`bootstrap.min.js`
- **AND** the HTML does not reference `clean-blog.css` or `clean-blog.js`

### Requirement: Theme selection, toggle, and persistence
The site SHALL support both light and dark themes. The site SHALL select the initial theme using `prefers-color-scheme` unless an explicit reader preference has been persisted. The site SHALL provide a theme toggle control that allows the reader to switch themes, and SHALL persist that preference for future visits.

#### Scenario: OS preference controls initial theme when no stored preference
- **WHEN** a reader has `prefers-color-scheme: dark`
- **AND** no explicit preference is stored
- **THEN** the site renders with the dark theme

#### Scenario: Stored preference overrides OS preference
- **WHEN** a reader has a stored theme preference
- **THEN** the site renders using the stored preference
- **AND** it ignores `prefers-color-scheme` for theme selection

#### Scenario: Reader toggles theme
- **WHEN** a reader activates the theme toggle
- **THEN** the site switches between light and dark themes
- **AND** the selected theme preference is persisted

#### Scenario: Theme toggle is accessible
- **WHEN** a reader navigates using keyboard-only controls
- **THEN** the theme toggle can be focused and activated
- **AND** it has an accessible label describing its purpose

### Requirement: Homepage header provides search input
The homepage header area SHALL present a search input as the primary call-to-action.

#### Scenario: Homepage hero is replaced by search
- **WHEN** a reader visits the homepage
- **THEN** the header area contains a search input
- **AND** the previous "Devon Burriss' Blog" header title/tagline content is not rendered in that location

### Requirement: Navigation includes DEVON BURRISS badge
The top navigation SHALL include a right-aligned badge with the text "DEVON BURRISS".

#### Scenario: Badge appears on larger viewports
- **WHEN** a reader views the site at a larger viewport width
- **THEN** the top navigation displays the "DEVON BURRISS" badge to the right side of the navigation menu

#### Scenario: Badge is hidden on smaller viewports
- **WHEN** a reader views the site at a smaller viewport width
- **THEN** the badge is hidden via a CSS media query

### Requirement: Use existing navigation structure
The implementation SHALL use the existing navigation structure and the currently commented-out navigation element as the insertion point for the badge.

#### Scenario: Navigation remains consistent
- **WHEN** a reader navigates between pages
- **THEN** the navigation links and theme toggle remain present and functional
- **AND** the badge is implemented within the existing nav structure

### Requirement: Diagram Theme Integration
Mermaid diagrams SHALL integrate seamlessly with the site's light and dark themes using the Jade Pebble Morning palette.

#### Scenario: Light theme diagram styling
**Given** the site is using the light theme
**When** a Mermaid diagram is rendered
**Then** the diagram SHALL use light theme colors derived from the Jade Pebble Morning palette
**And** diagram backgrounds SHALL be transparent to inherit page background
**And** text and line colors SHALL provide adequate contrast on light backgrounds

#### Scenario: Dark theme diagram styling  
**Given** the site is using the dark theme
**When** a Mermaid diagram is rendered
**Then** the diagram SHALL use dark theme colors derived from the Jade Pebble Morning palette
**And** diagram backgrounds SHALL be transparent to inherit page background
**And** text and line colors SHALL provide adequate contrast on dark backgrounds

#### Scenario: Diagram container styling
**Given** any page contains Mermaid diagrams
**When** the diagrams are displayed
**Then** diagram containers SHALL have consistent margin and padding with other content elements
**And** diagrams SHALL be horizontally centered within their containers
**And** wide diagrams SHALL support horizontal scrolling without breaking layout

### Requirement: Diagram Loading States
The system SHALL provide visual feedback during diagram processing.

#### Scenario: Diagram loading appearance
**Given** a page contains Mermaid diagrams that are still processing
**When** the page is viewed before rendering completes
**Then** the raw diagram source SHALL be visible in a styled code block
**And** the loading state SHALL be visually distinct but not jarring
**And** the loading state SHALL use theme-appropriate background colors

#### Scenario: Diagram error styling
**Given** a Mermaid diagram has invalid syntax
**When** the error state is displayed
**Then** the error message SHALL be styled with theme-appropriate error colors
**And** the error state SHALL maintain consistency with other error messages on the site
**And** the error container SHALL use monospace font for diagram source display

