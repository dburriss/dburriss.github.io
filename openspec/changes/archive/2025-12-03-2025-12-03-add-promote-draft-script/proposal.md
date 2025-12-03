# Change: Add promote-draft script for publishing drafts

## Why
Authors currently need to manually move draft files from `_drafts/` to `_posts/` and edit the front matter to set `published: true`. This manual process is error-prone and can lead to inconsistent formatting or incorrect metadata. A dedicated script automates this workflow while ensuring proper file naming, front matter updates, and validation.

## What Changes
- Add `promote-draft.ps1` (PowerShell) and `promote-draft.sh` (Bash) scripts that:
  - List available drafts for selection
  - Move the selected draft from `_drafts/` to `_posts/` with proper date prefix
  - Update front matter to set `published: true`
  - Validate the file structure and front matter after promotion
  - Provide options to specify a custom publish date (defaults to today)
  - Handle edge cases like missing files, invalid front matter, or conflicts

## Impact
- Affected specs: site-publishing
- Affected code: New script files `promote-draft.ps1` and `promote-draft.sh`
- Documentation: Update project.md to include the new workflow