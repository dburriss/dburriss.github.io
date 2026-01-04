// cleanup-legacy-metadata.fsx - Remove legacy category/tags from front matter
//
// DESCRIPTION:
//   Scans Markdown files in `_posts/` and `_drafts/` and removes legacy
//   `category:`, `categories:`, `tags:`, and `TAGS:` keys from the YAML front
//   matter. Preserves all other keys, including `topics` and `keywords`.
//   Safe and idempotent: running multiple times produces no further changes.
//
// USAGE:
//   dotnet fsi script/topic-prep/cleanup-legacy-metadata.fsx            // Clean all files
//   dotnet fsi script/topic-prep/cleanup-legacy-metadata.fsx --dry-run  // Show files to change
//
// NOTES:
//   - Only modifies the front matter block (between leading/trailing ---).
//   - Files without front matter are skipped.

#load "./Common.fsx"

open System
open System.IO
open System.Text.RegularExpressions

open TopicPrep.Common

let parseArgs (args: string array) =
    let dryRun = args |> Array.exists (fun a -> a = "--dry-run")
    dryRun

let rootDir = __SOURCE_DIRECTORY__ |> Directory.GetParent |> fun p -> p.Parent.FullName
let dryRun = parseArgs fsi.CommandLineArgs

let shouldRemoveLine (line: string) =
    // Match start of line keys in front matter (simple scalar or flow sequence on same line)
    // Keys: category, categories, tags, TAGS
    let patterns =
        [ "^category:[\\s]"; "^categories:[\\s]"; "^tags:[\\s]"; "^TAGS:[\\s]" ]
    patterns |> List.exists (fun pat -> Regex.IsMatch(line, pat))

let cleanFrontMatter (frontMatter: string) =
    // Remove legacy keys line-by-line, preserving order and other content
    let lines =
        frontMatter.Split([|"\r\n"; "\n"|], StringSplitOptions.None)
        |> Array.toList
    let filtered = lines |> List.filter (fun l -> not (shouldRemoveLine l))
    // Preserve original newline style by joining later via writeFrontMatterFile
    String.Join("\n", filtered)

let mutable changedCount = 0
let mutable unchangedCount = 0

for path in readAllMarkdownFiles rootDir do
    let relPath = Path.GetRelativePath(rootDir, path).Replace("\\", "/")
    let content = File.ReadAllText(path)
    match tryGetFrontMatterRegion content with
    | None ->
        // No front matter — skip
        unchangedCount <- unchangedCount + 1
    | Some region ->
        let cleanedFm = cleanFrontMatter region.frontMatter
        if cleanedFm <> region.frontMatter then
            changedCount <- changedCount + 1
            if dryRun then
                printfn "Would clean: %s" relPath
            else
                writeFrontMatterFile path cleanedFm region.suffix region.newline
                printfn "Cleaned: %s" relPath
        else
            unchangedCount <- unchangedCount + 1

printfn "Changed: %d, unchanged: %d" changedCount unchangedCount
if dryRun then
    printfn "Dry-run: no files written"
