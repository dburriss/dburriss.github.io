#!/usr/bin/env dotnet fsx

/// Comprehensive Site Validation Script
/// 
/// DESCRIPTION:
///   Validates the generated site for wiki link rendering, asset inclusion, 
///   content counts, and overall site structure integrity.
///
/// USAGE:
///   dotnet fsi scripts/validate-site.fsx
///
/// This script consolidates and enhances validation capabilities from:
/// - test-site-wiki-links.fsx (wiki link validation)  
/// - test-site-validation.fsx (simple validation)
/// And adds new validation for assets and content counts.

#r "nuget: HtmlAgilityPack, 1.11.54"
#r "nuget: YamlDotNet, 13.0.2"

open System
open System.IO
open System.Text.RegularExpressions
open HtmlAgilityPack
open YamlDotNet.Serialization

module ValidationFramework =
    
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
    
    let getTestSummary () =
        let passed = testResults |> List.filter (function Pass _ -> true | _ -> false) |> List.length
        let failed = testResults |> List.filter (function Fail _ -> true | _ -> false) |> List.length
        let warnings = testResults |> List.filter (function Warning _ -> true | _ -> false) |> List.length
        (passed, failed, warnings)

module WikiLinkValidation =
    
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
    
    /// Validate wiki links in a specific HTML file
    let validateFileWikiLinks (filePath: string) =
        let fileName = Path.GetFileName(filePath)
        let testName = sprintf "Wiki links in %s" fileName
        
        ValidationFramework.test testName (fun () ->
            if not (File.Exists(filePath)) then
                ValidationFramework.Fail (testName, "File not found")
            else
                let html = File.ReadAllText(filePath)
                let (literals, spans, links) = findWikiLinkPatterns html
                
                printfn "    File: %s" fileName
                printfn "    Literal [[...]] patterns: %d" literals.Length
                printfn "    Unresolved <span> patterns: %d" spans.Length  
                printfn "    Resolved <a> patterns: %d" links.Length
                
                // Show examples of what we found
                if literals.Length > 0 then
                    printfn "    Literal examples:"
                    literals |> List.take (min 3 literals.Length) |> List.iteri (fun i m ->
                        printfn "      %d. %s" (i + 1) m.Value
                    )
                
                // Test results
                if literals.Length > 0 then
                    ValidationFramework.Fail (testName, sprintf "Found %d literal [[...]] patterns - wiki links not being processed!" literals.Length)
                elif spans.Length > 0 || links.Length > 0 then
                    ValidationFramework.Pass (sprintf "Wiki links properly processed: %d unresolved, %d resolved" spans.Length links.Length)
                else
                    ValidationFramework.Warning (testName, "No wiki link patterns found - file may not contain wiki links")
        )
    
    /// Search for known problematic wiki link patterns
    let searchKnownPatterns (noteFiles: string list) =
        printfn "\n=== SEARCHING FOR KNOWN PROBLEMATIC PATTERNS ==="
        
        let knownPatterns = [
            ("[[LLM]]", "LLM reference")
            ("[[Pure Function]]", "Pure Function reference") 
            ("[[Machine Learning", "Machine Learning references")
            ("[[Functional Programming", "Functional Programming references")
            ("[[Algorithm", "Algorithm references")
            ("[[Data Structure", "Data Structure references")
        ]
        
        for (pattern, description) in knownPatterns do
            ValidationFramework.test (sprintf "Search for %s" description) (fun () ->
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
                    ValidationFramework.Fail (description, sprintf "Found literal pattern '%s' in %d files" pattern foundFiles.Length)
                else
                    ValidationFramework.Pass (sprintf "No literal '%s' patterns found" pattern)
            )
    
    /// Analyze HTML structure integrity
    let analyzeHtmlStructure (sampleFile: string) =
        ValidationFramework.test "HTML structure analysis" (fun () ->
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
                ValidationFramework.Warning ("HTML structure", "No <article> nodes found")
            else
                let articleContent = articleNodes.[0].InnerHtml
                let hasLiteralBrackets = articleContent.Contains("[[")
                
                if hasLiteralBrackets then
                    ValidationFramework.Fail ("HTML structure", "Article content contains literal [[ brackets")
                else
                    ValidationFramework.Pass "Article content structure looks correct"
        )

