# Design: Configurable Static Asset Copying

## Context
The SiteRenderer currently copies static assets from four hardcoded directories. The site has additional static content (`tkd/`, `CNAME`, `sitemap.xml`) that must be manually added to `copyStaticAssets` whenever new static content is introduced. This change introduces glob-pattern-based configuration to make static asset copying flexible and author-controlled.

## Goals / Non-Goals
- **Goals:**
  - Allow authors to specify static asset patterns in `_config.yml`
  - Support standard glob syntax (`**`, `*`, exact filenames)
  - Maintain backward compatibility when no patterns are configured
- **Non-Goals:**
  - Exclude/ignore patterns (can be added later if needed)
  - Watching for file changes (out of scope for static generation)

## Decisions

### Decision: Use `Microsoft.Extensions.FileSystemGlobbing`
- **What:** Use the official .NET globbing library instead of custom regex or third-party packages.
- **Why:** Mature, well-tested, included in .NET SDK, supports standard glob syntax (`**/*.css`, `dir/**`, exact paths).
- **Alternatives considered:**
  - Custom regex matching: More brittle, harder to maintain
  - DotNet.Glob: External dependency, less integrated
  - Directory enumeration per-pattern: Simpler but less flexible

### Decision: `include` as YAML sequence in `_config.yml`
- **What:** Add an `include` key containing a list of glob patterns.
- **Why:** Consistent with Jekyll/Pretzel convention, familiar to static site authors.
- **Format:**
  ```yaml
  include:
    - css/**
    - js/**
    - img/**
    - fonts/**
    - tkd/**
    - CNAME
    - sitemap.xml
  ```

### Decision: Default to current behavior when `include` is empty
- **What:** If `include` is empty or absent, use `["css/**", "js/**", "img/**", "fonts/**"]`.
- **Why:** Ensures existing sites without explicit config continue to work.

### Decision: Pass config to `copyStaticAssets`
- **What:** Modify `copyStaticAssets` signature to accept `SiteConfig` (or just the include list).
- **Why:** Keeps the function pure and testable by receiving patterns as input.

## Risks / Trade-offs
- **Risk:** Glob patterns could accidentally include unwanted files (e.g., `**/*`).
  - **Mitigation:** Document recommended patterns; exclude `_*` and `.git` by default in implementation.
- **Risk:** Performance on large pattern lists.
  - **Mitigation:** `FileSystemGlobbing` is optimized; typical blog has <10 patterns.

## Migration Plan
1. Add the new package reference and model field.
2. Update parsing to populate `Include`.
3. Replace `copyStaticAssets` implementation.
4. Update `_config.yml` with explicit patterns.
5. Test that build copies all expected files.

No rollback needed; this is additive and backward-compatible.

## Open Questions
None at this time.
