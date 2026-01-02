## ADDED Requirements
### Requirement: Legacy Metadata Cleanup Script
The project SHALL provide a script that removes legacy `category` and `tags` keys from Markdown front matter across `_posts/` and `_drafts/`.

#### Scenario: Idempotent cleanup
- **WHEN** the cleanup script runs on files with or without legacy `category` and `tags`
- **THEN** files containing those keys are updated to remove them
- **AND** files without those keys remain unchanged

#### Scenario: Preserve topics and keywords
- **WHEN** the cleanup script updates front matter
- **THEN** `topics` and `keywords` keys and values remain unchanged
- **AND** YAML formatting remains valid

#### Scenario: Report summary
- **WHEN** the cleanup script completes
- **THEN** it reports the number of files examined and modified

## MODIFIED Requirements
### Requirement: Topics and Keywords in Markdown Front Matter
Every Markdown post and draft SHALL include a `topics` YAML key containing a list of topic IDs and a `keywords` YAML key containing the legacy `category` plus all legacy `tags`. Legacy `category` and `tags` fields SHALL be optional and MAY be absent.

#### Scenario: Adding topics and keywords does not remove legacy metadata
- **WHEN** `topics` and `keywords` are added to a Markdown file
- **THEN** existing `category` and `tags` values (if present) remain unchanged
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

#### Scenario: Legacy category/tags not required
- **WHEN** creating or promoting posts
- **THEN** the absence of legacy `category` and `tags` does not block creation or promotion
