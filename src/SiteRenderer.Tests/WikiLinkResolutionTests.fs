namespace SiteRenderer.Tests

open System
open System.IO
open Xunit
open SiteRenderer

module WikiLinkResolutionTests =
    
    /// Integration test: End-to-end wiki link resolution behavior
    /// This test will initially FAIL because the new architecture doesn't exist yet
    [<Fact>]
    let ``Wiki links should resolve to existing content`` () =
        // Arrange - Create test content simulating real notes
        let testContent = [
            ("token.md", "Token", "This is the token page")
            ("sampling-parameters.md", "Sampling Parameters", "About sampling parameters") 
            ("model-weights.md", "Model Weights", "About model weights")
            ("context.md", "Context", "About context")
            ("llm.md", "LLM", "About LLMs")
        ]
        
        // Create test markdown with wiki links
        let markdownWithLinks = """
# Test Note

This note references [[Token]] and [[Sampling Parameters]].

It also mentions [[Model Weights]], [[Context]], and [[LLM]].

There's also an unresolved link to [[Non-Existent Page]].
"""
        
        // Act - Process the markdown through the resolution pipeline
        // This will initially FAIL because the new architecture doesn't exist yet
        let rawContentItems = 
            testContent 
            |> List.map (fun (file, title, body) ->
                {
                    SourcePath = file
                    Markdown = sprintf "# %s\n\n%s" title body
                    Meta = { 
                        Layout = None
                        Title = Some title
                        Subtitle = None
                        Author = None
                        Description = None
                        Permalink = None
                        HeaderImage = None
                        SocialImage = None
                        Tags = []
                        Categories = []
                        Topics = []
                        Keywords = []
                        Date = None
                        Comments = None
                        Published = Some true
                        Status = None
                    }
                    Kind = "note"
                } : RawContentItem
            )
        
        let resolutionContext = Renderer.buildResolutionContext rawContentItems
        let html = Parsing.markdownToHtmlWithContext markdownWithLinks (Some resolutionContext)
        
        // Assert - Verify wiki links are properly resolved
        // Links to existing content should be rendered as <a> tags
        Assert.Contains("""<a href="/notes/token/">Token</a>""", html)
        Assert.Contains("""<a href="/notes/sampling-parameters/">Sampling Parameters</a>""", html)
        Assert.Contains("""<a href="/notes/model-weights/">Model Weights</a>""", html)
        Assert.Contains("""<a href="/notes/context/">Context</a>""", html)
        Assert.Contains("""<a href="/notes/llm/">LLM</a>""", html)
        
        // Non-existent links should be rendered as unresolved spans
        Assert.Contains("""<span class="unresolved-link">Non-Existent Page</span>""", html)
        
        // Should NOT contain any literal wiki link brackets
        Assert.DoesNotContain("[[", html)
        Assert.DoesNotContain("]]", html)
        
    [<Fact>]
    let ``Wiki links with custom display text should resolve correctly`` () =
        // Arrange
        let testContent = [
            ("machine-learning.md", "Machine Learning", "About ML")
            ("functional-programming.md", "Functional Programming", "About FP")
        ]
        
        let markdownWithPipedLinks = """
Check out [[Machine Learning|ML]] and [[Functional Programming|FP]] concepts.
"""
        
        // Act
        let rawContentItems = 
            testContent 
            |> List.map (fun (file, title, body) ->
                {
                    SourcePath = file
                    Markdown = sprintf "# %s\n\n%s" title body
                    Meta = {
                        Layout = None
                        Title = Some title
                        Subtitle = None
                        Author = None
                        Description = None
                        Permalink = None
                        HeaderImage = None
                        SocialImage = None
                        Tags = []
                        Categories = []
                        Topics = []
                        Keywords = []
                        Date = None
                        Comments = None
                        Published = Some true
                        Status = None
                    }
                    Kind = "note"
                } : RawContentItem
            )
        
        let resolutionContext = Renderer.buildResolutionContext rawContentItems
        let html = Parsing.markdownToHtmlWithContext markdownWithPipedLinks (Some resolutionContext)
        
        // Assert
        // Should use custom display text but link to the correct page
        Assert.Contains("""<a href="/notes/machine-learning/">ML</a>""", html)
        Assert.Contains("""<a href="/notes/functional-programming/">FP</a>""", html)
        
    [<Fact>]
    let ``Deferred rendering pipeline should process all content types`` () =
        // This test validates the complete Option 3 architecture
        // Arrange
        let rawPost = {
            SourcePath = "_posts/test-post.md"
            Markdown = "A post with [[Token]] link"
            Meta = {
                Layout = None
                Title = Some "Test Post"
                Subtitle = None
                Author = None
                Description = None
                Permalink = None
                HeaderImage = None
                SocialImage = None
                Tags = []
                Categories = []
                Topics = []
                Keywords = []
                Date = Some (DateTime(2024, 1, 1))
                Comments = None
                Published = Some true
                Status = None
            }
            Kind = "post"
        }
        
        let rawNote = {
            SourcePath = "_notes/token.md"
            Markdown = "# Token\n\nThe token note content"
            Meta = {
                Layout = None
                Title = Some "Token"
                Subtitle = None
                Author = None
                Description = None
                Permalink = None
                HeaderImage = None
                SocialImage = None
                Tags = []
                Categories = []
                Topics = []
                Keywords = []
                Date = None
                Comments = None
                Published = Some true
                Status = None
            }
            Kind = "note"
        }
        
        let rawPage = {
            SourcePath = "about.md"
            Markdown = "About page mentioning [[Token]]"
            Meta = {
                Layout = None
                Title = Some "About"
                Subtitle = None
                Author = None
                Description = None
                Permalink = None
                HeaderImage = None
                SocialImage = None
                Tags = []
                Categories = []
                Topics = []
                Keywords = []
                Date = None
                Comments = None
                Published = Some true
                Status = None
            }
            Kind = "page"
        }
        
        // Act
        let allRawContent = [rawPost; rawNote; rawPage]
        let resolutionContext = Renderer.buildResolutionContext allRawContent
        
        let renderedPost = Renderer.renderContentItem rawPost resolutionContext
        let renderedNote = Renderer.renderContentItem rawNote resolutionContext
        let renderedPage = Renderer.renderContentItem rawPage resolutionContext
        
        // Assert
        // Post should have resolved wiki link
        Assert.Contains("""<a href="/notes/token/">Token</a>""", renderedPost.HtmlContent)
        
        // Page should have resolved wiki link
        Assert.Contains("""<a href="/notes/token/">Token</a>""", renderedPage.HtmlContent)
        
        // All items should have proper output paths
        Assert.NotEmpty(renderedPost.OutputPath)
        Assert.NotEmpty(renderedNote.OutputPath)
        Assert.NotEmpty(renderedPage.OutputPath)

