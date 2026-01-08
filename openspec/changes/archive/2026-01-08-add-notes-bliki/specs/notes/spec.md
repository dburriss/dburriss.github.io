## ADDED Requirements
### Requirement: Notes content type and storage
The site SHALL support a first-class `Note` content type distinct from blog posts.

#### Scenario: Notes stored under dedicated directory
- **WHEN** an author adds Markdown files under a `notes/` directory at the repository root
- **THEN** the renderer discovers them as `Note` content
- **AND** it treats them separately from `_posts/` and `_drafts/` content

#### Scenario: Notes share global theme and layout shell
- **WHEN** a reader visits any note page
- **THEN** the header, navigation, footer, and theme match the rest of the site

### Requirement: Notes front matter and metadata
Each note SHALL use minimal YAML front matter with at least a `title` field and MAY include `keywords`, `topics`, and `status` fields.

#### Scenario: Note with minimal front matter builds successfully
- **WHEN** an author creates `notes/example.md` with front matter containing `title: Example`
- **THEN** the renderer successfully generates a note page for it

#### Scenario: Notes reuse topic catalog
- **WHEN** a note specifies `topics: ["some-topic-id"]`
- **THEN** each topic ID MUST exist in the configured topics catalog in `_config.yml`

#### Scenario: Missing required title fails the build
- **WHEN** a note file under `notes/` is missing a `title` in its front matter
- **THEN** the build fails with a clear error referencing the note path

### Requirement: Deterministic note slugs and URLs
Notes MUST have deterministic, stable slugs and URLs under a `/notes/` prefix.

#### Scenario: Note URL derived from slug
- **WHEN** a note has slug `my-note` (derived from file name or title)
- **THEN** its output URL is `/notes/my-note/`
- **AND** the file is rendered to `_site/notes/my-note/index.html`

#### Scenario: Slug generation is stable
- **WHEN** a note file content changes but its title and path do not
- **THEN** the generated slug and URL remain the same across builds

### Requirement: Notes excluded from RSS/Atom feeds by default
Notes SHALL be excluded from existing RSS and Atom feeds unless a dedicated notes feed is introduced in a future change.

#### Scenario: Notes not present in main feeds
- **WHEN** the RSS and Atom feeds are generated
- **THEN** entries correspond only to posts and pages defined by existing feed behavior
- **AND** notes under `/notes/` do not appear in those feeds

### Requirement: Notes landing page and navigation entry
The site SHALL provide a landing page and navigation entry for notes.

#### Scenario: Notes landing page exists
- **WHEN** a reader visits `/notes/`
- **THEN** the site renders a notes index page listing available notes using concept-centric ordering (e.g., by title or grouping)

#### Scenario: Notes are reachable from primary navigation
- **WHEN** a reader views the primary navigation
- **THEN** there is a visible entry (e.g., "Notes" or "Bliki") linking to `/notes/`

### Requirement: Wiki-style internal links for notes
The Markdown pipeline SHALL support wiki-style internal links using `[[Page Name]]` syntax that resolve to notes or posts at build time.

#### Scenario: Wiki link resolves to a unique note
- **WHEN** a note body contains `[[Example]]`
- **AND** there is exactly one note titled "Example"
- **THEN** the renderer converts the wiki syntax into a link pointing to that note’s URL

#### Scenario: Wiki link resolves to a unique post
- **WHEN** a note or post body contains `[[Some Post]]`
- **AND** there is exactly one post titled "Some Post" and no note with that title
- **THEN** the renderer converts the wiki syntax into a link pointing to that post’s URL

#### Scenario: Unresolved wiki link triggers validation error or warning
- **WHEN** a wiki link label does not match any note or post title
- **THEN** the build emits a validation error or warning summarizing the unresolved link and its source document

#### Scenario: Ambiguous wiki link is reported
- **WHEN** a wiki label matches multiple notes and/or posts
- **THEN** the build reports the ambiguity and the candidate targets in its diagnostics

### Requirement: Backlinks and orphan detection for notes
The build pipeline SHALL compute backlinks and detect orphaned notes using link analysis.

#### Scenario: Notes display backlinks section
- **WHEN** one or more notes or posts link to a target note
- **THEN** the rendered target note page includes a "Linked from" section listing those sources

#### Scenario: Orphaned notes reported at build time
- **WHEN** a note has no inbound links and is not explicitly referenced from navigation or a landing page
- **THEN** the build reports it as an orphan in a summary (non-fatal by default)
