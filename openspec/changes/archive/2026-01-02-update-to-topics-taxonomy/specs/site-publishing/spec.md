## ADDED Requirements
### Requirement: Topic Overview and Landing Pages
The site SHALL provide a topics overview page listing all configured topics with their names and descriptions, and SHALL provide a landing page per topic ID listing all posts assigned to that topic.

#### Scenario: Topics overview lists all topics with descriptions
- **WHEN** a reader visits `/topics/`
- **THEN** the page displays every topic from `_config.yml` `topics` with `name` and `description`

#### Scenario: Per-topic page lists assigned articles
- **WHEN** a reader visits `/topics/{id}/`
- **THEN** the page lists all posts whose front matter `topics` contains `{id}`
- **AND** the page header shows the topic `name` and `description`

### Requirement: Legacy Category/Tag Redirect Pages
The site SHALL render legacy category and tag pages as static HTML pages that perform a meta `http-equiv="refresh"` redirect to the most relevant topic page. Legacy categories and tags SHALL be sourced from `_config.yml` `topics` entries (`legacy_category` and `legacy_tags`), and these legacy pages SHALL be generated even if no posts currently include legacy `category`/`categories`/`tags` front matter. When no mapping exists, the redirect SHALL point to the topics overview page.

#### Scenario: Legacy category redirects to mapped topic
- **WHEN** a reader visits `/category/{category}/`
- **THEN** a meta refresh redirects to `/topics/{mapped-id}/`

#### Scenario: Legacy tag redirects to mapped topic
- **WHEN** a reader visits `/tag/{tag}/`
- **THEN** a meta refresh redirects to `/topics/{mapped-id}/`

#### Scenario: Fallback redirect to topics overview
- **WHEN** a reader visits a legacy page with no mapping
- **THEN** a meta refresh redirects to `/topics/`

## MODIFIED Requirements
### Requirement: Layouts converted to F# components
The F# Giraffe components SHALL render post and page layouts using topics for navigation and labeling instead of categories/tags.

#### Scenario: Layout uses topics in sidebar and post metadata
- **WHEN** the renderer builds index, post, and page views
- **THEN** the sidebar and post metadata sections display topic labels instead of categories/tags
- **AND** hovering a topic label shows its description via the HTML `title` attribute

### Requirement: Draft creation script available
The new-draft scripts SHALL prompt for and require `topics` (list of topic IDs) and `keywords` in front matter; legacy `category`/`tags` SHALL NOT be required.

#### Scenario: Generated draft has valid front matter including topics and keywords
- **WHEN** a draft is created using `./new-draft.ps1` or `./new-draft.sh`
- **THEN** the file contains valid YAML front matter including `title`, `layout`, `published`, `topics`, and `keywords`
- **AND** the script fails with a helpful error if `topics` or `keywords` are missing
