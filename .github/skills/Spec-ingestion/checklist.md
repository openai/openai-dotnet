# Spec Ingestion Checklist

Use this checklist when performing a spec ingestion for any area.

## Pre-Ingestion

- [ ] Identify the target area(s) to ingest
- [ ] **Review reference PRs** in [references.md](references.md) for the area or similar areas — note visitor changes, deferred features, and custom C# patterns
- [ ] Pull latest from upstream `microsoft/openai-openapi-pr` (branch: `main`)
- [ ] Pull latest from `openai/openai-dotnet` (branch: `main`)
- [ ] Create a new branch: `{username}_{Area}SpecUpdate`

## Base Spec Update

- [ ] Sparse checkout upstream repo including **both** `{area}` and `common` folders
- [ ] Copy latest base spec from upstream `packages/openai-typespec/src/{area}/` to `specification/base/typespec/{area}/` — **exact copy, no modifications**
- [ ] **Keep the clone** until after first successful compile — you may need common types
- [ ] If compile shows missing common types, find the type definition in the upstream clone's `src/common/` and copy **only that type definition** into the local `specification/base/typespec/common/` file — do NOT copy the entire file or folder
- [ ] Delete temporary clone after compile succeeds

## Client TSP Update

- [ ] Extract operation names from new `operations.tsp` (`Select-String -Pattern "^op "` )
- [ ] Fix errors in `specification/client/{area}.client.tsp`
- [ ] Add `@@clientLocation` for **all** operations (no more `interface` blocks)
- [ ] Update `@@clientName` for any renamed operations
- [ ] Update `@@visibility`, `@@alternateType`, `@@usage` as needed
- [ ] Update client models TSP (`specification/client/models/{area}.models.tsp`) if applicable

## Compile and Generate Code

- [ ] Run `./scripts/Invoke-CodeGen.ps1` (no params) — this handles `npm ci`, build, compile, and code generation in one step
- [ ] Expect ~200 pre-existing warnings; only **errors** matter
- [ ] If `prohibited-namespace` errors appear, add `[CodeGenType]` stubs — internal types go in `Internal/GeneratorStubs.cs`, public types go in `GeneratorStubs.cs` (see patterns-and-gotchas.md §5)
- [ ] If client TSP fixes are needed, fix and re-run `./scripts/Invoke-CodeGen.ps1`
- [ ] Report any remaining base spec compile errors — **do NOT modify base spec directly**

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

## Post-Generation Verification

- [ ] Verify generated files: `Get-ChildItem src/Generated/Models/{Area}/ -Name`
- [ ] **Review numeric types** — check if any `long` or `double` properties, parameters, or fields were incorrectly converted to `int`/`float` by the `NumericTypesVisitor`; add exclusions if needed (see patterns-and-gotchas.md §3)
- [ ] Verify build: `dotnet build`
- [ ] Export API surface: `./scripts/Export-Api.ps1`

## Post-Generation Review

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