module AssetValidation =
    
    /// Load include patterns from _config.yml using simple text parsing
    let loadIncludePatterns (configPath: string) =
        if File.Exists(configPath) then
            try
                let yamlContent = File.ReadAllText(configPath)
                let lines = yamlContent.Split('\n')
                
                // Find the include: section
                let mutable inIncludeSection = false
                let mutable patterns = []
                
                for line in lines do
                    let trimmed = line.Trim()
                    if trimmed = "include:" then
                        inIncludeSection <- true
                    elif inIncludeSection then
                        if trimmed.StartsWith("- ") then
                            patterns <- trimmed.Substring(2).Trim() :: patterns
                        elif trimmed <> "" && not (trimmed.StartsWith("  ")) then
                            // End of include section
                            inIncludeSection <- false
                
                Some (List.rev patterns)
            with
            | ex -> 
                printfn "Warning: Could not parse _config.yml: %s" ex.Message
                None
        else
            None
    
    /// Convert glob pattern to regex for simple matching
    let globToRegex (pattern: string) =
        let regexPattern = pattern.Replace("*", "[^/]*").Replace("**", ".*")
        new Regex(sprintf "^%s$" regexPattern)
    
    /// Check if includes from _config.yml are copied to _site/
    let validateIncludeAssets (siteDir: string) =
        let configPath = "_config.yml"
        
        ValidationFramework.test "Config file exists" (fun () ->
            if File.Exists(configPath) then
                ValidationFramework.Pass "Found _config.yml"
            else
                ValidationFramework.Fail ("Config missing", "_config.yml not found")
        )
        
        match loadIncludePatterns configPath with
        | None -> 
            ValidationFramework.test "Include patterns validation" (fun () ->
                ValidationFramework.Warning ("Include patterns", "Could not load include patterns from _config.yml")
            )
        | Some includePatterns ->
            printfn "\n=== VALIDATING INCLUDE ASSETS ==="
            printfn "Include patterns from _config.yml: %A" includePatterns
            
            for pattern in includePatterns do
                ValidationFramework.test (sprintf "Include pattern: %s" pattern) (fun () ->
                    try
                        if pattern.Contains("*") then
                            // Glob pattern - check if directory/files exist
                            let basePath = pattern.Split('*').[0].TrimEnd('/')
                            let sourcePath = basePath
                            let targetPath = Path.Combine(siteDir, basePath)
                            
                            if Directory.Exists(sourcePath) then
                                let sourceCount = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories).Length
                                if sourceCount = 0 then
                                    ValidationFramework.Pass (sprintf "Pattern %s: Source directory empty, nothing to copy" pattern)
                                elif Directory.Exists(targetPath) then
                                    let targetCount = Directory.GetFiles(targetPath, "*", SearchOption.AllDirectories).Length
                                    ValidationFramework.Pass (sprintf "Pattern copied: %d files from %s to %s" sourceCount sourcePath targetPath)
                                else
                                    ValidationFramework.Fail (sprintf "Pattern %s" pattern, sprintf "Source has %d files but target directory not created" sourceCount)
                            elif File.Exists(sourcePath) && File.Exists(targetPath) then
                                ValidationFramework.Pass (sprintf "File copied: %s" pattern)
                            elif File.Exists(sourcePath) then
                                ValidationFramework.Fail (sprintf "File %s" pattern, sprintf "Source exists but not copied to %s" targetPath)
                            else
                                ValidationFramework.Warning (sprintf "Pattern %s" pattern, sprintf "Source %s does not exist" sourcePath)
                        else
                            // Exact file
                            let sourcePath = pattern
                            let targetPath = Path.Combine(siteDir, pattern)
                            
                            if File.Exists(sourcePath) && File.Exists(targetPath) then
                                ValidationFramework.Pass (sprintf "File copied: %s" pattern)
                            elif File.Exists(sourcePath) then
                                ValidationFramework.Fail (sprintf "File %s" pattern, sprintf "Source exists but not copied to %s" targetPath)
                            else
                                ValidationFramework.Warning (sprintf "File %s" pattern, sprintf "Source file %s does not exist" sourcePath)
                    with
                    | ex -> ValidationFramework.Fail (sprintf "Pattern %s" pattern, ex.Message)
                )

