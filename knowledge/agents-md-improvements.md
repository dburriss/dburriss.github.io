# Agents.md Improvements (Guidance for Agents)

Audience: Agents working in this codebase

Goals
- Make Agents.md a single, reliable quick-start for common tasks: formatting, building, testing, rendering, serving, and spec-driven changes.

Suggested additions
1) Link to OpenSpec Ops Guide prominently
- Add a one-line quick link at the top:
  - See openspec/AGENTS.md for proposal/spec workflow, naming, validation, and archiving.

2) Testing: create, run, and troubleshoot
- Add commands:
  - Run tests: `dotnet test ./src/SiteRenderer.Tests/SiteRenderer.Tests.fsproj`
  - Run solution/test filters: `dotnet test -v minimal` (or use `--filter FullyQualifiedName~WikiLink`)
  - Coverage (already included via coverlet.collector): `dotnet test /p:CollectCoverage=true`
- Mention .NET 10 test runner notes:
  - "dotnet test" supports VSTest and Microsoft.Testing.Platform runners. See Microsoft Learn article for details.
- Encourage adding tests under src/SiteRenderer.Tests/ with xUnit.

3) Formatting and build order
- Clarify standard workflow:
  - Format: `dotnet fantomas ./src/SiteRenderer/`
  - Build: `dotnet build ./src/SiteRenderer/SiteRenderer.fsproj`
  - Render: `./render.sh`
  - Serve: `dotnet serve -o -d ./_site/`

4) Local run convenience
- Reinforce `./run.sh --serve` as the simplest path.

5) Links and extensions overview (quick pointers)
- The markdown pipeline uses Markdig with a custom WikiLink extension.
- For HTML views, use Giraffe.ViewEngine.
- See knowledge/links-implementation.md and knowledge/extension-wikilink.md.

6) Common troubleshooting
- If render succeeds but links show unresolved markers, check notes and titles; see link warnings in render logs.
- If tests don’t run, ensure .NET SDK is installed and restore succeeds.

7) CI cautionary notes
- Keep changes small, validate locally before PR.
- Use Fantomas prior to building to avoid formatting lint failures.

Useful docs (fetched)
- Giraffe.ViewEngine documentation: https://giraffe.wiki/view-engine
- dotnet serve usage: https://github.com/natemcmaster/dotnet-serve
- Fantomas Getting Started: https://fsprojects.github.io/fantomas/docs/end-users/GettingStarted.html
- Testing with dotnet test (.NET 10 runner modes): https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test

Proposed placement in Agents.md
- After the General and Stack sections, add a "Testing" section followed by a "Workflow" section that shows the typical sequence: format → build → test → render → serve.
