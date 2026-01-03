## ADDED Requirements
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

### Requirement: No search UI introduced
This change MUST NOT add a search bar UI or search behavior.

#### Scenario: Homepage contains no search bar
- **WHEN** a reader visits the homepage
- **THEN** no search input is rendered

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
