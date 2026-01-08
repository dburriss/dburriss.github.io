## ADDED Requirements
### Requirement: Notes discovery and rendering in F# pipeline
The F# rendering pipeline SHALL discover and render notes from a dedicated source directory in addition to posts.

#### Scenario: Renderer processes notes directory
- **WHEN** the renderer runs on a repository that contains a `notes/` directory
- **THEN** it parses Markdown notes from that directory using the same Markdown and front matter tooling as posts (with additional wiki-link semantics)
- **AND** it generates HTML pages under `_site/notes/...`

#### Scenario: Notes build is optional when directory absent
- **WHEN** the repository does not contain a `notes/` directory
- **THEN** the renderer still completes successfully
- **AND** no notes-related pages are generated

### Requirement: Build-time validation for notes links
The rendering phase SHALL validate wiki-style links for notes at build time and report unresolved or ambiguous links.

#### Scenario: Unresolved wiki links reported with context
- **WHEN** unresolved wiki links are found in notes or posts
- **THEN** the renderer reports them with file path, line or section context, and the unresolved label

#### Scenario: Configurable severity for wiki link issues
- **WHEN** a configuration option or environment setting is used to control severity
- **THEN** unresolved wiki links MAY be treated as warnings or errors according to that configuration

### Requirement: Orphaned notes detection in publishing output
The build/publish process SHALL detect orphaned notes and surface them in build output.

#### Scenario: Orphaned notes summary
- **WHEN** a build completes successfully
- **AND** one or more notes have no inbound links and no explicit navigation references
- **THEN** the build emits a summary listing those orphaned notes so authors can decide whether to link or archive them
