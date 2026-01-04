## ADDED Requirements
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

## REMOVED Requirements
### Requirement: No search UI introduced

#### Scenario: Homepage contains no search bar
- **WHEN** a reader visits the homepage
- **THEN** no search input is rendered

**Reason**: This change explicitly introduces a search UI and behavior.

**Migration**: Replace the prior prohibition with the new search-related requirements in `site-search` and the homepage header requirement above.
