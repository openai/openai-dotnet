# Spec Ingestion — Step-by-Step Process

## Step 1: Review Reference PRs

Before starting, review the **Reference PRs** in [references.md](references.md) from **previous area ingestions**. Since you're ingesting a new area, there won't be an existing PR for it — instead, study PRs from other areas to learn the patterns and common pitfalls. Pay particular attention to:

- What types of changes were made to the codegen visitors (e.g., `NumericPropertiesVisitor`)
- What features were deferred to follow-up PRs
- What custom C# code changes were needed

Review the reference PRs to understand current patterns before proceeding.

---

## Step 2: Copy the Latest Base Spec

1. Clone the upstream repo to a temporary directory using a sparse checkout — include **both** the area folder **and** `common` (you may need common types later):
   ```powershell
   $tempDir = Join-Path $env:TEMP "openai-openapi-pr"
   git clone --depth 1 --filter=blob:none --sparse https://github.com/microsoft/openai-openapi-pr.git $tempDir
   Push-Location $tempDir
   git sparse-checkout set packages/openai-typespec/src/{area} packages/openai-typespec/src/common
   Pop-Location
   ```
2. Copy the **entire contents** of the area from the clone into the local spec:
   ```
   FROM: $tempDir/packages/openai-typespec/src/{area}/*
   TO:   specification/base/typespec/{area}/
   ```
3. Replace all existing files in the local area directory with the upstream files.
4. **Do NOT delete the temporary clone yet** — keep it until after the first successful compile (Step 4). If compile errors reference a type that doesn't exist locally, find its definition in the upstream `common/models.tsp` folder and copy **only that type definition** into the corresponding local common file — do NOT copy the entire file or folder:
   ```powershell
   # Example: compile error says type "Foo" is not found
   # 1. Find the type definition in the upstream common files:
   Select-String -Path "$tempDir/packages/openai-typespec/src/common/*.tsp" -Pattern "model Foo|union Foo|enum Foo|alias Foo|scalar Foo"
   # 2. Open that upstream file, copy ONLY the "Foo" type definition block
   # 3. Paste it into the local file: specification/base/typespec/common/models.tsp (or whichever local common file is appropriate)
   ```
   > **Do NOT** copy the entire upstream common `.tsp` file — only extract and add the specific missing type definition.
5. **Delete the temporary clone** after compile succeeds and no more upstream files are needed:
   ```powershell
   Remove-Item -Recurse -Force $tempDir
   ```

**Example for audio:**
```
Copy FROM: microsoft/openai-openapi-pr/packages/openai-typespec/src/audio/*
Copy TO:   specification/base/typespec/audio/
```

Each area typically contains:
- `main.tsp` — imports for the area
- `models.tsp` — type/model definitions
- `operations.tsp` — API operation definitions

---

## Step 3: Fix Client TSP Errors and Add `@@clientLocation`

**Before compiling**, update the client TSP files to fix broken references caused by the new base spec. This avoids unnecessary compile errors from stale client TSP references.

The client TSP files (`specification/client/{area}.client.tsp`) contain:
- `@@clientName` — renames operations/types for C# conventions
- `@@clientLocation` — assigns operations to specific client classes (replaces the `interface` pattern)
- `@@visibility` — controls property visibility
- `@@alternateType` — overrides property types
- `@@usage` — controls model usage pattern
- `@@dynamicModel` — marks models for dynamic serialization (JsonPatch)

### 3a. Why `@@clientLocation` is Required

The **latest upstream spec does NOT use `interface` blocks** for grouping operations into clients. The previous spec used `interface Audio { ... }` style grouping which automatically placed operations into client classes. Now operations are standalone (decorated with `@route`, `@post`, etc.) and you **must** explicitly add `@@clientLocation` for each operation to assign it to the correct client class.

**Pattern:**
```typespec
@@clientLocation(operationName, "ClientGroupName");
```

**Example (audio):**
```typespec
@@clientLocation(createSpeech, "Audio");
@@clientLocation(createTranscription, "Audio");
@@clientLocation(createTranslation, "Audio");
```

**Example (files):**
```typespec
@@clientLocation(createFile, "Files");
@@clientLocation(listFiles, "Files");
@@clientLocation(retrieveFile, "Files");
@@clientLocation(downloadFile, "Files");
@@clientLocation(deleteFile, "Files");
```

### 3b. Extract Operation Names from the New Spec

To find all operations that need `@@clientLocation`, scan the new `operations.tsp` for standalone `op` declarations:

```powershell
Select-String -Path "specification/base/typespec/{area}/operations.tsp" -Pattern "^op " | ForEach-Object { ($_ -split "\s+")[1] }
```

