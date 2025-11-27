## 1. Analysis
- [x] 1.1 Inventory current Pretzel layouts, includes, and feed templates to document required outputs.
- [x] 1.2 Define metadata requirements from front matter and `_config.yml` to ensure parity.

## 2. Renderer Implementation
- [x] 2.1 Build F# modules that parse Markdown content and YAML front matter.
  - `Models.fs`: Data types for SiteConfig, FrontMatter, PageMeta, ContentItem, SiteIndex, RenderContext
  - `Parsing.fs`: YAML parsing, Markdown processing with Markdig, front matter extraction
- [x] 2.2 Recreate layout/partial structure as Giraffe.ViewEngine components matching existing HTML.
  - `Layouts.fs`: Complete implementation with:
    - Head/meta tags with SEO and social sharing
    - Navigation bar
    - Post header with tags and author info
    - Sidebar with categories, tags, about section
    - Footer with scripts
    - Index page with featured post and paginated list
    - Category and tag listing pages
    - Comments (Disqus) and analytics (Google Analytics) support
- [x] 2.3 Implement RSS and Atom feed generation with F# XML builders mirroring current feeds.
  - `Feeds.fs`: RSS 2.0 and Atom feed generation using System.Xml.Linq
- [x] 2.4 Add command-line entry (e.g., `dotnet fsi` or `dotnet run`) to generate the static site locally.
  - `Renderer.fs`: Site rendering pipeline with post/page loading, index building, and output generation
  - `Program.fs`: CLI entry point with --source, --output, --config, --posts-per-page options

## 3. Migration
- [ ] 3.1 Swap Pretzel and Liquid pipeline for the new F# renderer in build scripts.
- [ ] 3.2 Remove Pretzel-specific configuration and dependencies once parity confirmed.
- [ ] 3.3 Update documentation describing new workflows for authors and maintainers.

## 4. Validation
- [ ] 4.1 Compare generated HTML, RSS, and Atom output against current production artifacts to confirm parity.
- [ ] 4.2 Manually verify canonical URLs, navigation, and styling.
- [ ] 4.3 Gather feedback from reviewers and adjust renderer gaps before cutover.
