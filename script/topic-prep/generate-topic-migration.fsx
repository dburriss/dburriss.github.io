#load "./Common.fsx"

open System
open System.IO
open System.Text.Json

open TopicPrep.Common

type TopicDefinition =
    { id: string
      name: string
      description: string
      legacy_tags: string list }

type MigrationTopic = { id: string; name: string }

type LegacyInfo =
    { category: string option
      tags: string list }

type UnmappedInfo =
    { category: string option
      tags: string list }

type MigrationItem =
    { source_path: string
      permalink: string option
      legacy: LegacyInfo
      topics: string list
      unmapped: UnmappedInfo }

type MigrationRoot =
    { topics: MigrationTopic list
      items: MigrationItem list }

let parseTopicsFromConfig (configPath: string) : TopicDefinition list =
    let lines = File.ReadAllLines(configPath) |> Array.toList

    let rec dropUntilTopics (xs: string list) =
        match xs with
        | [] -> []
        | h :: t when h.Trim() = "topics:" -> t
        | _ :: t -> dropUntilTopics t

    let rec parseTopic (acc: TopicDefinition list) (remaining: string list) =
        match remaining with
        | [] -> List.rev acc
        | line :: rest ->
            if line.TrimStart().StartsWith("-") && line.TrimStart().StartsWith("- id:") then
                let id = line.Split(':', 2).[1].Trim()

                let readField (key: string) (xs: string list) =
                    match xs with
                    | h :: t when h.TrimStart().StartsWith(key + ":") ->
                        Some(h.Split(':', 2).[1].Trim()), t
                    | _ -> None, xs

                let (nameOpt, rest1) = readField "name" rest
                let (descOpt, rest2) = readField "description" rest1
                let (legacyOpt, rest3) = readField "legacy_tags" rest2

                let legacy =
                    match legacyOpt with
                    | None -> []
                    | Some raw ->
                        // Expect flow sequence in config.
                        let tmpFrontMatter = sprintf "legacy_tags: %s" raw
                        tryGetFlowSeq tmpFrontMatter "legacy_tags"

                let topic =
                    { id = id
                      name = nameOpt |> Option.defaultValue ""
                      description = descOpt |> Option.defaultValue ""
                      legacy_tags = legacy }

                parseTopic (topic :: acc) rest3
            else
                // stop parsing when we hit a non-indented key at column 0
                if not (String.IsNullOrWhiteSpace line) && not (Char.IsWhiteSpace line.[0]) then
                    List.rev acc
                else
                    parseTopic acc rest

    dropUntilTopics lines |> parseTopic []

let computeTopics (topicDefs: TopicDefinition list) (category: string option) (tags: string list) =
    let legacyTerms =
        [ yield! category |> Option.toList
          yield! tags ]

    let legacyTermsNormalized = legacyTerms |> List.map normalize |> Set.ofList

    let topicMatches =
        topicDefs
        |> List.choose (fun t ->
            let normalizedLegacyTags = t.legacy_tags |> List.map normalize |> Set.ofList
            if Set.intersect legacyTermsNormalized normalizedLegacyTags |> Set.isEmpty then
                None
            else
                Some t.id)
        |> List.sort

    let mappedLegacyTerms =
        topicDefs
        |> List.collect (fun t -> t.legacy_tags)
        |> List.map normalize
        |> Set.ofList

    let unmappedTags =
        tags
        |> List.filter (fun t -> not (mappedLegacyTerms.Contains(normalize t)))

    let unmappedCategory =
        match category with
        | None -> None
        | Some c -> if mappedLegacyTerms.Contains(normalize c) then None else Some c

    topicMatches, unmappedCategory, unmappedTags

let rootDir = __SOURCE_DIRECTORY__ |> Directory.GetParent |> fun p -> p.Parent.FullName
let configPath = Path.Combine(rootDir, "_config.yml")
let outputPath = Path.Combine(rootDir, "topic-migration.json")

let topicDefs = parseTopicsFromConfig configPath

if topicDefs.Length = 0 then
    failwithf "No topics parsed from %s" configPath

let items =
    readAllMarkdownFiles rootDir
    |> List.map (fun path ->
        let relPath = Path.GetRelativePath(rootDir, path).Replace("\\", "/")
        let content = File.ReadAllText(path)
        let (fm, _, _) = splitFrontMatter content

        // Some drafts may be empty placeholders without front matter.
        // Treat these as having no legacy metadata.
        let category =
            match getLegacyCategory fm with
            | [] -> None
            | xs -> xs |> List.tryHead

        let tags = getLegacyTags fm
        let permalink = tryGetScalarLine fm "permalink"

        let (topicIds, unmappedCategory, unmappedTags) = computeTopics topicDefs category tags

        { source_path = relPath
          permalink = permalink
          legacy =
            { category = category
              tags = tags }
          topics = topicIds
          unmapped =
            { category = unmappedCategory
              tags = unmappedTags } })
    |> List.sortBy (fun i -> i.source_path)

let output =
    { topics = topicDefs |> List.map (fun t -> { id = t.id; name = t.name })
      items = items }

let jsonOptions = JsonSerializerOptions(WriteIndented = true)
let json = JsonSerializer.Serialize(output, jsonOptions) + "\n"
File.WriteAllText(outputPath, json)

printfn "Wrote %d items to %s" items.Length outputPath
