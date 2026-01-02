namespace SiteRenderer

open System
open System.Globalization
open System.Net
open Giraffe.ViewEngine

module Layouts =

    // Helper for custom attributes not in Giraffe.ViewEngine
    let private _itemprop value = attr "itemprop" value

    // Helper for fragment - just returns a div with no visible wrapper
    let private fragment (_attrs: XmlAttribute list) (children: XmlNode list) : XmlNode =
        span [ attr "style" "display:contents" ] children

    let private doctypeHtml (attrs: XmlAttribute list) (children: XmlNode list) : XmlNode = html attrs children

    let private joinUrl (root: string) (path: string) =
        let normalizedRoot =
            if String.IsNullOrEmpty(root) then "/"
            elif root.EndsWith("/", StringComparison.Ordinal) then root
            else root + "/"

        let trimmed = path.TrimStart('/')

        if String.IsNullOrEmpty(trimmed) then
            normalizedRoot
        else
            normalizedRoot + trimmed

    let private assetUrl (site: SiteConfig) (relative: string) = joinUrl site.BaseUrl relative

    let private absoluteUrl (site: SiteConfig) (relative: string) = joinUrl site.Url relative

    let private themeInitScript =
        // Apply stored theme early to avoid a flash.
        "(function(){try{var t=localStorage.getItem('theme');if(t==='dark'||t==='light'){document.documentElement.dataset.theme=t;}}catch(e){}})();"

    let private headNode (site: SiteConfig) (page: PageMeta) =
        let pageTitle =
            if String.IsNullOrWhiteSpace(page.Title) then
                site.Title
            else
                page.Title

        let description =
            page.Description
            |> Option.orElse page.Subtitle
            |> Option.defaultValue site.Description

        let canonical = absoluteUrl site page.Url

        let imageUrl =
            page.SocialImage
            |> Option.orElse site.SocialImage
            |> Option.defaultValue "img/explore-590.jpg"
            |> absoluteUrl site

        let canonicalNodes =
            match page.Subtitle, page.Date with
            | Some subtitle, Some date ->
                [ link [ _rel "canonical"; _href canonical ]
                  meta
                      [ _itemprop "datePublished"
                        _content (date.ToString("yyyy-MM-dd"))
                        _id "date" ]
                  meta
                      [ _itemprop "dateModified"
                        _content (date.ToString("yyyy-MM-dd"))
                        _id "mdate" ]
                  meta [ _itemprop "headline"; _content (sprintf "%s - %s" pageTitle subtitle) ]
                  meta [ _itemprop "mainEntityOfPage"; _content canonical ]
                  meta [ _name "headline"; _content (sprintf "%s - %s" pageTitle subtitle) ] ]
            | _ -> []

        head
            []
            ([ meta [ _charset "utf-8" ]
               meta [ _httpEquiv "X-UA-Compatible"; _content "IE=edge" ]
               meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
               meta [ _name "color-scheme"; _content "dark light" ]
               title [] [ str pageTitle ]
               meta [ _name "description"; _content description ]
               meta [ _name "author"; _content site.Author ]
               meta [ _itemprop "name"; _content pageTitle ]
               meta [ _itemprop "description"; _content description ]
               meta [ _itemprop "image"; _content imageUrl ]
               yield! canonicalNodes
               link
                   [ _rel "icon"
                     _type "image/png"
                     _href (assetUrl site "img/favicon16.png")
                     attr "sizes" "16x16" ]
               link
                   [ _rel "icon"
                     _type "image/png"
                     _href (assetUrl site "img/favicon32.png")
                     attr "sizes" "32x32" ]
               meta [ _name "twitter:card"; _content "summary_large_image" ]
               meta
                   [ _name "twitter:site"
                     _content (sprintf "@%s" (site.TwitterUsername |> Option.defaultValue "")) ]
               meta [ _name "twitter:title"; _content pageTitle ]
               meta [ _name "twitter:description"; _content description ]
               meta
                   [ _name "twitter:creator"
                     _content (sprintf "@%s" (site.TwitterUsername |> Option.defaultValue "")) ]
               meta [ _name "twitter:image"; _content imageUrl ]
               meta [ _property "og:title"; _content pageTitle ]
               meta [ _property "og:type"; _content "article" ]
               meta [ _property "og:url"; _content canonical ]
               meta [ _property "og:image"; _content imageUrl ]
               meta [ _property "og:description"; _content description ]
               meta [ _property "og:site_name"; _content site.Title ]
               link [ _rel "stylesheet"; _href (assetUrl site "css/site.css") ]
               link [ _rel "stylesheet"; _href (assetUrl site "css/highlight/atom-one-dark.css") ]
               // Theme init happens before styles load.
               script [] [ rawText themeInitScript ]
               script [ _src (assetUrl site "js/theme.js"); attr "defer" "defer" ] [] ])

    let private navNode (site: SiteConfig) =
        let home = assetUrl site ""

        header
            [ _class "site-header" ]
            [ nav
                  [ _class "site-nav" ]
                  [ // a [ _class "site-title"; _href home ] [ str site.Title ]
                    ul
                        [ _class "site-links" ]
                        [ li [] [ a [ _href home ] [ str "Home" ] ]
                          li [] [ a [ _href (assetUrl site "topics/") ] [ str "Topics" ] ]
                          li [] [ a [ _href (assetUrl site "recommended-reading.html") ] [ str "Reading" ] ]
                          li [] [ a [ _href (assetUrl site "about/") ] [ str "About" ] ]
                          li [] [ a [ _href (assetUrl site "rss.xml") ] [ str "RSS" ] ] ]
                    button
                        [ _type "button"
                          _class "theme-toggle"
                          _id "theme-toggle"
                          attr "aria-label" "Toggle light/dark theme" ]
                        [ str "Theme" ] ] ]

    let private footerNode (site: SiteConfig) =
        let copyright = site.CopyrightName |> Option.defaultValue site.Title

        footer
            [ _class "site-footer" ]
            [ p
                  []
                  [ str "Copyright © "
                    a [ _href (assetUrl site "about/") ] [ str copyright ]
                    str (sprintf " %d" DateTime.UtcNow.Year) ] ]

    let private commentsNode (site: SiteConfig) (page: PageMeta) =
        if site.IsProduction && page.CommentsEnabled then
            let disqus = site.DisqusUsername |> Option.defaultValue ""

            div
                [ _class "comments" ]
                [ div [ _id "disqus_thread" ] []
                  script
                      [ _type "text/javascript" ]
                      [ rawText (sprintf "var disqus_shortname = '%s';" disqus)
                        rawText
                            "(function() { var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;"
                        rawText (sprintf "dsq.src = 'https://%s.disqus.com/embed.js';" disqus)
                        rawText
                            "(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);})();" ]
                  noscript [] [ str "Please enable JavaScript to view the comments powered by Disqus." ] ]
        else
            rawText ""

    let private analyticsNode (site: SiteConfig) =
        match site.IsProduction, site.GoogleTrackingId with
        | true, Some tracking when not (String.IsNullOrWhiteSpace tracking) ->
            script
                []
                [ rawText "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){"
                  rawText "(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),"
                  rawText
                      "m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)})(window,document,'script','//www.google-analytics.com/analytics.js','ga');"
                  rawText (sprintf "ga('create', '%s', 'auto');" tracking)
                  rawText "ga('send', 'pageview');" ]
        | _ -> rawText ""

    let private layoutDocument (site: SiteConfig) (page: PageMeta) (mainNodes: XmlNode list) (includeComments: bool) =
        doctypeHtml
            [ attr "lang" "en" ]
            [ headNode site page
              body
                  []
                  [ navNode site
                    main [ _class "site-main" ] mainNodes
                    footerNode site
                    if includeComments then
                        commentsNode site page
                    else
                        rawText ""
                    script [ _src (assetUrl site "js/highlight.pack.js") ] []
                    script [] [ rawText "hljs.initHighlightingOnLoad();" ]
                    analyticsNode site ] ]

    let private formatDate (date: DateTime option) =
        match date with
        | Some d -> d.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture)
        | None -> ""

    let private topicLinks (site: SiteConfig) (topicIds: string list) =
        if List.isEmpty topicIds then
            rawText ""
        else
            let toLink id =
                let topicOpt = site.Topics |> List.tryFind (fun t -> t.Id = id)

                let name = topicOpt |> Option.map (fun t -> t.Name) |> Option.defaultValue id

                let desc = topicOpt |> Option.map (fun t -> t.Description) |> Option.defaultValue ""

                a
                    [ _class "topic-pill"
                      _href (assetUrl site (sprintf "topics/%s/" id))
                      _title desc ]
                    [ str name ]

            div [ _class "topic-pills" ] (topicIds |> List.map toLink)

    let private pageHeader
        (title: string)
        (subtitle: string option)
        (description: string option)
        (meta: XmlNode option)
        =
        header
            [ _class "page-header" ]
            [ h1 [] [ str title ]
              match subtitle with
              | Some sub -> p [ _class "page-subtitle" ] [ str sub ]
              | None -> rawText ""
              match description with
              | Some desc when not (String.IsNullOrWhiteSpace desc) -> p [ _class "page-description" ] [ str desc ]
              | _ -> rawText ""
              meta |> Option.defaultValue (rawText "") ]

    let private postHeader (site: SiteConfig) (page: PageMeta) =
        let author = page.Author |> Option.defaultValue site.Title
        let dateStr = formatDate page.Date

        let metaNode =
            if String.IsNullOrWhiteSpace dateStr then
                Some(p [ _class "post-meta" ] [ str author ])
            else
                Some(p [ _class "post-meta" ] [ str (sprintf "%s · %s" author dateStr) ])

        fragment
            []
            [ pageHeader page.Title page.Subtitle None metaNode
              topicLinks site page.Topics
              hr [] ]

    let private relatedSection (site: SiteConfig) (related: PostSummary list) =
        if List.isEmpty related then
            rawText ""
        else
            section
                [ _class "related" ]
                [ h2 [] [ str "Related" ]
                  ul
                      [ _class "link-list" ]
                      (related
                       |> List.map (fun rp -> li [] [ a [ _href (assetUrl site rp.Url) ] [ str rp.Title ] ])) ]

    let private pagerLinks (site: SiteConfig) (prev: PostSummary option) (next: PostSummary option) =
        let linkItem label (post: PostSummary) =
            li [] [ a [ _href (assetUrl site post.Url); _title post.Title ] [ str label ] ]

        let items =
            [ prev |> Option.map (linkItem "← Previous")
              next |> Option.map (linkItem "Next →") ]
            |> List.choose id

        if List.isEmpty items then
            rawText ""
        else
            nav [ _class "pager" ] [ ul [ _class "pager-links" ] items ]

    let postDocument
        (site: SiteConfig)
        (page: PageMeta)
        (htmlContent: string)
        (related: PostSummary list)
        (_categories: (string * int) list)
        (_tags: (string * int) list)
        =
        let contentNodes =
            [ postHeader site page
              article [ _class "prose" ] [ rawText htmlContent ]
              hr []
              relatedSection site related
              pagerLinks site page.Previous page.Next ]

        layoutDocument site page contentNodes true

    let pageDocument
        (site: SiteConfig)
        (page: PageMeta)
        (htmlContent: string)
        (_categories: (string * int) list)
        (_tags: (string * int) list)
        =
        let contentNodes =
            [ pageHeader page.Title page.Subtitle page.Description None
              article [ _class "prose" ] [ rawText htmlContent ] ]

        layoutDocument site page contentNodes false

    let simpleDocument (site: SiteConfig) (page: PageMeta) (htmlContent: string) =
        layoutDocument site page [ rawText htmlContent ] false

    let private topicButtons (site: SiteConfig) =
        if List.isEmpty site.Topics then
            rawText ""
        else
            section
                [ _class "home-section" ]
                [ h2 [] [ str "Topics" ]
                  div
                      [ _class "topic-grid" ]
                      (site.Topics
                       |> List.map (fun t ->
                           a
                               [ _class "topic-pill"
                                 _href (assetUrl site (sprintf "topics/%s/" t.Id))
                                 _title t.Description ]
                               [ str t.Name ])) ]

    let private recentPosts
        (site: SiteConfig)
        (posts: ContentItem list)
        (pageNumber: int)
        (olderUrl: string option)
        (newerUrl: string option)
        =
        let title =
            if pageNumber = 1 then
                "Recent Posts"
            else
                sprintf "Posts (page %d)" pageNumber

        let postItems =
            posts
            |> List.map (fun post ->
                let dateStr = formatDate post.PageMeta.Date

                li
                    []
                    [ a [ _href (assetUrl site post.PageMeta.Url) ] [ str post.PageMeta.Title ]
                      span [ _class "post-date" ] [ str dateStr ] ])

        let pagerItem label url =
            a [ _class "pager-link"; _href (assetUrl site url) ] [ str label ]

        let pagerNodes =
            [ newerUrl |> Option.map (pagerItem "Newer")
              olderUrl |> Option.map (pagerItem "Older") ]
            |> List.choose id

        section
            [ _class "home-section" ]
            [ h2 [] [ str title ]
              ul [ _class "post-list" ] postItems
              if List.isEmpty pagerNodes then
                  rawText ""
              else
                  div [ _class "pager" ] pagerNodes ]

    let indexDocument
        (site: SiteConfig)
        (page: PageMeta)
        (posts: ContentItem list)
        (pageNumber: int)
        (totalPages: int)
        (_defaultSocialImg: string option)
        (_categories: (string * int) list)
        (_tags: (string * int) list)
        =
        let olderUrl =
            if pageNumber < totalPages then
                Some(sprintf "page/%d/" (pageNumber + 1))
            else
                None

        let newerUrl =
            if pageNumber > 2 then
                Some(sprintf "page/%d/" (pageNumber - 1))
            elif pageNumber = 2 then
                Some ""
            else
                None

        let hero =
            if pageNumber = 1 then
                Some(
                    header
                        [ _class "home-hero" ]
                        [ h1 [] [ str site.Title ]
                          p [ _class "home-tagline" ] [ str site.Description ]
                          hr [] ]
                )
            else
                None

        let contentNodes =
            [ hero |> Option.defaultValue (rawText "")
              div [ _class "home-grid" ] [ topicButtons site; recentPosts site posts pageNumber olderUrl newerUrl ] ]

        layoutDocument site page contentNodes false

    let categoryDocument
        (site: SiteConfig)
        (categoryName: string)
        (posts: ContentItem list)
        (categories: (string * int) list)
        (tags: (string * int) list)
        =
        let page =
            { Title = sprintf "Category: %s" categoryName
              Subtitle = None
              Description = Some(sprintf "Posts in category %s" categoryName)
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = sprintf "category/%s/" (Parsing.slugify categoryName)
              Tags = []
              Categories = []
              Topics = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "categories" }

        let postLinks =
            posts
            |> List.map (fun p ->
                li
                    []
                    [ a [ _href (assetUrl site p.PageMeta.Url) ] [ str p.PageMeta.Title ]
                      span [ _class "post-date" ] [ str (formatDate p.PageMeta.Date) ] ])

        pageDocument site page (RenderView.AsString.htmlNodes [ ul [ _class "post-list" ] postLinks ]) categories tags

    let tagDocument
        (site: SiteConfig)
        (tagName: string)
        (posts: ContentItem list)
        (categories: (string * int) list)
        (tags: (string * int) list)
        =
        let page =
            { Title = sprintf "Tag: %s" tagName
              Subtitle = None
              Description = Some(sprintf "Posts tagged with %s" tagName)
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = sprintf "tag/%s/" (Parsing.slugify tagName)
              Tags = []
              Categories = []
              Topics = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "tags" }

        let postLinks =
            posts
            |> List.map (fun p ->
                li
                    []
                    [ a [ _href (assetUrl site p.PageMeta.Url) ] [ str p.PageMeta.Title ]
                      span [ _class "post-date" ] [ str (formatDate p.PageMeta.Date) ] ])

        pageDocument site page (RenderView.AsString.htmlNodes [ ul [ _class "post-list" ] postLinks ]) categories tags
