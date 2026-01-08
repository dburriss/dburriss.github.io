namespace SiteRenderer.Tests

open System
open Xunit
open SiteRenderer
open Markdig

module WikiLinkTests =

    let createPipeline () =
        MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePipeTables()
            .UseYamlFrontMatter()
            .UseWikiLinks()
            .Build()

    [<Fact>]
    let ``Extension registration should work`` () =
        let builder = MarkdownPipelineBuilder()
        let extension = WikiLinkExtension()
        builder.Extensions.Add(extension)
        
        // Check that extension was added
        Assert.True(builder.Extensions.Count > 0)
        
        let pipeline = builder.Build()
        Assert.NotNull(pipeline)

    [<Fact>]
    let ``Basic wiki link parsing should work`` () =
        let markdown = "This is a [[Test Link]] in text."
        let pipeline = createPipeline ()
        let html = Markdown.ToHtml(markdown, pipeline)
        
        // Debug: print what we actually got
        printfn "Generated HTML: %s" html
        
        // Should NOT contain literal [[ ]] brackets in output
        Assert.False(html.Contains("[["), sprintf "HTML should not contain literal [[ brackets. Got: %s" html)
        Assert.False(html.Contains("]]"), sprintf "HTML should not contain literal ]] brackets. Got: %s" html)
        
        // Should contain either a link or unresolved link span
        let hasLink = html.Contains("<a href=") || html.Contains("unresolved-link")
        Assert.True(hasLink, sprintf "HTML should contain either link or unresolved span. Got: %s" html)

    [<Fact>]
    let ``Wiki link with pipe syntax should parse title and display text separately`` () =
        let markdown = "This is a [[Target|Display Text]] link."
        let pipeline = createPipeline ()
        let html = Markdown.ToHtml(markdown, pipeline)
        
        // Should not contain literal brackets
        Assert.False(html.Contains("[["), "HTML should not contain literal [[ brackets")
        Assert.False(html.Contains("]]"), "HTML should not contain literal ]] brackets")
        
        // Should contain the display text, not the target
        Assert.True(html.Contains("Display Text"), sprintf "HTML should contain 'Display Text'. Got: %s" html)
        Assert.False(html.Contains("Target"), sprintf "HTML should not contain 'Target' as display text. Got: %s" html)

    [<Fact>]
    let ``Wiki link without pipe should use title for display`` () =
        let markdown = "This is a [[Simple Link]] without pipe."
        let pipeline = createPipeline ()
        let html = Markdown.ToHtml(markdown, pipeline)
        
        // Should contain the title text
        Assert.True(html.Contains("Simple Link"), sprintf "HTML should contain 'Simple Link'. Got: %s" html)

    [<Fact>]
    let ``Multiple wiki links should all be processed`` () =
        let markdown = "Links: [[First]] and [[Second|Display]] and [[Third]]."
        let pipeline = createPipeline ()
        let html = Markdown.ToHtml(markdown, pipeline)
        
        // Should not contain any literal brackets
        Assert.False(html.Contains("[["), "HTML should not contain literal [[ brackets")
        Assert.False(html.Contains("]]"), "HTML should not contain literal ]] brackets")
        
        // Should contain all the expected text
        Assert.True(html.Contains("First"), "Should contain 'First'")
        Assert.True(html.Contains("Display"), "Should contain 'Display'")
        Assert.True(html.Contains("Third"), "Should contain 'Third'")

    [<Fact>]
    let ``Empty or malformed wiki links should be handled gracefully`` () =
        let testCases = [
            "[[]]"  // Empty
            "[[]"   // Incomplete
            "[[|]]" // Empty parts
            "[[Title|]]" // Empty display
        ]
        
        let pipeline = createPipeline ()
        
        for testCase in testCases do
            let html = Markdown.ToHtml(testCase, pipeline)
            // Should not crash and should produce some output
            Assert.NotNull(html)
            Assert.NotEmpty(html)

module ParsingTests =

    [<Fact>]
    let ``extractWikiLinks should return title and display text tuples`` () =
        let markdown = "Text with [[Title1]] and [[Title2|Display2]] links."
        let links = Parsing.extractWikiLinks markdown
        
        Assert.Equal(2, links.Length)
        
        let (title1, display1) = links.[0]
        Assert.Equal("Title1", title1)
        Assert.Equal(None, display1)
        
        let (title2, display2) = links.[1]
        Assert.Equal("Title2", title2)
        Assert.Equal(Some "Display2", display2)

    [<Fact>]
    let ``extractWikiLinks should handle multiple pipes correctly`` () =
        let markdown = "Link with [[Title|Display|Extra]] pipes."
        let links = Parsing.extractWikiLinks markdown
        
        Assert.Equal(1, links.Length)
        
        let (title, display) = links.[0]
        Assert.Equal("Title", title)
        Assert.Equal(Some "Display|Extra", display) // Everything after first pipe

    [<Fact>]
    let ``extractWikiLinks should ignore bracket patterns`` () =
        let markdown = "Command [[:space:]] should be ignored but [[Real Link]] should not."
        let links = Parsing.extractWikiLinks markdown
        
        Assert.Equal(1, links.Length)
        
        let (title, _) = links.[0]
        Assert.Equal("Real Link", title)

module WikiLinkInlineTests =

    [<Fact>]
    let ``WikiLinkInline constructor should accept title and display text`` () =
        let wikiLink = WikiLinkInline("TestTitle", Some "TestDisplay")
        
        Assert.Equal("TestTitle", wikiLink.Title)
        Assert.Equal(Some "TestDisplay", wikiLink.DisplayText)
        Assert.Equal("TestDisplay", wikiLink.Label) // Backward compatibility

    [<Fact>]
    let ``WikiLinkInline without display text should use title for label`` () =
        let wikiLink = WikiLinkInline("TestTitle", None)
        
        Assert.Equal("TestTitle", wikiLink.Title)
        Assert.Equal(None, wikiLink.DisplayText)
        Assert.Equal("TestTitle", wikiLink.Label) // Backward compatibility

module RendererTests =

    [<Fact>]
    let ``Wiki link with resolved URL should render as anchor`` () =
        let wikiLink = WikiLinkInline("TestTitle", Some "TestDisplay")
        wikiLink.ResolvedUrl <- Some "/test/url/"
        
        let renderer = Markdig.Renderers.HtmlRenderer(System.IO.StringWriter())
        let htmlRenderer = WikiLinkHtmlRenderer()
        
        htmlRenderer.Write(renderer, wikiLink)
        let html = renderer.Writer.ToString()
        
        Assert.Contains("<a href=\"/test/url/\">", html)
        Assert.Contains("TestDisplay", html)
        Assert.Contains("</a>", html)

    [<Fact>]
    let ``Wiki link without resolved URL should render as unresolved span`` () =
        let wikiLink = WikiLinkInline("TestTitle", Some "TestDisplay")
        // ResolvedUrl is None by default
        
        let renderer = Markdig.Renderers.HtmlRenderer(System.IO.StringWriter())
        let htmlRenderer = WikiLinkHtmlRenderer()
        
        htmlRenderer.Write(renderer, wikiLink)
        let html = renderer.Writer.ToString()
        
        Assert.Contains("<span class=\"unresolved-link\">", html)
        Assert.Contains("TestDisplay", html)
        Assert.Contains("</span>", html)