This gives you the exact list of operation names to add `@@clientLocation` entries for.

### 3c. Fix Broken References

If a type or operation was renamed upstream, update the `@@clientName`, `@@visibility`, and other decorators in the client TSP to reference the new names.

### 3d. Client Models TSP

If the area has a client models file (`specification/client/models/{area}.models.tsp`), update it as well. These files contain:
- Discriminated union wrappers (using `@discriminator`)
- Extended models for .NET-specific patterns
- Custom model shapes needed by the C# generator

**Important:** When the upstream spec uses **type unions** (e.g., `model | string`), we use **discriminators** instead to avoid binary data types in the generated code. See Step 8 for details.

---

## Step 4: Compile and Verify — Report Issues (Do NOT Modify Base Spec)

After fixing the client TSP, run `Invoke-CodeGen.ps1` to compile and generate code in one step:

```powershell
./scripts/Invoke-CodeGen.ps1
```

> **Do NOT run `npx tsp compile .` separately.** The `Invoke-CodeGen.ps1` script handles everything: `npm ci` → build codegen plugin → `npx tsp compile .` (which both compiles TypeSpec and runs code generation). If there are compile errors, the script will fail and show them — fix the errors and re-run the script.

> **CRITICAL:** The base spec at `specification/base/typespec/` must remain an **exact copy** of the upstream spec. Do NOT modify it to fix compile errors. If there are issues, **report them** so they can be addressed upstream or in the client TSP layer.

### Expect ~200 Pre-existing Warnings

The compile will produce **~200 warnings from other areas** (e.g., `union-enums-invalid-kind`, `multiple-response-types`). These are pre-existing and **not related to your ingestion**. Only **errors** matter.

A successful run shows `Found 0 errors` and `Found N warnings` where N is roughly the same as before your changes.

### What to look for and report:

- **Missing types** — A type referenced in the new spec doesn't exist locally. Check if it exists in `common/` or another area upstream and needs to be copied over (as an exact copy).

For each issue found, document it and determine if it can be resolved by:
1. Copying additional files from upstream (e.g., updated `common/` types) — always as exact copies
2. Handling it in the client TSP layer (`specification/client/`)
3. Suggesting it as an issue that should be filed against the upstream spec repo (do NOT file the issue yourself)

If you fix additional client TSP issues, re-run the compile to verify.

---

## Step 5: Compare Old vs. New Spec — Fix Custom C# Code

This is the most critical step for maintaining backward compatibility.

### 5a. Identify Renames

Compare the **old** base spec with the **new** base spec to find renamed types, properties, or operations. For each rename:

1. **Update `[CodeGenType("...")]` attributes** in `src/Custom/{Area}/Internal/GeneratorStubs.cs`
   - The `CodeGenType` attribute maps the generated type name to a custom partial class
   - If a type was renamed from `Foo` to `Bar`, update `[CodeGenType("Foo")]` → `[CodeGenType("Bar")]`

2. **Update custom code files** in `src/Custom/{Area}/`
   - Any file that references the old type name must be updated

**Example (`src/Custom/Audio/Internal/GeneratorStubs.cs`):**
```csharp
[CodeGenType("CreateSpeechRequestModel")] internal readonly partial struct InternalCreateSpeechRequestModel { }
[CodeGenType("CreateTranscriptionRequestModel")] internal readonly partial struct InternalCreateTranscriptionRequestModel { }
[CodeGenType("CreateTranscriptionResponseJson")] internal partial class InternalCreateTranscriptionResponseJson { }
```

If the generated type name changes (e.g., because the TypeSpec model was renamed), update the string in `CodeGenType(...)` to match the **new** generated name.

### 5b. Identify New Types/Properties

List all new types, properties, enums, or operations added in the updated spec. For each:
- Determine if it should be **public** or **internal**
- If internal, add a `[CodeGenType("...")]` stub in `src/custom/{Area}/Internal/GeneratorStubs.cs`
- If public, check if it is generated correctly in `src/Generated`
- New features that require significant work should be listed as suggested follow-up items

### 5c. Identify Removed Types/Properties

If types or properties were removed in the new spec:
- Remove corresponding `[CodeGenType("...")]` entries from `GeneratorStubs.cs`
- Remove or update any custom code that references them

---

## Step 6: List New Features and Changes

After reviewing the old vs. new spec, compile a list of:
- **New types** added to the spec
- **New properties** on existing types
- **New operations** (API endpoints)
- **Renamed types/properties** (and the mapping from old → new)
- **Removed types/properties**
- **Changed property types** (e.g., `long` → `int`, `string` → enum)
- **New features** that may need follow-up implementation

