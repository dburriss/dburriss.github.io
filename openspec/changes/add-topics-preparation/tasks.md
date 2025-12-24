
## 1. Topic Catalog (Config)
- [ ] 1.1 Add `topics` section to `_config.yml` (8 topics)
- [ ] 1.2 Ensure each topic has `id`, `name`, `description`, `legacy_tags`

## 2. Topic Migration Artifact
- [ ] 2.1 Define `topic-migration.json` schema (documented in `design.md`)
- [ ] 2.2 Add generation script under `script/topic-prep/` to produce `topic-migration.json`
- [ ] 2.3 Run generation script and commit `topic-migration.json`

## 3. Apply Topics and Keywords to Content
- [ ] 3.1 Add apply script under `script/topic-prep/` to write `topics:` and `keywords:` into `_posts/**` and `_drafts/**`
- [ ] 3.2 Ensure apply script supports `--dry-run`
- [ ] 3.3 Run apply script and commit updated Markdown files

## 4. Validation
- [ ] 4.1 Run `dotnet fantomas ./src/SiteRenderer/` if any F# code is added
- [ ] 4.2 Run `dotnet build ./src/SiteRenderer/SiteRenderer.fsproj`
- [ ] 4.3 Run `openspec validate add-topics-preparation --strict`
