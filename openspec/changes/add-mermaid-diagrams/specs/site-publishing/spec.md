# site-publishing Specification Delta

## ADDED Requirements

### Requirement: Mermaid Asset Management
The build pipeline SHALL manage Mermaid.js library and initialization scripts as static assets.

#### Scenario: Mermaid library copying
**Given** the build process is copying static assets
**When** the F# renderer processes site assets
**Then** the Mermaid.js library file SHALL be copied to `js/vendor/mermaid.min.js` in the output
**And** the Mermaid initialization script SHALL be copied to `js/mermaid-init.js` in the output
**And** the copied files SHALL maintain their relative paths for proper script loading

### Requirement: Conditional Script Loading
The F# layout components SHALL conditionally include Mermaid scripts only on pages containing diagrams.

#### Scenario: Pages with diagrams include scripts
**Given** a page's HTML content contains `class="mermaid"` elements
**When** the layout renders the page head section
**Then** the Mermaid.js library script tag SHALL be included
**And** the Mermaid initialization script tag SHALL be included
**And** both scripts SHALL load with appropriate `src` attributes pointing to copied assets

#### Scenario: Pages without diagrams exclude scripts
**Given** a page's HTML content contains no `class="mermaid"` elements  
**When** the layout renders the page head section
**Then** no Mermaid-related script tags SHALL be included
**And** the page weight SHALL not be impacted by diagram-related assets

### Requirement: Markdig Extension Integration
The F# markdown processing pipeline SHALL include the Diagrams extension to convert mermaid code blocks.

#### Scenario: Diagrams extension enabled in pipelines
**Given** the F# renderer initializes markdown pipelines
**When** both the default pipeline and context-aware pipeline are created
**Then** both pipelines SHALL include the `.UseDiagrams()` extension
**And** the extension SHALL be compatible with existing extensions (WikiLinks, YamlFrontMatter, etc.)

#### Scenario: Mermaid code block transformation
**Given** markdown content contains a mermaid code block
**When** the content is processed through the markdown pipeline
**Then** the code block SHALL be converted to a `<pre class="mermaid">` element
**And** the diagram source SHALL be preserved exactly as provided
**And** the transformation SHALL not interfere with other code block types