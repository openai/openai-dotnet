---
name: spec-ingestion
description: Guide for ingesting the latest OpenAI TypeSpec specification into the openai-dotnet SDK. Use this when asked to update or ingest OpenAI API specs, copy base TypeSpec files from upstream, fix client TSP compile errors, or run code generation for new API areas.
---

# Spec Ingestion

## Overview

This skill describes how to ingest the latest OpenAI TypeSpec specification (from the upstream [`microsoft/openai-openapi-pr`](https://github.com/microsoft/openai-openapi-pr) repository) into the `openai-dotnet` SDK, area by area.

The process involves:
1. Copying updated base specs from upstream (exact copy, no modifications)
2. Reporting any compile errors in the base TSP (do NOT fix — base spec must stay unmodified)
3. Fixing compile errors in the client TSP layer
4. Preserving custom C# code (renames, stubs)
5. Running code generation
6. Verifying the output

## Skill Documents

This skill is split across multiple files for easier navigation:

| Document | Description |
|----------|-------------|
| [steps.md](steps.md) | **Step-by-step process** — the full 9-step workflow from copying spec to post-generation verification |
| [file-locations.md](file-locations.md) | **Key file locations** — quick reference for all upstream and local paths, area mappings |
| [patterns-and-gotchas.md](patterns-and-gotchas.md) | **Common patterns & gotchas** — lessons learned, pitfalls, and conventions to follow |
| [checklist.md](checklist.md) | **Checklist** — a task-by-task checklist for tracking progress during an ingestion |
| [references.md](references.md) | **Reference PRs** — detailed notes on past ingestion PRs with lessons learned |

## Quick Start

1. Review [references.md](references.md) for examples of past ingestions in your area
2. Read [file-locations.md](file-locations.md) to understand the repo layout
3. Follow [steps.md](steps.md) for the full ingestion workflow
4. Use [checklist.md](checklist.md) to track your progress
5. Consult [patterns-and-gotchas.md](patterns-and-gotchas.md) when you hit issues

## Available Areas

Areas that can be ingested independently:

`administration` · `assistants` · `audio` · `batch` · `chat` · `containers` · `conversations` · `embeddings` · `evals` · `files` · `fine-tuning` · `graders` · `images` · `models` · `moderations` · `realtime` · `responses` · `runs` · `threads` · `vector-stores` · `videos`

## Key Rules

1. **Always add `@@clientLocation`** for every operation in the client TSP (the latest spec no longer uses `interface` blocks)
2. **NEVER modify the base spec** — it must be an exact copy of upstream. Handle all issues (type unions, suppressions, etc.) in `specification/client/` instead
3. **Update `[CodeGenType]` stubs** in `src/Custom/{Area}/Internal/GeneratorStubs.cs` for any renamed types
4. **Defer complex features** — suggest them as follow-up items rather than implementing in the same ingestion
5. **Run `./scripts/Invoke-CodeGen.ps1`** to generate code, then `dotnet build` to verify
6. **Work locally only** — do NOT create PRs or file issues. Instead, suggest a list of issues that may need to be filed upstream
