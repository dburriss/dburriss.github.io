namespace SiteRenderer

open System
open System.IO

module Program =

    let private printHelp () =
        printfn "SiteRenderer - Static site generator using F# and Giraffe.ViewEngine"
        printfn ""
        printfn "Usage: SiteRenderer [options]"
        printfn ""
        printfn "Options:"

        printfn
            "  --source <path>    Source directory containing _posts, _notes, _config.yml, etc. (default: current directory)"

        printfn "  --output <path>    Output directory for generated site (default: _site)"
        printfn "  --config <path>    Path to _config.yml (default: <source>/_config.yml)"
        printfn "  --posts-per-page   Number of posts per index page (default: 10)"
        printfn "  --help             Show this help message"
        printfn ""
        printfn "Example:"
        printfn "  SiteRenderer --source /path/to/blog --output /path/to/output"

    type CliOptions =
        { SourceDir: string
          OutputDir: string
          ConfigPath: string option
          PostsPerPage: int }

    let private defaultOptions =
        { SourceDir = Environment.CurrentDirectory
          OutputDir = "_site"
          ConfigPath = None
          PostsPerPage = 10 }

    let rec private parseArgs (args: string list) (options: CliOptions) =
        match args with
        | [] -> Some options
        | "--help" :: _ -> None
        | "--source" :: path :: rest -> parseArgs rest { options with SourceDir = path }
        | "--output" :: path :: rest -> parseArgs rest { options with OutputDir = path }
        | "--config" :: path :: rest -> parseArgs rest { options with ConfigPath = Some path }
        | "--posts-per-page" :: num :: rest ->
            match Int32.TryParse(num) with
            | true, n -> parseArgs rest { options with PostsPerPage = n }
            | _ ->
                printfn "Invalid number for --posts-per-page: %s" num
                None
        | unknown :: _ ->
            printfn "Unknown option: %s" unknown
            None

    /// Cleans the output directory while preserving .git (submodule pointer)
    let private cleanOutputDirectory (outputDir: string) =
        if Directory.Exists(outputDir) then
            // Delete all files except .git
            for file in Directory.GetFiles(outputDir) do
                let fileName = Path.GetFileName(file)

                if fileName <> ".git" then
                    File.Delete(file)

            // Delete all subdirectories except .git (which is a file for submodules, but check anyway)
            for dir in Directory.GetDirectories(outputDir) do
                let dirName = Path.GetFileName(dir)

                if dirName <> ".git" then
                    Directory.Delete(dir, true)
        else
            Directory.CreateDirectory(outputDir) |> ignore

    let private run (options: CliOptions) =
        let sourceDir = Path.GetFullPath(options.SourceDir)

        let outputDir =
            if Path.IsPathRooted(options.OutputDir) then
                options.OutputDir
            else
                Path.Combine(sourceDir, options.OutputDir)

        let configPath =
            options.ConfigPath
            |> Option.defaultValue (Path.Combine(sourceDir, "_config.yml"))

        printfn "Source: %s" sourceDir
        printfn "Output: %s" outputDir
        printfn "Config: %s" configPath

        if not (File.Exists(configPath)) then
            printfn "Error: Config file not found: %s" configPath
            1
        else
            try
                let config = Parsing.parseSiteConfig configPath

                let postsDir = Path.Combine(sourceDir, "_posts")
                let posts = Renderer.loadPosts postsDir
                printfn "Loaded %d posts" posts.Length

                let pages = Renderer.loadPages sourceDir
                printfn "Loaded %d pages" pages.Length

                let notesDir = Path.Combine(sourceDir, "_notes")
                let notes = Renderer.loadNotes notesDir
                printfn "Loaded %d notes" notes.Length

                let initialIndex = Renderer.buildSiteIndex posts pages notes
                let index, linkWarnings = Renderer.resolveWikiLinks initialIndex

                // Print link resolution warnings
                if linkWarnings.Length > 0 then
                    printfn ""
                    printfn "Link resolution warnings:"

                    for warning in linkWarnings do
                        printfn "  - %s" warning

                    printfn ""

                // Detect and report orphaned notes
                let orphanedNotes = Renderer.detectOrphanedNotes index

                if orphanedNotes.Length > 0 then
                    printfn ""
                    printfn "Orphaned notes detected:"

                    for orphan in orphanedNotes do
                        printfn "  - %s" orphan

                    printfn ""

                let ctx =
                    { Config = config
                      Index = index
                      OutputRoot = outputDir }

                let defaultSocialImg = Some "img/explore-590.jpg"
                let rendered = Renderer.renderSite ctx options.PostsPerPage defaultSocialImg
                printfn "Rendered %d pages" rendered.Length

                // Clean output directory (preserves .git for submodule)
                cleanOutputDirectory outputDir

                Renderer.writeOutput outputDir rendered
                printfn "Wrote output to %s" outputDir

                // Generate search docs
                let searchDir = Path.Combine(outputDir, "search")

                if not (Directory.Exists(searchDir)) then
                    Directory.CreateDirectory(searchDir) |> ignore

                let searchDocsPath = Path.Combine(searchDir, "docs.json")
                Search.generateDocs config.BaseUrl posts (index.Notes) searchDocsPath
                printfn "Generated search docs at %s" searchDocsPath

                Renderer.copyStaticAssets sourceDir outputDir config.Include
                printfn "Copied static assets"

                printfn "Done!"
                0
            with ex ->
                printfn "Error: %s" ex.Message
                printfn "%s" ex.StackTrace
                1

    [<EntryPoint>]
    let main argv =
        let args = argv |> Array.toList

        match parseArgs args defaultOptions with
        | None ->
            printHelp ()
            0
        | Some options -> run options