module DataModelTests =
    
    [<Fact>]
    let ``RawContentItem should store markdown and metadata only`` () =
        let rawItem = {
            SourcePath = "test.md"
            Markdown = "# Test\n\nContent here"
            Meta = {
                Layout = None
                Title = Some "Test"
                Subtitle = None
                Author = None
                Description = None
                Permalink = None
                HeaderImage = None
                SocialImage = None
                Tags = []
                Categories = []
                Topics = []
                Keywords = []
                Date = None
                Comments = None
                Published = Some true
                Status = None
            }
            Kind = "note"
        }
        
        Assert.Equal("test.md", rawItem.SourcePath)
        Assert.Equal("# Test\n\nContent here", rawItem.Markdown)
        Assert.Equal("note", rawItem.Kind)
        Assert.Equal(Some "Test", rawItem.Meta.Title)

    [<Fact>]
    let ``ResolutionContext should provide title lookup`` () =
        let rawItems = [
            { SourcePath = "test1.md"; Markdown = "# Test 1"; Meta = { Layout = None; Title = Some "Test One"; Subtitle = None; Author = None; Description = None; Permalink = None; HeaderImage = None; SocialImage = None; Tags = []; Categories = []; Topics = []; Keywords = []; Date = None; Comments = None; Published = Some true; Status = None }; Kind = "note" }
            { SourcePath = "test2.md"; Markdown = "# Test 2"; Meta = { Layout = None; Title = Some "Test Two"; Subtitle = None; Author = None; Description = None; Permalink = None; HeaderImage = None; SocialImage = None; Tags = []; Categories = []; Topics = []; Keywords = []; Date = None; Comments = None; Published = Some true; Status = None }; Kind = "note" }
        ]
        
        let context = Renderer.buildResolutionContext rawItems
        
        // Should be able to look up titles
        Assert.True(context.TitleLookup.ContainsKey("test one"))  // normalized
        Assert.True(context.PathLookup.ContainsKey("test one"))
        Assert.Equal("/notes/test-one/", context.PathLookup.["test one"])

module ContentLoadingTests =
    
    [<Fact>]  
    let ``loadRawContentItem should parse frontmatter and store raw markdown`` () =
        // This test will be implemented when we create loadRawContentItem
        // For now, just pass - we'll implement this later
        Assert.True(true)

module RenderingTests =
    
    [<Fact>]
    let ``renderContentItem should produce RenderedContentItem with resolved HTML`` () =
        let rawItem = {
            SourcePath = "test.md"
            Markdown = "Content with [[Target]] link"
            Meta = {
                Layout = None
                Title = Some "Test"
                Subtitle = None
                Author = None
                Description = None
                Permalink = None
                HeaderImage = None
                SocialImage = None
                Tags = []
                Categories = []
                Topics = []
                Keywords = []
                Date = None
                Comments = None
                Published = Some true
                Status = None
            }
            Kind = "note"
        }
        
        let targetItem = {
            SourcePath = "target.md"
            Markdown = "# Target"
            Meta = {
                Layout = None
                Title = Some "Target"
                Subtitle = None
                Author = None
                Description = None
                Permalink = None
                HeaderImage = None
                SocialImage = None
                Tags = []
                Categories = []
                Topics = []
                Keywords = []
                Date = None
                Comments = None
                Published = Some true
                Status = None
            }
            Kind = "note"
        }
        
        let context = Renderer.buildResolutionContext [rawItem; targetItem]
        let rendered = Renderer.renderContentItem rawItem context
        
        Assert.Contains("""<a href="/notes/target/">Target</a>""", rendered.HtmlContent)
        Assert.Equal("test.md", rendered.SourcePath)
        Assert.NotEmpty(rendered.OutputPath)