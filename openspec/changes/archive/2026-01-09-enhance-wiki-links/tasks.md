# Implementation Tasks

## Task 1: Enhance WikiLinkInline Type
- [x] **Complete Task 1**: Enhance WikiLinkInline Type
- **File**: `src/SiteRenderer/WikiLinkExtension.fs`
- **Description**: Modify WikiLinkInline to store separate Title and DisplayText properties
- **Changes**:
  - [x] Update constructor to accept title and optional displayText
  - [x] Add Title and DisplayText properties  
  - [x] Keep Label property for backward compatibility
  - [x] Update ResolvedUrl property initialization
- **Validation**: WikiLinkInline can be constructed with both title and display text

## Task 2: Update WikiLinkParser Logic
- [x] **Complete Task 2**: Update WikiLinkParser Logic
- **File**: `src/SiteRenderer/WikiLinkExtension.fs`  
- **Description**: Enhance parser to split content on pipe separator
- **Changes**:
  - [x] Parse content between [[...]] for pipe character
  - [x] Split on first pipe only (handle multiple pipes correctly)
  - [x] Trim whitespace from both title and display text
  - [x] Handle edge cases (empty title, empty display text)
  - [x] Create WikiLinkInline with parsed values
- **Validation**: Parser correctly extracts title and display text from various inputs
- **Dependencies**: Requires Task 1 (WikiLinkInline changes)

## Task 3: Update WikiLinkHtmlRenderer
- [x] **Complete Task 3**: Update WikiLinkHtmlRenderer
- **File**: `src/SiteRenderer/WikiLinkExtension.fs`
- **Description**: Modify renderer to use appropriate display text
- **Changes**:
  - [x] Use DisplayText when available, fall back to Title
  - [x] Update both resolved and unresolved link rendering
  - [x] Ensure proper HTML escaping of display text
- **Validation**: Links render with correct anchor text
- **Dependencies**: Requires Tasks 1-2 (WikiLinkInline and parser changes)

## Task 4: Update WikiLink Model
- [x] **Complete Task 4**: Update WikiLink Model
- **File**: `src/SiteRenderer/Models.fs`
- **Description**: Enhance WikiLink record to store title and display text separately
- **Changes**:
  - [x] Replace or supplement TargetLabel with TargetTitle
  - [x] Add optional TargetDisplayText field
  - [x] Update record construction sites
- **Validation**: WikiLink model correctly stores both values
- **Dependencies**: None (can be done in parallel with other tasks)

## Task 5: Enhance Link Extraction Logic
- [x] **Complete Task 5**: Enhance Link Extraction Logic
- **File**: `src/SiteRenderer/Parsing.fs`
- **Description**: Update extractWikiLinks to return title and display text
- **Changes**:
  - [x] Modify regex or parsing logic to extract title for resolution
  - [x] Return tuples of (title, displayText option)
  - [x] Update function signature and callers
  - [x] Preserve existing ignore patterns behavior
- **Validation**: Extraction returns correct title for resolution
- **Dependencies**: Requires Task 4 (WikiLink model changes)

## Task 6: Update Link Resolution Logic  
- [x] **Complete Task 6**: Update Link Resolution Logic
- **File**: `src/SiteRenderer/Renderer.fs`
- **Description**: Modify resolveWikiLinks to use title-only for resolution
- **Changes**:
  - [x] Use only title part for matching against notes/posts
  - [x] Preserve display text in WikiLink records
  - [x] Implement note priority over posts when both match
  - [x] Add warning for ambiguous matches
  - [x] Update link graph construction
- **Validation**: Resolution correctly prioritizes notes and logs warnings
- **Dependencies**: Requires Tasks 4-5 (model and extraction changes)

## Task 7: Build and Test Integration
- [x] **Complete Task 7**: Build and Test Integration
- **Description**: Verify all changes work together correctly
- **Changes**:
  - [x] Build site with enhanced wiki link parsing
  - [x] Test with existing notes containing pipe syntax
  - [x] Verify backward compatibility with [[Title]] syntax
  - [x] Check that unresolved links render correctly
- **Validation**: 
  - [x] All existing notes render correctly
  - [x] New pipe syntax works as specified
  - [x] Build succeeds without errors
  - [x] No regressions in existing functionality
- **Dependencies**: Requires all previous tasks

## Implementation Notes

### Parallel Work Opportunities
- Tasks 1-3 (WikiLinkExtension.fs changes) should be done sequentially
- Task 4 (Models.fs) can be done in parallel with Tasks 1-3
- Tasks 5-6 depend on Task 4 but can be done together

### Validation Strategy
- After each task, run `dotnet build` to ensure compilation
- Create test cases for each scenario in the specification
- Use existing notes with pipe syntax as integration tests
- Verify no changes to generated URLs or site structure

### Risk Mitigation
- Maintain backward compatibility throughout implementation
- Test incrementally after each task
- Keep existing Label property until all consumers are updated
- Use feature flags or graceful degradation if needed during transition