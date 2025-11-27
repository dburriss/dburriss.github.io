namespace SiteRenderer

open System
open System.Globalization
open System.Net
open Giraffe.ViewEngine

module Layouts =

    let private htmlEncode value = WebUtility.HtmlEncode(value)

    let private joinUrl (root: string) (path: string) =
        let normalizedRoot =
            if String.IsNullOrEmpty(root) then "/"
            elif root.EndsWith("/", StringComparison.Ordinal) then root
            else root + "/"
        let trimmed = path.TrimStart('/')
        if String.IsNullOrEmpty(trimmed) then normalizedRoot else normalizedRoot + trimmed

    let private assetUrl (site: SiteConfig) (relative: string) =
        joinUrl site.BaseUrl relative

    let private absoluteUrl (site: SiteConfig) (relative: string) =
        joinUrl site.Url relative

    let private headNode (site: SiteConfig) (page: PageMeta) =
        let title = if String.IsNullOrWhiteSpace(page.Title) then site.Title else page.Title
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
                  meta [ _itemprop "datePublished"; _content (date.ToString("yyyy-MM-dd")); _id "date" ]
                  meta [ _itemprop "dateModified"; _content (date.ToString("yyyy-MM-dd")); _id "mdate" ]
                  meta [ _itemprop "headline"; _content (sprintf "%s - %s" title subtitle) ]
                  meta [ _itemprop "mainEntityOfPage"; _content canonical ]
                  meta [ _name "headline"; _content (sprintf "%s - %s" title subtitle) ] ]
            | _ -> []
        head [] (
            [ rawText "<!-- META -->"
              meta [ _charset "utf-8" ]
              meta [ _httpEquiv "X-UA-Compatible"; _content "IE=edge" ]
              meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
              title [] [ str title ]
              meta [ _name "description"; _content description ]
              meta [ _name "author"; _content site.Author ]
              yield! canonicalNodes
              link [ _rel "icon"; _type "image/png"; _href (assetUrl site "img/favicon16.png"); attr "sizes" "16x16" ]
              link [ _rel "icon"; _type "image/png"; _href (assetUrl site "img/favicon32.png"); attr "sizes" "32x32" ]
              meta [ _itemprop "name"; _content title ]
              meta [ _itemprop "description"; _content description ]
              meta [ _itemprop "image"; _content imageUrl ]
              meta [ _name "twitter:card"; _content "summary_large_image" ]
              meta [ _name "twitter:site"; _content (sprintf "@%s" (site.TwitterUsername |> Option.defaultValue "")) ]
              meta [ _name "twitter:title"; _content title ]
              meta [ _name "twitter:description"; _content description ]
              meta [ _name "twitter:creator"; _content (sprintf "@%s" (site.TwitterUsername |> Option.defaultValue "")) ]
              meta [ _name "twitter:image"; _content imageUrl ]
              meta [ _property "og:title"; _content title ]
              meta [ _property "og:type"; _content "article" ]
              meta [ _property "og:url"; _content canonical ]
              meta [ _property "og:image"; _content imageUrl ]
              meta [ _property "og:description"; _content description ]
              meta [ _property "og:site_name"; _content site.Title ]
              link [ _rel "stylesheet"; _href (assetUrl site "css/bootstrap.min.css") ]
              link [ _rel "stylesheet"; _href (assetUrl site "css/clean-blog.css") ]
              link [ _rel "stylesheet"; attr "charset" "UTF-8"; _href (assetUrl site "css/highlight/atom-one-light.css") ]
              link [ _href "//maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css"; _rel "stylesheet"; attr "type" "text/css" ]
              link [ _href "//fonts.googleapis.com/css?family=Lora:400,700,400italic,700italic"; _rel "stylesheet"; attr "type" "text/css" ]
              link [ _href "//fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,600italic,700italic,800italic,400,300,600,700,800"; _rel "stylesheet"; attr "type" "text/css" ]
              rawText "<!--[if lt IE 9]>\n        <script src=\"https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js\"></script>\n        <script src=\"https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js\"></script>\n    <![endif]-->" ])

    let private navNode (site: SiteConfig) =
        let home = assetUrl site ""
        nav [ _class "navbar navbar-default navbar-custom navbar-fixed-top" ]
            [ div [ _class "container-fluid" ]
                [ div [ _class "navbar-header page-scroll" ]
                    [ button [ _type "button"; _class "navbar-toggle"; attr "data-toggle" "collapse"; attr "data-target" "#bs-example-navbar-collapse-1" ]
                        [ span [ _class "sr-only" ] [ str "Toggle navigation" ]
                          span [ _class "icon-bar" ] []
                          span [ _class "icon-bar" ] []
                          span [ _class "icon-bar" ] [] ]
                      a [ _class "navbar-brand"; _href home ] [ str site.Title ] ]
                  div [ _class "collapse navbar-collapse"; _id "bs-example-navbar-collapse-1" ]
                    [ ul [ _class "nav navbar-nav navbar-right" ]
                        [ li [] [ a [ _href home ] [ str "Home" ] ]
                          li [] [ a [ _href (assetUrl site "recommended-reading.html") ] [ str "Recommended Reading" ] ]
                          li []
                            [ a [ _href (assetUrl site "rss.xml") ]
                                [ span [ _class "fa-stack fa-lg" ]
                                    [ i [ _class "fa fa-rss fa-stack-1x" ] [] ] ] ] ] ] ] ]

    let private socialNode (site: SiteConfig) =
        let link icon background url =
            li []
                [ a [ _href url ]
                    [ span [ _class "fa-stack fa-lg" ]
                        [ i [ _class background ] []
                          i [ _class icon ] [] ] ] ]
        [ li [] [ a [ _href (assetUrl site "rss.xml") ] [ span [ _class "fa-stack fa-lg" ] [ i [ _class "fa fa-square fa-stack-2x" ] []; i [ _class "fa fa-rss fa-stack-1x fa-inverse" ] [] ] ] ]
          li [] [ a [ _href (assetUrl site "atom.xml") ] [ span [ _class "fa-stack fa-lg" ] [ i [ _class "fa fa-circle fa-stack-2x" ] []; i [ _class "fa fa-rss fa-stack-1x fa-inverse" ] [] ] ] ]
          match site.TwitterUsername with
          | Some handle -> link "fa fa-twitter fa-stack-1x fa-inverse" "fa fa-circle fa-stack-2x" (sprintf "https://twitter.com/%s" handle)
          | None -> rawText ""
          match site.FacebookUsername with
          | Some name -> link "fa fa-facebook fa-stack-1x fa-inverse" "fa fa-circle fa-stack-2x" (sprintf "https://www.facebook.com/%s" name)
          | None -> rawText ""
          match site.GithubUsername with
          | Some user -> link "fa fa-github fa-stack-1x fa-inverse" "fa fa-circle fa-stack-2x" (sprintf "https://github.com/%s" user)
          | None -> rawText ""
          match site.EmailUsername with
          | Some email -> link "fa fa-envelope fa-stack-1x fa-inverse" "fa fa-circle fa-stack-2x" (sprintf "mailto:%s" email)
          | None -> rawText "" ]
        |> fun items -> ul [ _class "list-inline text-center" ] items

    let private footerNode (site: SiteConfig) =
        let copyright = site.CopyrightName |> Option.defaultValue site.Title
        fragment []
            [ footer []
                [ div [ _class "container" ]
                    [ div [ _class "row" ]
                        [ div [ _class "col-lg-8 col-lg-offset-2 col-md-10 col-md-offset-1" ]
                            [ p [ _class "copyright text-muted" ]
                                [ str "Copyright © "
                                  a [ _href (assetUrl site "about.html") ] [ str copyright ]
                                  str (sprintf " %d" DateTime.UtcNow.Year) ] ] ] ] ]
              script [ _src (assetUrl site "js/jquery.min.js") ] []
              script [ _src (assetUrl site "js/bootstrap.min.js") ] []
              script [ _src (assetUrl site "js/clean-blog.min.js") ] []
              script [ _src (assetUrl site "js/highlight.pack.js") ] []
              script [] [ rawText "hljs.initHighlightingOnLoad();" ] ]

    let private commentsNode (site: SiteConfig) (page: PageMeta) =
        if site.IsProduction && page.CommentsEnabled then
            let disqus = site.DisqusUsername |> Option.defaultValue ""
            div [ _class "container-fluid" ]
                [ div [ _id "disqus_thread" ] []
                  script [ _type "text/javascript" ]
                      [ rawText (sprintf "var disqus_shortname = '%s';" disqus)
                        rawText "(function() { var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;"
                        rawText (sprintf "dsq.src = 'https://%s.disqus.com/embed.js';" disqus)
                        rawText "(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);})();" ]
                  noscript [] [ str "Please enable JavaScript to view the comments powered by Disqus." ] ]
        else
            rawText ""

    let private analyticsNode (site: SiteConfig) =
        match site.IsProduction, site.GoogleTrackingId with
        | true, Some tracking when not (String.IsNullOrWhiteSpace tracking) ->
            script []
                [ rawText "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){"
                  rawText "(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),"
                  rawText "m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)})(window,document,'script','//www.google-analytics.com/analytics.js','ga');"
                  rawText (sprintf "ga('create', '%s', 'auto');" tracking)
                  rawText "ga('send', 'pageview');" ]
        | _ -> rawText ""

    let private categoryWidget (site: SiteConfig) (categories: (string * int) list) =
        let links =
            categories
            |> List.map (fun (name, _) ->
                a [ _class "badge badge-info"; _href (assetUrl site (sprintf "category/%s" (Parsing.slugify name))) ] [ str name ])
        fragment []
            [ h2 [] [ i [ _class "glyphicon glyphicon-folder-open" ] []; str "  Categories" ]
              yield! links ]

    let private tagWidget (site: SiteConfig) (tags: (string * int) list) =
        let maxCount = tags |> List.map snd |> function | [] -> 1 | xs -> xs |> List.max
        let renderTag (name: string, count: int) =
            let font = 80 + (count * 4)
            a [ _class "badge badge-info"; attr "style" (sprintf "font-size: %d%%" font); _href (assetUrl site (sprintf "tag/%s/" (Parsing.slugify name))) ]
                [ span [ _class "site-tag" ] [ str (name.Replace("-", " ")) ] ]
        fragment []
            [ h2 [] [ i [ _class "glyphicon glyphicon-tags" ] []; str "  Tags" ]
              div [ _class "well" ] (tags |> List.map renderTag) ]

    let private aboutWidget (site: SiteConfig) =
        fragment []
            [ h3 [] [ str "About me" ]
              div []
                  [ div [ attr "itemprop" "publisher"; attr "itemscope" ""; attr "itemtype" "https://schema.org/Organization" ]
                      [ div [ attr "itemprop" "logo"; attr "itemscope" ""; attr "itemtype" "https://schema.org/ImageObject" ]
                          [ img [ _class "img-rounded pull-right"; _src (assetUrl site "img/avatar.jpg"); attr "width" "33%"; attr "style" "margin-left: 1em;" ]
                            meta [ attr "itemprop" "url"; _content (assetUrl site "img/avatar.jpg") ] ]
                        meta [ attr "itemprop" "name"; _content site.Author ] ]
                    p [ attr "align" "justify" ]
                        [ str "A South African living in Rotterdam, Netherlands. Software development, clean code, functional programming, Domain-Driven Design, TDD, other acronyms. Basically I just like learning new things and want to get better at what I love doing. Occassionally I write about what I learn here." ] ] ]

    let private postHeader (site: SiteConfig) (page: PageMeta) =
        let headerImage =
            match page.HeaderImage, site.HeaderImage with
            | Some img, _ -> assetUrl site img
            | None, Some img -> assetUrl site img
            | _ -> assetUrl site "img/backgrounds/explore-bg.jpg"
        let author = page.Author |> Option.defaultValue site.Title
        let dateStr =
            match page.Date with
            | Some d -> d.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)
            | None -> ""
        header [ _class "intro-header"; attr "style" (sprintf "background-image: url('%s')" headerImage) ]
            [ div [ _class "container-fluid" ]
                [ div [ _class "row" ]
                    [ div [ _class "col-lg-8 col-lg-offset-2 col-md-10 col-md-offset-1" ]
                        [ div [ _class "post-heading" ]
                            [ div [ _class "frame" ]
                                [ h1 [] [ str page.Title ]
                                  match page.Subtitle with
                                  | Some sub -> h2 [ _class "subheading" ] [ str sub ]
                                  | None -> rawText ""
                                  span [ _class "meta"; attr "itemprop" "author"; attr "itemscope" ""; attr "itemtype" "https://schema.org/Person" ]
                                    [ str "Posted by "
                                      span [ attr "itemprop" "name" ] [ str author ]
                                      str (sprintf " on %s" dateStr) ]
                                  if not (List.isEmpty page.Tags) then
                                      fragment []
                                          [ hr []
                                            yield! (page.Tags |> List.map (fun tag -> a [ _href (assetUrl site (sprintf "tag/%s" (Parsing.slugify tag))); _class "label label-default" ] [ str tag ])) ]
                                  else
                                      rawText ""
                                  div [] [] ] ] ] ] ] ]

    let private relatedSection (site: SiteConfig) (related: PostSummary list) =
        if List.isEmpty related then
            rawText ""
        else
            section [ _class "related" ]
                [ h4 [] [ str "You may be interested in" ]
                  ul [] (related |> List.map (fun rp ->
                      li [] [ a [ _href (assetUrl site rp.Url) ] [ str rp.Title ] ])) ]

    let private pagerLinks (site: SiteConfig) (prev: PostSummary option) (next: PostSummary option) =
        ul [ _class "pager" ]
            [ match prev with
              | Some p ->
                  li [ _class "previous" ]
                    [ a [ _href (assetUrl site p.Url); attr "data-toggle" "tooltip"; attr "data-placement" "top"; _title p.Title ]
                        [ rawText "&larr; Previous Post" ] ]
              | None -> rawText ""
              match next with
              | Some n ->
                  li [ _class "next" ]
                    [ a [ _href (assetUrl site n.Url); attr "data-toggle" "tooltip"; attr "data-placement" "top"; _title n.Title ]
                        [ rawText "Next Post &rarr;" ] ]
              | None -> rawText "" ]

    let private postBody (htmlContent: string) =
        rawText htmlContent

    let private sidebar (site: SiteConfig) (categories: (string * int) list) (tags: (string * int) list) includeAbout =
        let blocks =
            [ socialNode site
              categoryWidget site categories
              tagWidget site tags ]
        let blocks = if includeAbout then blocks @ [ aboutWidget site ] else blocks
        blocks

    let postDocument (site: SiteConfig) (page: PageMeta) (htmlContent: string) (related: PostSummary list) (categories: (string * int) list) (tags: (string * int) list) =
        doctypeHtml []
            [ headNode site page
              body []
                [ navNode site
                  postHeader site page
                  article []
                    [ div [ _class "container-fluid" ]
                        [ div [ _class "row" ]
                            [ div [ _class "col-lg-8" ]
                                [ postBody htmlContent
                                  hr []
                                  relatedSection site related
                                  pagerLinks site page.Previous page.Next ]
                              div [ _class "col-lg-4 widget-column" ] (sidebar site categories tags false) ] ]
                      hr [] ]
                  footerNode site
                  analyticsNode site
                  commentsNode site page ] ]

    let pageDocument (site: SiteConfig) (page: PageMeta) (htmlContent: string) (categories: (string * int) list) (tags: (string * int) list) =
        let headerImage =
            match page.HeaderImage, site.HeaderImage with
            | Some img, _ -> assetUrl site img
            | None, Some img -> assetUrl site img
            | _ -> assetUrl site "img/backgrounds/explore-bg.jpg"
        doctypeHtml []
            [ headNode site page
              body []
                [ navNode site
                  header [ _class "intro-header"; attr "style" (sprintf "background-image: url('%s')" headerImage) ]
                    [ div [ _class "container-fluid" ]
                        [ div [ _class "row" ]
                            [ div [ _class "col-lg-8 col-lg-offset-2 col-md-10 col-md-offset-1" ]
                                [ div [ _class "site-heading" ]
                                    [ div [ _class "frame" ]
                                        [ h1 [] [ str page.Title ]
                                          hr [ _class "small" ]
                                          span [ _class "subheading" ] [ str (page.Description |> Option.defaultValue site.Description) ] ] ] ] ] ]
                  div [ _class "container-fluid" ]
                    [ div [ _class "row" ]
                        [ div [ _class "col-lg-9" ] [ rawText htmlContent ]
                          div [ _class "col-lg-3 widget-column" ] (sidebar site categories tags true) ] ]
                  footerNode site
                  analyticsNode site ] ]

    let simpleDocument (site: SiteConfig) (page: PageMeta) (htmlContent: string) =
        doctypeHtml []
            [ headNode site page
              body []
                [ navNode site
                  rawText htmlContent
                  footerNode site
                  analyticsNode site ] ]

    let private featuredPostPreview (site: SiteConfig) (post: ContentItem) =
        let author = post.Meta.Author |> Option.defaultValue site.Title
        let dateStr =
            match post.PageMeta.Date with
            | Some d -> d.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)
            | None -> ""
        let socialImgUrl = post.Meta.SocialImage |> Option.map (assetUrl site)
        div [ _class "post-preview jumbotron" ]
            [ h1 [ _class "post-title" ]
                [ a [ _href (assetUrl site post.PageMeta.Url) ] [ str post.PageMeta.Title ] ]
              match post.PageMeta.Subtitle with
              | Some sub -> em [ _class "post-subtitle" ] [ str sub ]
              | None -> rawText ""
              p [ _class "post-meta" ] [ str (sprintf "Posted by %s on %s" author dateStr) ]
              yield! (post.PageMeta.Tags |> List.map (fun tag ->
                  a [ _href (assetUrl site (sprintf "tag/%s" (Parsing.slugify tag))) ]
                      [ span [ _class "label label-default" ] [ str tag ] ]))
              match socialImgUrl with
              | Some imgUrl -> img [ _src imgUrl; _alt "Social image"; _class "img-rounded pull-right"; attr "width" "280"; attr "style" "margin-left: 1em;" ]
              | None -> rawText ""
              match post.ExcerptHtml with
              | Some excerpt -> p [ attr "align" "justify" ] [ rawText excerpt ]
              | None -> rawText ""
              p [ _class "text-left" ]
                [ a [ _class "btn btn-primary btn-lg"; _href (assetUrl site post.PageMeta.Url); attr "role" "button" ]
                    [ str "Read more" ] ]
              div [ _class "clearfix" ] []
              hr [] ]

    let private postListItem (site: SiteConfig) (post: ContentItem) (defaultSocialImg: string option) =
        let author = post.Meta.Author |> Option.defaultValue site.Title
        let dateStr =
            match post.PageMeta.Date with
            | Some d -> d.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)
            | None -> ""
        let socialImg =
            post.Meta.SocialImage
            |> Option.orElse defaultSocialImg
            |> Option.map (assetUrl site)
            |> Option.defaultValue (assetUrl site "img/explore-590.jpg")
        div [ _class "post-preview" ]
            [ img [ _src socialImg; _alt "Social image"; _class "img-rounded pull-left"; attr "width" "110"; attr "style" "margin-right: 1em;" ]
              a [ _href (assetUrl site post.PageMeta.Url) ]
                [ h2 [ _class "post-title" ] [ str post.PageMeta.Title ]
                  match post.PageMeta.Subtitle with
                  | Some sub -> h3 [ _class "post-subtitle" ] [ str sub ]
                  | None -> rawText "" ]
              p [ _class "post-meta" ] [ str (sprintf "Posted by %s on %s" author dateStr) ]
              yield! (post.PageMeta.Tags |> List.map (fun tag ->
                  a [ _href (assetUrl site (sprintf "tag/%s" (Parsing.slugify tag))) ]
                      [ span [ _class "label label-default" ] [ str tag ] ])) ]

    let private indexPager (site: SiteConfig) (olderUrl: string option) (newerUrl: string option) =
        ul [ _class "pager" ]
            [ match olderUrl with
              | Some url -> li [ _class "previous" ] [ a [ _href (assetUrl site url) ] [ rawText "&larr; Older" ] ]
              | None -> rawText ""
              match newerUrl with
              | Some url -> li [ _class "next" ] [ a [ _class "pagination-item newer"; _href (assetUrl site url) ] [ rawText "Newer &rarr;" ] ]
              | None -> rawText "" ]

    let indexDocument (site: SiteConfig) (page: PageMeta) (posts: ContentItem list) (pageNumber: int) (totalPages: int) (defaultSocialImg: string option) (categories: (string * int) list) (tags: (string * int) list) =
        let headerImage =
            match page.HeaderImage, site.HeaderImage with
            | Some img, _ -> assetUrl site img
            | None, Some img -> assetUrl site img
            | _ -> assetUrl site "img/backgrounds/explore-bg.jpg"
        let isFirstPage = pageNumber = 1
        let olderUrl = if pageNumber < totalPages then Some (sprintf "page/%d/" (pageNumber + 1)) else None
        let newerUrl =
            if pageNumber > 2 then Some (sprintf "page/%d/" (pageNumber - 1))
            elif pageNumber = 2 then Some ""
            else None
        let headerNode =
            if isFirstPage then
                header [ _class "intro-header"; attr "style" (sprintf "background-image: url('%s')" headerImage) ]
                    [ div [ _class "container-fluid" ]
                        [ div [ _class "row" ]
                            [ div [ _class "col-lg-8 col-lg-offset-2 col-md-10 col-md-offset-1" ]
                                [ div [ _class "site-heading" ]
                                    [ div [ _class "frame" ]
                                        [ h1 [] [ str page.Title ]
                                          hr [ _class "small" ]
                                          span [ _class "subheading" ] [ str (page.Description |> Option.defaultValue site.Description) ] ] ] ] ] ] ]
            else
                header [ _class "intro-header header-sliver"; attr "style" "background-image: url('/img/backgrounds/blog-sliver.jpg')" ] []
        let content =
            match posts with
            | [] -> []
            | featured :: rest when isFirstPage ->
                [ featuredPostPreview site featured
                  yield! (rest |> List.collect (fun p -> [ postListItem site p defaultSocialImg; hr [] ])) ]
            | _ ->
                posts |> List.collect (fun p -> [ postListItem site p defaultSocialImg; hr [] ])
        doctypeHtml []
            [ headNode site page
              body []
                [ navNode site
                  headerNode
                  div [ _class "container-fluid" ]
                    [ div [ _class "row" ]
                        [ div [ _class "col-lg-9" ] (content @ [ indexPager site olderUrl newerUrl ])
                          div [ _class "col-lg-3 widget-column" ] (sidebar site categories tags true) ] ]
                  footerNode site
                  analyticsNode site ] ]

    let categoryDocument (site: SiteConfig) (categoryName: string) (posts: ContentItem list) (categories: (string * int) list) (tags: (string * int) list) =
        let page =
            { Title = sprintf "Category: %s" categoryName
              Subtitle = None
              Description = Some (sprintf "Posts in category %s" categoryName)
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = sprintf "category/%s/" (Parsing.slugify categoryName)
              Tags = []
              Categories = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "categories" }
        let postLinks =
            posts
            |> List.map (fun p ->
                li []
                    [ a [ _href (assetUrl site p.PageMeta.Url) ] [ str p.PageMeta.Title ]
                      str (sprintf " - %s" (match p.PageMeta.Date with Some d -> d.ToString("MMM dd, yyyy") | None -> "")) ])
        pageDocument site page (RenderView.AsString.htmlNodes [ h2 [] [ str categoryName ]; ul [] postLinks ]) categories tags

    let tagDocument (site: SiteConfig) (tagName: string) (posts: ContentItem list) (categories: (string * int) list) (tags: (string * int) list) =
        let page =
            { Title = sprintf "Tag: %s" tagName
              Subtitle = None
              Description = Some (sprintf "Posts tagged with %s" tagName)
              Author = None
              Date = None
              HeaderImage = None
              SocialImage = None
              Url = sprintf "tag/%s/" (Parsing.slugify tagName)
              Tags = []
              Categories = []
              Related = []
              Previous = None
              Next = None
              CommentsEnabled = false
              Layout = "tags" }
        let postLinks =
            posts
            |> List.map (fun p ->
                li []
                    [ a [ _href (assetUrl site p.PageMeta.Url) ] [ str p.PageMeta.Title ]
                      str (sprintf " - %s" (match p.PageMeta.Date with Some d -> d.ToString("MMM dd, yyyy") | None -> "")) ])
        pageDocument site page (RenderView.AsString.htmlNodes [ h2 [] [ str tagName ]; ul [] postLinks ]) categories tags
