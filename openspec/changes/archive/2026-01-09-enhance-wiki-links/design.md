# Technical Design: Enhanced Wiki Link Parsing

## Architecture Overview
This enhancement modifies the existing wiki link processing pipeline to parse and handle the pipe syntax correctly while maintaining backward compatibility.

## Component Changes

### 1. WikiLinkInline Type Enhancement (WikiLinkExtension.fs)
**Current**: Single `Label` property stores the complete text
**Enhanced**: Split into separate properties for title and display text

```fsharp
type WikiLinkInline(title: string, displayText: string option) =
    inherit LeafInline()
    member val Title = title with get, set
    member val DisplayText = displayText with get, set
    member val ResolvedUrl: string option = None with get, set
    
    // Convenience property for backward compatibility
    member this.Label = 
        match this.DisplayText with
        | Some display -> display
        | None -> this.Title
```

### 2. WikiLinkParser Enhancement (WikiLinkExtension.fs)
**Current**: Extracts label as single string from [[...]]
**Enhanced**: Parse pipe syntax to split title and display text

```fsharp
// Extract the content between [[...]]
let content = slice.Text.Substring(startPosition + 2, endPosition - startPosition - 2)
let parts = content.Split([|'|'|], 2, StringSplitOptions.None)

let title = parts.[0].Trim()
let displayText = 
    if parts.Length > 1 && not (String.IsNullOrWhiteSpace(parts.[1]))
    then Some (parts.[1].Trim())
    else None

let wikiLink = WikiLinkInline(title, displayText)
```

### 3. WikiLinkHtmlRenderer Enhancement (WikiLinkExtension.fs)
**Current**: Uses `link.Label` for both resolution and display
**Enhanced**: Use appropriate text for display

```fsharp
override this.Write(renderer: HtmlRenderer, link: WikiLinkInline) =
    let displayText = 
        match link.DisplayText with
        | Some display -> display
        | None -> link.Title
    
    match link.ResolvedUrl with
    | Some url -> // render as link with displayText
    | None -> // render as unresolved with displayText
```

### 4. WikiLink Model Update (Models.fs)
**Current**: Single `TargetLabel` field
**Enhanced**: Separate title and display text fields

```fsharp
type WikiLink =
    { SourceUrl: string
      TargetTitle: string        // Used for resolution
      TargetDisplayText: string option  // Used for display
      ResolvedUrl: string option
      IsResolved: bool }
```

### 5. Link Resolution Enhancement (Renderer.fs, Parsing.fs)
**Current**: `extractWikiLinks` extracts complete label for resolution
**Enhanced**: Extract title only for resolution, preserve display text

```fsharp
let extractWikiLinks (markdown: string) : (string * string option) list =
    // Returns list of (title, displayText option) tuples
```

**Resolution Logic**: Use only the title part for matching against note/post titles

## Parsing Logic
The parser will handle these cases:
- `[[Title]]` → title="Title", displayText=None
- `[[Title|Display]]` → title="Title", displayText=Some("Display") 
- `[[Title|]]` → title="Title", displayText=None (empty display text ignored)
- `[[|Display]]` → title="", displayText=Some("Display") (invalid, treat as unresolved)

## Priority Resolution
When both a note and post have the same title:
1. Prioritize notes over posts
2. Log a warning about the ambiguity
3. Use the note's URL for resolution

## Backward Compatibility
- All existing [[Title]] syntax continues to work unchanged
- The Label property remains available for compatibility
- No changes to resolution behavior for single-part wiki links
- Existing WikiLink consumers that use TargetLabel will need minor updates

## Error Handling
- Empty titles are treated as unresolved links
- Malformed syntax falls back to treating entire content as title
- Multiple pipe characters: only the first pipe is used as separator
- Whitespace is trimmed from both title and display text

## Performance Considerations
- Minimal performance impact: only adds string parsing for pipe detection
- No additional regex patterns or complex parsing
- Memory overhead: one optional string per wiki link (typically small)

## Testing Strategy
- Verify backward compatibility with existing [[Title]] syntax
- Test all pipe syntax variations
- Verify resolution priority (notes over posts)
- Test unresolved link rendering with display text
- Validate that existing notes render correctly after changes