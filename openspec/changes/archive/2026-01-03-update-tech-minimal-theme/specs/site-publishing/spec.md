## MODIFIED Requirements
### Requirement: F# Rendering Pipeline
The site SHALL render Markdown content using F# scripts with Giraffe.ViewEngine components, producing HTML that preserves current URLs, metadata, and feed structure. The visual layout and styling MAY evolve over time as governed by the active theme specifications.

#### Scenario: Posts rendered with preserved permalinks
- **WHEN** the renderer processes an existing Markdown post with front matter
- **THEN** it emits HTML whose permalink and canonical URL remain stable
- **AND** it preserves essential metadata (title, description, author, date)

### Requirement: Local Rendering Tooling
The project SHALL provide documented commands for authors to run the F# renderer locally using `run.ps1` (PowerShell) or `run.sh` (Bash) scripts. The project SHALL also provide `new-draft.ps1` (PowerShell) and `new-draft.sh` (Bash) scripts for creating new draft posts with proper front matter and file naming conventions.

#### Scenario: Local build command available
- **WHEN** a developer executes `./run.ps1` or `./run.sh`
- **THEN** the renderer outputs the full static site to `_site/` ready for review

#### Scenario: Local preview with serve option
- **WHEN** a developer executes `./run.ps1 -Serve` or `./run.sh --serve`
- **THEN** the site is built and served locally for browser preview
