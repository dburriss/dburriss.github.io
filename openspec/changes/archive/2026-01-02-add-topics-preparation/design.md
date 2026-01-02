
## Context
The proposed topic taxonomy (8 topics) is defined in `knowledge/moving-to-topics.md`. This change prepares data needed for a later change that will update rendering and navigation to use topics instead of categories/tags.

## Goals / Non-Goals
- Goals:
  - Define a stable topic catalog in `_config.yml`.
  - Produce an auditable mapping (`topic-migration.json`) from each post/draft to topic IDs.
  - Add `topics` to Markdown front matter while preserving existing `category`/`tags`.
  - Make changes reproducible via `.fsx` scripts (generate + apply), not hand-edited file-by-file.
- Non-Goals:
  - No changes to HTML layouts, CSS, routes, tag/category pages, or feeds.
  - No removal/renaming of existing `category` or `tags` keys.
  - No changes to “related posts” logic yet.

## Data Model
### Topic definition (in `_config.yml`)
A topic is represented as a YAML mapping with:
- `id`: stable, URL-safe identifier (kebab-case), e.g. `software-design`.
- `name`: display name, e.g. `Software Design`.
- `description`: short UI-friendly description (1 sentence).
- `legacy_tags`: list of strings that represent legacy tags/categories that map to this topic.

Proposed `_config.yml` structure:
```yml
topics:
  - id: software-design
    name: Software Design
    description: ...
    legacy_tags: [Architecture, DDD, C4, Modeling, Design Patterns, Distributed Systems, Temporal Modeling, Functional Modeling]
```

Notes:
- The renderer currently parses only known keys from `_config.yml`, so adding `topics:` is expected to be backward-compatible.
- `legacy_tags` is explicitly “legacy vocabulary”, not necessarily exhaustive.

### Post/draft assignment (in Markdown front matter)
Each post/draft gets a new YAML key:
- `topics`: list of topic IDs (kebab-case) defined in `_config.yml`.

Example:
```yml
---
layout: post
category: Software Development
tags: [Distributed Systems, API Design, Reliability]
topics: [software-design, reliability-observability]
---
```

Notes:
- This change preserves existing tags/categories so current rendering stays identical.
- The renderer currently ignores unknown front matter keys, so adding `topics:` is expected to be backward-compatible.

## Migration Artifact
### `topic-migration.json`
A committed JSON file at repo root that makes the planned migration explicit and reviewable.

Proposed structure:
```json
{
  "topics": [{ "id": "software-design", "name": "Software Design" }],
  "items": [
    {
      "source_path": "_posts/2021-08-22-reliable-api-part1.md",
      "permalink": "/reliable-api-part1",
      "legacy": { "category": "Software Development", "tags": ["Distributed Systems", "API Design", "Reliability"] },
      "topics": ["software-design", "reliability-observability"],
      "unmapped": { "tags": ["API Design"], "category": null }
    }
  ]
}
```

Notes:
- Including `unmapped` helps audit which legacy terms need manual decisions.
- The file is intended as an intermediate artifact for the stepwise migration (data prep now; renderer changes later).

## Script Strategy
Provide two F# scripts to keep the process reproducible:
1. `generate-topic-migration.fsx`
   - Reads `_config.yml` (topics + legacy mapping) and all Markdown in `_posts/**` and `_drafts/**`.
   - Produces/overwrites `topic-migration.json` deterministically.
2. `apply-topics-and-keywords.fsx`
   - Reads `topic-migration.json`.
   - Adds/updates the `topics:` and `keywords:` YAML keys in each file’s front matter.
   - Preserves all other front matter keys and the Markdown body.
   - Supports `--dry-run` and prints a summary of planned changes.

Location:
- Scripts should live under the root `script/` folder (e.g. `script/topic-prep/`) rather than `samples/`, to distinguish “maintenance tooling” from “example snippets”.

Idempotency & safety:
- Re-running scripts should produce the same results.
- The apply script should avoid reformatting unrelated front matter lines.
- The apply script should handle drafts without YAML front matter by initializing a minimal front matter block containing only the new keys.

## Risks / Trade-offs
- Ambiguous mapping: Some tags/categories will not map cleanly to the 8-topic set.
  - Mitigation: emit `unmapped` in JSON and require a deliberate decision (no silent defaulting).
- Formatting drift: YAML formatting in existing posts is inconsistent (spacing, quoting).
  - Mitigation: patch only the minimal front matter region and keep changes localized.

## Migration Plan
1. Add `topics:` catalog to `_config.yml`.
2. Generate `topic-migration.json` and review unmapped terms.
3. Apply `topics:` front matter updates using the apply script.
4. Build the renderer to confirm no behavioural changes.

## Open Questions
- None for this step; apply-stage can proceed with the defaults below.

Defaults for this step:
- Posts/drafts with no mapped legacy terms receive `topics: []`.
- Mapping uses both legacy `category` and `tags` values (treat category as another legacy term).
- Scripts live under `script/` (e.g. `script/topic-prep/`).
