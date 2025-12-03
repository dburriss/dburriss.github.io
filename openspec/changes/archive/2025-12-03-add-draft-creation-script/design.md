# Design: Draft Creation Script

## Context
The blog authoring workflow currently requires manual creation of draft files with proper YAML front matter. This creates friction for authors who need to remember the exact format and naming conventions.

## Goals / Non-Goals
- **Goals:**
  - Provide interactive scripts for creating drafts
  - Ensure consistent front matter format
  - Support both PowerShell and Bash environments
  - Generate appropriate filenames automatically

- **Non-Goals:**
  - Modify the F# renderer or build process
  - Add complex validation or templating features
  - Support for post creation (only drafts)

## Decisions

### Decision: Interactive Command-Line Scripts
- **What:** Create `new-draft.ps1` and `new-draft.sh` with interactive prompts
- **Why:** Simple, familiar interface for authors; no need for GUI or web interface
- **Alternatives considered:** Web form, config file approach

### Decision: Consistent Front Matter Template
- **What:** Use the same YAML structure as existing posts with `published: false`
- **Why:** Ensures compatibility with the F# renderer and maintains consistency
- **Format:** Standard Jekyll-style front matter with blog-specific fields

### Decision: Automatic Slug Generation
- **What:** Generate URL-safe slugs from titles automatically
- **Why:** Reduces manual work and ensures clean URLs
- **Algorithm:** Lowercase, remove special chars, replace spaces with hyphens

## Risks / Trade-offs
- **Risk:** Platform-specific script differences
  - **Mitigation:** Keep logic simple and well-tested on both platforms
- **Risk:** Authors might still need to edit front matter manually
  - **Mitigation:** Make the script comprehensive but allow overrides