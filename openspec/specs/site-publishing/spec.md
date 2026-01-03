# site-publishing Specification

## Purpose
TBD - created by archiving change migrate-blog-to-fsharp-renderer. Update Purpose after archive.
## Requirements
### Requirement: F# Rendering Pipeline
The site SHALL render Markdown content using F# scripts with Giraffe.ViewEngine components, producing HTML that preserves current URLs, metadata, and feed structure. The visual layout and styling MAY evolve over time as governed by the active theme specifications.

#### Scenario: Posts rendered with preserved permalinks
- **WHEN** the renderer processes an existing Markdown post with front matter
- **THEN** it emits HTML whose permalink and canonical URL remain stable
- **AND** it preserves essential metadata (title, description, author, date)

### Requirement: Layouts converted to F# components
The F# Giraffe components SHALL render post and page layouts using topics for navigation and labeling instead of categories/tags.

#### Scenario: Layout uses topics in sidebar and post metadata
- **WHEN** the renderer builds index, post, and page views
- **THEN** the sidebar and post metadata sections display topic labels instead of categories/tags
- **AND** hovering a topic label shows its description via the HTML `title` attribute

### Requirement: Feed Generation in F#
The site SHALL regenerate RSS and Atom feeds using F# XML builders that mirror the current feed structure and content.

#### Scenario: RSS feed parity
- **WHEN** the F# feed generator runs
- **THEN** the produced RSS feed contains the same entries, ordering, and metadata as the Pretzel-generated feed.

#### Scenario: Atom feed parity
- **WHEN** the generator produces the Atom feed
- **THEN** its XML structure and entry data align with the existing Atom feed consumers expect.

### Requirement: Local Rendering Tooling
The project SHALL provide documented commands for authors to run the F# renderer locally using `run.ps1` (PowerShell) or `run.sh` (Bash) scripts. The project SHALL also provide `new-draft.ps1` (PowerShell) and `new-draft.sh` (Bash) scripts for creating new draft posts with proper front matter and file naming conventions.

#### Scenario: Local build command available
- **WHEN** a developer executes `./run.ps1` or `./run.sh`
- **THEN** the renderer outputs the full static site to `_site/` ready for review

#### Scenario: Local preview with serve option
- **WHEN** a developer executes `./run.ps1 -Serve` or `./run.sh --serve`
- **THEN** the site is built and served locally for browser preview

### Requirement: Draft creation script available
The new-draft scripts SHALL prompt for and require `topics` (list of topic IDs) and `keywords` in front matter; legacy `category`/`tags` SHALL NOT be required.

#### Scenario: Generated draft has valid front matter including topics and keywords
- **WHEN** a draft is created using `./new-draft.ps1` or `./new-draft.sh`
- **THEN** the file contains valid YAML front matter including `title`, `layout`, `published`, `topics`, and `keywords`
- **AND** the script fails with a helpful error if `topics` or `keywords` are missing

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

### Requirement: Topic Overview and Landing Pages
The site SHALL provide a topics overview page listing all configured topics with their names and descriptions, and SHALL provide a landing page per topic ID listing all posts assigned to that topic.

#### Scenario: Topics overview lists all topics with descriptions
- **WHEN** a reader visits `/topics/`
- **THEN** the page displays every topic from `_config.yml` `topics` with `name` and `description`

#### Scenario: Per-topic page lists assigned articles
- **WHEN** a reader visits `/topics/{id}/`
- **THEN** the page lists all posts whose front matter `topics` contains `{id}`
- **AND** the page header shows the topic `name` and `description`

### Requirement: Legacy Category/Tag Redirect Pages
The site SHALL render legacy category and tag pages as static HTML pages that perform a meta `http-equiv="refresh"` redirect to the most relevant topic page. Legacy categories and tags SHALL be sourced from `_config.yml` `topics` entries (`legacy_category` and `legacy_tags`), and these legacy pages SHALL be generated even if no posts currently include legacy `category`/`categories`/`tags` front matter. When no mapping exists, the redirect SHALL point to the topics overview page.

#### Scenario: Legacy category redirects to mapped topic
- **WHEN** a reader visits `/category/{category}/`
- **THEN** a meta refresh redirects to `/topics/{mapped-id}/`

#### Scenario: Legacy tag redirects to mapped topic
- **WHEN** a reader visits `/tag/{tag}/`
- **THEN** a meta refresh redirects to `/topics/{mapped-id}/`

#### Scenario: Fallback redirect to topics overview
- **WHEN** a reader visits a legacy page with no mapping
- **THEN** a meta refresh redirects to `/topics/`

