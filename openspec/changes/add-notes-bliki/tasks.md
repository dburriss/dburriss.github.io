## 1. Notes Content Model and Routing
- [x] 1.1 Introduce `Note` domain type and front matter parsing
- [x] 1.2 Implement deterministic note slug and URL generation (`/notes/...`)
- [x] 1.3 Add notes directory discovery and loading from the repository (e.g. `notes/`)

## 2. Markdown and Linking Extensions
- [x] 2.1 Extend Markdown pipeline to recognize and parse `[[Note Title]]` wiki links
- [x] 2.2 Implement build-time resolution from wiki titles to note slugs/URLs
- [x] 2.3 Add support for cross-linking between notes and posts via wiki links

## 3. Rendering and Layout
- [x] 3.1 Add layouts for individual notes and any index/landing views
- [x] 3.2 Integrate notes into navigation (e.g. top-level "Notes" entry)
- [x] 3.3 Ensure notes share the existing tech-minimal theme and topics metadata presentation

## 4. Backlinks, Validation, and Orphans
- [x] 4.1 Implement build-time collection of outbound and inbound links for notes
- [x] 4.2 Render a "Linked from"/backlinks section on note pages
- [x] 4.3 Add validation for unresolved wiki links (warnings vs. failures)
- [x] 4.4 Detect orphaned notes and report them in build output

## 5. Search and Feeds
- [x] 5.1 Extend `docs.json` generation to include notes (title, headings, body)
- [x] 5.2 Update the Bun/FlexSearch indexing step to include note documents
- [x] 5.3 Exclude notes from existing RSS/Atom feeds by default; decide on any dedicated notes feed later

## 6. Hardening and Documentation
- [x] 6.1 Add build-time tests or scripts to validate link resolution and orphan detection
- [x] 6.2 Update authoring documentation to cover notes front matter, wiki links, and expectations
- [ ] 6.3 Run `openspec validate add-notes-bliki --strict` and keep specs synced during implementation
