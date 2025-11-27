## ADDED Requirements
### Requirement: F# Rendering Pipeline
The site SHALL render Markdown content using F# scripts with Giraffe.ViewEngine components, producing HTML that matches current layouts, metadata, and URLs.

#### Scenario: Posts rendered with preserved permalinks
- **WHEN** the renderer processes an existing Markdown post with front matter
- **THEN** it emits HTML whose permalink, canonical URL, and metadata match the current Pretzel output.

#### Scenario: Layouts converted to F# components
- **WHEN** a page previously composed via Liquid layouts or includes is rendered
- **THEN** the F# Giraffe components generate identical structure and styling without relying on Liquid templates.

### Requirement: Feed Generation in F#
The site SHALL regenerate RSS and Atom feeds using F# XML builders that mirror the current feed structure and content.

#### Scenario: RSS feed parity
- **WHEN** the F# feed generator runs
- **THEN** the produced RSS feed contains the same entries, ordering, and metadata as the Pretzel-generated feed.

#### Scenario: Atom feed parity
- **WHEN** the generator produces the Atom feed
- **THEN** its XML structure and entry data align with the existing Atom feed consumers expect.

### Requirement: Local Rendering Tooling
The project SHALL provide a documented command for authors to run the F# renderer locally without Pretzel.

#### Scenario: Local build command available
- **WHEN** a developer executes the documented build or preview command
- **THEN** the renderer outputs the full static site ready for review with the same layout and feed structure.
