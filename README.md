# Devon Burriss' Blog

A personal blog built with F# and Giraffe.ViewEngine.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [dotnet-serve](https://github.com/natemcmaster/dotnet-serve) (for local preview)

Install dotnet-serve:
```bash
dotnet tool install dotnet-serve
```

## Quick Start

### Linux/macOS

```bash
# Build and generate site
./run.sh

# Build and serve locally
./run.sh --serve

# Build in debug mode and serve on a custom port
./run.sh --serve --debug --port 5000
```

### Windows (PowerShell)

```powershell
# Build and generate site
./run.ps1

# Build and serve locally
./run.ps1 -Serve

# Build in debug mode and serve on a custom port
./run.ps1 -Serve -Debug -Port 5000
```

## Project Structure

```
.
├── _posts/           # Blog posts in Markdown with YAML front matter
├── _drafts/          # Draft posts (not published)
├── _config.yml       # Site configuration
├── _site/            # Generated output (git submodule -> dburriss.github.io)
├── src/SiteRenderer/ # F# static site generator
│   ├── Models.fs     # Data types
│   ├── Parsing.fs    # Markdown and YAML parsing
│   ├── Layouts.fs    # HTML templates using Giraffe.ViewEngine
│   ├── Feeds.fs      # RSS and Atom feed generation
│   ├── Renderer.fs   # Site rendering pipeline
│   └── Program.fs    # CLI entry point
├── css/              # Stylesheets
├── js/               # JavaScript files
├── img/              # Images
├── fonts/            # Web fonts
├── run.sh            # Build script (Linux/macOS)
├── run.ps1           # Build script (Windows)
├── publish.sh        # Deploy script (Linux/macOS)
└── publish.ps1       # Deploy script (Windows)
```

## Writing Posts

Create a new Markdown file in `_posts/` with the naming format `YYYY-MM-DD-slug.md`.

### Creating Drafts

Use the interactive scripts to create new draft posts in `_drafts/`:

#### Linux/macOS

```bash
./new-draft.sh
```

#### Windows (PowerShell)

```powershell
./new-draft.ps1
```

The scripts will:
- Prompt for title, subtitle, category, tags, and header image
- Automatically generate a URL-safe slug from the title
- Create a file with today's date prefix (e.g., `2025-12-03-my-post-title.md`)
- Generate complete YAML front matter
- Optionally open the file in your default editor

To skip opening the editor, use:
```bash
./new-draft.sh --no-editor
# or
./new-draft.ps1 -NoEditor
```

### Front Matter

Each post should have YAML front matter at the top:

```yaml
---
layout: post
title: "Your Post Title"
subtitle: "An optional subtitle"
date: 2024-01-15
author: "Devon Burriss"
header-img: "img/backgrounds/your-header.jpg"
social-img: "img/posts/your-social-image.jpg"
tags:
  - Tag1
  - Tag2
categories:
  - Category
comments: true
---
```

### Front Matter Fields

| Field | Required | Description |
|-------|----------|-------------|
| `layout` | No | Layout template (default: `post`) |
| `title` | Yes | Post title |
| `subtitle` | No | Subtitle shown below title |
| `date` | No | Post date (extracted from filename if not specified) |
| `author` | No | Author name (default: site author) |
| `header-img` | No | Header background image |
| `social-img` | No | Image for social media sharing |
| `tags` | No | List of tags |
| `categories` | No | List of categories |
| `comments` | No | Enable/disable Disqus comments (default: true) |
| `published` | No | Set to `false` to exclude from build |
| `permalink` | No | Custom URL path |

## Configuration

Edit `_config.yml` to customize site settings:

```yaml
# Site settings
title: Your Blog Title
description: "Blog description"
author: "Your Name"
url: "https://yourdomain.com/"

# Social
twitter_username: YourTwitter
github_username: YourGitHub
disqus_username: your-disqus-shortname

# Analytics
google_tracking_id: UA-XXXXXXXX-X
```

## Renderer Options

The SiteRenderer supports these command-line options:

```
--source <path>      Source directory (default: current directory)
--output <path>      Output directory (default: _site)
--config <path>      Path to _config.yml (default: <source>/_config.yml)
--posts-per-page     Number of posts per index page (default: 10)
--help               Show help message
```

## Development

### Building the Renderer

```bash
dotnet build ./src/SiteRenderer/SiteRenderer.fsproj
```

### Formatting Code

```bash
dotnet fantomas ./src/SiteRenderer/
```

## Deployment

### CI/CD with GitHub Actions

The site is automatically built and deployed via GitHub Actions when changes are pushed to the `source` branch. The workflow:

1. Checks out the `source` branch with submodules
2. Builds the F# SiteRenderer
3. Generates the static site into `_site/`
4. Pushes the generated content to the `master` branch of `dburriss.github.io` for GitHub Pages hosting

### Repository Setup

The `_site/` directory is a **git submodule** pointing to `dburriss/dburriss.github.io.git`. This is where the generated site is published.

**Required GitHub Secret:**

A `DEPLOY_TOKEN` secret must be configured in the repository settings. This should be a Personal Access Token (PAT) with `repo` scope to allow pushing to the `dburriss.github.io` repository.

### Manual Deployment

You can publish the site manually using the publish scripts:

```bash
# Linux/macOS
./publish.sh

# Windows (PowerShell)
./publish.ps1

# Dry-run to see what would be published (no changes made)
./publish.sh --dry-run
./publish.ps1 -DryRun
```

The publish scripts will:
1. Reinitialize the `_site` submodule if needed (the renderer wipes the directory)
2. Checkout the `master` branch
3. Stage and commit all changes
4. Force-push to `origin/master`

### Submodule Setup

If cloning the repository fresh:

```bash
git clone --recurse-submodules https://github.com/dburriss/dburriss.github.io.git
# Or if already cloned:
git submodule update --init
```

### Troubleshooting Submodule Issues

If you encounter this error when running `./publish.sh`:

```
fatal: destination path '/home/deebo/Projects/blog/source/_site' already exists and is not an empty directory.
fatal: clone of 'git@github.com:dburriss/dburriss.github.io.git' into submodule path '/home/deebo/Projects/blog/source/_site' failed
```

This happens because the site renderer wipes the `_site` directory, removing the git repository, but the directory still contains generated files. To fix:

```bash
# Remove the generated files and properly initialize the submodule
rm -rf _site
git submodule update --init _site

# Then regenerate the site before publishing
./render.sh
```
