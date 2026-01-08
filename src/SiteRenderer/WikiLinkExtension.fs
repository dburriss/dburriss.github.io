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
type WikiLinkInline(title: string, displayText: string option) =
    inherit LeafInline()
    member val Title = title with get, set
    member val DisplayText = displayText with get, set
    member val ResolvedUrl: string option = None with get, set

    // Convenience property for backward compatibility
    member this.Label =
        match this.DisplayText with
        | Some display -> display
        | None -> this.Title

/// Parser for wiki links [[Page Name]]
type WikiLinkParser() =
    inherit InlineParser()

    do base.OpeningCharacters <- [| '[' |]

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
                // Extract the content between [[...]]
                let content =
                    slice.Text.Substring(startPosition + 2, endPosition - startPosition - 2)

                let parts = content.Split([| '|' |], 2, StringSplitOptions.None)

                let title = parts.[0].Trim()

                let displayText =
                    if parts.Length > 1 && not (String.IsNullOrWhiteSpace(parts.[1])) then
                        Some(parts.[1].Trim())
                    else
                        None

                let wikiLink = WikiLinkInline(title, displayText)
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
        let displayText =
            match link.DisplayText with
            | Some display -> display
            | None -> link.Title

        match link.ResolvedUrl with
        | Some url ->
            renderer.Write("<a href=\"") |> ignore
            renderer.WriteEscapeUrl(url) |> ignore
            renderer.Write("\">") |> ignore
            renderer.WriteEscape(displayText) |> ignore
            renderer.Write("</a>") |> ignore
        | None ->
            // Unresolved link - render as plain text with a marker
            renderer.Write("<span class=\"unresolved-link\">") |> ignore
            renderer.WriteEscape(displayText) |> ignore
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
