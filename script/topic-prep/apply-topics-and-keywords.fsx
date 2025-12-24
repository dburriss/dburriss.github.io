#load "./Common.fsx"

open System
open System.IO
open System.Text.Json

open TopicPrep.Common

type MigrationItem =
    { source_path: string
      topics: string list }

type MigrationRoot =
    { items: MigrationItem list }

let parseArgs (args: string array) =
    let dryRun = args |> Array.exists (fun a -> a = "--dry-run")
    let only =
        args
        |> Array.tryFindIndex (fun a -> a = "--only")
        |> Option.bind (fun idx -> if idx + 1 < args.Length then Some args.[idx + 1] else None)
    dryRun, only

let rootDir = __SOURCE_DIRECTORY__ |> Directory.GetParent |> fun p -> p.Parent.FullName
let migrationPath = Path.Combine(rootDir, "topic-migration.json")

if not (File.Exists migrationPath) then
    failwithf "Missing %s. Run generate-topic-migration.fsx first." migrationPath

let (dryRun, onlyPathOpt) = parseArgs fsi.CommandLineArgs

let json = File.ReadAllText(migrationPath)
let migration = JsonSerializer.Deserialize<MigrationRoot>(json)

let items =
    migration.items
    |> List.sortBy (fun i -> i.source_path)
    |> fun xs ->
        match onlyPathOpt with
        | None -> xs
        | Some p -> xs |> List.filter (fun i -> i.source_path = p)

let mutable changedCount = 0
let mutable unchangedCount = 0

for item in items do
    let fullPath = Path.Combine(rootDir, item.source_path.Replace("/", Path.DirectorySeparatorChar.ToString()))

    if not (File.Exists fullPath) then
        failwithf "File missing: %s" item.source_path

    let original = File.ReadAllText(fullPath)

    let fmRegionOpt = tryGetFrontMatterRegion original

    // Some drafts may be empty placeholders without front matter.
    // In that case, initialize front matter with just the new keys.
    let fm = fmRegionOpt |> Option.map (fun r -> r.frontMatter) |> Option.defaultValue ""

    let category = getLegacyCategory fm |> List.tryHead
    let tags = getLegacyTags fm

    let keywords =
        [ yield! category |> Option.toList
          yield! tags ]
        |> stableDistinct

    let fm1 = upsertFrontMatterKey fm "topics" (formatFlowSeq item.topics)
    let fm2 = upsertFrontMatterKey fm1 "keywords" (formatFlowSeq keywords)

    let updated =
        match fmRegionOpt with
        | Some r -> r.prefix + fm2 + r.suffix
        | None ->
            let newline = if original.Contains("\r\n") then "\r\n" else "\n"
            "---" + newline + fm2.TrimEnd('\r', '\n') + newline + "---" + newline + original

    if updated <> original then
        changedCount <- changedCount + 1
        if dryRun then
            printfn "Would update %s" item.source_path
        else
            File.WriteAllText(fullPath, updated)
            printfn "Updated %s" item.source_path
    else
        unchangedCount <- unchangedCount + 1

printfn "Changed: %d, unchanged: %d" changedCount unchangedCount

if dryRun then
    printfn "Dry-run: no files written"
