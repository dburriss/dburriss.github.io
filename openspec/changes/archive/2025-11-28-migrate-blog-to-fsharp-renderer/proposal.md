# Change: Rebuild blog rendering with F# and Giraffe

## Why
The blog currently relies on Pretzel and Liquid templates to render Markdown into HTML. Pretzel limits customization, requires Windows-centric tooling, and makes it difficult to evolve layouts and feeds programmatically. Replacing Pretzel with F# scripts using Giraffe.ViewEngine gives us full control over HTML generation while preserving the existing site structure and URLs.

## What Changes
- Replace Pretzel processing of posts, pages, and feeds with F# scripts that emit HTML using Giraffe.ViewEngine while maintaining current layout structure.
- Convert layout and include templates into F# modules so the final markup matches today’s output but no Liquid templates remain in the build process.
- Regenerate RSS and Atom feeds using F# XML builders to match existing feeds.
- Provide command-line tooling to run the new renderer locally for authoring and validation.

## Impact
- Affected specs: site-publishing
- Affected code: `_layouts/`, `_includes/`, Pretzel configuration/scripts, new F# renderer scripts, feed templates, documentation outlining the new build process.
