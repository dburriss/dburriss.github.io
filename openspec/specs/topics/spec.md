# topics Specification

## Purpose
TBD - created by archiving change add-topics-preparation. Update Purpose after archive.
## Requirements
### Requirement: Topic Catalog in Configuration
The project SHALL define a topic catalog in `_config.yml` under a top-level `topics` key. Each topic entry SHALL include `id`, `name`, `description`, `legacy_category` and `legacy_tags`.

#### Scenario: Config defines stable topic IDs and metadata
- **WHEN** an author opens `_config.yml`
- **THEN** they can see a `topics:` list
- **AND** each topic includes `id`, `name`, `description`, and `legacy_tags`
- **AND** topic `id` values are stable identifiers suitable for use in front matter

### Requirement: Topic Assignment Mapping Artifact
The project SHALL provide a committed `topic-migration.json` file that maps each Markdown post and draft to the topic IDs it should receive.

#### Scenario: Topic assignment mapping includes every post and draft
- **WHEN** `topic-migration.json` is generated
- **THEN** it contains an entry for every Markdown file under `_posts/**` and `_drafts/**`
- **AND** each entry lists the topic IDs assigned to that item

#### Scenario: Topic assignment mapping captures unmapped legacy terms
- **WHEN** a post has legacy tags or categories that do not map to any topic
- **THEN** the corresponding entry records those unmapped terms for review

#### Scenario: Topic assignment mapping permits empty topic assignments
- **WHEN** a post or draft has no legacy terms that map to any topic
- **THEN** the corresponding entry uses an empty `topics` list
- **AND** the entry still exists for auditability

### Requirement: Topics and Keywords in Markdown Front Matter
Every Markdown post and draft SHALL include a `topics` YAML key containing a list of topic IDs. Every Markdown post and draft SHALL include a `keywords` YAML key containing the legacy `category` plus all legacy `tags`.

#### Scenario: Adding topics and keywords does not remove legacy metadata
- **WHEN** `topics` and `keywords` are added to a Markdown file
- **THEN** existing `category` and `tags` values remain unchanged
- **AND** the Markdown body content remains unchanged

#### Scenario: Topic IDs match configured topic catalog
- **WHEN** a Markdown file specifies `topics: [...]`
- **THEN** every topic ID listed exists in `_config.yml` under `topics`

#### Scenario: Topics may be empty
- **WHEN** a Markdown file has no mapped legacy terms
- **THEN** it specifies `topics: []`

#### Scenario: Keywords mirror legacy category and tags
- **WHEN** a Markdown file has `category` and `tags`
- **THEN** its `keywords` contains the category value plus all tag values
- **AND** the `keywords` ordering preserves category first then tags

### Requirement: Reproducible Topic Preparation Scripts
The project SHALL provide F# scripts (`.fsx`) under the root `script/` folder to generate `topic-migration.json` and apply `topics`/`keywords` front matter updates to Markdown files.

#### Scenario: Generation script produces deterministic output
- **WHEN** an author runs the generation script multiple times without changing inputs
- **THEN** the resulting `topic-migration.json` content is identical

#### Scenario: Apply script supports dry-run
- **WHEN** an author runs the apply script with `--dry-run`
- **THEN** it reports which files would change
- **AND** it does not modify any files

#### Scenario: Apply script is idempotent
- **WHEN** an author runs the apply script multiple times
- **THEN** subsequent runs report no additional changes

#### Scenario: Apply script initializes missing front matter
- **WHEN** a draft file has no YAML front matter
- **THEN** the apply script adds a minimal YAML front matter block
- **AND** the block includes `topics` and `keywords`

