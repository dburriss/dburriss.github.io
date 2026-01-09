#!/usr/bin/env -S dotnet fsi

(*
clean-site.fsx - Core cleaning logic for the blog site

DESCRIPTION:
  Removes all generated content from _site directory while preserving the
  .git submodule, and runs dotnet clean on the solution.
  
USAGE:
  Called by clean.sh/clean.ps1 wrapper scripts:
  dotnet fsi scripts/clean-site.fsx [true|false]
  
ARGUMENTS:
  First argument: "true" for dry-run mode, "false" for actual cleaning
*)

open System
open System.IO
open System.Diagnostics

// Parse arguments
let isDryRun =
    match fsi.CommandLineArgs with
    | [| _; "true" |] -> true
    | [| _; "false" |] -> false
    | _ ->
        printfn "Error: Invalid arguments. Expected 'true' or 'false' for dry-run mode."
        exit 1

// Get script and project directories
let scriptDir = __SOURCE_DIRECTORY__
let projectRoot = Path.GetFullPath(Path.Combine(scriptDir, ".."))
let siteDir = Path.Combine(projectRoot, "_site")
let solution = Path.Combine(projectRoot, "blog.slnx")

// Safety check - ensure we're in the correct directory
if not (File.Exists(solution)) then
    printfn "Error: Cannot find blog.slnx. Are you in the correct directory?"
    exit 1

printfn "Cleaning generated content and build artifacts..."
printfn ""

// Clean _site directory
if Directory.Exists(siteDir) then
    printfn "Cleaning _site directory..."

    // Get all entries in _site
    let allEntries =
        let dirs = Directory.GetDirectories(siteDir)
        let files = Directory.GetFiles(siteDir)
        Array.append dirs files

    // Filter out .git (whether file or directory)
    let entriesToDelete =
        allEntries
        |> Array.filter (fun path ->
            let name = Path.GetFileName(path)
            name <> ".git")

    // Count items for reporting
    let countItemsRecursive (path: string) =
        if File.Exists(path) then
            (0, 1) // 0 dirs, 1 file
        elif Directory.Exists(path) then
            let dirCount =
                Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length

            let fileCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length
            (dirCount + 1, fileCount) // +1 for the directory itself
        else
            (0, 0)

    let totalCounts =
        entriesToDelete
        |> Array.map countItemsRecursive
        |> Array.fold (fun (d1, f1) (d2, f2) -> (d1 + d2, f1 + f2)) (0, 0)

    let (dirCount, fileCount) = totalCounts

    if isDryRun then
        printfn "  [DRY-RUN] Would clean the following:"
        printfn "    Directories: %d" dirCount
        printfn "    Files: %d" fileCount
        printfn "    Items to remove:"

        entriesToDelete
        |> Array.map Path.GetFileName
        |> Array.sort
        |> Array.iter (fun name -> printfn "      - %s" name)

        printfn "    Total items to remove: %d" (dirCount + fileCount)

        // Check if .git exists
        let gitPath = Path.Combine(siteDir, ".git")

        if File.Exists(gitPath) || Directory.Exists(gitPath) then
            printfn "    Preserving: .git submodule"
    else
        // Delete each entry
        entriesToDelete
        |> Array.iter (fun path ->
            if Directory.Exists(path) then
                Directory.Delete(path, true)
            elif File.Exists(path) then
                File.Delete(path))

        printfn "  Removed %d directories and %d files" dirCount fileCount

        // Check if .git was preserved
        let gitPath = Path.Combine(siteDir, ".git")

        if File.Exists(gitPath) || Directory.Exists(gitPath) then
            printfn "  Preserved .git submodule"

    printfn ""
else
    printfn "  _site directory does not exist, nothing to clean"
    printfn ""

// Run dotnet clean
printfn "Running dotnet clean on solution..."

if isDryRun then
    printfn "  [DRY-RUN] Would run: dotnet clean \"%s\"" solution
else
    let psi = ProcessStartInfo("dotnet", sprintf "clean \"%s\"" solution)
    psi.UseShellExecute <- false
    psi.WorkingDirectory <- projectRoot

    let proc = Process.Start(psi)
    proc.WaitForExit()

    if proc.ExitCode <> 0 then
        printfn "Error: dotnet clean failed with exit code %d" proc.ExitCode
        exit 1

printfn ""

if isDryRun then
    printfn "Dry run completed. No changes were made."
else
    printfn "Clean completed successfully!"
