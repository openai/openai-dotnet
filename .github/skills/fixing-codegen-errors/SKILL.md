---
name: fixing-codegen-errors
description: Guide for fixing TypeSpec codegen errors in the openai-dotnet repository. Use this when codegen (`Invoke-CodeGen.ps1`) fails with prohibited-namespace errors, missing types, client TSP decorator errors, npm/plugin build failures, or post-generation dotnet build errors.
---

# Fixing Codegen Errors

## Overview

After running `scripts/Invoke-CodeGen.ps1`, various categories of errors can occur. This skill
covers diagnosing and fixing each category.

> **Ground rule:** Never modify the base spec directory — these are upstream copies. All fixes go
> in the client customization layer or custom C# code directories.
>
> See [file-locations.md](../ingesting-spec/file-locations.md) for the canonical list of all paths.

## Error Categories

| Category | Symptom | Fix Document |
|----------|---------|--------------|
| 1 — Prohibited namespace | `prohibited-namespace` error naming a type | [prohibited-namespace.md](prohibited-namespace.md) |
| 2 — Missing type | `type-not-found` or unresolved reference | [missing-type.md](missing-type.md) |
| 3 — Client TSP decorators | Errors on `@@clientLocation`, `@@clientName`, etc. | [client-tsp-decorators.md](client-tsp-decorators.md) |
| 4 — npm/plugin build | Errors during `npm ci` or `npm run build` | [npm-plugin-build.md](npm-plugin-build.md) |
| 5 — Post-generation build | `dotnet build OpenAI.slnx` fails after codegen succeeds | [post-generation-build.md](post-generation-build.md) |

## Triage

Identify the failing phase from the codegen output:

- `npm ci` or `npm run build` failure → **Category 4**
- `tsp compile` error referencing a namespace → **Category 1**
- `tsp compile` error for a missing type → **Category 2**
- `tsp compile` error on a decorator → **Category 3**
- Codegen succeeds but `dotnet build` fails → **Category 5**
