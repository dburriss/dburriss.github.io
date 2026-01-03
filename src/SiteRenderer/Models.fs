namespace SiteRenderer

open System

[<CLIMutable>]
type TopicDef =
    { Id: string
      Name: string
      Description: string
      LegacyCategory: string option
      LegacyTags: string list }

[<CLIMutable>]
type SiteConfig =
    { Title: string
      Description: string
      Author: string
      Url: string
      BaseUrl: string
      HeaderImage: string option
      SocialImage: string option
      TwitterUsername: string option
      GithubUsername: string option
      FacebookUsername: string option
      EmailUsername: string option
      DisqusUsername: string option
      GoogleTrackingId: string option
      CopyrightName: string option
      IsProduction: bool
      Include: string list
      Topics: TopicDef list }

[<CLIMutable>]
type FrontMatter =
    { Layout: string option
      Title: string option
      Subtitle: string option
      Author: string option
      Description: string option
      Permalink: string option
      HeaderImage: string option
      SocialImage: string option
      Tags: string list
      Categories: string list
      Topics: string list
      Keywords: string list
      Date: DateTime option
      Comments: bool option
      Published: bool option }

[<CLIMutable>]
type PageMeta =
    { Title: string
      Subtitle: string option
      Description: string option
      Author: string option
      Date: DateTime option
      HeaderImage: string option
      SocialImage: string option
      Url: string
      Tags: string list
      Categories: string list
      Topics: string list
      Related: PostSummary list
      Previous: PostSummary option
      Next: PostSummary option
      CommentsEnabled: bool
      Layout: string }

and PostSummary =
    { Title: string
      Url: string
      Date: DateTime option
      Subtitle: string option
      SocialImage: string option }

[<CLIMutable>]
type ContentItem =
    { SourcePath: string
      OutputPath: string
      Markdown: string option
      HtmlContent: string
      ExcerptHtml: string option
      Meta: FrontMatter
      PageMeta: PageMeta
      Kind: string }

[<CLIMutable>]
type SiteIndex =
    { Posts: ContentItem list
      Pages: ContentItem list
      Categories: Map<string, ContentItem list>
      Tags: Map<string, ContentItem list>
      Topics: Map<string, ContentItem list> }

[<CLIMutable>]
type RenderContext =
    { Config: SiteConfig
      Index: SiteIndex
      OutputRoot: string }

[<CLIMutable>]
type RenderedPage = { OutputPath: string; Content: string }

[<CLIMutable>]
type SearchDoc =
    { id: string
      url: string
      title: string
      body: string
      excerpt: string option
      date: string option
      keywords: string list
      topics: string list }
