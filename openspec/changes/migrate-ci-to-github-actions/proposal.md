# Change: Migrate CI from AppVeyor to GitHub Actions and Remove Legacy Pretzel Files

## Why
The blog has migrated from Pretzel to a custom F# SiteRenderer, making AppVeyor CI configuration and Pretzel-related files obsolete. GitHub Actions provides better integration with the GitHub repository hosting and modern CI/CD features. The `_layouts/` and `_includes/` directories are also obsolete since the F# renderer uses its own `Layouts.fs` module.

## What Changes
- **REMOVED** `appveyor.yml` - Legacy CI configuration
- **REMOVED** `pretzel.cake` - Cake build script for Pretzel
- **REMOVED** `pretzel.ps1` - PowerShell bootstrapper for Cake/Pretzel
- **REMOVED** `_plugins/` directory - Pretzel plugin DLLs and readme
- **REMOVED** `_layouts/` directory - Liquid templates (replaced by F# Layouts.fs)
- **REMOVED** `_includes/` directory - Liquid partials (replaced by F# Layouts.fs)
- **ADDED** `.github/workflows/deploy.yml` - GitHub Actions workflow for building and deploying the site
- **ADDED** `publish.ps1` / `publish.sh` - Scripts to push `_site` to master branch (testable locally)

## Impact
- Affected specs: `site-publishing` (MODIFIED to reflect new CI tooling)
- Affected code: CI/CD pipeline, repository root files
- **No breaking changes** to site content or rendering
- Developers can use `run.ps1` or `run.sh` for local builds
- Developers can use `publish.ps1` or `publish.sh` to test deployment locally before CI runs
