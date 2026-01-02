## MODIFIED Requirements
### Requirement: Draft Promotion Scripts
The project SHALL provide `promote-draft.ps1` (PowerShell) and `promote-draft.sh` (Bash) scripts that automate moving draft posts to publication and validate required front matter fields including topics and keywords.

#### Scenario: Interactive draft selection and promotion
- **WHEN** an author executes `./promote-draft.sh` or `./promote-draft.ps1`
- **THEN** the script lists available drafts in `_drafts/` and allows selection for promotion
- **AND** moves the selected file to `_posts/` with today's date prefix
- **AND** updates the front matter to set `published: true`

#### Scenario: Custom publish date specification
- **WHEN** an author provides a custom date parameter to the promote script
- **THEN** the script uses the specified date for the filename prefix instead of today's date
- **AND** validates the date format is YYYY-MM-DD

#### Scenario: Front matter validation and update
- **WHEN** a draft is promoted
- **THEN** the script parses the YAML front matter
- **AND** verifies `topics` and `keywords` exist and are non-empty lists/values
- **AND** updates `published: false` to `published: true`
- **AND** preserves all other front matter fields unchanged
- **AND** validates the resulting front matter is properly formatted

#### Scenario: Error handling for invalid drafts
- **WHEN** a selected draft file has invalid or missing front matter
- **THEN** the script displays an error message explaining the issue
- **AND** aborts the promotion process without moving the file
- **AND** provides guidance on how to fix the problem

#### Scenario: Conflict detection and prevention
- **WHEN** promoting a draft would create a filename conflict in `_posts/`
- **THEN** the script warns about the conflict and suggests alternatives
- **AND** does not overwrite existing files unless explicitly authorized

#### Scenario: Help and usage documentation
- **WHEN** an author executes the script with `--help` or `-h` parameter
- **THEN** the script displays usage instructions, examples, and available options
- **AND** explains the promotion workflow and requirements for `topics` and `keywords`

#### Scenario: Legacy category/tags not required during promotion
- **WHEN** promoting a draft lacking `category` or `tags`
- **THEN** the script proceeds as long as `topics` and `keywords` are present
