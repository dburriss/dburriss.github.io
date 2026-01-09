namespace SiteRenderer

open System
open System.Globalization
open System.IO
open System.Text.RegularExpressions
open System.Collections.Generic
open Giraffe.ViewEngine
open Microsoft.Extensions.FileSystemGlobbing

module Renderer =

    let private postUrlFromPath (sourcePath: string) (date: DateTime option) =
        let fileName = Path.GetFileNameWithoutExtension(sourcePath)
        // Posts typically have format: yyyy-MM-dd-slug.md
        let datePattern = Regex(@"^(\d{4})-(\d{2})-(\d{2})-(.+)$")
        let m = datePattern.Match(fileName)

        if m.Success then
            let year = m.Groups.[1].Value
            let month = m.Groups.[2].Value
            let day = m.Groups.[3].Value
            let slug = m.Groups.[4].Value
            sprintf "%s/%s/%s/%s/" year month day slug
        else
            // Fallback: use the filename as slug
            sprintf "%s/" fileName

    let private pageUrlFromPath (sourcePath: string) =
        let fileName = Path.GetFileNameWithoutExtension(sourcePath)

        if fileName = "index" then
            ""
        else
            sprintf "%s.html" fileName

    let private noteUrlFromPath (sourcePath: string) =
        let fileName = Path.GetFileNameWithoutExtension(sourcePath)
        let slug = Parsing.slugify fileName
        sprintf "notes/%s/" slug

    let private buildPostSummary (item: ContentItem) : PostSummary =
        { Title = item.PageMeta.Title
          Url = item.PageMeta.Url
          Date = item.PageMeta.Date
          Subtitle = item.PageMeta.Subtitle
          SocialImage = item.Meta.SocialImage }

    let private loadContentItem (sourcePath: string) (kind: string) : ContentItem =
        let frontMatter, body = Parsing.parseMarkdownFile sourcePath
        let meta = Parsing.parseFrontMatter frontMatter
        let htmlContent = Parsing.markdownToHtml body
        let excerpt = Parsing.extractExcerpt htmlContent

        let date =
            match meta.Date with
            | Some d -> Some d
            | None ->
                // Try extracting date from filename
                let fileName = Path.GetFileNameWithoutExtension(sourcePath)
                let datePattern = Regex(@"^(\d{4})-(\d{2})-(\d{2})")
                let m = datePattern.Match(fileName)

                if m.Success then
                    let dateStr =
                        sprintf "%s-%s-%s" m.Groups.[1].Value m.Groups.[2].Value m.Groups.[3].Value

                    match DateTime.TryParse(dateStr) with
                    | true, d -> Some d
                    | _ -> None
                else
                    None

        let url =
            match meta.Permalink with
            | Some p -> p.TrimStart('/').TrimEnd('/') + "/"
            | None ->
                match kind with
                | "post" -> postUrlFromPath sourcePath date
                | "note" -> noteUrlFromPath sourcePath
                | _ -> pageUrlFromPath sourcePath

        let outputPath =
            if url.EndsWith("/") then url + "index.html"
            elif url.EndsWith(".html") then url
            elif url = "" then "index.html"
            else url + "/index.html"

        let layout =
            meta.Layout
            |> Option.defaultValue (
                match kind with
                | "post" -> "post"
                | "note" -> "note"
                | _ -> "page"
            )

        let commentsEnabled =
            match meta.Comments with
            | Some c -> c
            | None -> kind = "post"

        let pageMeta =
            { Title = meta.Title |> Option.defaultValue (Path.GetFileNameWithoutExtension(sourcePath))
              Subtitle = meta.Subtitle
              Description = meta.Description
              Author = meta.Author
              Date = date
              HeaderImage = meta.HeaderImage
              SocialImage = meta.SocialImage
              Url = "/" + url
              Tags = meta.Tags
              Categories = meta.Categories
              Topics = meta.Topics
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = commentsEnabled
              Layout = layout
              // Additional metadata fields
              Status = meta.Status
              Published = meta.Published
              Keywords = meta.Keywords
              Permalink = meta.Permalink }

        { SourcePath = sourcePath
          OutputPath = outputPath
          Markdown = Some body
          HtmlContent = htmlContent
          ExcerptHtml = excerpt
          Meta = meta
          PageMeta = pageMeta
          Kind = kind }

    let loadPosts (postsDir: string) : ContentItem list =
        if Directory.Exists(postsDir) then
            Parsing.readAllMarkdown postsDir "*.md"
            |> List.map (fun path -> loadContentItem path "post")
            |> List.filter (fun item ->
                match item.Meta.Published with
                | Some false -> false
                | _ -> true)
            |> List.sortByDescending (fun item -> item.PageMeta.Date |> Option.defaultValue DateTime.MinValue)
        else
            []

    let loadPages (root: string) : ContentItem list =
        let pagesDir = root

        Directory.GetFiles(pagesDir, "*.md", SearchOption.TopDirectoryOnly)
        |> Array.toList
        |> List.filter (fun path ->
            let fileName = Path.GetFileName(path)
            not (fileName.StartsWith("_")))
        |> List.map (fun path -> loadContentItem path "page")

    let loadNotes (notesDir: string) : ContentItem list =
        if Directory.Exists(notesDir) then
            Parsing.readAllMarkdown notesDir "*.md"
            |> List.map (fun path -> loadContentItem path "note")
            |> List.sortBy (fun item -> item.PageMeta.Title)
        else
            []

    let private assignPreviousNext (posts: ContentItem list) : ContentItem list =
        let indexed = posts |> List.mapi (fun i p -> (i, p))

        indexed
        |> List.map (fun (i, post) ->
            let prev =
                if i < posts.Length - 1 then
                    Some(buildPostSummary posts.[i + 1])
                else
                    None

            let next = if i > 0 then Some(buildPostSummary posts.[i - 1]) else None

            { post with
                PageMeta =
                    { post.PageMeta with
                        Previous = prev
                        Next = next } })

    let buildSiteIndex (posts: ContentItem list) (pages: ContentItem list) (notes: ContentItem list) : SiteIndex =
        let postsWithNav = assignPreviousNext posts

        // Combine posts and notes for topic indexing
        let allContent = postsWithNav @ notes

        let categories =
            postsWithNav
            |> List.collect (fun p -> p.PageMeta.Categories |> List.map (fun c -> (c, p)))
            |> List.groupBy fst
            |> List.map (fun (cat, items) -> (cat, items |> List.map snd))
            |> Map.ofList

        let tags =
            postsWithNav
            |> List.collect (fun p -> p.PageMeta.Tags |> List.map (fun t -> (t, p)))
            |> List.groupBy fst
            |> List.map (fun (tag, items) -> (tag, items |> List.map snd))
            |> Map.ofList

        let topics =
            allContent
            |> List.collect (fun p -> p.PageMeta.Topics |> List.map (fun t -> (t, p)))
            |> List.groupBy fst
            |> List.map (fun (topic, items) -> (topic, items |> List.map snd))
            |> Map.ofList

        { Posts = postsWithNav
          Pages = pages
          Notes = notes
          Categories = categories
          Tags = tags
          Topics = topics
          LinkGraph = Map.empty }

    /// Build a lookup map from normalized titles to content items (notes and posts)
    let private buildTitleLookup (allContent: ContentItem list) : Map<string, ContentItem list> =
        allContent
        |> List.groupBy (fun item -> Parsing.normalizeWikiLabel item.PageMeta.Title)
        |> Map.ofList

    /// Resolve wiki links in all content and build the link graph
    let resolveWikiLinks (index: SiteIndex) : SiteIndex * string list =
        let allContent = index.Posts @ index.Notes
        let titleLookup = buildTitleLookup allContent

        let mutable warnings = []

        // For each content item, extract wiki links from its markdown and resolve them
        let linkGraph =
            allContent
            |> List.map (fun item ->
                match item.Markdown with
                | Some markdown ->
                    let wikiLinkTuples = Parsing.extractWikiLinks markdown

                    let outboundLinks =
                        wikiLinkTuples
                        |> List.map (fun (title, displayText) ->
                            let normalizedTitle = Parsing.normalizeWikiLabel title

                            match Map.tryFind normalizedTitle titleLookup with
                            | Some targets ->
                                match targets with
                                | [ target ] ->
                                    // Exactly one match - resolved
                                    { SourceUrl = item.PageMeta.Url
                                      TargetTitle = title
                                      TargetDisplayText = displayText
                                      ResolvedUrl = Some target.PageMeta.Url
                                      IsResolved = true }
                                | _ :: _ ->
                                    // Multiple matches - ambiguous, prioritize notes over posts
                                    let notes = targets |> List.filter (fun t -> t.Kind = "note")

                                    let resolvedTarget =
                                        match notes with
                                        | [ singleNote ] ->
                                            warnings <-
                                                (sprintf
                                                    "Wiki link [[%s]] in %s is ambiguous but resolved to note (over post)"
                                                    title
                                                    item.PageMeta.Url)
                                                :: warnings

                                            Some singleNote
                                        | _ -> None

                                    match resolvedTarget with
                                    | Some target ->
                                        { SourceUrl = item.PageMeta.Url
                                          TargetTitle = title
                                          TargetDisplayText = displayText
                                          ResolvedUrl = Some target.PageMeta.Url
                                          IsResolved = true }
                                    | None ->
                                        warnings <-
                                            (sprintf
                                                "Ambiguous wiki link [[%s]] in %s (matches %d items)"
                                                title
                                                item.PageMeta.Url
                                                targets.Length)
                                            :: warnings

                                        { SourceUrl = item.PageMeta.Url
                                          TargetTitle = title
                                          TargetDisplayText = displayText
                                          ResolvedUrl = None
                                          IsResolved = false }
                                | [] ->
                                    // Empty list - unresolved
                                    warnings <-
                                        (sprintf "Unresolved wiki link [[%s]] in %s" title item.PageMeta.Url)
                                        :: warnings

                                    { SourceUrl = item.PageMeta.Url
                                      TargetTitle = title
                                      TargetDisplayText = displayText
                                      ResolvedUrl = None
                                      IsResolved = false }
                            | None ->
                                // No match - unresolved
                                warnings <-
                                    (sprintf "Unresolved wiki link [[%s]] in %s" title item.PageMeta.Url)
                                    :: warnings

                                { SourceUrl = item.PageMeta.Url
                                  TargetTitle = title
                                  TargetDisplayText = displayText
                                  ResolvedUrl = None
                                  IsResolved = false })

                    (item.PageMeta.Url,
                     { OutboundLinks = outboundLinks
                       InboundLinks = [] })
                | None ->
                    (item.PageMeta.Url,
                     { OutboundLinks = []
                       InboundLinks = [] }))
            |> Map.ofList

        // Build inbound links by inverting the outbound links
        let linkGraphWithBacklinks =
            linkGraph
            |> Map.map (fun sourceUrl metadata ->
                let inboundLinks =
                    linkGraph
                    |> Map.toList
                    |> List.collect (fun (otherUrl, otherMetadata) ->
                        otherMetadata.OutboundLinks
                        |> List.choose (fun link ->
                            if link.IsResolved && link.ResolvedUrl = Some sourceUrl && otherUrl <> sourceUrl then
                                Some otherUrl
                            else
                                None))

                { metadata with
                    InboundLinks = inboundLinks })

        ({ index with
            LinkGraph = linkGraphWithBacklinks },
         warnings)

    /// Detect orphaned notes (notes with no inbound links and not in navigation)
    let detectOrphanedNotes (index: SiteIndex) : string list =
        index.Notes
        |> List.filter (fun note ->
            // Only consider published notes
            match note.PageMeta.Published with
            | Some false -> false
            | _ ->
                // Check if the note has any inbound links
                match Map.tryFind note.PageMeta.Url index.LinkGraph with
                | Some metadata -> List.isEmpty metadata.InboundLinks
                | None -> true)
        |> List.map (fun note -> sprintf "Orphaned note: %s (%s)" note.PageMeta.Title note.PageMeta.Url)

    let private categoryCounts (index: SiteIndex) : (string * int) list =
        index.Categories
        |> Map.toList
        |> List.map (fun (cat, posts) -> (cat, posts.Length))
        |> List.sortBy fst

    let private tagCounts (index: SiteIndex) : (string * int) list =
        index.Tags
        |> Map.toList
        |> List.map (fun (tag, posts) -> (tag, posts.Length))
        |> List.sortBy fst

    let private tryMapCategoryToTopic (site: SiteConfig) (category: string) : string option =
        site.Topics
        |> List.tryFind (fun t ->
            t.LegacyCategory
            |> Option.map (fun c -> String.Equals(c, category, StringComparison.OrdinalIgnoreCase))
            |> Option.defaultValue false)
        |> Option.map (fun t -> t.Id)

    let private tryMapTagToTopic (site: SiteConfig) (tag: string) : string option =
        site.Topics
        |> List.tryFind (fun t ->
            t.LegacyTags
            |> List.exists (fun lt -> String.Equals(lt, tag, StringComparison.OrdinalIgnoreCase)))
        |> Option.map (fun t -> t.Id)

    let private distinctIgnoreCase (values: string list) : string list =
        let seen = HashSet<string>(StringComparer.OrdinalIgnoreCase)

        values
        |> List.choose (fun v ->
            let trimmed = v.Trim()

            if String.IsNullOrWhiteSpace trimmed then None
            else if seen.Add trimmed then Some trimmed
            else None)

    let private configuredLegacyCategories (site: SiteConfig) : string list =
        site.Topics |> List.choose (fun t -> t.LegacyCategory) |> distinctIgnoreCase

    let private configuredLegacyTags (site: SiteConfig) : string list =
        site.Topics |> List.collect (fun t -> t.LegacyTags) |> distinctIgnoreCase

    let private redirectHtml (target: string) =
        // Minimal HTML document with meta refresh
        sprintf
            "<!DOCTYPE html><html><head><meta http-equiv=\"refresh\" content=\"0; url='%s'\" /></head><body><p>Redirecting to <a href=\"%s\">%s</a>…</p></body></html>"
            target
            target
            target

    let renderTopicsOverviewPage (ctx: RenderContext) : RenderedPage =
        let pageMeta =
            { Title = "Topics"
              Subtitle = None
              Description = Some "Browse by topic"
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = "topics/"
              Tags = []
              Categories = []
              Topics = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "page"
              Status = None
              Published = None
              Keywords = []
              Permalink = None }

        let topicNodes =
            ctx.Config.Topics
            |> List.map (fun t ->
                li
                    []
                    [ a
                          [ _class "topic-pill"
                            _href (Parsing.combineUrl ctx.Config.BaseUrl (sprintf "topics/%s/" t.Id))
                            _title t.Description ]
                          [ str t.Name ]
                      p [] [ str t.Description ] ])

        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index

        let html = RenderView.AsString.htmlNodes [ ul [] topicNodes ]

        let doc = Layouts.pageDocument ctx.Config pageMeta html cats tags

        { OutputPath = "topics/index.html"
          Content = RenderView.AsString.htmlDocument doc }

    let renderTopicPage (ctx: RenderContext) (topicId: string) (posts: ContentItem list) : RenderedPage =
        let topicOpt = ctx.Config.Topics |> List.tryFind (fun t -> t.Id = topicId)

        let title, desc =
            match topicOpt with
            | Some t -> sprintf "Topic: %s" t.Name, Some t.Description
            | None -> sprintf "Topic: %s" topicId, None

        let page =
            { Title = title
              Subtitle = None
              Description = desc
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = sprintf "topics/%s/" topicId
              Tags = []
              Categories = []
              Topics = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "topics"
              Status = None
              Published = None
              Keywords = []
              Permalink = None }

        let formatDate (date: DateTime option) =
            match date with
            | Some d -> d.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture)
            | None -> ""

        // Separate posts and notes
        let postItems = posts |> List.filter (fun p -> p.Kind = "post")
        let noteItems = posts |> List.filter (fun p -> p.Kind = "note")

        let postLinks =
            postItems
            |> List.map (fun p ->
                li
                    []
                    [ a [ _href (Parsing.combineUrl ctx.Config.BaseUrl p.PageMeta.Url) ] [ str p.PageMeta.Title ]
                      span [ _class "post-date" ] [ str (formatDate p.PageMeta.Date) ] ])

        let noteLinks =
            noteItems
            |> List.map (fun n ->
                li [] [ a [ _href (Parsing.combineUrl ctx.Config.BaseUrl n.PageMeta.Url) ] [ str n.PageMeta.Title ] ])

        let htmlContent =
            [ if not (List.isEmpty postItems) then
                  h2 [] [ str "Posts" ]
                  ul [ _class "post-list" ] postLinks
              if not (List.isEmpty noteItems) then
                  h2 [] [ str "Notes" ]
                  ul [ _class "note-list" ] noteLinks ]

        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let html = RenderView.AsString.htmlNodes htmlContent
        let doc = Layouts.pageDocument ctx.Config page html cats tags

        { OutputPath = sprintf "topics/%s/index.html" topicId
          Content = RenderView.AsString.htmlDocument doc }

    let renderPost (ctx: RenderContext) (post: ContentItem) : RenderedPage =
        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let related = post.PageMeta.Related

        let doc =
            Layouts.postDocument ctx.Config post.PageMeta post.HtmlContent related cats tags

        { OutputPath = post.OutputPath
          Content = RenderView.AsString.htmlDocument doc }

    let renderPage (ctx: RenderContext) (page: ContentItem) : RenderedPage =
        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let doc = Layouts.pageDocument ctx.Config page.PageMeta page.HtmlContent cats tags

        { OutputPath = page.OutputPath
          Content = RenderView.AsString.htmlDocument doc }

    let renderNote (ctx: RenderContext) (note: ContentItem) : RenderedPage =
        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index

        // Get backlinks for this note
        let backlinks =
            match Map.tryFind note.PageMeta.Url ctx.Index.LinkGraph with
            | Some metadata -> metadata.InboundLinks
            | None -> []

        // Get all content for backlinks section
        let allContent = ctx.Index.Posts @ ctx.Index.Notes

        let doc =
            Layouts.noteDocument ctx.Config note.PageMeta note.HtmlContent backlinks allContent cats tags

        { OutputPath = note.OutputPath
          Content = RenderView.AsString.htmlDocument doc }

    let renderNotesIndex (ctx: RenderContext) : RenderedPage =
        let pageMeta =
            { Title = "Notes"
              Subtitle = None
              Description = Some "A collection of evolving notes and thoughts"
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = "notes/"
              Tags = []
              Categories = []
              Topics = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "page"
              Status = None
              Published = None
              Keywords = []
              Permalink = None }

        // Only show published notes in the index
        let publishedNotes =
            ctx.Index.Notes
            |> List.filter (fun note ->
                match note.PageMeta.Published with
                | Some false -> false
                | _ -> true)

        let noteLinks =
            publishedNotes
            |> List.map (fun n ->
                let displayTitle = Layouts.formatTitleWithStatus n.PageMeta
                li [] [ a [ _href (Parsing.combineUrl ctx.Config.BaseUrl n.PageMeta.Url) ] [ str displayTitle ] ])

        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let html = RenderView.AsString.htmlNodes [ ul [ _class "note-list" ] noteLinks ]
        let doc = Layouts.pageDocument ctx.Config pageMeta html cats tags

        { OutputPath = "notes/index.html"
          Content = RenderView.AsString.htmlDocument doc }

    let renderIndex
        (ctx: RenderContext)
        (pageNumber: int)
        (postsPerPage: int)
        (defaultSocialImg: string option)
        : RenderedPage =
        let allPosts = ctx.Index.Posts
        let totalPages = (allPosts.Length + postsPerPage - 1) / postsPerPage
        let skip = (pageNumber - 1) * postsPerPage
        let pagePosts = allPosts |> List.skip skip |> List.truncate postsPerPage
        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index

        let pageMeta =
            { Title = ctx.Config.Title
              Subtitle = None
              Description = Some ctx.Config.Description
              Author = None
              Date = None
              HeaderImage = ctx.Config.HeaderImage
              SocialImage = ctx.Config.SocialImage
              Url = if pageNumber = 1 then "" else sprintf "page/%d/" pageNumber
              Tags = []
              Categories = []
              Topics = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "page"
              Status = None
              Published = None
              Keywords = []
              Permalink = None }

        let doc =
            Layouts.indexDocument ctx.Config pageMeta pagePosts pageNumber totalPages defaultSocialImg cats tags

        let outputPath =
            if pageNumber = 1 then
                "index.html"
            else
                sprintf "page/%d/index.html" pageNumber

        { OutputPath = outputPath
          Content = RenderView.AsString.htmlDocument doc }

    let renderCategoryPage (ctx: RenderContext) (category: string) (_posts: ContentItem list) : RenderedPage =
        let mapped = tryMapCategoryToTopic ctx.Config category

        let target =
            match mapped with
            | Some topicId -> sprintf "topics/%s/" (Parsing.slugify topicId)
            | None -> "topics/"

        let slug = Parsing.slugify category
        let outputPath = sprintf "category/%s/index.html" slug
        let targetUrl = Parsing.combineUrl ctx.Config.BaseUrl target

        { OutputPath = outputPath
          Content = redirectHtml targetUrl }

    let renderTagPage (ctx: RenderContext) (tag: string) (_posts: ContentItem list) : RenderedPage =
        let mapped = tryMapTagToTopic ctx.Config tag

        let target =
            match mapped with
            | Some topicId -> sprintf "topics/%s/" (Parsing.slugify topicId)
            | None -> "topics/"

        let slug = Parsing.slugify tag
        let outputPath = sprintf "tag/%s/index.html" slug
        let targetUrl = Parsing.combineUrl ctx.Config.BaseUrl target

        { OutputPath = outputPath
          Content = redirectHtml targetUrl }

    let renderFeeds (ctx: RenderContext) : RenderedPage list =
        let posts = ctx.Index.Posts |> List.truncate 20

        [ { OutputPath = "rss.xml"
            Content = Feeds.generateRss ctx.Config posts }
          { OutputPath = "atom.xml"
            Content = Feeds.generateAtom ctx.Config posts } ]

    let renderSite (ctx: RenderContext) (postsPerPage: int) (defaultSocialImg: string option) : RenderedPage list =
        let postPages = ctx.Index.Posts |> List.map (renderPost ctx)
        let pagePages = ctx.Index.Pages |> List.map (renderPage ctx)
        let notePages = ctx.Index.Notes |> List.map (renderNote ctx)
        let notesIndexPage = [ renderNotesIndex ctx ]

        let totalPosts = ctx.Index.Posts.Length
        let totalIndexPages = (totalPosts + postsPerPage - 1) / postsPerPage

        let indexPages =
            [ 1..totalIndexPages ]
            |> List.map (fun page -> renderIndex ctx page postsPerPage defaultSocialImg)

        let topicPages =
            ctx.Index.Topics
            |> Map.toList
            |> List.map (fun (tid, posts) -> renderTopicPage ctx tid posts)

        let topicsOverview = [ renderTopicsOverviewPage ctx ]

        let categoryNames =
            configuredLegacyCategories ctx.Config
            @ (ctx.Index.Categories |> Map.toList |> List.map fst)
            |> distinctIgnoreCase

        let categoryPages =
            categoryNames |> List.map (fun cat -> renderCategoryPage ctx cat [])

        let tagNames =
            configuredLegacyTags ctx.Config @ (ctx.Index.Tags |> Map.toList |> List.map fst)
            |> distinctIgnoreCase

        let tagPages = tagNames |> List.map (fun tag -> renderTagPage ctx tag [])

        let feeds = renderFeeds ctx

        postPages
        @ pagePages
        @ notePages
        @ notesIndexPage
        @ indexPages
        @ topicPages
        @ topicsOverview
        @ categoryPages
        @ tagPages
        @ feeds

    let writeOutput (outputRoot: string) (pages: RenderedPage list) =
        pages
        |> List.iter (fun page ->
            let fullPath = Path.Combine(outputRoot, page.OutputPath)
            let dir = Path.GetDirectoryName(fullPath)

            if not (Directory.Exists(dir)) then
                Directory.CreateDirectory(dir) |> ignore

            File.WriteAllText(fullPath, page.Content))

    let private defaultIncludePatterns = [ "css/**"; "js/**"; "img/**"; "fonts/**" ]

    let copyStaticAssets (sourceRoot: string) (outputRoot: string) (includePatterns: string list) =
        let patterns =
            if List.isEmpty includePatterns then
                defaultIncludePatterns
            else
                includePatterns

        let matcher = Matcher()

        patterns |> List.iter (fun pattern -> matcher.AddInclude(pattern) |> ignore)

        let result =
            matcher.Execute(
                Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(DirectoryInfo(sourceRoot))
            )

        result.Files
        |> Seq.iter (fun fileMatch ->
            let sourcePath = Path.Combine(sourceRoot, fileMatch.Path)
            let targetPath = Path.Combine(outputRoot, fileMatch.Path)
            let targetDir = Path.GetDirectoryName(targetPath)

            if not (Directory.Exists(targetDir)) then
                Directory.CreateDirectory(targetDir) |> ignore

            File.Copy(sourcePath, targetPath, true))

    // Option 3: Deferred HTML Generation Functions

    /// Build resolution context from raw content items for wiki link resolution
    let buildResolutionContext (rawContentItems: RawContentItem list) : ResolutionContext =
        let titleLookupPairs =
            rawContentItems
            |> List.choose (fun item ->
                match item.Meta.Title with
                | Some title ->
                    let normalizedTitle = Parsing.normalizeWikiLabel title
                    Some(normalizedTitle, item)
                | None -> None)
            |> List.groupBy fst
            |> List.map (fun (normalizedTitle, items) -> (normalizedTitle, items |> List.map snd))

        let pathLookupPairs =
            rawContentItems
            |> List.choose (fun item ->
                match item.Meta.Title with
                | Some title ->
                    let normalizedTitle = Parsing.normalizeWikiLabel title

                    let outputPath =
                        match item.Kind with
                        | "note" ->
                            let slug = Parsing.slugify title
                            sprintf "notes/%s/" slug
                        | "post" ->
                            let date = item.Meta.Date
                            postUrlFromPath item.SourcePath date
                        | "page" -> pageUrlFromPath item.SourcePath
                        | _ -> sprintf "%s/" (Path.GetFileNameWithoutExtension item.SourcePath)

                    Some(normalizedTitle, "/" + outputPath)
                | None -> None)

        { TitleLookup = Map.ofList titleLookupPairs
          PathLookup = Map.ofList pathLookupPairs }

    /// Render a raw content item to a fully rendered content item using resolution context
    let renderContentItem (rawItem: RawContentItem) (resolutionContext: ResolutionContext) : RenderedContentItem =
        // Generate HTML with resolution context
        let htmlContent =
            Parsing.markdownToHtmlWithContext rawItem.Markdown (Some resolutionContext)

        // Build PageMeta (similar to existing loadContentItem logic)
        let url =
            match rawItem.Meta.Permalink with
            | Some p -> p.TrimStart('/').TrimEnd('/') + "/"
            | None ->
                match rawItem.Kind with
                | "note" ->
                    let title = Option.defaultValue "untitled" rawItem.Meta.Title
                    let slug = Parsing.slugify title
                    sprintf "notes/%s/" slug
                | "post" ->
                    let date = rawItem.Meta.Date
                    postUrlFromPath rawItem.SourcePath date
                | "page" -> pageUrlFromPath rawItem.SourcePath
                | _ -> sprintf "%s/" (Path.GetFileNameWithoutExtension rawItem.SourcePath)

        let outputPath =
            if url.EndsWith("/") then url + "index.html"
            elif url.EndsWith(".html") then url
            elif url = "" then "index.html"
            else url + "/index.html"

        let fullUrl = "/" + url.TrimStart('/')

        let pageMeta =
            { Title = Option.defaultValue "Untitled" rawItem.Meta.Title
              Subtitle = rawItem.Meta.Subtitle
              Description = rawItem.Meta.Description
              Author = rawItem.Meta.Author
              Date = rawItem.Meta.Date
              HeaderImage = rawItem.Meta.HeaderImage
              SocialImage = rawItem.Meta.SocialImage
              Url = fullUrl
              Tags = rawItem.Meta.Tags
              Categories = rawItem.Meta.Categories
              Topics = rawItem.Meta.Topics
              Related = [] // TODO: Implement related posts logic
              Previous = None // TODO: Implement previous/next logic
              Next = None
              CommentsEnabled = Option.defaultValue false rawItem.Meta.Comments
              Layout = Option.defaultValue "default" rawItem.Meta.Layout
              // Additional metadata fields
              Status = rawItem.Meta.Status
              Published = rawItem.Meta.Published
              Keywords = rawItem.Meta.Keywords
              Permalink = rawItem.Meta.Permalink }

        // Generate excerpt (simplified for now)
        let excerptHtml =
            let excerptText = htmlContent.Substring(0, min 200 htmlContent.Length) + "..."
            Some excerptText

        { SourcePath = rawItem.SourcePath
          OutputPath = outputPath
          HtmlContent = htmlContent
          ExcerptHtml = excerptHtml
          Meta = rawItem.Meta
          PageMeta = pageMeta
          Kind = rawItem.Kind }

    // Raw content loading functions for Option 3

    let private loadRawContentItem (sourcePath: string) (kind: string) : RawContentItem =
        let frontMatter, body = Parsing.parseMarkdownFile sourcePath
        let meta = Parsing.parseFrontMatter frontMatter

        { SourcePath = sourcePath
          Markdown = body
          Meta = meta
          Kind = kind }

    let loadRawPosts (directory: string) =
        if Directory.Exists(directory) then
            Directory.GetFiles(directory, "*.md", SearchOption.AllDirectories)
            |> Array.map (fun path -> loadRawContentItem path "post")
            |> Array.toList
        else
            []

    let loadRawPages (directory: string) =
        if Directory.Exists(directory) then
            Directory.GetFiles(directory, "*.md", SearchOption.TopDirectoryOnly)
            |> Array.map (fun path -> loadRawContentItem path "page")
            |> Array.toList
        else
            []

    let loadRawNotes (directory: string) =
        if Directory.Exists(directory) then
            Directory.GetFiles(directory, "*.md", SearchOption.AllDirectories)
            |> Array.map (fun path -> loadRawContentItem path "note")
            |> Array.toList
        else
            []

    /// Convert rendered content items back to ContentItem format for compatibility
    let renderedContentItemToContentItem (rendered: RenderedContentItem) : ContentItem =
        { SourcePath = rendered.SourcePath
          OutputPath = rendered.OutputPath
          Markdown = None // Don't store markdown in final ContentItem
          HtmlContent = rendered.HtmlContent
          ExcerptHtml = rendered.ExcerptHtml
          Meta = rendered.Meta
          PageMeta = rendered.PageMeta
          Kind = rendered.Kind }
