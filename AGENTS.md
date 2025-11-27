<!-- OPENSPEC:START -->
# OpenSpec Instructions

These instructions are for AI assistants working in this project.

Always open `@/openspec/AGENTS.md` when the request:
- Mentions planning or proposals (words like proposal, spec, change, plan)
- Introduces new capabilities, breaking changes, architecture shifts, or big performance/security work
- Sounds ambiguous and you need the authoritative spec before coding

Use `@/openspec/AGENTS.md` to learn:
- How to create and apply change proposals
- Spec format and conventions
- Project structure and guidelines

Keep this managed block so 'openspec update' can refresh the instructions.

<!-- OPENSPEC:END -->

## General

- Prefer simplicity
- Work in small steps
- After making code changes, build and test to verify

## Stack

- F# 10
- .NET 10
- [Giraffe.ViewEngine](https://giraffe.wiki/view-engine)

## Commands

Build the renderer project with:
```bash

dotnet build ./src/SiteRenderer/SiteRenderer.fsproj
```

Format with Fantomas after code changes (before build): 

```bash
dotnet fantomas ./src/SiteRenderer/
```

