## MODIFIED Requirements
### Requirement: Local Rendering Tooling
The project SHALL provide documented commands for authors to run the F# renderer locally using `run.ps1` (PowerShell) or `run.sh` (Bash) scripts.

#### Scenario: Local build command available
- **WHEN** a developer executes `./run.ps1` or `./run.sh`
- **THEN** the renderer outputs the full static site to `_site/` ready for review with the same layout and feed structure.

#### Scenario: Local preview with serve option
- **WHEN** a developer executes `./run.ps1 -Serve` or `./run.sh --serve`
- **THEN** the site is built and served locally for browser preview.

## ADDED Requirements
### Requirement: Publish Scripts
The project SHALL provide `publish.ps1` (PowerShell) and `publish.sh` (Bash) scripts that push the generated `_site` contents to the `master` branch.

#### Scenario: Local publish dry-run
- **WHEN** a developer executes `./publish.ps1 -DryRun` or `./publish.sh --dry-run`
- **THEN** the script shows what would be committed and pushed without making changes.

#### Scenario: Publish to master branch
- **WHEN** a developer or CI executes `./publish.ps1` or `./publish.sh`
- **THEN** the script commits the `_site` contents and force-pushes to the `master` branch.

### Requirement: GitHub Actions CI/CD Pipeline
The project SHALL use GitHub Actions to automatically build and deploy the site when changes are pushed to the `source` branch.

#### Scenario: Automated build on push
- **WHEN** commits are pushed to the `source` branch
- **THEN** GitHub Actions builds the F# SiteRenderer and generates the static site.

#### Scenario: Automated deployment using publish script
- **WHEN** the build succeeds
- **THEN** GitHub Actions executes the publish script to push the generated site to the `master` branch.

#### Scenario: Submodule handling
- **WHEN** the workflow runs
- **THEN** git submodules are initialized and updated before the build.

## REMOVED Requirements
### Requirement: AppVeyor CI Configuration
**Reason**: Replaced by GitHub Actions for better GitHub integration and modern CI features.
**Migration**: Use GitHub Actions workflow at `.github/workflows/deploy.yml` instead.

### Requirement: Pretzel Build Tooling
**Reason**: The blog has migrated to a custom F# SiteRenderer; Pretzel and its Cake build scripts are no longer used.
**Migration**: Use `run.ps1` or `run.sh` for local builds, GitHub Actions for CI/CD.

### Requirement: Liquid Template Files
**Reason**: The F# SiteRenderer uses `Layouts.fs` for all HTML generation; Liquid templates in `_layouts/` and `_includes/` are no longer used.
**Migration**: All layout and partial functionality is now in `src/SiteRenderer/Layouts.fs`.
