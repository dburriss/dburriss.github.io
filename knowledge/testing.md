# Creating and Running Tests

Audience: Agents working in this codebase

Test project
- Location: src/SiteRenderer.Tests/
- Framework: .NET 10
- Test framework: xUnit
- Key file: SiteRenderer.Tests.fsproj with references:
  - Microsoft.NET.Test.Sdk
  - xunit, xunit.runner.visualstudio
  - coverlet.collector (coverage)
  - ProjectReference to src/SiteRenderer/SiteRenderer.fsproj

Structure
- Tests.fs contains multiple modules:
  - WikiLinkTests: integration-level tests via Markdig pipeline
  - ParsingTests: unit tests for extractWikiLinks and ignore patterns
  - WikiLinkInlineTests: unit tests for inline type behavior
  - RendererTests: unit tests for Html rendering outcomes

Common commands
- Restore and build:
  - dotnet build ./src/SiteRenderer/SiteRenderer.fsproj
- Run all tests:
  - dotnet test ./src/SiteRenderer.Tests/SiteRenderer.Tests.fsproj
- Filter tests:
  - dotnet test --filter FullyQualifiedName~WikiLink
- Coverage (if desired):
  - dotnet test /p:CollectCoverage=true
- Verbosity:
  - dotnet test -v minimal

Notes on .NET 10 test runners
- dotnet test supports two modes (VSTest and Microsoft.Testing.Platform). Most typical usage in this repo will work with default settings.
- Reference: Microsoft Learn - Testing with dotnet test: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test

Authoring guidance
- Prefer small, focused tests; validate both happy path and edge cases.
- Keep tests independent; avoid relying on file system state unless necessary.
- For renderer behavior, assert on generated HTML fragments and warnings.

Local workflow reminder
- Format code before running tests: `dotnet fantomas ./src/SiteRenderer/`
- Build, test, then render site: `./render.sh`
- Serve locally for manual verification: `dotnet serve -o -d ./_site/` or `./run.sh --serve`

Useful docs (fetched)
- Fantomas Getting Started: https://fsprojects.github.io/fantomas/docs/end-users/GettingStarted.html
- dotnet serve usage: https://github.com/natemcmaster/dotnet-serve
