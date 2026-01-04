namespace SiteRenderer

open System
open System.Text.RegularExpressions
open Markdig
open Markdig.Helpers
open Markdig.Parsers
open Markdig.Renderers
open Markdig.Renderers.Html
open Markdig.Syntax
open Markdig.Syntax.Inlines

/// Represents a wiki-style [[link]] inline element
type WikiLinkInline(label: string) =
    inherit LeafInline()
    member val Label = label with get, set
    member val ResolvedUrl: string option = None with get, set

/// Parser for wiki links [[Page Name]]
type WikiLinkParser() =
    inherit InlineParser()

    override this.Match(processor: InlineProcessor, slice: StringSlice byref) =
        let current = slice.CurrentChar

        // Check if we're at the start of a potential wiki link
        if current = '[' && slice.PeekChar(1) = '[' then
            let startPosition = slice.Start
            let mutable endPosition = -1
            let mutable i = startPosition + 2

            // Find the closing ]]
            while i < slice.End - 1 && endPosition = -1 do
                if slice.[i] = ']' && slice.[i + 1] = ']' then
                    endPosition <- i

                i <- i + 1

            if endPosition > startPosition + 2 then
                // Extract the label
                let label = slice.Text.Substring(startPosition + 2, endPosition - startPosition - 2)

                let wikiLink = WikiLinkInline(label.Trim())
                wikiLink.Span <- SourceSpan(startPosition, endPosition + 1)

                processor.Inline <- wikiLink
                slice.Start <- endPosition + 2
                true
            else
                false
        else
            false

/// Renderer for wiki links
type WikiLinkHtmlRenderer() =
    inherit HtmlObjectRenderer<WikiLinkInline>()

    override this.Write(renderer: HtmlRenderer, link: WikiLinkInline) =
        match link.ResolvedUrl with
        | Some url ->
            renderer.Write("<a href=\"") |> ignore
            renderer.WriteEscapeUrl(url) |> ignore
            renderer.Write("\">") |> ignore
            renderer.WriteEscape(link.Label) |> ignore
            renderer.Write("</a>") |> ignore
        | None ->
            // Unresolved link - render as plain text with a marker
            renderer.Write("<span class=\"unresolved-link\">") |> ignore
            renderer.WriteEscape(link.Label) |> ignore
            renderer.Write("</span>") |> ignore

/// Markdig extension for wiki links
type WikiLinkExtension() =
    interface IMarkdownExtension with
        member this.Setup(pipeline: MarkdownPipelineBuilder) =
            if not (pipeline.InlineParsers.Contains<WikiLinkParser>()) then
                // Insert at the beginning
                pipeline.InlineParsers.Insert(0, WikiLinkParser())

        member this.Setup(pipeline: MarkdownPipeline, renderer: IMarkdownRenderer) =
            match renderer with
            | :? HtmlRenderer as htmlRenderer ->
                if not (htmlRenderer.ObjectRenderers.Contains<WikiLinkHtmlRenderer>()) then
                    htmlRenderer.ObjectRenderers.Insert(0, WikiLinkHtmlRenderer())
            | _ -> ()

/// Extension method for pipeline builder
[<AutoOpen>]
module WikiLinkExtensions =
    type MarkdownPipelineBuilder with

        member this.UseWikiLinks() =
            this.Extensions.AddIfNotAlready<WikiLinkExtension>()
            this
