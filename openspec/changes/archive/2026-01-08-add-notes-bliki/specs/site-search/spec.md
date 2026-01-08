## ADDED Requirements
### Requirement: Notes participate in search index
The search system SHALL index notes alongside posts using the existing FlexSearch-based pipeline.

#### Scenario: Notes included in docs.json
- **WHEN** the renderer produces the search document feed (e.g. `docs.json`)
- **THEN** it includes entries for notes under `/notes/...` with at least `id`, `url`, and `title`

#### Scenario: Notes searchable by title, headings, and body
- **WHEN** a reader searches for text that appears only in the title, headings, or body of a note
- **THEN** the search UI returns that note in the results

### Requirement: Notes preserve deterministic index generation
Notes MUST NOT compromise the determinism of search artifact generation.

#### Scenario: Index including notes is reproducible
- **WHEN** the index is generated multiple times from the same set of posts and notes
- **THEN** the resulting search artifacts (manifest, docs, index) remain byte-identical

### Requirement: Notes are excluded from feeds but visible in search
Notes SHALL be discoverable through search even though they are excluded from existing RSS/Atom feeds by default.

#### Scenario: Notes appear in search-only
- **WHEN** a reader uses search to find a term that appears only in notes
- **THEN** the search results include the matching notes
- **AND** those notes do not appear in the main RSS/Atom feeds
