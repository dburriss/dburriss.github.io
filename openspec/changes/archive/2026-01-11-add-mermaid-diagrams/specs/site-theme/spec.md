# site-theme Specification Delta

## ADDED Requirements

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