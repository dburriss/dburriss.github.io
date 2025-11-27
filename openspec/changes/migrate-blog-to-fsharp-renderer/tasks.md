## 1. Analysis
- [ ] 1.1 Inventory current Pretzel layouts, includes, and feed templates to document required outputs.
- [ ] 1.2 Define metadata requirements from front matter and `_config.yml` to ensure parity.

## 2. Renderer Implementation
- [ ] 2.1 Build F# modules that parse Markdown content and YAML front matter.
- [ ] 2.2 Recreate layout/partial structure as Giraffe.ViewEngine components matching existing HTML.
- [ ] 2.3 Implement RSS and Atom feed generation with F# XML builders mirroring current feeds.
- [ ] 2.4 Add command-line entry (e.g., `dotnet fsi` or `dotnet run`) to generate the static site locally.

## 3. Migration
- [ ] 3.1 Swap Pretzel and Liquid pipeline for the new F# renderer in build scripts.
- [ ] 3.2 Remove Pretzel-specific configuration and dependencies once parity confirmed.
- [ ] 3.3 Update documentation describing new workflows for authors and maintainers.

## 4. Validation
- [ ] 4.1 Compare generated HTML, RSS, and Atom output against current production artifacts to confirm parity.
- [ ] 4.2 Manually verify canonical URLs, navigation, and styling.
- [ ] 4.3 Gather feedback from reviewers and adjust renderer gaps before cutover.
