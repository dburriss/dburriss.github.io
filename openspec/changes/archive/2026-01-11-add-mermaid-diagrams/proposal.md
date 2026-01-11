# Change: Add Mermaid diagram support

## Why
Technical content benefits significantly from visual diagrams - flowcharts, sequence diagrams, class diagrams, and architecture diagrams make complex concepts more accessible. Mermaid's text-based syntax allows diagrams to be versioned alongside code and content, integrated seamlessly into the markdown authoring workflow.

## What Changes
- Enable Markdig's built-in Diagrams extension to convert `mermaid` code blocks to `<pre class="mermaid">` elements
- Add client-side Mermaid.js library for SVG rendering 
- Create theme-aware initialization script that adapts diagrams to dark/light themes
- Add CSS styles for diagram containers, loading states, and error handling
- Implement conditional script loading - Mermaid.js only loads on pages containing diagrams
- Update build pipeline to copy Mermaid assets to output

## Constraints
- Client-side rendering only (preserves static site architecture)
- Conditional loading (no performance impact on pages without diagrams)  
- Theme compatibility (diagrams must adapt to existing dark/light theme system)
- Security considerations (use appropriate Mermaid security level)

## Impact
- Affected specs:
  - `openspec/specs/site-theme/spec.md` (theme integration and CSS styling)
  - `openspec/specs/site-publishing/spec.md` (asset copying and script loading)
  - New capability: `markdown-diagrams` (Mermaid processing, rendering, and theming)
- Affected areas (implementation stage):
  - `src/SiteRenderer/Parsing.fs` (enable Diagrams extension)
  - `src/SiteRenderer/Layouts.fs` (conditional script loading)
  - `src/SiteRenderer/Renderer.fs` (asset copying)
  - `js/vendor/` (add Mermaid.js library)
  - `js/` (add initialization script)
  - `css/site.css` (diagram styling)

## Open Questions
- Should we include error handling for invalid diagram syntax?
- What security level should be used for Mermaid (strict vs loose)?
- Should we support additional diagram types beyond Mermaid (Nomnoml, PlantUML)?