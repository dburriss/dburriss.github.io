# Implementation Tasks

## Phase 1: Core Diagram Processing
- [x] Enable Markdig Diagrams extension in both markdown pipelines in `src/SiteRenderer/Parsing.fs`
- [x] Test that mermaid code blocks convert to `<pre class="mermaid">` elements (converted to `<div class="mermaid">`)
- [x] Verify other code blocks remain unchanged

## Phase 2: Client-Side Assets  
- [x] Download Mermaid.js v10.x library to `js/vendor/mermaid.min.js`
- [x] Create `js/mermaid-init.js` with theme-aware initialization
- [x] Implement theme switching detection and diagram re-rendering
- [x] Add error handling for invalid diagram syntax

## Phase 3: Styling and UI
- [x] Add CSS styles for `.mermaid` containers in `css/site.css`
- [x] Style loading states (show raw source while processing)
- [x] Add dark mode theme variables and colors
- [x] Style error states for invalid diagrams
- [x] Ensure SVG responsiveness and container overflow handling

## Phase 4: Build Integration
- [x] Update `src/SiteRenderer/Layouts.fs` for conditional script loading
- [x] Implement diagram detection in HTML content
- [x] Load Mermaid scripts only when diagrams are present
- [x] Update `src/SiteRenderer/Renderer.fs` to copy new JavaScript assets (already handled by `js/**` pattern)
- [x] Test that pages without diagrams don't load Mermaid.js

## Phase 5: Testing and Validation
- [x] Create test content with various diagram types (flowchart, sequence, class)
- [x] Test diagram rendering in posts, pages, and notes
- [x] Verify theme switching updates diagram colors correctly
- [x] Test multiple diagrams on single page
- [x] Validate graceful handling of invalid syntax
- [x] Check performance impact (library only loads when needed)
- [x] Build and serve tests to ensure everything works end-to-end

## Phase 6: Documentation
- [x] Update usage documentation with diagram examples (spec covers this)
- [x] Document theme customization for diagrams (covered in js/mermaid-init.js)
- [x] Add troubleshooting guide for common issues (covered in design.md)