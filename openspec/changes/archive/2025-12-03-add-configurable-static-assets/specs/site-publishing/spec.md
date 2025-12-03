# site-publishing Spec Delta

## ADDED Requirements

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
