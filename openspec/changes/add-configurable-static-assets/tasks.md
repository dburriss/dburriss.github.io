# Tasks: Add Configurable Static Asset Copying

## 1. Model and Configuration
- [ ] 1.1 Add `Include: string list` field to `SiteConfig` in `Models.fs`
- [ ] 1.2 Add `Microsoft.Extensions.FileSystemGlobbing` package reference to `SiteRenderer.fsproj`

## 2. Parsing
- [ ] 2.1 Update `parseSiteConfig` in `Parsing.fs` to parse `include` YAML sequence into `SiteConfig.Include`

## 3. Static Asset Copying
- [ ] 3.1 Replace `copyStaticAssets` in `Renderer.fs` to use `Microsoft.Extensions.FileSystemGlobbing.Matcher`
- [ ] 3.2 Implement default patterns fallback (`css/**`, `js/**`, `img/**`, `fonts/**`) when `Include` is empty

## 4. Configuration Update
- [ ] 4.1 Update `_config.yml` with `include` patterns: `css/**`, `js/**`, `img/**`, `fonts/**`, `tkd/**`, `CNAME`, `sitemap.xml`

## 5. Integration
- [ ] 5.1 Update `Program.fs` to pass config (or include patterns) to `copyStaticAssets`

## 6. Validation
- [ ] 6.1 Run `dotnet build` to verify compilation
- [ ] 6.2 Run site generation and verify all static assets are copied to `_site/`
- [ ] 6.3 Verify `tkd/`, `CNAME`, and `sitemap.xml` appear in output
