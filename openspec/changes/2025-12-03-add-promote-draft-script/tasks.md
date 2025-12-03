## 1. Analysis
- [ ] 1.1 Examine existing draft and post file structures to understand naming conventions
- [ ] 1.2 Review front matter patterns in drafts vs posts to identify required changes
- [ ] 1.3 Check for any existing promotion workflows or scripts

## 2. Script Implementation
- [ ] 2.1 Create `promote-draft.sh` (Bash) script with:
  - Draft listing and selection functionality
  - File movement from `_drafts/` to `_posts/`
  - Front matter parsing and updating (published: false → true)
  - Date prefix handling with optional custom date parameter
  - Validation and error handling
- [ ] 2.2 Create `promote-draft.ps1` (PowerShell) script with:
  - Interactive draft selection menu
  - File operations and front matter manipulation
  - Parameter support for custom publish date
  - Comprehensive error handling and validation
- [ ] 2.3 Add help documentation and usage examples to both scripts

## 3. Validation
- [ ] 3.1 Test script with various draft formats and edge cases
- [ ] 3.2 Verify front matter updates are correctly applied
- [ ] 3.3 Ensure proper file naming and date handling
- [ ] 3.4 Test error scenarios (missing files, invalid front matter, conflicts)

## 4. Documentation
- [ ] 4.1 Update `project.md` to include the new promote-draft workflow
- [ ] 4.2 Add usage examples to the script help text
- [ ] 4.3 Document integration with existing authoring workflows