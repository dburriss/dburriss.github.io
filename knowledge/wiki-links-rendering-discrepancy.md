# Wiki Links Rendering Discrepancy

Summary: The enhanced wiki-link feature works in tests but the rendered site still shows literal `[[...]]` text instead of HTML spans/links. Parsing clearly runs (warnings appear), yet the HTML output is not transformed as expected.

## Context
During the enhance-wiki-links implementation, all unit tests passed for parsing and rendering. However, when rendering the actual site, Markdown containing wiki links like `[[Pure Function]]` remains unchanged in the final HTML.

## Successful implementation
- Enhanced `WikiLinkInline` type to carry target and display text, including pipe syntax support (`[[display|target]]`)
- Parser updated to recognize pipe syntax and produce the enhanced inline
- HTML renderer emits appropriate `<span>` or `<a>` elements for resolved/unresolved links
- Comprehensive tests cover parsing and HTML rendering; all tests pass

## Critical fix discovered
- Adding `OpeningCharacters` to `WikiLinkParser` was required so Markdig properly triggers the parser when encountering `[[` in the pipeline
- Without `OpeningCharacters`, the site pipeline may not invoke the parser even though direct or test-specific parsing succeeds

## Remaining discrepancy
- In the site build, wiki links such as `[[Pure Function]]` are rendered verbatim in HTML
- Evidence shows parsing occurs: "Unresolved wiki link" warnings are logged during rendering, proving the extension runs
- Despite warnings, the final HTML still contains literal `[[Pure Function]]` text rather than the expected `<span class="wikilink">Pure Function</span>` or `<a>` output

## Potential causes
- Extension registration: The site’s Markdig pipeline may not include the wiki-link extension, or it’s added in the wrong order
- Caching: Stale content in `_site/` or a caching layer may serve old HTML (clear output and rerender)
- Dependency/version mismatch: Tests and site may use different Markdig or extension assemblies/versions
- Environment differences: Debug vs Release builds, trimming, or runtime differences could alter pipeline behavior
- Alternate rendering path: The site may use a different Markdown processing path than tests
- HTML encoding step: Another processor may post-process HTML/Markdown and escape or bypass inline transformations
- Build artifacts: Old `bin/obj` outputs used by the renderer even after source changes

## Current status
- Core enhancement is complete and validated by tests (parsing + HTML rendering)
- Site-specific rendering mismatch remains and requires investigation

## Files involved
- `WikiLinkExtension.fs` (extension registration)
- `Parsing.fs` (parser implementation, including `OpeningCharacters`)
- `Renderer.fs` (HTML rendering for wiki links)
- Test files (unit tests for parser and renderer)

## Evidence
- Logs show: "Unresolved wiki link" warnings during site rendering
- Final HTML contains literal `[[Pure Function]]` instead of transformed HTML elements

## Investigation checklist (next steps)
- Verify extension registration in the site’s Markdig pipeline (e.g., in `Renderer.fs`)
  - Ensure the wiki-link extension is added and ordered correctly relative to other extensions
  - Confirm `WikiLinkParser.OpeningCharacters = new[] { '[' }` (or equivalent) is set
- Clean and rebuild
  - `dotnet clean` and rebuild the renderer
  - Delete `_site/` and rerun `./render.sh`
  - Ensure `dotnet serve` is serving freshly generated content
- Confirm consistent dependencies
  - Inspect package versions used by tests vs site runtime (Markdig and the wiki-link extension assembly)
  - Reinstall/restore packages to ensure consistency
- Validate the exact pipeline used by the site
  - Log the list/order of extensions added to the pipeline at startup
  - Run a minimal content sample through the same pipeline the site uses and diff the output
- Inspect rendering flow
  - Check if the site has a second pass or sanitizer that re-encodes/escapes inline HTML
  - Ensure the wiki-link renderer is registered for the correct inline type
- Add an integration test for the SiteRenderer
  - Construct a test that invokes the actual site pipeline/build to render a page containing `[[Pure Function]]` and assert on HTML output

## Decision log
- Enhancement completed and passing tests
- Critical fix applied: `OpeningCharacters` added to `WikiLinkParser`
- Discrepancy remains only in site rendering; likely configuration, caching, or environment issue

## References
- Files: `WikiLinkExtension.fs`, `Parsing.fs`, `Renderer.fs`, test files
- Markdig extension behavior depends on proper parser configuration (including `OpeningCharacters`) and pipeline registration
