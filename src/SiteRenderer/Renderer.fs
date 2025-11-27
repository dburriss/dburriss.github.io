namespace SiteRenderer

open System
open System.IO
open System.Text.RegularExpressions
open Giraffe.ViewEngine

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

        { Posts = postsWithNav
          Pages = pages
          Categories = categories
          Tags = tags }

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

    let renderCategoryPage (ctx: RenderContext) (category: string) (posts: ContentItem list) : RenderedPage =
        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let doc = Layouts.categoryDocument ctx.Config category posts cats tags
        let slug = Parsing.slugify category

        { OutputPath = sprintf "category/%s/index.html" slug
          Content = RenderView.AsString.htmlDocument doc }

    let renderTagPage (ctx: RenderContext) (tag: string) (posts: ContentItem list) : RenderedPage =
        let cats = categoryCounts ctx.Index
        let tags = tagCounts ctx.Index
        let doc = Layouts.tagDocument ctx.Config tag posts cats tags
        let slug = Parsing.slugify tag

        { OutputPath = sprintf "tag/%s/index.html" slug
          Content = RenderView.AsString.htmlDocument doc }

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

        let categoryPages =
            ctx.Index.Categories
            |> Map.toList
            |> List.map (fun (cat, posts) -> renderCategoryPage ctx cat posts)

        let tagPages =
            ctx.Index.Tags
            |> Map.toList
            |> List.map (fun (tag, posts) -> renderTagPage ctx tag posts)

        let feeds = renderFeeds ctx

        postPages @ pagePages @ indexPages @ categoryPages @ tagPages @ feeds

    let writeOutput (outputRoot: string) (pages: RenderedPage list) =
        pages
        |> List.iter (fun page ->
            let fullPath = Path.Combine(outputRoot, page.OutputPath)
            let dir = Path.GetDirectoryName(fullPath)

            if not (Directory.Exists(dir)) then
                Directory.CreateDirectory(dir) |> ignore

            File.WriteAllText(fullPath, page.Content))

    let copyStaticAssets (sourceRoot: string) (outputRoot: string) =
        let staticDirs = [ "css"; "js"; "img"; "fonts" ]

        staticDirs
        |> List.iter (fun dir ->
            let sourceDir = Path.Combine(sourceRoot, dir)
            let targetDir = Path.Combine(outputRoot, dir)

            if Directory.Exists(sourceDir) then
                if not (Directory.Exists(targetDir)) then
                    Directory.CreateDirectory(targetDir) |> ignore

                let files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories)

                files
                |> Array.iter (fun file ->
                    let relativePath =
                        file.Substring(sourceDir.Length).TrimStart(Path.DirectorySeparatorChar)

                    let targetPath = Path.Combine(targetDir, relativePath)
                    let targetSubDir = Path.GetDirectoryName(targetPath)

                    if not (Directory.Exists(targetSubDir)) then
                        Directory.CreateDirectory(targetSubDir) |> ignore

                    File.Copy(file, targetPath, true)))
