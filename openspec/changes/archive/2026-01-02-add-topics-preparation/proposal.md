
## Why
The blog’s current categories and tags are inconsistent and have significant time decay. The long-term goal is to consolidate navigation around a small, stable set of human-friendly “topics” (see `knowledge/moving-to-topics.md`). This change is the first step: prepare the new topic data and per-post topic assignments without changing layouts, URLs, or rendering behaviour.

## What Changes
- Add a `topics` configuration section to `_config.yml` defining each topic with `id`, `name`, `description`, and `legacy_tags`.
- Add a generated `topic-migration.json` file mapping each post/draft to the topic IDs it should receive.
- Add `topics` and `keywords` fields to the YAML front matter for every Markdown post and draft.
- Provide reproducible F# scripts (`.fsx`) under `script/topic-prep/` to generate the migration mapping and apply front matter changes, minimizing manual edits.

## Impact
- Affected specs: new capability `topics`
- Affected content: `_config.yml`, `_posts/**`, `_drafts/**`
- New artifacts/scripts: `topic-migration.json`, topic prep `.fsx` scripts (location defined in `design.md`)
- Non-goals: no layout changes; no replacement of categories/tags/keywords; no new topic pages yet
