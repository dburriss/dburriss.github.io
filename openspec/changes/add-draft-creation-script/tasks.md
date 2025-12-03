# Tasks: Add Draft Creation Script

## 1. Create PowerShell Script
- [ ] 1.1 Create `new-draft.ps1` with interactive prompts for title, subtitle, tags, categories
- [ ] 1.2 Implement automatic slug generation from title
- [ ] 1.3 Add filename generation with date prefix option
- [ ] 1.4 Generate complete YAML front matter template
- [ ] 1.5 Add option to open file in editor after creation

## 2. Create Bash Script
- [ ] 2.1 Create `new-draft.sh` with interactive prompts for title, subtitle, tags, categories
- [ ] 2.2 Implement automatic slug generation from title
- [ ] 2.3 Add filename generation with date prefix option
- [ ] 2.4 Generate complete YAML front matter template
- [ ] 2.5 Add option to open file in editor after creation

## 3. Update Documentation
- [ ] 3.1 Update `README.md` to include draft creation instructions
- [ ] 3.2 Update `openspec/project.md` to document the new workflow

## 4. Validation
- [ ] 4.1 Test PowerShell script on Windows (if available)
- [ ] 4.2 Test Bash script on Linux/macOS
- [ ] 4.3 Verify generated files have correct front matter
- [ ] 4.4 Verify generated files can be built by the renderer
- [ ] 4.5 Make scripts executable (chmod +x for bash script)