## 1. Add Publish Scripts
- [x] 1.1 Create `build.ps1` (Powershell) for building the SiteRenderer solution
- [x] 1.2 Create `build.sh` (Bash) for building the SiteRenderer solution
- [x] 1.3 Create `render.ps1` (Powershell) for using SiteRenderer to generate the site content to _site
- [x] 1.4 Create `render.sh` (Bash) for using SiteRenderer to generate the site content to _site
- [x] 1.5 Create `publish.ps1` (PowerShell) script to push `_site` to master branch
- [x] 1.6 Create `publish.sh` (Bash) script to push `_site` to master branch
- [x] 1.7 Scripts should handle git configuration, commit, and force-push to master

## 2. Add GitHub Actions Workflow
- [x] 2.1 Create `.github/workflows/deploy.yml` with build steps
- [x] 2.2 Configure workflow to trigger on `source` branch pushes
- [x] 2.3 Add steps to build F# SiteRenderer and generate site
- [x] 2.4 Call publish script for deployment step

## 3. Remove Legacy Pretzel Files
- [x] 3.1 Delete `pretzel.cake` (Cake build script)
- [x] 3.2 Delete `pretzel.ps1` (PowerShell bootstrapper)
- [x] 3.3 Delete `_plugins/` directory (Pretzel plugin DLLs)

## 4. Remove Legacy Liquid Template Directories
- [x] 4.1 Delete `_layouts/` directory (Liquid templates)
- [x] 4.2 Delete `_includes/` directory (Liquid partials)

## 5. Remove AppVeyor Configuration
- [x] 5.1 Delete `appveyor.yml`

## 6. Update Project Documentation
- [x] 6.1 Update `openspec/project.md` to reflect GitHub Actions CI and removed directories
- [x] 6.2 Remove references to AppVeyor, Pretzel, Liquid templates from documentation

## 7. Validation
- [x] 7.1 Verify GitHub Actions workflow syntax is valid
- [x] 7.2 Ensure `run.ps1` and `run.sh` still work for local development
- [x] 7.3 Test `publish.ps1` / `publish.sh` locally (dry-run mode)
- [x] 7.4 Test deployment to `master` branch works correctly (requires CI run)
