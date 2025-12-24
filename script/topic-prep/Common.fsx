module TopicPrep.Common

open System
open System.IO
open System.Text.RegularExpressions

let normalize (value: string) =
    value.Trim().ToLowerInvariant()

let slugify (value: string) =
    if String.IsNullOrWhiteSpace value then
        value
    else
        value
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
        |> fun s -> Regex.Replace(s, "[^a-z0-9\-]", "")

let readAllMarkdownFiles (root: string) =
    [ "_posts"; "_drafts" ]
    |> List.collect (fun dir ->
        let full = Path.Combine(root, dir)
        if Directory.Exists full then
            Directory.GetFiles(full, "*.md", SearchOption.AllDirectories) |> Array.toList
        else
            [])

let splitFrontMatter (content: string) : string * string * string =
    // returns (frontMatter, body, newline)
    // newline is preserved as either "\r\n" or "\n" based on file content
    let newline = if content.Contains("\r\n") then "\r\n" else "\n"

    if content.StartsWith("---") then
        let parts = Regex.Split(content, "^---\\s*$", RegexOptions.Multiline)

        if parts.Length >= 3 then
            let frontMatter = parts.[1].Trim('\r', '\n')
            let body = String.Join(newline, parts.[2..]).TrimStart('\r', '\n')
            frontMatter, body, newline
        else
            "", content, newline
    else
        "", content, newline

type FrontMatterRegion =
    { prefix: string
      frontMatter: string
      suffix: string
      newline: string }

let tryGetFrontMatterRegion (content: string) : FrontMatterRegion option =
    let newline = if content.Contains("\r\n") then "\r\n" else "\n"

    if not (content.StartsWith("---")) then
        None
    else
        let firstLineEnd = content.IndexOf(newline, StringComparison.Ordinal)

        if firstLineEnd < 0 then
            None
        else
            let prefixLen = firstLineEnd + newline.Length
            let fmStart = prefixLen

            let rec findClosing (searchStart: int) =
                let idx = content.IndexOf(newline + "---", searchStart, StringComparison.Ordinal)

                if idx < 0 then
                    None
                else
                    let lineStart = idx + newline.Length
                    let lineEnd =
                        match content.IndexOf(newline, lineStart, StringComparison.Ordinal) with
                        | -1 -> content.Length
                        | x -> x

                    let lineText = content.Substring(lineStart, lineEnd - lineStart)

                    if lineText.Trim() = "---" then
                        Some idx
                    else
                        findClosing (lineStart + 1)

            match findClosing fmStart with
            | None -> None
            | Some idx ->
                let fm = content.Substring(fmStart, idx - fmStart)

                Some
                    { prefix = content.Substring(0, fmStart)
                      frontMatter = fm.Trim('\r', '\n')
                      suffix = content.Substring(idx)
                      newline = newline }

let tryGetScalarLine (frontMatter: string) (key: string) =
    let pattern = sprintf "(?m)^%s:\\s*(.+)\\s*$" (Regex.Escape key)
    let m = Regex.Match(frontMatter, pattern)
    if m.Success then Some(m.Groups.[1].Value.Trim()) else None

let private trimYamlQuotes (value: string) =
    let trimmed = value.Trim()
    if (trimmed.StartsWith("\"") && trimmed.EndsWith("\"")) || (trimmed.StartsWith("'") && trimmed.EndsWith("'")) then
        trimmed.Substring(1, trimmed.Length - 2)
    else
        trimmed

let tryGetFlowSeq (frontMatter: string) (key: string) : string list =
    match tryGetScalarLine frontMatter key with
    | None -> []
    | Some raw ->
        let v = raw.Trim()

        let inner =
            if v.StartsWith("[") && v.EndsWith("]") then
                v.Substring(1, v.Length - 2)
            else
                v

        inner.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun s -> trimYamlQuotes s |> fun x -> x.Trim())
        |> Array.filter (fun s -> s <> "")
        |> Array.toList

let getLegacyCategory (frontMatter: string) =
    match tryGetFlowSeq frontMatter "categories" with
    | cats when cats.Length > 0 -> cats
    | _ ->
        match tryGetScalarLine frontMatter "category" with
        | Some c when c <> "" -> [ trimYamlQuotes c ]
        | _ -> []

let getLegacyTags (frontMatter: string) =
    let tags = tryGetFlowSeq frontMatter "tags"
    if tags.Length > 0 then tags else tryGetFlowSeq frontMatter "TAGS"

let formatFlowSeq (values: string list) =
    match values with
    | [] -> "[]"
    | xs ->
        // No quoting for now; existing content uses unquoted values.
        // If a value contains commas, this would break; we assume tags do not.
        xs |> String.concat ", " |> sprintf "[%s]"

let upsertFrontMatterKey (frontMatter: string) (key: string) (valueText: string) : string =
    let pattern = sprintf "(?m)^%s:\\s*.*$" (Regex.Escape key)

    if Regex.IsMatch(frontMatter, pattern) then
        Regex.Replace(frontMatter, pattern, sprintf "%s: %s" key valueText)
    else
        // Insert before end-of-front-matter. Keep a trailing newline for nice formatting.
        if String.IsNullOrWhiteSpace frontMatter then
            sprintf "%s: %s" key valueText
        else
            frontMatter.TrimEnd('\r', '\n') + "\n" + sprintf "%s: %s" key valueText

let writeFrontMatterFile (path: string) (frontMatter: string) (body: string) (newline: string) =
    let updated = String.Join(newline, [ "---"; frontMatter.TrimEnd('\r', '\n'); "---"; body ]) + newline
    File.WriteAllText(path, updated)

let stableDistinct (values: string list) =
    let seen = System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
    values
    |> List.filter (fun v ->
        let trimmed = v.Trim()
        if trimmed = "" then false
        else seen.Add(trimmed))
