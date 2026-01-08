#!/usr/bin/env dotnet fsi

/// Simple validation script to check if wiki links are rendering correctly in the generated site
/// This confirms whether the issue described in the knowledge note still exists

open System
open System.IO
open System.Text.RegularExpressions

let siteDir = "_site"
let notesDir = Path.Combine(siteDir, "notes")

let checkSiteGenerated () =
    if not (Directory.Exists(siteDir)) then
        printfn "❌ ERROR: Site not generated. Run './render.sh' first"
        false
    elif not (Directory.Exists(notesDir)) then
        printfn "❌ ERROR: Notes directory missing in generated site"
        false
    else
        printfn "✓ Site generated and notes directory exists"
        true

let findNoteFiles () =
    Directory.GetFiles(notesDir, "index.html", SearchOption.AllDirectories)
    |> Array.toList

let analyzeWikiLinks (filePath: string) =
    let content = File.ReadAllText(filePath)
    let fileName = Path.GetRelativePath(".", filePath)
    
    // Count literal [[...]] patterns (the problem we're checking for)
    let literalPattern = @"\[\[([^\]]+)\]\]"
    let literalMatches = Regex.Matches(content, literalPattern)
    
    // Count proper unresolved spans (what we expect)
    let spanPattern = @"<span class=""unresolved-link"">([^<]+)</span>"
    let spanMatches = Regex.Matches(content, spanPattern)
    
    // Count resolved links (what we hope for)
    let linkPattern = @"<a href=""[^""]*notes/[^""]*"">([^<]+)</a>"
    let linkMatches = Regex.Matches(content, linkPattern)
    
    (fileName, literalMatches.Count, spanMatches.Count, linkMatches.Count, literalMatches, spanMatches, linkMatches)

let main () =
    printfn "Wiki Link Site Validation"
    printfn "========================="
    
    if not (checkSiteGenerated()) then
        exit 1
    
    let noteFiles = findNoteFiles ()
    printfn "\nFound %d note files to check" noteFiles.Length
    
    if noteFiles.Length = 0 then
        printfn "❌ No note files found to test"
        exit 1
    
    let mutable totalLiterals = 0
    let mutable totalSpans = 0  
    let mutable totalLinks = 0
    let mutable filesWithIssues = []
    
    printfn "\nAnalyzing wiki link rendering..."
    printfn "================================"
    
    for file in noteFiles do
        let (fileName, literals, spans, links, literalMatches, spanMatches, linkMatches) = analyzeWikiLinks file
        
        totalLiterals <- totalLiterals + literals
        totalSpans <- totalSpans + spans
        totalLinks <- totalLinks + links
        
        let status = 
            if literals > 0 then "❌ BROKEN"
            elif spans > 0 || links > 0 then "✓ WORKING"
            else "⚪ NO LINKS"
        
        printfn "%s: %s (literal: %d, unresolved: %d, resolved: %d)" 
            fileName status literals spans links
        
        // Show examples if there are issues
        if literals > 0 then
            filesWithIssues <- fileName :: filesWithIssues
            printfn "    Literal examples:"
            for i in 0 .. min 2 (literals - 1) do
                let m = literalMatches.[i]
                printfn "      - %s" m.Value
        
        // Show examples of working links
        if spans > 0 then
            printfn "    Unresolved examples:"
            for i in 0 .. min 2 (spans - 1) do
                let m = spanMatches.[i] 
                printfn "      - %s" m.Groups.[1].Value
                
        if links > 0 then
            printfn "    Resolved examples:"  
            for i in 0 .. min 2 (links - 1) do
                let m = linkMatches.[i]
                printfn "      - %s" m.Groups.[1].Value
    
    printfn "\n=== FINAL RESULTS ==="
    printfn "Total literal [[...]] patterns: %d" totalLiterals
    printfn "Total unresolved spans: %d" totalSpans  
    printfn "Total resolved links: %d" totalLinks
    printfn "Files with issues: %d" filesWithIssues.Length
    
    if totalLiterals > 0 then
        printfn "\n❌ ISSUE CONFIRMED: Wiki links are NOT rendering properly"
        printfn "   Found %d literal [[...]] patterns in the generated HTML" totalLiterals
        printfn "   Files with problems:"
        for file in List.rev filesWithIssues do
            printfn "     - %s" file
        printfn "\n   This confirms the issue described in knowledge/wiki-links-rendering-discrepancy.md"
        false
    else
        printfn "\n✅ SUCCESS: Wiki links are rendering correctly!"
        printfn "   All wiki links are being processed into proper HTML elements"
        printfn "   - %d unresolved links rendered as <span> elements" totalSpans
        printfn "   - %d resolved links rendered as <a> elements" totalLinks
        printfn "\n   The issue described in the knowledge note appears to be RESOLVED"
        true

// Run the validation
let success = main ()
exit (if success then 0 else 1)