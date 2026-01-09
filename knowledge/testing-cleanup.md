# Testing Cleanup Plan

## Overview

This document outlines the plan to clean up test duplication and organize tests properly in the blog project. The goal is to eliminate duplication between test scripts and the formal test project, while enhancing validation capabilities.

## Current State Analysis

### Existing Test Files

1. **`src/SiteRenderer.Tests/Tests.fs`** (Unit Tests - xUnit)
   - **Purpose**: Unit tests for wiki link parsing and rendering functionality
   - **Tests**: Extension registration, basic parsing, pipe syntax, multiple links, error handling, extraction functions, WikiLinkInline class, HTML rendering
   - **Type**: Pure unit tests testing individual components in isolation

2. **`test-wiki-links.fsx`** (Script Tests)
   - **Purpose**: Custom test framework testing wiki link parsing and markdown processing
   - **Duplication**: **HIGH** - Almost identical tests to Tests.fs
   - **Type**: Unit tests with custom test framework
   - **Status**: REDUNDANT - should be deleted

3. **`test-site-wiki-links.fsx`** (Script Tests)
   - **Purpose**: Tests actual rendered HTML files in `_site/` directory
   - **Tests**: Site directory existence, HTML output analysis, literal `[[...]]` pattern detection, HTML structure validation, known problematic pattern searches
   - **Type**: **Site validation** - tests against rendered output

4. **`test-site-validation.fsx`** (Script Tests)
   - **Purpose**: Simple validation of wiki links in generated site
   - **Duplication**: **MEDIUM** - Overlaps with test-site-wiki-links.fsx but simpler
   - **Type**: **Site validation** - tests against rendered output

### Content Analysis

- **Posts**: 68 markdown files in `_posts/`
- **Notes**: 5 markdown files in `_notes/`
- **Includes**: 7 patterns defined in `_config.yml`:
  - `css/**`
  - `js/**`
  - `img/**`
  - `fonts/**`
  - `tkd/**`
  - `CNAME`
  - `sitemap.xml`

### Test Integration

Both `render.sh` and `render.ps1` integrate tests via:
- `--test`: Runs validation after site generation
- `--test-only`: Runs tests without site generation
- Currently calls `test-site-validation.fsx` and `test-wiki-links.fsx`

## Cleanup Strategy

### Phase 1: Consolidate Unit Tests

**Goal**: Move all unit test functionality into `src/SiteRenderer.Tests/Tests.fs` and eliminate duplication

**Actions**:
1. **Enhance `Tests.fs`** - Add any missing test functionality from `test-wiki-links.fsx`:
   - Add tests for `Parsing.markdownToHtml` function specifically
   - Add complex markdown integration tests
   - Ensure full coverage of wiki link extraction edge cases

2. **Delete `test-wiki-links.fsx`** - Pure duplication of unit tests

**Benefits**:
- Single source of truth for unit tests
- Better IDE integration and debugging
- Consistent test reporting
- Integration with CI/CD pipelines

### Phase 2: Create Comprehensive Site Validation

**Goal**: Create single, comprehensive site validation script with enhanced capabilities

**Actions**:
1. **Create `scripts/validate-site.fsx`** - Comprehensive validation script combining best features
2. **Delete redundant scripts**:
   - `test-site-wiki-links.fsx`
   - `test-site-validation.fsx`

**Benefits**:
- Single script for all site validation
- Enhanced validation capabilities
- Clear separation between unit tests and site validation
- Better organization in scripts/ folder

## Enhanced Validation Features

The new `scripts/validate-site.fsx` will include:

### 1. Wiki Link Validation (from existing scripts)
- ✅ Check for literal `[[...]]` patterns in HTML
- ✅ Verify proper `<span class="unresolved-link">` rendering
- ✅ Verify resolved links as `<a href="">` elements
- ✅ HTML structure integrity
- ✅ Known problematic pattern searches

