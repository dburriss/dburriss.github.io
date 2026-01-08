#!/usr/bin/env dotnet fsx

/// Script to verify wiki link rendering in the actual generated site
/// Tests the _site/ directory output to confirm the current rendering issue

#r "nuget: HtmlAgilityPack, 1.11.54"

open System
open System.IO
open System.Text.RegularExpressions
open HtmlAgilityPack

module SiteWikiLinkTests =
    
    type TestResult = 
        | Pass of string
        | Fail of string * string  // test name, failure reason
        | Warning of string * string  // test name, warning message
        
    let mutable testResults = []
    
    let test (name: string) (action: unit -> TestResult) =
        try
            let result = action()
            testResults <- result :: testResults
            match result with
            | Pass msg -> printfn "✓ PASS: %s - %s" name msg
            | Fail (_, reason) -> printfn "✗ FAIL: %s - %s" name reason  
            | Warning (_, warning) -> printfn "⚠ WARN: %s - %s" name warning
        with 
        | ex -> 
            let failResult = Fail (name, ex.Message)
            testResults <- failResult :: testResults
            printfn "✗ ERROR: %s - %s" name ex.Message

    let siteDir = "_site"
    let notesDir = Path.Combine(siteDir, "notes")
    
    /// Find all HTML files in the notes directory
    let findNoteFiles () =
        if Directory.Exists(notesDir) then
            Directory.GetFiles(notesDir, "*.html", SearchOption.AllDirectories)
            |> Array.toList
        else
            []
    
    /// Find wiki link patterns in HTML content
    let findWikiLinkPatterns (html: string) =
        let literalPattern = @"\[\[([^\]]+)\]\]"  // Literal [[...]] patterns
        let literalMatches = Regex.Matches(html, literalPattern)
        
        let spanPattern = @"<span class=""unresolved-link"">([^<]+)</span>"  // Expected unresolved spans
        let spanMatches = Regex.Matches(html, spanPattern)
        
        let linkPattern = @"<a href=""[^""]*"">([^<]+)</a>"  // Expected resolved links
        let linkMatches = Regex.Matches(html, linkPattern)
        
        (literalMatches |> Seq.cast<Match> |> Seq.toList,
         spanMatches |> Seq.cast<Match> |> Seq.toList,
         linkMatches |> Seq.cast<Match> |> Seq.toList)

    let checkSiteExists () =
        test "Site directory exists" (fun () ->
            if Directory.Exists(siteDir) then
                Pass "Site directory found"
            else
                Fail ("Site directory missing", sprintf "Directory %s not found - run ./render.sh first" siteDir)
        )
        
        test "Notes directory exists" (fun () ->
            if Directory.Exists(notesDir) then
                Pass "Notes directory found"
            else
                Fail ("Notes directory missing", sprintf "Directory %s not found" notesDir)
        )

    let analyzeNoteFiles () =
        let noteFiles = findNoteFiles ()
        printfn "\n=== ANALYZING NOTE FILES ==="
        printfn "Found %d note HTML files:" noteFiles.Length
        
        if noteFiles.Length = 0 then
            test "Note files exist" (fun () ->
                Fail ("No note files", "No HTML files found in notes directory")
            )
        else
            noteFiles |> List.iteri (fun i file -> 
                printfn "  %d. %s" (i + 1) (Path.GetRelativePath(".", file))
            )
            
        noteFiles

    let checkSpecificNoteForWikiLinks (filePath: string) =
        let fileName = Path.GetFileName(filePath)
        let testName = sprintf "Wiki links in %s" fileName
        
        test testName (fun () ->
            if not (File.Exists(filePath)) then
                Fail (testName, "File not found")
            else
                let html = File.ReadAllText(filePath)
                let (literals, spans, links) = findWikiLinkPatterns html
                
                printfn "    File: %s" fileName
                printfn "    Literal [[...]] patterns: %d" literals.Count
                printfn "    Unresolved <span> patterns: %d" spans.Count  
                printfn "    Resolved <a> patterns: %d" links.Count
                
                // Show examples of what we found
                if literals.Count > 0 then
                    printfn "    Literal examples:"
                    literals |> List.take (min 3 literals.Count) |> List.iteri (fun i m ->
                        printfn "      %d. %s" (i + 1) m.Value
                    )
                
                if spans.Count > 0 then
                    printfn "    Unresolved examples:"
                    spans |> List.take (min 3 spans.Count) |> List.iteri (fun i m ->
                        printfn "      %d. %s" (i + 1) m.Groups.[1].Value
                    )
                
                if links.Count > 0 then
                    printfn "    Resolved examples:"
                    links |> List.take (min 3 links.Count) |> List.iteri (fun i m ->
                        printfn "      %d. %s" (i + 1) m.Groups.[1].Value
                    )
                
                // Test results
                if literals.Count > 0 then
                    Fail (testName, sprintf "Found %d literal [[...]] patterns - wiki links not being processed!" literals.Count)
                elif spans.Count > 0 || links.Count > 0 then
                    Pass (sprintf "Wiki links properly processed: %d unresolved, %d resolved" spans.Count links.Count)
                else
                    Warning (testName, "No wiki link patterns found - file may not contain wiki links")
        )

    let searchForKnownWikiLinks () =
        printfn "\n=== SEARCHING FOR KNOWN PROBLEMATIC PATTERNS ==="
        
        // Based on the knowledge note, look for specific patterns like [[LLM]]
        let knownPatterns = [
            ("[[LLM]]", "LLM reference")
            ("[[Pure Function]]", "Pure Function reference") 
            ("[[Machine Learning", "Machine Learning references")
            ("[[Functional Programming", "Functional Programming references")
        ]
        
        let noteFiles = findNoteFiles ()
        
        for (pattern, description) in knownPatterns do
            test (sprintf "Search for %s" description) (fun () ->
                let foundFiles = 
                    noteFiles 
                    |> List.filter (fun file ->
                        let content = File.ReadAllText(file)
                        content.Contains(pattern)
                    )
                
                if foundFiles.Length > 0 then
                    printfn "    Found '%s' in %d files:" pattern foundFiles.Length
                    foundFiles |> List.iter (fun file ->
                        printfn "      - %s" (Path.GetRelativePath(".", file))
                    )
                    Fail (description, sprintf "Found literal pattern '%s' in %d files" pattern foundFiles.Length)
                else
                    Pass (sprintf "No literal '%s' patterns found" pattern)
            )

    let analyzeHtmlStructure () =
        printfn "\n=== ANALYZING HTML STRUCTURE ==="
        
        let noteFiles = findNoteFiles ()
        if noteFiles.Length > 0 then
            let sampleFile = noteFiles.[0]
            test "HTML structure analysis" (fun () ->
                let html = File.ReadAllText(sampleFile)
                let doc = HtmlDocument()
                doc.LoadHtml(html)
                
                let articleNodes = doc.DocumentNode.SelectNodes("//article")
                let contentNodes = doc.DocumentNode.SelectNodes("//*[contains(@class, 'post-content') or contains(@class, 'note-content') or contains(@class, 'content')]")
                
                printfn "    Sample file: %s" (Path.GetFileName(sampleFile))
                printfn "    <article> nodes: %d" (if isNull articleNodes then 0 else articleNodes.Count)
                printfn "    Content nodes: %d" (if isNull contentNodes then 0 else contentNodes.Count)
                
                // Check if content is properly structured
                if isNull articleNodes then
                    Warning ("HTML structure", "No <article> nodes found")
                else
                    let articleContent = articleNodes.[0].InnerHtml
                    let hasLiteralBrackets = articleContent.Contains("[[")
                    
                    if hasLiteralBrackets then
                        Fail ("HTML structure", "Article content contains literal [[ brackets")
                    else
                        Pass "Article content structure looks correct"
            )

    let generateReport () =
        printfn "\n=== FINAL REPORT ==="
        let passed = testResults |> List.filter (function Pass _ -> true | _ -> false) |> List.length
        let failed = testResults |> List.filter (function Fail _ -> true | _ -> false) |> List.length
        let warnings = testResults |> List.filter (function Warning _ -> true | _ -> false) |> List.length
        
        printfn "Tests Passed: %d" passed
        printfn "Tests Failed: %d" failed  
        printfn "Warnings: %d" warnings
        
        if failed > 0 then
            printfn "\nFAILED TESTS (indicating wiki link issues):"
            testResults 
            |> List.rev 
            |> List.iter (function 
                | Fail (name, reason) -> printfn "  ✗ %s: %s" name reason 
                | _ -> ())
        
        if warnings > 0 then
            printfn "\nWARNINGS:"
            testResults 
            |> List.rev 
            |> List.iter (function 
                | Warning (name, warning) -> printfn "  ⚠ %s: %s" name warning 
                | _ -> ())
                
        if failed = 0 then
            printfn "\n🎉 SUCCESS: All wiki links are rendering correctly!"
        else
            printfn "\n❌ CONFIRMED: Wiki links are not rendering properly in the site"
            printfn "   The site renderer is outputting literal [[...]] instead of HTML elements"
            printfn "   This confirms the issue described in the knowledge note."
        
        failed = 0

    let runAllTests () =
        printfn "Wiki Link Site Rendering Test"
        printfn "============================="
        printfn "Testing actual rendered HTML in %s directory\n" siteDir
        
        checkSiteExists ()
        let noteFiles = analyzeNoteFiles ()
        
        if noteFiles.Length > 0 then
            // Test each note file
            noteFiles |> List.iter checkSpecificNoteForWikiLinks
            
            searchForKnownWikiLinks ()
            analyzeHtmlStructure ()
        
        generateReport ()

// Run the tests
SiteWikiLinkTests.runAllTests ()