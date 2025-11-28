# Project Context

## Purpose
This repository stores the source for Devon Burriss' static blog. The goal is to publish essays and tutorials focused on software development, functional programming, testing practices, and technical leadership. Contributors manage Markdown content and shared theme assets that the F# SiteRenderer transforms into the production site.

## Tech Stack
- F# 10 SiteRenderer with Giraffe.ViewEngine for HTML generation
- .NET 10 runtime
- Markdown posts with YAML front matter for metadata and routing
- Bootstrap, custom Less/CSS, and Highlight.js for styling and code samples
- jQuery-based enhancements and local JavaScript utilities under `js/`
- GitHub Actions for CI/CD

## Project Conventions

### Code Style
- Posts live in `_posts/` and follow the `YYYY-MM-DD-slug.md` naming convention with YAML front matter describing `layout`, `title`, `tags`, `permalink`, and `excerpt_separator`.
- Drafts remain under `_drafts/` until published; move to `_posts/` and set `published: true` once finalized.
- Favor semantic Markdown headings over raw HTML; wrap lines naturally around ~100 characters but prioritize readability over strict width limits.
- Keep styles, scripts, and images in their respective directories (`css/`, `js/`, `img/`, `fonts/`) and avoid embedding inline assets inside Markdown posts.
- F# layouts are defined in `src/SiteRenderer/Layouts.fs` using Giraffe.ViewEngine.

### Architecture Patterns
- The F# SiteRenderer renders the site using Giraffe.ViewEngine components, combining front matter metadata with `_config.yml` settings.
- Layouts and partials are implemented as F# functions in `src/SiteRenderer/Layouts.fs`.
- Build and serve scripts (`run.ps1`, `run.sh`) orchestrate local development workflows.
- The `_site/` directory is a **git submodule** pointing to `dburriss/dburriss.github.io.git` for GitHub Pages hosting.
- The renderer wipes `_site/` during build; publish scripts reinitialize the submodule automatically.
- Generated artifacts in `_site/` should not be committed to the `source` branch (they go to `master` of the submodule).

### Testing Strategy
- No automated tests currently; CI validates that the F# build and site generation complete successfully.
- Preview changes locally with `./run.ps1 -Serve` or `./run.sh --serve` to confirm layout, links, and code formatting before pushing.
- Manually spot-check changes in multiple viewports during local preview sessions.

### Git Workflow
- Author content and theme changes on the `source` branch; GitHub Actions builds this branch and pushes the generated site to `master` of `dburriss.github.io` for hosting.
- Use short-lived feature branches for substantial updates, rebasing or squashing before merging into `source`.
- Write descriptive commit messages referencing the post or asset touched. Add `[skip ci]` when commits should not trigger deployment.
- The `_site` submodule is reinitialized during publish since the renderer clears the output directory.

## Domain Context
The blog explores professional software development topics, with emphasis on .NET tooling, F#, testing strategies, architecture, telemetry, and team leadership. Expect terminology from functional programming, DevOps observability, and agile process discussions.

## Important Constraints
- `_config.yml` contains site configuration including `url` and `is_production` settings.
- Production analytics and comments depend on configured Google Analytics and Disqus identifiers.
- **`DEPLOY_TOKEN` secret required**: GitHub Actions needs a Personal Access Token with `repo` scope to push to the `dburriss.github.io` repository (stored as `DEPLOY_TOKEN` secret).

## External Dependencies
- GitHub Actions for building and deploying from `source` to the published branch.
- GitHub Pages hosting via the `master` branch of `dburriss/dburriss.github.io.git`.
- Disqus (`devonburriss`) for comments and Google Analytics (`UA-45750611-2`).
- Bundled Bootstrap, jQuery, and Highlight.js libraries that power the theme and interactions.

## Local Development

### Initial Setup
```bash
# Clone with submodules
git clone --recurse-submodules <repo-url>

# Or initialize submodules after clone
git submodule update --init
```

### Build and Preview
```bash
# PowerShell
./run.ps1 -Serve

# Bash
./run.sh --serve
```

### Publish (manual deployment)
```bash
# PowerShell
./publish.ps1

# Bash
./publish.sh

# Dry-run to see what would be published
./publish.ps1 -DryRun
./publish.sh --dry-run
```

The publish scripts handle:
1. Reinitializing the `_site` submodule (wiped by renderer)
2. Checking out the `master` branch
3. Committing and force-pushing changes to `origin/master`