### 2. Include Assets Validation (NEW)
- ✅ Verify all 7 include patterns from `_config.yml` are copied to `_site/`
- ✅ Check `css/**`, `js/**`, `img/**`, `fonts/**`, `tkd/**`
- ✅ Verify `CNAME` and `sitemap.xml` exist
- ✅ Report missing assets with specific paths

### 3. Content Count Validation (NEW)
- ✅ Count posts: Verify `_posts/` count (68) matches `_site/` generated pages
- ✅ Count notes: Verify `_notes/` count (5) matches `_site/notes/` generated pages
- ✅ Report discrepancies with specific missing items
- ✅ Validate that each source file has corresponding output
- ✅ Check for orphaned generated files

### 4. Site Structure Validation (NEW)
- ✅ Verify expected directories exist (`_site/notes/`, etc.)
- ✅ Check HTML structure integrity across pages
- ✅ Validate navigation and cross-references

## File Organization Changes

### Files to CREATE:
```
scripts/validate-site.fsx      # NEW: Comprehensive site validation
```

### Files to DELETE:
```
test-wiki-links.fsx            # DELETE: Duplicate of unit tests
test-site-wiki-links.fsx       # DELETE: Consolidated into validate-site.fsx
test-site-validation.fsx       # DELETE: Consolidated into validate-site.fsx
```

### Files to ENHANCE:
```
src/SiteRenderer.Tests/Tests.fs  # ADD: Missing unit test coverage
render.sh                       # UPDATE: Use new script path
render.ps1                      # UPDATE: Use new script path
```

## Implementation Steps

### Step 1: Enhance Unit Tests
- Add missing test coverage to `src/SiteRenderer.Tests/Tests.fs`
- Tests for `Parsing.markdownToHtml` function
- Complex markdown integration tests
- Site markdown pipeline tests

### Step 2: Create Comprehensive Validation Script
Create `scripts/validate-site.fsx` with modular structure:

```fsharp
module WikiLinkValidation = 
    // Best logic from existing site validation scripts

module AssetValidation = 
    // NEW: Check includes from _config.yml

module ContentCountValidation = 
    // NEW: Verify post/note counts

module SiteStructureValidation = 
    // NEW: Overall site integrity checks

module ValidationRunner = 
    // Unified test runner and reporting
```

### Step 3: Update Render Scripts
Update both `render.sh` and `render.ps1`:
- Change from `test-site-validation.fsx` to `scripts/validate-site.fsx`
- Remove call to `test-wiki-links.fsx` (now in unit tests)
- Update help text and comments

### Step 4: Clean Up
- Delete the 3 obsolete test script files
- Run unit tests to verify no functionality lost
- Test new validation script

## Final State

After cleanup:

- **`src/SiteRenderer.Tests/Tests.fs`** - All unit tests using xUnit framework
- **`scripts/validate-site.fsx`** - Comprehensive site validation script
- **Clear separation**: Unit tests vs site validation
- **Enhanced capabilities**: Include validation, content count validation, structure validation
- **No duplication**: Single source of truth for each type of test

## Validation Script Structure

The new validation script will have these validation modules:

### WikiLinkValidation
- Literal bracket detection
- Unresolved link span validation  
- Resolved link validation
- HTML structure analysis

### AssetValidation
- Config-driven include checking
- Asset existence verification
- Missing asset reporting

### ContentCountValidation
- Post count verification (source vs generated)
- Note count verification (source vs generated)
- Orphaned file detection
- Missing file identification

### SiteStructureValidation
- Directory structure validation
- Navigation integrity
- Cross-reference validation
- Overall site health checks

## Benefits

✅ **No duplication**: Clear separation between unit and validation tests  
✅ **Enhanced validation**: Comprehensive site integrity checking  
✅ **Clean organization**: Tests properly organized in appropriate locations  
✅ **Maintained integration**: Render scripts continue to work with `--test` flag  
✅ **Better maintainability**: Single comprehensive validation script  
✅ **Improved coverage**: New validation capabilities for assets and content counts