namespace SiteRenderer

open System
open System.Globalization
open System.Net
open System.Xml.Linq

module Feeds =

    let private xn name = XName.Get(name)
    let private xns ns name = XName.Get(name, ns)

    let private atomNs = "http://www.w3.org/2005/Atom"
    let private rssAtomNs = "http://www.w3.org/2005/Atom"

    let private xmlEscape (value: string) =
        if String.IsNullOrEmpty value then
            ""
        else
            WebUtility.HtmlEncode(value)

    let private rfc822Date (date: DateTime) =
        date.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture)

    let private iso8601Date (date: DateTime) =
        date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)

    let private combineUrl (baseUrl: string) (relative: string) =
        let trimmedBase = if baseUrl.EndsWith("/") then baseUrl else baseUrl + "/"
        let trimmed = relative.TrimStart('/')
        trimmedBase + trimmed

    let generateRss (site: SiteConfig) (posts: ContentItem list) =
        let now = DateTime.UtcNow
        let siteUrl = site.Url
        let rssUrl = combineUrl siteUrl "rss.xml"

        let items =
            posts
            |> List.map (fun post ->
                let postUrl = combineUrl siteUrl post.PageMeta.Url

                let pubDate =
                    post.PageMeta.Date
                    |> Option.map rfc822Date
                    |> Option.defaultValue (rfc822Date now)

                let author = post.Meta.Author |> Option.defaultValue site.Author

                let description =
                    post.Meta.Description
                    |> Option.orElse post.PageMeta.Subtitle
                    |> Option.defaultValue ""

                XElement(
                    xn "item",
                    XElement(xn "title", post.PageMeta.Title),
                    XElement(xn "link", postUrl),
                    XElement(xn "pubDate", pubDate),
                    XElement(xn "author", author),
                    XElement(xn "guid", postUrl),
                    XElement(xn "description", description)
                ))

        let channel =
            XElement(
                xn "channel",
                XElement(xn "title", site.Title),
                XElement(xn "link", siteUrl),
                XElement(
                    xns rssAtomNs "link",
                    XAttribute(xn "href", rssUrl),
                    XAttribute(xn "rel", "self"),
                    XAttribute(xn "type", "application/rss+xml")
                ),
                XElement(xn "description", site.Description),
                XElement(xn "language", "en-gb"),
                XElement(xn "pubDate", rfc822Date now),
                XElement(xn "lastBuildDate", rfc822Date now)
            )

        items |> List.iter (fun item -> channel.Add(item))

        let rss =
            XElement(
                xn "rss",
                XAttribute(xn "version", "2.0"),
                XAttribute(XNamespace.Xmlns + "atom", rssAtomNs),
                channel
            )

        let doc = XDocument(XDeclaration("1.0", "utf-8", null), rss :> obj)
        doc.ToString()

    let generateAtom (site: SiteConfig) (posts: ContentItem list) =
        let now = DateTime.UtcNow
        let siteUrl = site.Url
        let atomUrl = combineUrl siteUrl "atom.xml"

        let entries =
            posts
            |> List.map (fun post ->
                let postUrl = combineUrl siteUrl post.PageMeta.Url

                let updated =
                    post.PageMeta.Date
                    |> Option.map iso8601Date
                    |> Option.defaultValue (iso8601Date now)

                let author = post.Meta.Author |> Option.defaultValue site.Author

                let summary =
                    post.Meta.Description
                    |> Option.orElse (Some post.HtmlContent)
                    |> Option.defaultValue ""

                XElement(
                    xns atomNs "entry",
                    XElement(xns atomNs "id", postUrl),
                    XElement(
                        xns atomNs "link",
                        XAttribute(xn "type", "text/html"),
                        XAttribute(xn "rel", "alternate"),
                        XAttribute(xn "href", postUrl)
                    ),
                    XElement(xns atomNs "title", post.PageMeta.Title),
                    XElement(xns atomNs "updated", updated),
                    XElement(
                        xns atomNs "author",
                        XElement(xns atomNs "name", author),
                        XElement(xns atomNs "uri", postUrl)
                    ),
                    XElement(xns atomNs "summary", XAttribute(xn "type", "html"), summary)
                ))

        let feed =
            XElement(
                xns atomNs "feed",
                XElement(xns atomNs "title", site.Title),
                XElement(xns atomNs "link", XAttribute(xn "href", siteUrl)),
                XElement(
                    xns atomNs "link",
                    XAttribute(xn "type", "application/atom+xml"),
                    XAttribute(xn "rel", "self"),
                    XAttribute(xn "href", atomUrl)
                ),
                XElement(xns atomNs "updated", iso8601Date now),
                XElement(xns atomNs "id", siteUrl),
                XElement(
                    xns atomNs "author",
                    XElement(xns atomNs "name", site.Author),
                    XElement(xns atomNs "email", "")
                )
            )

        entries |> List.iter (fun entry -> feed.Add(entry))

        let doc = XDocument(XDeclaration("1.0", "utf-8", null), feed :> obj)
        doc.ToString()
