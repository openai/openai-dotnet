---
name: fixing-codegen-errors
description: Guide for diagnosing and fixing errors from ./scripts/Invoke-CodeGen.ps1. Use this when code generation fails, produces prohibited-namespace errors, missing type errors, client TSP compile errors, or when a generator version update (Update TypeSpec Generator Version workflow) breaks the build.
---

# Fixing Code Generation Errors

## Overview

This skill describes how to diagnose and fix errors produced by `./scripts/Invoke-CodeGen.ps1`. These errors commonly arise during:
- Spec ingestion (new base spec changes)
- Generator version updates (the `Update TypeSpec Generator Version` workflow)
- Client TSP modifications

The script runs: `npm ci` → `npm run build` (codegen plugin) → `npx tsp compile .` (TypeSpec compilation + C# generation).

## Error Categories

### 1. `prohibited-namespace` Errors

**Symptom:** Compiler error naming a type with `prohibited-namespace`.

**Cause:** A TypeSpec type exists under `OpenAI` namespace but has no `[CodeGenType]` stub to place it in the correct area namespace (e.g., `OpenAI.Audio`).

**Fix:**
1. Identify the type name from the error message
2. Determine if it's internal or public (internal types are prefixed with `Internal`)
3. Add a stub in the correct file:
   - Internal → `src/Custom/{Area}/Internal/GeneratorStubs.cs`
   - Public → `src/Custom/{Area}/GeneratorStubs.cs`

```csharp
// Internal example
[CodeGenType("SomeNewInternalType")] internal partial class InternalSomeNewInternalType { }

// Public example
[CodeGenType("SomeNewPublicType")] public partial class SomeNewPublicType { }
```

4. Re-run `./scripts/Invoke-CodeGen.ps1`

### 2. Missing Type / Unresolved Reference Errors

**Symptom:** `error type-not-found: Type "Foo" is not defined` or similar.

**Cause:** The base spec references a type from `common/` that hasn't been copied locally yet.

**Fix:**
1. Search the upstream repo for the type definition:
   ```powershell
   Select-String -Path "specification/base/typespec/common/*.tsp" -Pattern "model Foo|union Foo|enum Foo|alias Foo|scalar Foo"
   ```
2. If not found locally, get it from upstream `microsoft/openai-openapi-pr` → `packages/openai-typespec/src/common/`
3. Copy **only the specific type definition** into the local common file — do NOT copy entire files
4. Re-run `./scripts/Invoke-CodeGen.ps1`

### 3. Client TSP Decorator Errors

**Symptom:** Errors referencing `@@clientLocation`, `@@clientName`, `@@visibility`, `@@alternateType`, or `@@usage` decorators.

**Cause:** A type or operation was renamed/removed in the new base spec, but the client TSP still references the old name.

**Fix:**
1. Open the client TSP file: `specification/client/{area}.client.tsp`
2. Find the stale reference named in the error
3. Update it to match the new name from the base spec
4. If an operation was removed, remove its `@@clientLocation` line
5. Re-run `./scripts/Invoke-CodeGen.ps1`

### 4. npm / Build Errors (Plugin Compilation)

**Symptom:** Errors during `npm ci` or `npm run build` in the codegen plugin.

**Cause:** Version incompatibilities after a generator update, or breaking API changes in the TypeSpec SDK.

**Fix:**
1. Check `codegen/package.json` for version mismatches
2. Delete `node_modules` and `package-lock.json`, then re-run:
   ```powershell
   npm install
   npm run build -w codegen
   ```
3. If the codegen plugin (`codegen/generator/src/`) has TypeScript compile errors, fix the TypeScript visitor code
4. Re-run `./scripts/Invoke-CodeGen.ps1`

### 5. Post-Generation Build Errors (`dotnet build`)

**Symptom:** `./scripts/Invoke-CodeGen.ps1` succeeds but `dotnet build src/OpenAI.csproj` fails.

**Cause:** Generated C# code references renamed/removed custom types, or numeric type conversions need updating.

**Fix:**
1. Check for renamed types → update `[CodeGenType]` attributes in `GeneratorStubs.cs`
2. Check for numeric type issues → update exclusion lists in `codegen/generator/src/Visitors/NumericTypesVisitor.cs`
3. Check custom code in `src/Custom/{Area}/` for broken references
4. Rebuild: `dotnet build src/OpenAI.csproj`

## Triage Steps

When `./scripts/Invoke-CodeGen.ps1` fails:

1. **Read the error message carefully** — it identifies the phase (npm, build, compile, or generation)
2. **Identify the phase:**
   - `npm ci` failure → dependency issue (Category 4)
   - `npm run build` failure → plugin compilation (Category 4)
   - `tsp compile` failure → TypeSpec errors (Categories 1-3)
3. **For TypeSpec errors**, check whether the error references:
   - A namespace → Category 1 (prohibited-namespace)
   - A missing type → Category 2 (unresolved reference)
   - A decorator → Category 3 (client TSP)
4. **Fix the specific error**, then re-run `./scripts/Invoke-CodeGen.ps1`
5. **After codegen succeeds**, run `dotnet build src/OpenAI.csproj` to verify
6. **After build succeeds**, run `./scripts/Export-Api.ps1` to update API surface files

## Agent Rules

- **NEVER modify files in `specification/base/`** — the base spec must remain an exact upstream copy
- **Only fix errors**, not warnings — warnings from `tsp compile` are expected and acceptable
- **Re-run the full `./scripts/Invoke-CodeGen.ps1`** after each fix to verify
- **Always run `dotnet build src/OpenAI.csproj`** after successful code generation
- **Always run `./scripts/Export-Api.ps1`** after a successful build

## Key File Locations

| File | Purpose |
|------|---------|
| `scripts/Invoke-CodeGen.ps1` | Main code generation script |
| `scripts/Submit-GeneratorUpdatePr.ps1` | Script used by the Update TypeSpec Generator Version workflow |
| `codegen/package.json` | Generator dependencies and version |
| `specification/client/{area}.client.tsp` | Client-side TypeSpec decorators per area |
| `specification/client/models/{area}.models.tsp` | Client-side model overrides (not all areas) |
| `specification/base/typespec/{area}/` | Base spec (DO NOT MODIFY) |
| `src/Custom/{Area}/Internal/GeneratorStubs.cs` | Internal type stubs with `[CodeGenType]` |
| `src/Custom/{Area}/GeneratorStubs.cs` | Public type stubs with `[CodeGenType]` |
| `codegen/generator/src/Visitors/` | C# codegen visitors (e.g., NumericTypesVisitor) |
| `.github/workflows/update-generator.yml` | The Update TypeSpec Generator Version workflow |