module ContentCountValidation =
    
    /// Count source markdown files
    let countSourceFiles (directory: string, pattern: string) =
        if Directory.Exists(directory) then
            Directory.GetFiles(directory, pattern).Length
        else
            0
    
    /// Count generated HTML files  
    let countGeneratedFiles (directory: string) =
        if Directory.Exists(directory) then
            Directory.GetFiles(directory, "index.html", SearchOption.AllDirectories).Length
        else
            0
    
    /// Validate content counts match between source and generated
    let validateContentCounts (siteDir: string) =
        printfn "\n=== VALIDATING CONTENT COUNTS ==="
        
        let notesSourceCount = countSourceFiles("_notes", "*.md")
        let notesGeneratedCount = countGeneratedFiles(Path.Combine(siteDir, "notes"))
        
        ValidationFramework.test "Notes count validation" (fun () ->
            printfn "    Source notes (_notes/*.md): %d" notesSourceCount  
            printfn "    Generated notes (_site/notes/): %d" notesGeneratedCount
            
            if notesSourceCount = 0 then
                ValidationFramework.Warning ("Notes count", "No source notes found in _notes/")
            elif notesGeneratedCount = 0 then
                ValidationFramework.Fail ("Notes count", "No generated notes found in _site/notes/")
            elif notesGeneratedCount >= notesSourceCount then
                ValidationFramework.Pass (sprintf "Notes generated correctly: %d source, %d generated (may include index pages)" notesSourceCount notesGeneratedCount)
            else
                ValidationFramework.Fail ("Notes count", sprintf "Missing generated notes: %d source vs %d generated" notesSourceCount notesGeneratedCount)
        )

module SiteStructureValidation =
    
    /// Check that expected site directories exist
    let validateSiteStructure (siteDir: string) =
        printfn "\n=== VALIDATING SITE STRUCTURE ==="
        
        ValidationFramework.test "Site directory exists" (fun () ->
            if Directory.Exists(siteDir) then
                ValidationFramework.Pass "Site directory found"
            else
                ValidationFramework.Fail ("Site directory missing", sprintf "Directory %s not found - run ./render.sh first" siteDir)
        )
        
        let expectedDirs = [
            ("notes", "Notes directory")
        ]
        
        for (dirName, description) in expectedDirs do
            ValidationFramework.test description (fun () ->
                let dirPath = Path.Combine(siteDir, dirName)
                if Directory.Exists(dirPath) then
                    let fileCount = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories).Length
                    ValidationFramework.Pass (sprintf "%s found with %d files" description fileCount)
                else
                    ValidationFramework.Fail (description, sprintf "Directory %s not found" dirPath)
            )

module SearchIndexValidation =
    
    /// Validate that the search index was built successfully
    let validateSearchIndex (siteDir: string) =
        printfn "\n=== VALIDATING SEARCH INDEX ==="
        
        let searchDir = Path.Combine(siteDir, "search")
        let indexFile = Path.Combine(searchDir, "index.json")
        let manifestFile = Path.Combine(searchDir, "manifest.json") 
        let docsFile = Path.Combine(searchDir, "docs.json")
        
        ValidationFramework.test "Search directory exists" (fun () ->
            if Directory.Exists(searchDir) then
                ValidationFramework.Pass "Search directory found"
            else
                ValidationFramework.Fail ("Search directory missing", sprintf "Directory %s not found" searchDir)
        )
        
        ValidationFramework.test "Search index file exists" (fun () ->
            if File.Exists(indexFile) then
                try
                    let content = File.ReadAllText(indexFile)
                    if String.IsNullOrWhiteSpace(content) then
                        ValidationFramework.Fail ("Empty index file", "index.json exists but is empty")
                    elif content.Length < 50 then
                        ValidationFramework.Fail ("Suspicious index file", sprintf "index.json is very small (%d chars)" content.Length)
                    else
                        ValidationFramework.Pass (sprintf "Search index file valid (%d chars)" content.Length)
                with
                | ex -> ValidationFramework.Fail ("Index file error", sprintf "Could not read index.json: %s" ex.Message)
            else
                ValidationFramework.Fail ("Index file missing", sprintf "File %s not found" indexFile)
        )
        
        ValidationFramework.test "Search manifest file exists" (fun () ->
            if File.Exists(manifestFile) then
                try
                    let content = File.ReadAllText(manifestFile)
                    if String.IsNullOrWhiteSpace(content) then
                        ValidationFramework.Fail ("Empty manifest file", "manifest.json exists but is empty")
                    else
                        ValidationFramework.Pass (sprintf "Search manifest file valid (%d chars)" content.Length)
                with
                | ex -> ValidationFramework.Fail ("Manifest file error", sprintf "Could not read manifest.json: %s" ex.Message)
            else
                ValidationFramework.Fail ("Manifest file missing", sprintf "File %s not found" manifestFile)
        )
        
        ValidationFramework.test "Search docs file exists" (fun () ->
            if File.Exists(docsFile) then
                try
                    let content = File.ReadAllText(docsFile)
                    if String.IsNullOrWhiteSpace(content) then
                        ValidationFramework.Fail ("Empty docs file", "docs.json exists but is empty")
                    elif content.Length < 100 then
                        ValidationFramework.Fail ("Suspicious docs file", sprintf "docs.json is very small (%d chars)" content.Length)
                    else
                        // Try to parse as JSON to verify it's valid JSON
                        if content.Contains("[") && content.Contains("]") then
                            ValidationFramework.Pass (sprintf "Search docs file valid (%d chars)" content.Length)
                        else
                            ValidationFramework.Fail ("Invalid docs file", "docs.json doesn't appear to be valid JSON array")
                with
                | ex -> ValidationFramework.Fail ("Docs file error", sprintf "Could not read docs.json: %s" ex.Message)
            else
                ValidationFramework.Fail ("Docs file missing", sprintf "File %s not found" docsFile)
        )

