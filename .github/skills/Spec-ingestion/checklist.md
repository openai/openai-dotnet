# Spec Ingestion Checklist

Use this checklist when performing a spec ingestion for any area.

## Pre-Ingestion

- [ ] Identify the target area(s) to ingest
- [ ] Pull latest from upstream `microsoft/openai-openapi-pr` (branch: `main`)
- [ ] Pull latest from `openai/openai-dotnet` (branch: `main`)
- [ ] Create a new branch: `{username}_{Area}SpecUpdate`

## Base Spec Update

- [ ] Copy latest base spec from upstream `packages/openai-typespec/src/{area}/` to `specification/base/typespec/{area}/` — **exact copy, no modifications**
- [ ] Copy any new/updated common types from upstream `src/common/` to `specification/base/typespec/common/` if needed — **exact copy, no modifications**
- [ ] Compile and list any errors in the base TSP — **do NOT fix the base spec directly; report issues instead**

## Client TSP Update

- [ ] Fix errors in `specification/client/{area}.client.tsp`
- [ ] Add `@@clientLocation` for **all** operations (no more `interface` blocks)
- [ ] Update `@@clientName` for any renamed operations
- [ ] Update `@@visibility`, `@@alternateType`, `@@usage` as needed
- [ ] Update client models TSP (`specification/client/models/{area}.models.tsp`) if applicable

## Custom C# Code Update

- [ ] Compare old vs. new spec for **renames** → update `[CodeGenType]` stubs in `src/Custom/{Area}/Internal/GeneratorStubs.cs`
- [ ] Update any custom code in `src/Custom/{Area}/` referencing renamed types
- [ ] Add `[CodeGenType]` stubs for new internal types
- [ ] Remove stubs for deleted types

## Documentation

- [ ] List all **new** types, properties, and operations
- [ ] List all **renamed** types/properties (old → new mapping)
- [ ] List all **removed** types, properties, and operations
- [ ] Note any **type unions** that need discriminator treatment (don't modify base spec)

## Code Generation

- [ ] Run code generation: `./scripts/Invoke-CodeGen.ps1`
- [ ] Verify build: `dotnet build`
- [ ] Verify tests: `dotnet test`
- [ ] Export API surface: `./scripts/Export-Api.ps1`

## Post-Generation Verification

- [ ] Diff generated code (`src/Generated/`) — list new, removed, and changed files
- [ ] Diff API surface (`api/`) — identify breaking changes
- [ ] List compile issues in generated code
- [ ] List items needing discriminator patterns
- [ ] Identify features needing follow-up work

## Final Summary (Local Work Only)

> Do NOT create PRs or file issues. Present a summary for the user to act on.

- [ ] Summarize all changes made locally
- [ ] List suggested upstream issues (spec bugs, missing types, etc.)
- [ ] List suggested follow-up items (deferred features, complex implementations)
