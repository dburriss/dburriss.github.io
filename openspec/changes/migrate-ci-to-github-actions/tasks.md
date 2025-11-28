## 1. Add Publish Scripts
- [ ] 1.1 Create `publish.ps1` (PowerShell) script to push `_site` to master branch
- [ ] 1.2 Create `publish.sh` (Bash) script to push `_site` to master branch
- [ ] 1.3 Scripts should handle git configuration, commit, and force-push to master

## 2. Add GitHub Actions Workflow
- [ ] 2.1 Create `.github/workflows/deploy.yml` with build steps
- [ ] 2.2 Configure workflow to trigger on `source` branch pushes
- [ ] 2.3 Add steps to build F# SiteRenderer and generate site
- [ ] 2.4 Call publish script for deployment step

## 3. Remove Legacy Pretzel Files
- [ ] 3.1 Delete `pretzel.cake` (Cake build script)
- [ ] 3.2 Delete `pretzel.ps1` (PowerShell bootstrapper)
- [ ] 3.3 Delete `_plugins/` directory (Pretzel plugin DLLs)

## 4. Remove Legacy Liquid Template Directories
- [ ] 4.1 Delete `_layouts/` directory (Liquid templates)
- [ ] 4.2 Delete `_includes/` directory (Liquid partials)

## 5. Remove AppVeyor Configuration
- [ ] 5.1 Delete `appveyor.yml`

## 6. Update Project Documentation
- [ ] 6.1 Update `openspec/project.md` to reflect GitHub Actions CI and removed directories
- [ ] 6.2 Remove references to AppVeyor, Pretzel, Liquid templates from documentation
- [ ] 6.3 Update README.md to document CI, workflow changes

## 7. Validation
- [ ] 7.1 Verify GitHub Actions workflow syntax is valid
- [ ] 7.2 Ensure `run.ps1` and `run.sh` still work for local development
- [ ] 7.3 Test `publish.ps1` / `publish.sh` locally (dry-run mode)
- [ ] 7.4 Test deployment to `master` branch works correctly
