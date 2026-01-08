#!/usr/bin/env dotnet fsx

/// Script to test wiki link parsing and rendering functionality
/// This validates the wiki link implementation before investigating site rendering issues

#r "nuget: Markdig, 0.37.0"
#r "nuget: YamlDotNet, 13.0.2"
#load "src/SiteRenderer/Models.fs"
#load "src/SiteRenderer/WikiLinkExtension.fs"
#load "src/SiteRenderer/Parsing.fs"

open System
open System.IO
open Markdig
open SiteRenderer

module WikiLinkUnitTests =
    
    /// Test result type for clear reporting
    type TestResult = 
        | Pass of string
        | Fail of string * string  // test name, failure reason
        
    let mutable testResults = []
    
    let test (name: string) (action: unit -> unit) =
        try
            action()
            testResults <- Pass name :: testResults
            printfn "✓ PASS: %s" name
        with 
        | ex -> 
            testResults <- Fail (name, ex.Message) :: testResults
            printfn "✗ FAIL: %s - %s" name ex.Message
    
    /// Create the same pipeline used by the site
    let createTestPipeline () =
        MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePipeTables()
            .UseYamlFrontMatter()
            .UseWikiLinks()
            .Build()
    
    let runBasicParsingTests () =
        printfn "\n=== BASIC WIKI LINK PARSING TESTS ==="
        
        test "Extension registration" (fun () ->
            let builder = MarkdownPipelineBuilder()
            let pipeline = builder.UseWikiLinks().Build()
            if isNull pipeline then failwith "Pipeline creation failed"
        )
        
        test "Simple wiki link parsing" (fun () ->
            let markdown = "This is a [[Test Link]] in text."
            let pipeline = createTestPipeline ()
            let html = Markdown.ToHtml(markdown, pipeline)
            printfn "    Input:  %s" markdown
            printfn "    Output: %s" html
            
            if html.Contains("[[") || html.Contains("]]") then 
                failwith (sprintf "HTML contains literal brackets: %s" html)
            
            if not (html.Contains("Test Link")) then 
                failwith (sprintf "HTML missing expected text: %s" html)
                
            let hasWikiLink = html.Contains("<span class=\"unresolved-link\">") || html.Contains("<a href=")
            if not hasWikiLink then 
                failwith (sprintf "HTML should contain wiki link markup: %s" html)
        )
        
        test "Wiki link with pipe syntax" (fun () ->
            let markdown = "This is a [[Target|Display Text]] link."
            let pipeline = createTestPipeline ()
            let html = Markdown.ToHtml(markdown, pipeline)
            printfn "    Input:  %s" markdown
            printfn "    Output: %s" html
            
            if html.Contains("[[") || html.Contains("]]") then 
                failwith "HTML should not contain literal brackets"
            
            if not (html.Contains("Display Text")) then 
                failwith "HTML should contain display text"
            
            if html.Contains("Target") then 
                failwith "HTML should not show target as display text"
        )
        
        test "Multiple wiki links" (fun () ->
            let markdown = "Links: [[First]] and [[Second|Display]] and [[Third]]."
            let pipeline = createTestPipeline ()
            let html = Markdown.ToHtml(markdown, pipeline)
            printfn "    Input:  %s" markdown
            printfn "    Output: %s" html
            
            if html.Contains("[[") || html.Contains("]]") then 
                failwith "HTML should not contain literal brackets"
                
            let expectedTexts = ["First"; "Display"; "Third"]
            for text in expectedTexts do
                if not (html.Contains(text)) then 
                    failwith (sprintf "HTML missing expected text '%s'" text)
        )
        
        test "Empty wiki links handled gracefully" (fun () ->
            let testCases = ["[[]]"; "[[]"; "[[|]]"; "[[Title|]]"]
            let pipeline = createTestPipeline ()
            
            for testCase in testCases do
                let html = Markdown.ToHtml(testCase, pipeline)
                printfn "    Input:  %s -> Output: %s" testCase html
                if String.IsNullOrEmpty(html) then 
                    failwith (sprintf "Empty output for input: %s" testCase)
        )

    let runWikiLinkExtractionTests () =
        printfn "\n=== WIKI LINK EXTRACTION TESTS ==="
        
        test "extractWikiLinks basic functionality" (fun () ->
            let markdown = "Text with [[Title1]] and [[Title2|Display2]] links."
            let links = Parsing.extractWikiLinks markdown
            printfn "    Found %d links: %A" links.Length links
            
            if links.Length <> 2 then 
                failwith (sprintf "Expected 2 links, got %d" links.Length)
            
            let (title1, display1) = links.[0]
            if title1 <> "Title1" || display1 <> None then 
                failwith "First link incorrect"
            
            let (title2, display2) = links.[1]
            if title2 <> "Title2" || display2 <> Some "Display2" then 
                failwith "Second link incorrect"
        )
        
        test "extractWikiLinks ignores bracket patterns" (fun () ->
            let markdown = "Command [[:space:]] should be ignored but [[Real Link]] should not."
            let links = Parsing.extractWikiLinks markdown
            printfn "    Found %d links: %A" links.Length links
            
            if links.Length <> 1 then 
                failwith (sprintf "Expected 1 link, got %d" links.Length)
            
            let (title, _) = links.[0]
            if title <> "Real Link" then 
                failwith "Wrong link extracted"
        )

    let runSiteMarkdownPipelineTests () =
        printfn "\n=== SITE MARKDOWN PIPELINE TESTS ==="
        
        test "Site's markdownPipeline processes wiki links" (fun () ->
            let markdown = "This contains [[LLM]] and [[Machine Learning|ML]] references."
            let html = Parsing.markdownToHtml markdown
            printfn "    Input:  %s" markdown
            printfn "    Output: %s" html
            
            if html.Contains("[[") || html.Contains("]]") then 
                failwith (sprintf "Site pipeline failed to process wiki links: %s" html)
                
            // Should contain the expected text content
            if not (html.Contains("LLM")) then 
                failwith "Missing LLM text"
            if not (html.Contains("ML")) then 
                failwith "Missing ML display text"
        )
        
        test "Complex markdown with wiki links" (fun () ->
            let markdown = """
# Article Title

Some content with [[Pure Function]] reference and [[Functional Programming|FP]] concepts.

## Another Section

More text with [[Data Structures]] and normal [regular link](http://example.com).
"""
            let html = Parsing.markdownToHtml markdown
            printfn "    Input markdown length: %d chars" markdown.Length
            printfn "    Output HTML: %s" (html.Substring(0, min 200 html.Length) + "...")
            
            // Should NOT contain literal wiki link brackets (they should be processed)
            if html.Contains("[[") || html.Contains("]]") then 
                failwith "Site pipeline failed to process wiki links"
                
            // Should maintain other markdown formatting (headers with IDs)
            if not (html.Contains("<h1") && html.Contains("<h2")) then 
                failwith "Lost other markdown formatting"
                
            // Should preserve regular links
            if not (html.Contains("href=") && html.Contains("http://example.com")) then 
                failwith "Lost regular links"
                
            // Should contain processed wiki links
            if not (html.Contains("unresolved-link") || html.Contains("Pure Function")) then
                failwith "Wiki links not processed correctly"
        )

    let runAllTests () =
        printfn "Starting Wiki Link Tests..."
        printfn "=============================="
        
        runBasicParsingTests ()
        runWikiLinkExtractionTests ()
        runSiteMarkdownPipelineTests ()
        
        printfn "\n=== TEST SUMMARY ==="
        let passed = testResults |> List.filter (function Pass _ -> true | _ -> false) |> List.length
        let failed = testResults |> List.filter (function Fail _ -> true | _ -> false) |> List.length
        
        printfn "Passed: %d" passed
        printfn "Failed: %d" failed
        
        if failed > 0 then
            printfn "\nFAILED TESTS:"
            testResults 
            |> List.rev 
            |> List.iter (function 
                | Fail (name, reason) -> printfn "  ✗ %s: %s" name reason 
                | _ -> ())
            
        printfn "\nOverall: %s" (if failed = 0 then "SUCCESS" else "FAILURE")
        failed = 0

// Run the tests if this script is executed directly
if fsi.CommandLineArgs.Length > 0 && fsi.CommandLineArgs.[0].EndsWith("test-wiki-links.fsx") then
    let success = WikiLinkUnitTests.runAllTests ()
    exit (if success then 0 else 1)