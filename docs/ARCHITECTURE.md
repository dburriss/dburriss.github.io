# Architecture

This document provides a high-level overview of the architecture for the blog.

## Core Technologies

- **Static Site Generator:** Custom F# application (.NET 10).
- **Templating:** [Giraffe.ViewEngine](https://github.com/giraffe-fsharp/Giraffe.ViewEngine) (HTML DSL).
- **Markdown Parsing:** [Markdig](https://github.com/xoofx/markdig).
- **Search:** [FlexSearch](https://github.com/nextapps-de/flexsearch) (client-side) with a custom build-time indexer using [Bun](https://bun.sh/).
- **Styling:** Custom CSS and Highlight for syntax highlighting.

## System Overview

The blog is a static site generated from Markdown files. The generation process has two distinct phases:

1.  **Site Rendering (F#):**
    - Parses Markdown files from `_posts/` using `Markdig` and `YamlDotNet`.
    - Generates HTML pages using `Giraffe.ViewEngine` layouts.
    - Generates RSS/Atom feeds.
    - Produces a raw `docs.json` containing searchable content.
    - Copies static assets (`css`, `js`, `img`, etc.) to the output directory.

2.  **Search Indexing (TypeScript/Bun):**
    - Reads the `docs.json` produced by the F# renderer.
    - Builds a pre-computed `FlexSearch` index.
    - Exports the index to `index.json` and `manifest.json` for fast client-side loading.

## Project Structure

### Source (`/`)

- `_posts/`: Markdown content files (posts).
- `_drafts/`: Draft posts.
- `src/SiteRenderer/`: The F# static site generator source code.
- `scripts/`: Helper scripts (e.g., search index generation).
- `_site/`: The generated static output (Git submodule).
- `_config.yml`: Main site configuration (metadata, navigation, included assets).

### Site Renderer (`src/SiteRenderer/`)

The F# application is structured into logical modules:

- **Program.fs**: CLI entry point and orchestration.
- **Models.fs**: Domain types (`Post`, `Page`, `SiteConfig`).
- **Parsing.fs**: Markdown and YAML parsing logic.
- **Layouts.fs**: HTML page templates defined as F# functions.
- **Renderer.fs**: Core build pipeline logic.
- **Search.fs**: Generates the intermediate `docs.json` for the search indexer.
- **Feeds.fs**: RSS/Atom feed generation.

## Search Architecture

The search functionality is "static-dynamic":

1.  **Build Time:** The `scripts/build-search-index.ts` script (run via Bun) consumes the content extracted by the F# renderer and creates a memory-efficient `FlexSearch` index.
2.  **Runtime (Client):** The browser fetches `index.json` and initializes `FlexSearch` with this pre-built index, allowing for instant, offline-capable search without a backend API.

## Deployment

The site is deployed via GitHub Actions:
1.  Code is pushed to the `source` branch.
2.  The F# app builds and renders the site.
3.  The search indexer runs.
4.  The content of `_site/` is pushed to the `master` branch of the `dburriss.github.io` repository (linked as a submodule).
