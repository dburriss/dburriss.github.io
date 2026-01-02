## 1. Implementation
- [x] 1.1 Update layout components to render topics instead of categories/tags
- [x] 1.2 Add topics overview page listing all topics with descriptions
- [x] 1.3 Add per-topic landing pages listing articles by topic
- [x] 1.4 Add hover tooltips for topic labels using topic descriptions
- [x] 1.5 Implement legacy category/tag pages as meta-refresh redirects
- [x] 1.6 Add fallback redirect to topics overview when unmapped
- [x] 1.7 Update draft creation scripts to require topics and keywords
- [x] 1.8 Update draft promotion scripts to validate topics and keywords
- [x] 1.9 Adjust parsing to ignore missing legacy category/tags
- [x] 1.10 Verify `_config.yml` topics catalog drives IDs and descriptions
- [x] 1.11 Implement legacy metadata cleanup script (remove category/tags)

## 2. Validation
- [x] 2.1 Build site and confirm topics appear in sidebar and post metadata
- [x] 2.2 Verify topics overview lists all configured topics with descriptions
- [x] 2.3 Verify each topic page lists relevant posts
- [x] 2.4 Hovering a topic label shows its description
- [x] 2.5 Category/tag pages redirect to mapped topic, or overview if unmapped
- [x] 2.6 New draft script rejects missing topics/keywords and succeeds with valid input
- [x] 2.7 Promotion script rejects drafts missing topics/keywords
- [x] 2.8 Confirm legacy category/tags in front matter are optional and not required
- [x] 2.9 Cleanup script removes category/tags and preserves topics/keywords
- [x] 2.10 Cleanup script is idempotent (no changes on second run)
