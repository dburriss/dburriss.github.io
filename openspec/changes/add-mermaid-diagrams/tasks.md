# Implementation Tasks

## Phase 1: Core Diagram Processing
- [ ] Enable Markdig Diagrams extension in both markdown pipelines in `src/SiteRenderer/Parsing.fs`
- [ ] Test that mermaid code blocks convert to `<pre class="mermaid">` elements
- [ ] Verify other code blocks remain unchanged

## Phase 2: Client-Side Assets  
- [ ] Download Mermaid.js v10.x library to `js/vendor/mermaid.min.js`
- [ ] Create `js/mermaid-init.js` with theme-aware initialization
- [ ] Implement theme switching detection and diagram re-rendering
- [ ] Add error handling for invalid diagram syntax

## Phase 3: Styling and UI
- [ ] Add CSS styles for `.mermaid` containers in `css/site.css`
- [ ] Style loading states (show raw source while processing)
- [ ] Add dark mode theme variables and colors
- [ ] Style error states for invalid diagrams
- [ ] Ensure SVG responsiveness and container overflow handling

## Phase 4: Build Integration
- [ ] Update `src/SiteRenderer/Layouts.fs` for conditional script loading
- [ ] Implement diagram detection in HTML content
- [ ] Load Mermaid scripts only when diagrams are present
- [ ] Update `src/SiteRenderer/Renderer.fs` to copy new JavaScript assets
- [ ] Test that pages without diagrams don't load Mermaid.js

## Phase 5: Testing and Validation
- [ ] Create test content with various diagram types (flowchart, sequence, class)
- [ ] Test diagram rendering in posts, pages, and notes
- [ ] Verify theme switching updates diagram colors correctly
- [ ] Test multiple diagrams on single page
- [ ] Validate graceful handling of invalid syntax
- [ ] Check performance impact (library only loads when needed)
- [ ] Build and serve tests to ensure everything works end-to-end

## Phase 6: Documentation
- [ ] Update usage documentation with diagram examples
- [ ] Document theme customization for diagrams
- [ ] Add troubleshooting guide for common issues