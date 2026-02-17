# Spec Ingestion — Step-by-Step Process

## Step 1: Copy the Latest Base Spec

1. Clone or pull the latest from the upstream repo: `https://github.com/microsoft/openai-openapi-pr` (branch: `main`).
2. For the target area (e.g., `audio`), copy the **entire contents** of:
   ```
   upstream: packages/openai-typespec/src/{area}/
   ```
   into:
   ```
   local: specification/base/typespec/{area}/
   ```
3. Replace all existing files in the local area directory with the upstream files.

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

## Step 2: Verify the Base TSP Compiles — Report Issues (Do NOT Modify)

After copying, compile the TypeSpec to check for errors:

```powershell
cd specification
npx tsp compile .
```

> **CRITICAL:** The base spec at `specification/base/typespec/` must remain an **exact copy** of the upstream spec. Do NOT modify it to fix compile errors. If there are issues, **report them** so they can be addressed upstream or in the client TSP layer.

### What to look for and report:

- **Missing types** — A type referenced in the new spec doesn't exist locally. Check if it exists in `common/` or another area upstream and needs to be copied over (as an exact copy).

For each issue found, document it and determine if it can be resolved by:
1. Copying additional files from upstream (e.g., updated `common/` types) — always as exact copies
2. Handling it in the client TSP layer (`specification/client/`)
3. Suggesting it as an issue that should be filed against the upstream spec repo (do NOT file the issue yourself)

---

## Step 3: Fix Client TSP Errors and Add `@@clientLocation`

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

### 3b. Fix Broken References

If a type or operation was renamed upstream, update the `@@clientName`, `@@visibility`, and other decorators in the client TSP to reference the new names.

### 3c. Client Models TSP

If the area has a client models file (`specification/client/models/{area}.models.tsp`), update it as well. These files contain:
- Discriminated union wrappers (using `@discriminator`)
- Extended models for .NET-specific patterns
- Custom model shapes needed by the C# generator

**Important:** When the upstream spec uses **type unions** (e.g., `model | string`), we use **discriminators** instead to avoid binary data types in the generated code. See Step 7 for details.

---

## Step 4: Compare Old vs. New Spec — Fix Custom C# Code

This is the most critical step for maintaining backward compatibility.

### 4a. Identify Renames

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

### 4b. Identify New Types/Properties

List all new types, properties, enums, or operations added in the updated spec. For each:
- Determine if it should be **public** or **internal**
- If internal, add a `[CodeGenType("...")]` stub in `src/custom/{Area}/Internal/GeneratorStubs.cs`
- If public, check if it is generated correctly in `src/Generated`
- New features that require significant work should be listed as suggested follow-up items

### 4c. Identify Removed Types/Properties

If types or properties were removed in the new spec:
- Remove corresponding `[CodeGenType("...")]` entries from `GeneratorStubs.cs`
- Remove or update any custom code that references them

---

## Step 5: List New Features and Changes

Before running code generation, compile a list of:
- **New types** added to the spec
- **New properties** on existing types
- **New operations** (API endpoints)
- **Renamed types/properties** (and the mapping from old → new)
- **Removed types/properties**
- **Changed property types** (e.g., `long` → `int`, `string` → enum)
- **New features** that may need follow-up implementation

---

## Step 6: Run Code Generation

```powershell
./scripts/Invoke-CodeGen.ps1
```

This script:
1. Compiles the TypeSpec specification
2. Runs the C# emitter and custom OpenAI plugin
3. Generates code into `src/Generated/`

After generation, verify:
- The build succeeds: `dotnet build`
- Tests pass: `dotnet test`
- Export the API surface: `./scripts/Export-Api.ps1`

---

## Step 7: Handle Type Unions vs. Discriminators

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

## Step 8: Post-Generation Verification

After code generation completes successfully, produce a report covering:

### 8a. Build and Test Results
- Does the project compile without errors?
- Do existing tests pass?
- Are there new test failures?

### 8b. Diff Summary
Compare old generated code (`src/Generated/`) with new generated code:
- **New files** — list all newly generated files
- **Removed files** — list files no longer generated
- **Changed files** — summarize significant changes (new properties, renamed types, changed method signatures)

### 8c. API Surface Changes
Run `./scripts/Export-Api.ps1` and diff the `api/` files:
- `api/OpenAI.net8.0.cs`
- `api/OpenAI.net10.0.cs`
- `api/OpenAI.netstandard2.0.cs`

List any breaking changes (removed or renamed public API members).

### 8d. Issues Found
- Compile errors in generated code
- Missing custom code mappings
- Type unions that need discriminator treatment
- Properties/types that need follow-up work

### 8e. New Features Summary
- List new public types and properties with brief descriptions
- Note any features that need additional custom C# implementation
- Suggest follow-up items for significant new functionality

> **IMPORTANT:** All work is done locally. Do NOT create PRs or file issues. At the end, present a summary that includes a list of suggested issues (upstream spec issues, follow-up feature work, etc.) for the user to review and file manually.
