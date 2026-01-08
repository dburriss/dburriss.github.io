# Enhance Wiki Link System to Support [[Title|DisplayText]] Syntax

## Overview
Enhance the existing wiki link system to properly parse and handle the [[Title|DisplayText]] syntax that is already present in existing notes but currently not parsed correctly.

## Current State
The wiki link system currently:
- Supports basic [[Title]] syntax for linking between notes and posts
- Has [[Title|DisplayText]] syntax used in existing notes, but treats "Title|DisplayText" as a single label
- Provides link resolution, backlink tracking, and unresolved link detection
- Renders unresolved links with special styling

## Proposed Enhancement
Parse the pipe separator in wiki links to support:
- **[[Title]]** - Uses Title for both resolution and display (unchanged behavior)
- **[[Title|DisplayText]]** - Uses Title for resolution, DisplayText for anchor text
- Proper handling of unresolved links with pipe syntax

## Scope
This change enhances the existing wiki link capability within the notes specification and affects:
- **Parser**: WikiLinkInline type and WikiLinkParser to handle pipe syntax
- **Resolution**: Link resolution logic to use only the Title part for matching
- **Rendering**: HTML renderer to use DisplayText when available
- **Models**: WikiLink model to store separate title and display text

## Impact Analysis
- **Backward Compatible**: All existing [[Title]] syntax continues to work unchanged
- **Immediate Benefit**: Existing notes with pipe syntax will render correctly after implementation
- **Low Risk**: Self-contained enhancement to existing, working system
- **No Breaking Changes**: No changes to URLs, feeds, or navigation behavior

## Success Criteria
- [[My Note]] renders as link to "My Note" with "My Note" as display text (unchanged)
- [[My Note|Custom Display]] renders as link to "My Note" with "Custom Display" as display text  
- [[Non-Existent|Custom]] renders as `<span class="unresolved-link">Custom</span>`
- Notes take priority over posts when both match the same title
- All existing notes render correctly after the change

## Dependencies
- Builds upon existing notes specification requirements
- Requires no changes to site configuration or external dependencies
- Self-contained within the F# SiteRenderer codebase