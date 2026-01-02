## 1. Implementation
- [ ] 1.1 Add `about.md` page content (bio + avatar) with `permalink: /about/`
- [ ] 1.2 Add “About” link to site navigation (target `/about/`)
- [ ] 1.3 Update footer copyright link to `/about/`
- [ ] 1.4 Remove “About me” widget from sidebar rendering
- [ ] 1.5 Remove Bootstrap and existing Clean Blog styling/JS includes
- [ ] 1.6 Introduce tech-minimal site-wide theme styles (custom CSS and/or minimal CSS library like Skeleton)
- [ ] 1.7 Implement dark + light themes using Jade Pebble Morning palette tokens via CSS variables
- [ ] 1.8 Add theme toggle UI + persistence (localStorage), defaulting to `prefers-color-scheme`
- [ ] 1.9 Update homepage markup/styling to match `design/tech-min.png` structure (exclude search bar)
- [ ] 1.10 Update post/page/topic layouts for consistent typography, spacing, and dividers

## 2. Validation
- [ ] 2.1 Run `dotnet fantomas ./src/SiteRenderer/`
- [ ] 2.2 Run `dotnet build ./src/SiteRenderer/SiteRenderer.fsproj`
- [ ] 2.3 Run `./run.sh --serve` and manually spot-check:
  - [ ] Homepage (topics list + recent posts list; no search bar)
  - [ ] Post page (readability, code highlighting, spacing)
  - [ ] Topics overview and per-topic pages
  - [ ] About page at `/about/` (content + navigation link)
  - [ ] Light + dark modes (OS preference + manual toggle + persistence)
  - [ ] Mobile layout responsiveness
