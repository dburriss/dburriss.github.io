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
            | :? YamlSequenceNode as seqNode ->
                seqNode.Children
                |> Seq.choose yamlScalar
                |> Seq.toList
            | :? YamlScalarNode as scalar when not (String.IsNullOrWhiteSpace scalar.Value) ->
                scalar.Value.Split([|','|], StringSplitOptions.RemoveEmptyEntries)
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
                [| "yyyy-MM-dd"; "yyyy-MM-ddTHH:mm:ssZ"; "yyyy-MM-ddTHH:mm:ss"; "MM/dd/yyyy"; "yyyy/MM/dd" |]
            let mutable parsed = DateTime.MinValue
            if DateTime.TryParseExact(v, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, &parsed) then
                Some (parsed.ToUniversalTime())
            else
                match DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, &parsed) with
                | true -> Some (parsed.ToUniversalTime())
                | _ -> None
        | None -> None

    let private normalizeList (items: string list) =
        items
        |> List.map (fun s -> s.Trim())
        |> List.filter (fun s -> s <> "")

    let private loadYamlMapping (yamlText: string) =
        use reader = new StringReader(yamlText)
        let yaml = YamlStream()
        yaml.Load(reader)
        yaml.Documents.[0].RootNode :?> YamlMappingNode

    let parseSiteConfig (configPath: string) =
        let text = File.ReadAllText(configPath)
        let root = loadYamlMapping text
        let get = tryGetScalar root
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
            | _ -> false }

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
              Date = None
              Comments = None
              Published = None }
        else
            let mapping = loadYamlMapping frontMatter
            let get = tryGetScalar mapping
            let tags =
                let primary = tryGetSequence mapping "tags"
                if primary.Length = 0 then tryGetSequence mapping "TAGS" else primary
            let categories =
                let cats = tryGetSequence mapping "categories"
                if cats.Length > 0 then cats
                else
                    match get "category" with
                    | Some value -> [ value ]
                    | None -> []
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
              Date = parseDate (get "date")
              Comments = parseBool (get "comments")
              Published = parseBool (get "published") }

    let private excerptSeparator = "<!--more-->"

    let extractExcerpt (html: string) =
        let idx = html.IndexOf(excerptSeparator, StringComparison.OrdinalIgnoreCase)
        if idx >= 0 then
            Some (html.Substring(0, idx))
        else
            None

    let markdownPipeline =
        MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePipeTables()
            .UseYamlFrontMatter()
            .Build()

    let markdownToHtml (markdown: string) =
        Markdown.ToHtml(markdown, markdownPipeline)

    let readAllMarkdown (root: string) (pattern: string) =
        Directory.GetFiles(root, pattern, SearchOption.AllDirectories)
        |> Array.toList

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

    let combineUrl (baseUrl: string) (relative: string) =
        let trimmedBase = if baseUrl.EndsWith("/") then baseUrl else baseUrl + "/"
        let trimmed = relative.TrimStart('/')
        trimmedBase + trimmed
