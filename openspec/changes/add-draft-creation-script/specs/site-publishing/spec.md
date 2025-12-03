## MODIFIED Requirements

### Requirement: Local Rendering Tooling
The project SHALL provide documented commands for authors to run the F# renderer locally using `run.ps1` (PowerShell) or `run.sh` (Bash) scripts. The project SHALL also provide `new-draft.ps1` (PowerShell) and `new-draft.sh` (Bash) scripts for creating new draft posts with proper front matter and file naming conventions.

#### Scenario: Local build command available
- **WHEN** a developer executes `./run.ps1` or `./run.sh`
- **THEN** the renderer outputs the full static site to `_site/` ready for review with the same layout and feed structure.

#### Scenario: Local preview with serve option
- **WHEN** a developer executes `./run.ps1 -Serve` or `./run.sh --serve`
- **THEN** the site is built and served locally for browser preview.

#### Scenario: Draft creation script available
- **WHEN** an author executes `./new-draft.ps1` or `./new-draft.sh`
- **THEN** the script prompts for post metadata and creates a properly formatted draft file in `_drafts/` with complete YAML front matter.

#### Scenario: Generated draft has valid front matter
- **WHEN** a draft is created using the new-draft script
- **THEN** the file contains valid YAML front matter with title, layout, published status, and other required fields.