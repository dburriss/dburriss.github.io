# Change: Add Draft Creation Script

## Why
Currently, authors must manually create new draft files in `_drafts/` with proper YAML front matter, filename conventions, and directory structure. This process is error-prone and time-consuming, requiring authors to remember the exact front matter format and naming conventions. A dedicated script would streamline the draft creation workflow, ensuring consistency and reducing friction for content authors.

## What Changes
- **ADDED** `new-draft.ps1` (PowerShell) script for creating new drafts interactively
- **ADDED** `new-draft.sh` (Bash) script for creating new drafts interactively
- **MODIFIED** `openspec/project.md` to document the new draft creation workflow
- **MODIFIED** `README.md` to include draft creation instructions

## Impact
- Affected specs: `site-publishing` (MODIFIED to include draft creation tooling)
- Affected code: New scripts in repository root
- **No breaking changes** to existing functionality
- Authors can use `new-draft.ps1` or `new-draft.sh` for faster draft creation
- Scripts ensure consistent front matter and file naming
- Reduces time to start writing new content