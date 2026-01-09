# Design: Mermaid Diagram Support

## Architecture Overview

```
Markdown Input          Markdig Processing       HTML Output           Client-Side Rendering
─────────────          ─────────────────       ────────────          ─────────────────────
```mermaid      ───►   Diagrams Extension  ───► <pre class="mermaid"> ───► Mermaid.js ───► SVG
graph TD                recognizes mermaid      ...diagram source...       renders diagram
  A-->B                 code blocks             </pre>
```

## Processing Flow

### 1. Build-Time Processing
- **Markdig Diagrams Extension**: Converts mermaid code blocks to `<pre class="mermaid">` elements
- **Asset Pipeline**: Copies Mermaid.js library and initialization script to output
- **Conditional Loading**: Layout detects presence of diagrams in HTML content

### 2. Runtime Processing  
- **Library Loading**: Mermaid.js loads only on pages with diagrams
- **Theme Detection**: Initialization script reads current theme from `data-theme` attribute
- **Diagram Rendering**: Mermaid converts text to SVG with theme-appropriate colors
- **Theme Switching**: MutationObserver re-renders diagrams when theme changes

## Integration Points

### Markdig Pipeline Integration
Located in `src/SiteRenderer/Parsing.fs`:
- Add `.UseDiagrams()` to both `markdownPipeline` and `createContextAwarePipeline`
- Extension is already available in Markdig 0.31.0+ (current version)
- No additional dependencies required

### Layout Integration
Located in `src/SiteRenderer/Layouts.fs`:
- Detect `class="mermaid"` presence in HTML content
- Conditionally include Mermaid scripts in page head
- Follow existing pattern used by highlight.js for conditional loading

### Theme System Integration
- Leverage existing `data-theme` attribute system
- Map light/dark themes to appropriate Mermaid color schemes
- Automatically re-render diagrams when theme changes
- Use CSS custom properties for consistent theming

## Security Considerations

### Mermaid Security Levels
- **loose**: Allows HTML in labels, clickable elements (recommended for trusted content)
- **strict**: Blocks HTML in labels (more secure but limits functionality)
- **Decision**: Use 'loose' for author-controlled content, consider 'strict' if user-generated content is added

### Content Security Policy
- Mermaid generates inline SVG (no CSP issues)
- No external requests made by Mermaid
- No script evaluation beyond diagram parsing

## Performance Considerations

### Library Size
- Mermaid.js: ~500KB minified
- Only loaded on pages containing diagrams
- Cached across page visits
- Consider CDN with local fallback in future

### Rendering Performance
- Diagrams render after page load (no blocking)
- Large/complex diagrams may cause layout shift
- Raw source visible during processing (intentional UX)

### Build Performance
- No impact on build time (processing is client-side)
- Minimal HTML size increase (just `<pre>` elements)

## Error Handling Strategy

### Invalid Syntax
- Mermaid shows error message in diagram container
- CSS styles error state with appropriate colors
- Raw source remains visible for debugging

### Library Load Failures
- Graceful degradation: raw source visible
- No JavaScript errors thrown
- Consider local fallback for CDN scenarios

## Theme Integration Details

### Color Mapping
```javascript
light theme → {
  primaryColor: '#6200ee',
  background: 'transparent', 
  mainBkg: '#fff',
  lineColor: '#333'
}

dark theme → {
  primaryColor: '#bb86fc',
  background: 'transparent',
  mainBkg: '#1e1e1e', 
  lineColor: '#5e5e5e'
}
```

### CSS Variables
- Leverage existing CSS custom properties
- Consistent with site's color scheme
- Dark mode handled via `html[data-theme="dark"]` selectors

## Future Extension Points

### Server-Side Rendering
- Pre-render diagrams during build for better SEO
- Requires Node.js in build pipeline
- Would generate static SVG files

### Additional Diagram Types
- Markdig supports Nomnoml (enable with `.UseNomnoml()`)
- PlantUML via external rendering service
- Custom diagram types through Markdig extensions

### Caching Strategies
- Cache rendered SVGs in localStorage
- Detect unchanged diagram source to skip re-rendering
- Version cache based on diagram content hash