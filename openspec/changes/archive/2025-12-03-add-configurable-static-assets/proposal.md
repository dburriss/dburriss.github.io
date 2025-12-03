# Change: Add configurable static asset copying

## Why
The current `copyStaticAssets` function in `Renderer.fs` hardcodes four directories (`css`, `js`, `img`, `fonts`) as static assets. This prevents copying other files like `tkd/`, `CNAME`, or `sitemap.xml` without modifying code. A configurable `include` pattern list in `_config.yml` enables site authors to specify which files to copy without code changes.

## What Changes
- Add `Include: string list` field to `SiteConfig` in `Models.fs`
- Add `Microsoft.Extensions.FileSystemGlobbing` NuGet package to `SiteRenderer.fsproj`
- Parse `include` YAML sequence in `Parsing.fs` to populate the new config field
- Replace `copyStaticAssets` in `Renderer.fs` to use glob pattern matching; if `include` is empty, fall back to current behavior (`css/**`, `js/**`, `img/**`, `fonts/**`)
- Update `_config.yml` with default patterns: `css/**`, `js/**`, `img/**`, `fonts/**`, `tkd/**`, `CNAME`, `sitemap.xml`

## Impact
- Affected specs: `site-publishing`
- Affected code: `Models.fs`, `Parsing.fs`, `Renderer.fs`, `SiteRenderer.fsproj`, `_config.yml`
