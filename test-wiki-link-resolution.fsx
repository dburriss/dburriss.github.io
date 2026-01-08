#!/usr/bin/env dotnet fsi

/// Test to expose the wiki link resolution issue
/// This test checks for specific cases where wiki links should resolve but don't

open System
open System.IO
open System.Text.RegularExpressions

type LinkIssue = {
    SourceFile: string
    WikiLinkText: string
    ExpectedTarget: string
    IsResolved: bool
    ActualRendering: string
}

let siteDir = "_site"
let notesDir = Path.Combine(siteDir, "notes")

let findExpectedResolutions () =
    // Based on the directory structure, these links should resolve
    let noteDirectories = 
        if Directory.Exists(notesDir) then
            Directory.GetDirectories(notesDir)
            |> Array.map Path.GetFileName
            |> Array.toList
        else
            []
    
    printfn "Found note directories: %A" noteDirectories
    
    // Known mappings that should resolve based on directory structure
    let expectedResolutions = [
        ("Token", "token")
        ("Sampling Parameters", "sampling-parameters") 
        ("Model Weights", "model-weights")
        ("Context", "context")
        ("LLM", "llm")
    ]
    
    // Filter to only those that have corresponding directories
    expectedResolutions
    |> List.filter (fun (wikiText, dirName) -> List.contains dirName noteDirectories)

let analyzeFileForResolutionIssues (filePath: string) (expectedResolutions: (string * string) list) =
    let content = File.ReadAllText(filePath)
    let fileName = Path.GetRelativePath(".", filePath)
    
    let issues = ResizeArray<LinkIssue>()
    
    for (wikiText, expectedDir) in expectedResolutions do
        // Check if this wiki link appears as unresolved
        let unresolvedPattern = sprintf @"<span class=""unresolved-link"">%s</span>" (Regex.Escape(wikiText))
        let unresolvedMatch = Regex.Match(content, unresolvedPattern)
        
        // Check if this wiki link appears as resolved  
        let resolvedPattern = sprintf @"<a href=""[^""]*notes/%s/[^""]*"">%s</a>" (Regex.Escape(expectedDir)) (Regex.Escape(wikiText))
        let resolvedMatch = Regex.Match(content, resolvedPattern)
        
        if unresolvedMatch.Success then
            issues.Add({
                SourceFile = fileName
                WikiLinkText = wikiText
                ExpectedTarget = sprintf "/notes/%s/" expectedDir
                IsResolved = false
                ActualRendering = unresolvedMatch.Value
            })
        elif resolvedMatch.Success then
            // This is working correctly, no issue
            ()
        else
            // Check if it appears in any other form
            let anyMentionPattern = Regex.Escape(wikiText)
            let anyMatch = Regex.Match(content, anyMentionPattern)
            if anyMatch.Success then
                issues.Add({
                    SourceFile = fileName
                    WikiLinkText = wikiText
                    ExpectedTarget = sprintf "/notes/%s/" expectedDir
                    IsResolved = false
                    ActualRendering = "Found in unknown format"
                })
    
    issues |> Seq.toList

let runResolutionTest () =
    printfn "Wiki Link Resolution Issue Test"
    printfn "================================"
    
    if not (Directory.Exists(siteDir)) then
        printfn "❌ ERROR: Site not generated. Run './render.sh' first"
        exit 1
    
    let expectedResolutions = findExpectedResolutions ()
    printfn "\nExpected resolutions to test: %d" expectedResolutions.Length
    for (wikiText, target) in expectedResolutions do
        printfn "  - '%s' -> notes/%s/" wikiText target
    
    let noteFiles = 
        Directory.GetFiles(notesDir, "index.html", SearchOption.AllDirectories)
        |> Array.toList
    
    printfn "\nAnalyzing %d note files..." noteFiles.Length
    
    let mutable totalIssues = 0
    let mutable allIssues = []
    
    for file in noteFiles do
        let issues = analyzeFileForResolutionIssues file expectedResolutions
        totalIssues <- totalIssues + issues.Length
        allIssues <- allIssues @ issues
        
        if issues.Length > 0 then
            printfn "\n❌ Issues in %s:" (Path.GetRelativePath(".", file))
            for issue in issues do
                printfn "  - '%s' should link to %s but renders as: %s" 
                    issue.WikiLinkText 
                    issue.ExpectedTarget 
                    issue.ActualRendering
    
    printfn "\n=== FINAL RESULTS ==="
    if totalIssues = 0 then
        printfn "✅ SUCCESS: All expected wiki links are resolving correctly"
        printfn "   Found %d expected resolutions and all are working" expectedResolutions.Length
        true
    else
        printfn "❌ FAILURE: Found %d wiki link resolution issues" totalIssues
        printfn "\nSUMMARY OF ISSUES:"
        
        // Group by wiki link text
        let groupedIssues = allIssues |> List.groupBy (fun i -> i.WikiLinkText)
        for (linkText, issues) in groupedIssues do
            let sourceFiles = issues |> List.map (fun i -> i.SourceFile) |> List.distinct
            let expectedTarget = issues.[0].ExpectedTarget
            printfn "  • '%s' should resolve to %s" linkText expectedTarget
            printfn "    Failing in files: %s" (String.concat ", " sourceFiles)
        
        printfn "\nThis confirms the issue described in knowledge/wiki-links-rendering-discrepancy.md"
        printfn "The wiki link system is not properly resolving links that should have targets."
        false

// Run the test
let success = runResolutionTest ()
exit (if success then 0 else 1)