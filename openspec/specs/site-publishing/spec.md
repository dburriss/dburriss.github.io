# site-publishing Specification

## Purpose
TBD - created by archiving change migrate-blog-to-fsharp-renderer. Update Purpose after archive.
## Requirements
### Requirement: F# Rendering Pipeline
The site SHALL render Markdown content using F# scripts with Giraffe.ViewEngine components, producing HTML that matches current layouts, metadata, and URLs.

#### Scenario: Posts rendered with preserved permalinks
- **WHEN** the renderer processes an existing Markdown post with front matter
- **THEN** it emits HTML whose permalink, canonical URL, and metadata match the current Pretzel output.

#### Scenario: Layouts converted to F# components
- **WHEN** a page previously composed via Liquid layouts or includes is rendered
- **THEN** the F# Giraffe components generate identical structure and styling without relying on Liquid templates.

### Requirement: Feed Generation in F#
The site SHALL regenerate RSS and Atom feeds using F# XML builders that mirror the current feed structure and content.

#### Scenario: RSS feed parity
- **WHEN** the F# feed generator runs
- **THEN** the produced RSS feed contains the same entries, ordering, and metadata as the Pretzel-generated feed.

#### Scenario: Atom feed parity
- **WHEN** the generator produces the Atom feed
- **THEN** its XML structure and entry data align with the existing Atom feed consumers expect.

### Requirement: Local Rendering Tooling
The project SHALL provide documented commands for authors to run the F# renderer locally using `run.ps1` (PowerShell) or `run.sh` (Bash) scripts.

#### Scenario: Local build command available
- **WHEN** a developer executes `./run.ps1` or `./run.sh`
- **THEN** the renderer outputs the full static site to `_site/` ready for review with the same layout and feed structure.

#### Scenario: Local preview with serve option
- **WHEN** a developer executes `./run.ps1 -Serve` or `./run.sh --serve`
- **THEN** the site is built and served locally for browser preview.

### Requirement: Configurable Static Asset Copying
The renderer SHALL copy static assets to the output directory based on glob patterns specified in the `include` configuration list. If no patterns are configured, the renderer SHALL default to copying `css/**`, `js/**`, `img/**`, and `fonts/**`.

#### Scenario: Custom include patterns copy matching files
- **WHEN** `_config.yml` contains an `include` list with patterns `["css/**", "tkd/**", "CNAME"]`
- **THEN** the renderer copies all files matching those patterns from the source to the output directory.

#### Scenario: Default patterns used when include is empty
- **WHEN** `_config.yml` does not contain an `include` key or the list is empty
- **THEN** the renderer copies files matching `css/**`, `js/**`, `img/**`, and `fonts/**` by default.

#### Scenario: Single files copied by exact match
- **WHEN** an `include` pattern is an exact filename like `CNAME` or `sitemap.xml`
- **THEN** the renderer copies that specific file from the source root to the output root.

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