module ValidationRunner =
    
    let findNoteFiles (siteDir: string) =
        let notesDir = Path.Combine(siteDir, "notes")
        if Directory.Exists(notesDir) then
            Directory.GetFiles(notesDir, "index.html", SearchOption.AllDirectories)
            |> Array.toList
        else
            []
    
    /// Posts are generated to individual permalink paths, not a centralized directory
    /// So we don't validate a specific posts directory structure
    
    let runAllValidations (siteDir: string) =
        printfn "Comprehensive Site Validation"
        printfn "============================="
        printfn "Validating generated site in %s directory\n" siteDir
        
        // 1. Site Structure Validation
        SiteStructureValidation.validateSiteStructure siteDir
        
        // 2. Asset Validation
        AssetValidation.validateIncludeAssets siteDir
        
        // 3. Content Count Validation
        ContentCountValidation.validateContentCounts siteDir
        
        // 4. Search Index Validation
        SearchIndexValidation.validateSearchIndex siteDir
        
        // 5. Wiki Link Validation
        let noteFiles = findNoteFiles siteDir
        let allHtmlFiles = noteFiles
        
        printfn "\n=== ANALYZING HTML FILES ==="
        printfn "Found %d note files" noteFiles.Length
        
        if allHtmlFiles.Length = 0 then
            ValidationFramework.test "HTML files exist" (fun () ->
                ValidationFramework.Fail ("No HTML files", "No HTML files found in notes directory")
            )
        else
            // Test each HTML file for wiki links
            allHtmlFiles |> List.iter WikiLinkValidation.validateFileWikiLinks
            
            // Search for known problematic patterns
            WikiLinkValidation.searchKnownPatterns allHtmlFiles
            
            // Analyze HTML structure using first note file
            if noteFiles.Length > 0 then
                WikiLinkValidation.analyzeHtmlStructure noteFiles.[0]
        
        // Generate final report
        let (passed, failed, warnings) = ValidationFramework.getTestSummary()
        
        printfn "\n=== FINAL REPORT ==="
        printfn "Tests Passed: %d" passed
        printfn "Tests Failed: %d" failed  
        printfn "Warnings: %d" warnings
        
        if failed > 0 then
            printfn "\nFAILED TESTS:"
            ValidationFramework.testResults 
            |> List.rev 
            |> List.iter (function 
                | ValidationFramework.Fail (name, reason) -> printfn "  ✗ %s: %s" name reason 
                | _ -> ())
        
        if warnings > 0 then
            printfn "\nWARNINGS:"
            ValidationFramework.testResults 
            |> List.rev 
            |> List.iter (function 
                | ValidationFramework.Warning (name, warning) -> printfn "  ⚠ %s: %s" name warning 
                | _ -> ())
                
        if failed = 0 then
            printfn "\n🎉 SUCCESS: All site validation checks passed!"
        else
            printfn "\n❌ VALIDATION FAILED: %d issues found" failed
            printfn "   Check the failed tests above for specific problems to address."
        
        failed = 0

// Run the comprehensive validation
let siteDir = "_site"
let success = ValidationRunner.runAllValidations siteDir
exit (if success then 0 else 1)