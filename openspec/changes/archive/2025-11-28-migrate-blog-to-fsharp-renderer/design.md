## Context
The existing build uses Pretzel to parse Markdown posts and Liquid templates to assemble HTML layouts, RSS, and Atom feeds. Pretzel requires Windows tooling and constrains custom logic. The requested change replaces Pretzel with bespoke F# scripts leveraging Giraffe.ViewEngine for HTML and .NET XML builders for feeds while keeping all URLs and visual layouts unchanged.

## Goals / Non-Goals
- Goals: Preserve existing site structure, URLs, metadata, and styling; express layouts and feeds entirely in F#; provide a reproducible local build using the new renderer.
- Non-Goals: Alter site design, change navigation/content taxonomy, or introduce new deployment tooling.

## Decisions
- Decision: Implement a rendering pipeline in F# that reads Markdown (with front matter) and outputs HTML using Giraffe.ViewEngine views. *Rationale*: Offers strong composability, type safety, and full control over markup.
- Decision: Translate each Liquid layout/include into an F# module mirroring the current structure to minimize diff noise. *Rationale*: Ensures the generated HTML stays identical while removing Liquid dependencies.
- Decision: Regenerate RSS and Atom feeds via F# XML builders drawing from the same metadata as HTML pages. *Rationale*: Maintains feed compatibility while consolidating the rendering stack.
- Decision: Perform full site regeneration on every build rather than incremental updates. *Rationale*: Simplifies the renderer and matches current expectations without significant build-time penalties.
- Decision: Reimplement Pretzel category cloud and related posts behavior within the F# pipeline. *Rationale*: Preserves existing UX features that rely on current Pretzel plugins.
- Decision: Copy static assets (CSS/JS/images/fonts) directly from their source directories without additional processing. *Rationale*: Matches the current deployment behavior and avoids unnecessary complexity.

## Alternatives Considered
- Customize Pretzel with additional plugins. Rejected because it retains Liquid templates and tooling constraints, limiting flexibility.
- Adopt a different static site generator (e.g., Statiq, Hugo). Rejected to honor the requirement for Giraffe.ViewEngine and direct F# control.

## Risks / Trade-offs
- Risk: Re-implementing layouts may introduce subtle differences in HTML structure. → Mitigation: Snapshot existing output and use automated comparisons during validation.
- Risk: Feed consumers may be sensitive to formatting changes. → Mitigation: Diff new RSS/Atom XML against current feeds and test with validators.
- Risk: Contributors unfamiliar with F# may face learning curve. → Mitigation: Provide scripts, documentation, and simple CLI entry points masking complexity.

## Migration Plan
1. Build the F# renderer alongside Pretzel, generating outputs into a separate directory.
2. Compare outputs (HTML, RSS, Atom) and adjust until parity is achieved, including validating category cloud and related post logic.
3. Update build tooling to invoke the F# renderer, copy static assets unchanged, and remove Pretzel dependencies.
4. Announce cutover, document new workflows, and monitor production after deployment.

## Open Questions
- None at this time.
