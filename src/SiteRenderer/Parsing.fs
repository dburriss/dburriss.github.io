namespace SiteRenderer

open System
open System.IO
open System.Globalization
open System.Text.RegularExpressions
open Markdig
open YamlDotNet.RepresentationModel

module Parsing =

    let private yamlScalar (node: YamlNode) =
        match node with
        | :? YamlScalarNode as scalar when not (isNull scalar.Value) -> Some scalar.Value
        | _ -> None

    let private tryGetScalar (mapping: YamlMappingNode) (key: string) =
        let keyNode = YamlScalarNode(key)

        if mapping.Children.ContainsKey keyNode then
            mapping.Children.[keyNode] |> yamlScalar
        else
            None

    let private tryGetSequence (mapping: YamlMappingNode) (key: string) =
        let keyNode = YamlScalarNode(key)

        if mapping.Children.ContainsKey keyNode then
            match mapping.Children.[keyNode] with
            | :? YamlSequenceNode as seqNode -> seqNode.Children |> Seq.choose yamlScalar |> Seq.toList
            | :? YamlScalarNode as scalar when not (String.IsNullOrWhiteSpace scalar.Value) ->
                scalar.Value.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map (fun s -> s.Trim())
                |> Array.toList
            | _ -> []
        else
            []

    let private parseBool (value: string option) =
        match value with
        | Some v when v.Equals("true", StringComparison.OrdinalIgnoreCase) -> Some true
        | Some v when v.Equals("false", StringComparison.OrdinalIgnoreCase) -> Some false
        | _ -> None

    let private parseDate (value: string option) =
        match value with
        | Some v ->
            let formats =
                [| "yyyy-MM-dd"
                   "yyyy-MM-ddTHH:mm:ssZ"
                   "yyyy-MM-ddTHH:mm:ss"
                   "MM/dd/yyyy"
                   "yyyy/MM/dd" |]

            let mutable parsed = DateTime.MinValue

            if
                DateTime.TryParseExact(
                    v,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    &parsed
                )
            then
                Some(parsed.ToUniversalTime())
            else
                match DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, &parsed) with
                | true -> Some(parsed.ToUniversalTime())
                | _ -> None
        | None -> None

    let private normalizeList (items: string list) =
        items |> List.map (fun s -> s.Trim()) |> List.filter (fun s -> s <> "")

    let private loadYamlMapping (yamlText: string) =
        use reader = new StringReader(yamlText)
        let yaml = YamlStream()
        yaml.Load(reader)
        yaml.Documents.[0].RootNode :?> YamlMappingNode

    let parseSiteConfig (configPath: string) =
        let text = File.ReadAllText(configPath)
        let root = loadYamlMapping text
        let get = tryGetScalar root

        let topics =
            let keyNode = YamlScalarNode("topics")

            if root.Children.ContainsKey keyNode then
                match root.Children.[keyNode] with
                | :? YamlSequenceNode as seqNode ->
                    seqNode.Children
                    |> Seq.choose (fun node ->
                        match node with
                        | :? YamlMappingNode as m ->
                            let gt = tryGetScalar m
                            let id = gt "id" |> Option.defaultValue ""
                            let name = gt "name" |> Option.defaultValue id
                            let desc = gt "description" |> Option.defaultValue ""
                            let legacyCat = gt "legacy_category"
                            let legacyTags = tryGetSequence m "legacy_tags" |> List.map (fun s -> s.Trim())

                            Some
                                { Id = id
                                  Name = name
                                  Description = desc
                                  LegacyCategory = legacyCat
                                  LegacyTags = legacyTags }
                        | _ -> None)
                    |> Seq.toList
                | _ -> []
            else
                []

        { Title = get "title" |> Option.defaultValue ""
          Description = get "description" |> Option.defaultValue ""
          Author = get "author" |> Option.defaultValue ""
          Url = get "url" |> Option.defaultValue "/"
          BaseUrl = get "baseurl" |> Option.defaultValue "/"
          HeaderImage = get "header-img"
          SocialImage = get "social-img"
          TwitterUsername = get "twitter_username"
          GithubUsername = get "github_username"
          FacebookUsername = get "facebook_username"
          EmailUsername = get "email_username"
          DisqusUsername = get "disqus_username"
          GoogleTrackingId = get "google_tracking_id"
          CopyrightName = get "copyright_name"
          IsProduction =
            match get "is_production" with
            | Some v when v.Equals("true", StringComparison.OrdinalIgnoreCase) -> true
            | _ -> false
          Include = tryGetSequence root "include"
          Topics = topics }

    let private splitFrontMatter (content: string) =
        if content.StartsWith("---") then
            let parts = Regex.Split(content, "^---\\s*$", RegexOptions.Multiline)

            if parts.Length >= 3 then
                let frontMatter = parts.[1]
                let body = String.Join("\n", parts.[2..])
                frontMatter.Trim(), body.TrimStart()
            else
                "", content
        else
            "", content

    let parseMarkdownFile (path: string) =
        let raw = File.ReadAllText(path)
        splitFrontMatter raw

    let parseFrontMatter (frontMatter: string) =
        if String.IsNullOrWhiteSpace(frontMatter) then
            { Layout = None
              Title = None
              Subtitle = None
              Author = None
              Description = None
              Permalink = None
              HeaderImage = None
              SocialImage = None
              Tags = []
              Categories = []
              Topics = []
              Keywords = []
              Date = None
              Comments = None
              Published = None
              Status = None }
        else
            let mapping = loadYamlMapping frontMatter
            let get = tryGetScalar mapping

            let tags =
                let primary = tryGetSequence mapping "tags"

                if primary.Length = 0 then
                    tryGetSequence mapping "TAGS"
                else
                    primary

            let categories =
                let cats = tryGetSequence mapping "categories"

                if cats.Length > 0 then
                    cats
                else
                    match get "category" with
                    | Some value -> [ value ]
                    | None -> []

            let topics = tryGetSequence mapping "topics"
            let keywords = tryGetSequence mapping "keywords"

            { Layout = get "layout"
              Title = get "title"
              Subtitle = get "subtitle"
              Author = get "author"
              Description =
                match get "description" with
                | Some d -> Some d
                | None -> get "excerpt"
              Permalink =
                match get "permalink" with
                | Some p when not (String.IsNullOrWhiteSpace p) -> Some p
                | _ -> None
              HeaderImage =
                match get "header-img" with
                | Some v -> Some v
                | None -> get "header_img"
              SocialImage =
                match get "social-img" with
                | Some v -> Some v
                | None -> get "social_img"
              Tags = normalizeList tags
              Categories = normalizeList categories
              Topics = normalizeList topics
              Keywords = normalizeList keywords
              Date = parseDate (get "date")
              Comments = parseBool (get "comments")
              Published = parseBool (get "published")
              Status = get "status" }

    let private excerptSeparator = "<!--more-->"

    let extractExcerpt (html: string) =
        let idx = html.IndexOf(excerptSeparator, StringComparison.OrdinalIgnoreCase)
        if idx >= 0 then Some(html.Substring(0, idx)) else None

    let markdownPipeline =
        MarkdownPipelineBuilder().UseAdvancedExtensions().UsePipeTables().UseYamlFrontMatter().UseWikiLinks().Build()

    /// Create a context-aware markdown pipeline with wiki link resolution
    let createContextAwarePipeline (resolutionContext: ResolutionContext option) =
        match resolutionContext with
        | Some context ->
            // Build pipeline with specific extensions to avoid conflicts
            MarkdownPipelineBuilder()
                .UsePipeTables()
                .UseYamlFrontMatter()
                .UseWikiLinks(context)
                .UseEmphasisExtras()
                .UseListExtras()
                .UseAutoIdentifiers()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmojiAndSmiley()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                .UseMathematics()
                .UseMediaLinks()
                .UseSmartyPants()
                .UseSoftlineBreakAsHardlineBreak()
                .UseTaskLists()
                .Build()
        | None ->
            // Use default pipeline
            markdownPipeline

    let markdownToHtmlWithContext (markdown: string) (resolutionContext: ResolutionContext option) =
        let pipeline = createContextAwarePipeline resolutionContext
        Markdown.ToHtml(markdown, pipeline)

    let markdownToHtml (markdown: string) = markdownToHtmlWithContext markdown None

    let markdownToPlainText (markdown: string) =
        Markdown.ToPlainText(markdown, markdownPipeline)

    let readAllMarkdown (root: string) (pattern: string) =
        Directory.GetFiles(root, pattern, SearchOption.AllDirectories) |> Array.toList

    let slugify (value: string) =
        if String.IsNullOrWhiteSpace value then
            value
        else
            value.ToLowerInvariant().Replace(" ", "-").Replace("'", "").Replace("\"", "")
            |> fun s -> Regex.Replace(s, "[^a-z0-9\-]", "")

    let combineUrl (baseUrl: string) (relative: string) =
        let trimmedBase = if baseUrl.EndsWith("/") then baseUrl else baseUrl + "/"
        let trimmed = relative.TrimStart('/')
        trimmedBase + trimmed

    /// Check if a wiki link label should be ignored (not treated as a wiki link)
    let isWikiLinkIgnored (label: string) : bool =
        let trimmed = label.Trim()

        // Ignore common bash regex patterns and sed expressions
        let ignorePatterns =
            [ "^:space:$" // [[:space:]]
              "^:alnum:$" // [[:alnum:]]
              "^:alpha:$" // [[:alpha:]]
              "^:digit:$" // [[:digit:]]
              "^:upper:$" // [[:upper:]]
              "^:lower:$" // [[:lower:]]
              "^:blank:$" // [[:blank:]]
              "^:cntrl:$" // [[:cntrl:]]
              "^:graph:$" // [[:graph:]]
              "^:print:$" // [[:print:]]
              "^:punct:$" // [[:punct:]]
              "^:xdigit:$" ] // [[:xdigit:]]

        ignorePatterns |> List.exists (fun pattern -> Regex.IsMatch(trimmed, pattern))

    /// Extract wiki-style [[links]] from markdown text, returning title and display text tuples
    let extractWikiLinks (markdown: string) : (string * string option) list =
        let pattern = @"\[\[([^\]]+)\]\]"
        let matches = Regex.Matches(markdown, pattern)

        [ for m in matches do
              let content = m.Groups.[1].Value.Trim()
              let parts = content.Split([| '|' |], 2, StringSplitOptions.None)

              let title = parts.[0].Trim()

              let displayText =
                  if parts.Length > 1 && not (String.IsNullOrWhiteSpace(parts.[1])) then
                      Some(parts.[1].Trim())
                  else
                      None

              // Skip wiki links that match ignore patterns (using title for checking)
              if not (isWikiLinkIgnored title) then
                  (title, displayText) ]

    /// Normalize a wiki link label for matching (trim, normalize spaces, case-insensitive)
    let normalizeWikiLabel (label: string) : string =
        label.Trim().Replace("  ", " ").ToLowerInvariant()
