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
├── _site/            # Generated output (gitignored)
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
└── run.sh            # Build script (Linux/macOS)
```

## Writing Posts

Create a new Markdown file in `_posts/` with the naming format `YYYY-MM-DD-slug.md`.

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

## CI/CD

The site is automatically built and deployed via AppVeyor when changes are pushed to the `source` branch. The generated site is pushed to the `master` branch for GitHub Pages hosting.

## Legacy

The `_layouts/`, `_includes/`, and `_plugins/` directories contain the legacy Pretzel/Liquid templates that were used before the F# migration. These are kept for reference but are no longer used in the build process.
