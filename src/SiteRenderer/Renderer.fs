namespace SiteRenderer

open System
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
                if kind = "post" then
                    postUrlFromPath sourcePath date
                else
                    pageUrlFromPath sourcePath

        let outputPath =
            if url.EndsWith("/") then url + "index.html"
            elif url.EndsWith(".html") then url
            elif url = "" then "index.html"
            else url + "/index.html"

        let layout =
            meta.Layout |> Option.defaultValue (if kind = "post" then "post" else "page")

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
              Url = url
              Tags = meta.Tags
              Categories = meta.Categories
              Topics = meta.Topics
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = commentsEnabled
              Layout = layout }

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

    let buildSiteIndex (posts: ContentItem list) (pages: ContentItem list) : SiteIndex =
        let postsWithNav = assignPreviousNext posts

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
            postsWithNav
            |> List.collect (fun p -> p.PageMeta.Topics |> List.map (fun t -> (t, p)))
            |> List.groupBy fst
            |> List.map (fun (topic, items) -> (topic, items |> List.map snd))
            |> Map.ofList

        { Posts = postsWithNav
          Pages = pages
          Categories = categories
          Tags = tags
          Topics = topics }

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
              Layout = "page" }

        let topicNodes =
            ctx.Config.Topics
            |> List.map (fun t ->
                li
                    []
                    [ a
                          [ _href (Parsing.combineUrl ctx.Config.BaseUrl (sprintf "topics/%s/" t.Id))
                            _title t.Description ]
                          [ str t.Name ]
                      p [] [ str t.Description ] ])

        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index

        let html =
            RenderView.AsString.htmlNodes [ h2 [] [ str "Topics" ]; ul [] topicNodes ]

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
              Layout = "topics" }

        let postLinks =
            posts
            |> List.map (fun p ->
                li [] [ a [ _href (Parsing.combineUrl ctx.Config.BaseUrl p.PageMeta.Url) ] [ str p.PageMeta.Title ] ])

        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let html = RenderView.AsString.htmlNodes [ h2 [] [ str title ]; ul [] postLinks ]
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
              Layout = "page" }

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
