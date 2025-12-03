# Tasks: Add Draft Creation Script

## 1. Create PowerShell Script
- [x] 1.1 Create `new-draft.ps1` with interactive prompts for title, subtitle, tags, categories
- [x] 1.2 Implement automatic slug generation from title
- [x] 1.3 Add filename generation with date prefix option
- [x] 1.4 Generate complete YAML front matter template
- [x] 1.5 Add option to open file in editor after creation

## 2. Create Bash Script
- [x] 2.1 Create `new-draft.sh` with interactive prompts for title, subtitle, tags, categories
- [x] 2.2 Implement automatic slug generation from title
- [x] 2.3 Add filename generation with date prefix option
- [x] 2.4 Generate complete YAML front matter template
- [x] 2.5 Add option to open file in editor after creation

## 3. Update Documentation
- [x] 3.1 Update `README.md` to include draft creation instructions
- [x] 3.2 Update `openspec/project.md` to document the new workflow

## 4. Validation
- [x] 4.1 Test PowerShell script on Windows (if available)
- [x] 4.2 Test Bash script on Linux/macOS
- [x] 4.3 Verify generated files have correct front matter
- [x] 4.4 Verify generated files can be built by the renderer
- [x] 4.5 Make scripts executable (chmod +x for bash script)