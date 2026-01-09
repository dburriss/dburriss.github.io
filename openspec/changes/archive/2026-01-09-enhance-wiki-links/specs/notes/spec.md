# Notes Specification Deltas

## Purpose
Enhance the existing notes wiki link system to properly parse and handle [[Title|DisplayText]] syntax while maintaining backward compatibility with existing [[Title]] syntax.

## MODIFIED Requirements

### Requirement: Wiki-style internal links for notes
The Markdown pipeline SHALL support wiki-style internal links using both `[[Page Name]]` and `[[Page Name|Display Text]]` syntax that resolve to notes or posts at build time.

#### Scenario: Wiki link resolves to a unique note
- **WHEN** a note body contains `[[Example]]`
- **AND** there is exactly one note titled "Example"
- **THEN** the renderer converts the wiki syntax into a link pointing to that note's URL
- **AND** the link displays "Example" as the anchor text

#### Scenario: Wiki link resolves to a unique post
- **WHEN** a note or post body contains `[[Some Post]]`
- **AND** there is exactly one post titled "Some Post" and no note with that title
- **THEN** the renderer converts the wiki syntax into a link pointing to that post's URL
- **AND** the link displays "Some Post" as the anchor text

#### Scenario: Wiki link with custom display text resolves to note
- **WHEN** a note body contains `[[Example|Custom Display Text]]`
- **AND** there is exactly one note titled "Example"
- **THEN** the renderer converts the wiki syntax into a link pointing to that note's URL
- **AND** the link displays "Custom Display Text" as the anchor text

#### Scenario: Wiki link with custom display text resolves to post
- **WHEN** a note body contains `[[Some Post|Different Text]]`
- **AND** there is exactly one post titled "Some Post" and no note with that title
- **THEN** the renderer converts the wiki syntax into a link pointing to that post's URL
- **AND** the link displays "Different Text" as the anchor text

#### Scenario: Notes take priority over posts in resolution
- **WHEN** a wiki link contains `[[Shared Title]]` or `[[Shared Title|Display]]`
- **AND** there is both a note and post with the title "Shared Title"
- **THEN** the renderer links to the note's URL
- **AND** the build emits a warning about the ambiguous title match

#### Scenario: Unresolved wiki link with custom display text
- **WHEN** a wiki link contains `[[Non-Existent|Custom Display]]`
- **AND** no note or post matches the title "Non-Existent"
- **THEN** the renderer outputs `<span class="unresolved-link">Custom Display</span>`
- **AND** the build emits a validation warning about the unresolved link

#### Scenario: Empty display text is ignored
- **WHEN** a wiki link contains `[[Title|]]` (empty display text after pipe)
- **THEN** it is treated as `[[Title]]` with "Title" as both target and display text

#### Scenario: Unresolved wiki link triggers validation error or warning (updated)
- **WHEN** a wiki link title (the part before the pipe, or the entire content if no pipe) does not match any note or post title
- **THEN** the build emits a validation error or warning summarizing the unresolved link and its source document

#### Scenario: Ambiguous wiki link is reported (updated)
- **WHEN** a wiki link title (the part before the pipe) matches multiple notes and/or posts
- **THEN** the build reports the ambiguity and the candidate targets in its diagnostics

## ADDED Requirements

### Requirement: Wiki link pipe syntax parsing
The wiki link parser SHALL correctly parse the pipe separator to distinguish between target title and display text.

#### Scenario: Parser extracts title and display text from pipe syntax
- **WHEN** the markdown contains `[[Target Title|Display Text]]`
- **THEN** the parser extracts "Target Title" as the resolution target
- **AND** extracts "Display Text" as the display text
- **AND** both values have leading/trailing whitespace trimmed

#### Scenario: Parser handles multiple pipe characters
- **WHEN** the markdown contains `[[Title|Display|Extra]]`
- **THEN** the parser treats "Title" as the target title
- **AND** treats "Display|Extra" as the display text (only first pipe is a separator)

#### Scenario: Parser handles edge cases gracefully
- **WHEN** the markdown contains `[[|Display Only]]` (empty title)
- **THEN** the parser treats it as an unresolved link
- **WHEN** the markdown contains `[[Title With | Spaces|Display]]`
- **THEN** the parser correctly handles whitespace around the pipe separator