---

## Step 7: Verify Generated Code and Review Numeric Types

After `Invoke-CodeGen.ps1` completes successfully (from Step 4, or re-run after fixing issues in Steps 5–6), verify the output:

```powershell
# Check generated files exist
Get-ChildItem src/Generated/Models/{Area}/ -Name
Get-ChildItem src/Generated/{Area}Client*.cs -Name

# Build
dotnet build

# Export API surface
./scripts/Export-Api.ps1
```

### 7a. Review Numeric Properties

The `NumericPropertiesVisitor` (at `codegen/generator/src/Visitors/NumericPropertiesVisitor.cs`) automatically converts `long` → `int` and `double` → `float` for all generated properties unless explicitly excluded.

After generation, check the generated code for any numeric properties that should **stay `long`** (e.g., byte counts, large IDs) or **stay `double`** (high-precision values). If found, add them to the exclusion list:

```csharp
// In NumericPropertiesVisitor.cs
private static readonly HashSet<string> _excludedLongProperties = new(StringComparer.OrdinalIgnoreCase)
{
    "OpenAI.{Area}.{TypeName}.{PropertyName}",
};
```

See [PR #935 (VectorStore)](https://github.com/openai/openai-dotnet/pull/935) for an example where this visitor was enhanced.

### `Invoke-CodeGen.ps1` Parameter Sets (Reference)

The script has three modes. **Use Default (no parameters)** since you already copied the base spec manually:

| Mode | When to Use | What It Does |
|------|-------------|-------------|
| **Default** (no params) | You already have the base spec at `specification/base/` | Uses the existing base spec as-is, then runs `npm ci` → builds codegen plugin → `npx tsp compile .` |
| **GitHub** (`-GitHubOwner`, `-GitHubRepository`, `-CommitHash`, `-GitHubToken`) | Automated pipelines | Downloads the base spec from GitHub first (⚠️ **will overwrite** your manual copy if used with `-Force`) |
| **Local** (`-LocalRepositoryPath`) | You have a local clone of upstream | Copies from the local clone into `specification/base/` first |

> **WARNING:** Do NOT use `-GitHub` or `-Local` modes after manually copying the spec — they will overwrite your changes in `specification/base/`.

---

## Step 8: Handle Type Unions vs. Discriminators

> **CRITICAL RULE:** The base spec must remain an exact copy of upstream. If the upstream spec contains type unions (e.g., `model Foo { bar: TypeA | TypeB; }`), do NOT modify the base spec — instead:

1. **Keep the base spec as-is** (matching upstream exactly)
2. **After generation, list** all types/properties that use type unions that would need discriminator patterns
3. Handle discriminator patterns in the **client models TSP** (`specification/client/models/{area}.models.tsp`) where possible, using patterns like:

```typespec
@usage(Usage.output | Usage.json)
@discriminator("type")
model DotNetMyDiscriminatedModel {
  type: DotNetMyDiscriminatedModelType;
}

union DotNetMyDiscriminatedModelType {
  `variant_a`: "variant_a",
  `variant_b`: "variant_b",
  string
}

model DotNetVariantA extends DotNetMyDiscriminatedModel {
  ...VariantA;
}

model DotNetVariantB extends DotNetMyDiscriminatedModel {
  ...VariantB;
}
```

4. List any type unions that could not be resolved via client models as suggested upstream issues

---

## Step 9: Post-Generation Verification

After code generation completes successfully, produce a report covering:

### 9a. Build Results
- Does the project compile without errors?

### 9b. Diff Summary
Compare old generated code (`src/Generated/`) with new generated code:
- **New files** — list all newly generated files
- **Removed files** — list files no longer generated
- **Changed files** — summarize significant changes (new properties, renamed types, changed method signatures)

### 9c. API Surface Changes
Run `./scripts/Export-Api.ps1` and diff the `api/` files:
- `api/OpenAI.net8.0.cs`
- `api/OpenAI.net10.0.cs`
- `api/OpenAI.netstandard2.0.cs`

List any breaking changes (removed or renamed public API members).

### 9d. Issues Found
- Compile errors in generated code
- Missing custom code mappings
- Type unions that need discriminator treatment
- Properties/types that need follow-up work

### 9e. New Features Summary
- List new public types and properties with brief descriptions
- Note any features that need additional custom C# implementation
- Suggest follow-up items for significant new functionality

> **IMPORTANT:** All work is done locally. Do NOT create PRs or file issues. At the end, present a summary that includes a list of suggested issues (upstream spec issues, follow-up feature work, etc.) for the user to review and file manually.
