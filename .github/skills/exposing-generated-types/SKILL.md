---
name: exposing-generated-types
description: Guide for taking previously-hidden (internal) generated types and exposing them as a public, idiomatic API in the openai-dotnet SDK. Use this when asked to expose a tool, item, or model family that exists in the TypeSpec spec but is currently emitted as `Internal*` or marked with `[CodeGenVisibility(..., Internal)]`. Covers spec additions, namespace shifting, the prohibited-namespace gate, factory methods, and validation.
---

# Exposing Generated Types as Public

## Overview

Many TypeSpec models in `specification/base/typespec/` are emitted by the code generator into the `OpenAI` namespace as `internal` (often renamed `Internal*` via a stub in `OpenAI.Responses/src/Custom/Internal/GeneratorStubs.cs`, or hidden via `[CodeGenVisibility(name, Internal)]` on a kind enum).

When a feature graduates and needs to be exposed publicly, the work spans the spec layer, the C# customization layer, and post-generation validation. This skill documents that workflow end-to-end.

## When to use this skill

Trigger this skill when the user asks to:

- Expose a previously hidden tool, response item, streaming event, or model family
- Make a `ResponseTool`-derived or `ResponseItem`-derived type public
- Add new public surface for an API area whose TSP models exist but are internal
- "Unhide" a kind value (`ResponseToolKind.X`, `ResponseItemKind.X`) and surface the corresponding classes

## High-level steps

1. **Locate the spec types**
   - Enum entry: typically `union ToolType` / `union ItemType` in `specification/base/typespec/responses/models.tsp` / `models_items.tsp`
   - Model: search for `extends Tool`, `extends ItemResource`, `extends ItemParam`
   - Discriminated sub-types: search for `@discriminator("type")` bases
   - If the enum value exists but the model does not, the spec must be extended (see the upstream `openai/openai-openapi` `openapi.yaml` for the canonical shape; rename `Function*Shell*` → `Shell*` etc. to follow the SDK's clean naming).

2. **Extend the spec if needed**
   - Add the model + supporting discriminated bases + variants in `models.tsp` (for tool models) or `models_items.tsp` (for response items)
   - For items, follow the alias-base + split `*Param`/`*Resource` pattern used by LocalShell / Mcp:
     ```tsp
     alias FooToolCallItemBase = { call_id: string; action: FooAction; };
     model FooToolCallItemParam extends ItemParam { type: ItemType.foo_call; ...FooToolCallItemBase; }
     model FooToolCallItemResource extends ItemResource { type: ItemType.foo_call; status: FooCallStatus; ...FooToolCallItemBase; }
     ```
   - For "status set by server" properties, add `@@visibility(FooToolCallItemResource.status, Lifecycle.Read);` in `specification/client/responses.client.tsp`

3. **Register dynamic models**
   - Every new spec model **must** have a `@@dynamicModel(ModelName);` entry in `specification/client/responses.client.tsp`. Missing entries break JsonPatch round-tripping.

4. **Add C# customization stubs**
   - **Public types** must have a `[CodeGenType("SpecName")] public partial class ClientName { }` declaration in a file inside `OpenAI.Responses/src/Custom/` whose namespace is `OpenAI.Responses`. This shifts the generated type from `OpenAI` into `OpenAI.Responses` (the `ResponsesDirectoryVisitor` then routes the file to `OpenAI.Responses/src/Generated/`) and satisfies the `ProhibitedNamespaceVisitor` check.
   - **Internal stubs** (discriminator type enums, `*Param` variants the SDK hides, `Unknown*` discriminator unknowns) go into `OpenAI.Responses/src/Custom/Internal/GeneratorStubs.cs` as `[CodeGenType("SpecName")] internal partial class InternalClientName { }`.
   - A compact one-liner-per-type style file (see `Tools/ShellTool/ShellToolTypes.cs` for the canonical example) is preferred over one file per type when the customizations are pure renames/namespace-shifts.

5. **Remove the hide entries**
   - Strip the corresponding `[CodeGenVisibility(nameof(...), Internal)]` lines from `OpenAI.Responses/src/Custom/Tools/ResponseToolKind.cs` and/or `OpenAI.Responses/src/Custom/Items/ResponseItemKind.cs`.

6. **Add factory methods**
   - For tools: add `CreateXxxTool(...)` to `OpenAI.Responses/src/Custom/Tools/ResponseTool.cs`
   - For response items: add `CreateXxxItem(...)` / `CreateXxxOutputItem(...)` to `OpenAI.Responses/src/Custom/Items/ResponseItem.cs`
   - Use the existing factories (`CreateApplyPatchTool`, `CreateFunctionCallOutputItem`, etc.) as templates.

7. **Per-property customizations** (only if needed)
   - Nullable + setter for "set by server, optional on input" properties — see `ApplyPatch/ApplyPatchCallItem.cs`:
     ```csharp
     [CodeGenMember("Status")] public FooCallStatus? Status { get; set; }
     ```

8. **Run code generation**
   ```pwsh
   ./scripts/Invoke-CodeGen.ps1 -SkipExperimentalValidation
   ```
   - On the **first** run after adding spec types, use `-Clean` if the codegen project bin dirs have grown nested `bin\Debug\netX.Y\bin\Debug\...` paths.

9. **Validate**
   ```pwsh
   dotnet build OpenAI.slnx --nologo
   pwsh -NoProfile -File scripts/Test-ExperimentalAttributes.ps1
   pwsh -NoProfile -File scripts/Export-Api.ps1   # check api/OpenAI.*.cs diff
   dotnet test tests --nologo -f net10.0 --filter "FullyQualifiedName~Responses"
   ```

## Critical gotchas

### The prohibited-namespace gate

`ProhibitedNamespaceVisitor` (in `codegen/generator/src/Visitors/ProhibitedNamespaceVisitor.cs`) **errors** the build if any public type — or any `Unknown*`-prefixed type — lands in the bare `OpenAI` or `OpenAI.Models` namespace without a custom code anchor.

> Symptom: `error prohibited-namespace: [CodeGenType("Xxx")] internal readonly partial struct InternalXxx {}` printed at the end of `tsp compile`. The diagnostic text **is** the fix-it stub — paste it into `GeneratorStubs.cs` (for internal) or into a `Custom/.../*.cs` file with `public` instead of `internal` (for public).

Always emit a stub for:

- Every public type the spec produces
- Every `Unknown*` discriminator-unknown shim the generator emits (e.g. `UnknownFooEnvironment`, `UnknownFooSkill`)
- Every discriminator `*Type` enum (almost always internal)
- Every `*Param` half of a split Param/Resource pair when the SDK only exposes the Resource

### Namespace anchoring is the only way to route files

`ResponsesDirectoryVisitor` routes a generated file into `OpenAI.Responses/src/Generated/` **only if** the type's `.Type.Namespace` is `OpenAI.Responses`. There is no `@@clientNamespace` decorator in our TSP — the C# custom code's `namespace OpenAI.Responses;` declaration is what shifts the generator's idea of the type's namespace.

If you see your new public type land at `OpenAI/src/Generated/Models/Foo.cs` instead of `OpenAI.Responses/src/Generated/Models/Foo.cs`, you forgot the public stub.

### Naming conventions for renamed types

When renaming via `[CodeGenType("SpecName")]`:

- Tools: drop any vendor prefix and append `Tool` → `FunctionShellToolParam` → `ShellTool`
- Item resources: `FooToolCallItemResource` → `FooCallItem` (newer style, prefer this) or `FooCallResponseItem` (older style — keep if all other items in the family use this)
- Item outputs: `FooToolCallOutputItemResource` → `FooCallOutputItem` / `FooCallOutputResponseItem`
- Discriminator unions: `FooBase` → `Foo`, `FooXxxYyy` (concrete) → `XxxFoo` if it reads better (e.g. `ShellToolContainerAutoEnvironment` rather than `ContainerAutoShellToolEnvironment` — keep the model-family root prefix)
- Status / kind enums: `FooCallStatus` (singular, not pluralized)

### Status visibility convention

For most call/output items the server sets `status`. The convention is:

- **Make `status` optional in the spec** (`status?: FooCallStatus;`) so the codegen drops it from the public ctor *and* wraps the writer in `Optional.IsDefined(Status)`. Marking a spec-required field nullable purely in C# (e.g. `[CodeGenMember("Status")] public FooCallStatus? Status { get; set; }`) makes the property nullable but the generator will still emit unconditional `Status.Value.ToString()` in `JsonModelWriteCore` — that NRE's at request time when a caller constructs the item without a status. **Lifecycle.Read visibility alone is not sufficient either** for the same reason.
- ApplyPatch's input `status` is a deliberate exception: it is **set by the client** (success/failure of the patch apply) and is required, so leave it `status: ApplyPatchCallOutputStatus;` in spec.
- For shell-style items where the server sets status and the client must omit it on input, the working pattern is:
  ```tsp
  model FooToolCallOutputItemResource extends ItemResource {
    type: ItemType.foo_call_output;
    status?: FooCallStatus;
    ...FooToolCallOutputItemBase;
  }
  ```
  No `@@visibility(...status, Lifecycle.Read)`. No C# nullability customization needed.

### Renames + type changes on properties

- **Rename only**: `@@clientName(ModelName.spec_field_name, "ClientName");` in `responses.client.tsp`. The wire JSON property name stays as the spec name; only the C# property name changes.
- **Type change to TimeSpan with integer-ms wire encoding**: use `@encode(DurationKnownEncoding.milliseconds, integer)` directly in the spec and type the field as `duration | null`. The generator emits `TimeSpan?` with `Convert.ToInt64(Math.Round(value.TotalMilliseconds))` on write and `TimeSpan.FromMilliseconds(prop.Value.GetInt64())` on read. Combining `@@clientName` to rename (e.g. `timeout_ms` → `Timeout`) and `@encode` together is the standard idiom and the resulting `[Optional.IsDefined(Timeout)]` plumbing is correct out of the box. **Do not** add a custom serialization hook for this case (the realtime `AudioEndTime` hook predates the working encoder and shouldn't be copied for new properties).

### `bytes` in inline data fields

For inline file payloads (e.g. `ShellToolInlineSkillSource.data`), use TSP `bytes`. The generator emits `BinaryData` and handles base64 round-tripping automatically. Do **not** model these as `string` — JsonPatch and serialization break.

### Dynamic model registration

Every new model added to spec needs a `@@dynamicModel(ModelName);` entry in `responses.client.tsp`. Group with the existing block (custom items vs. base models vs. streaming events). Forgetting this won't break compilation but will break JsonPatch propagation at runtime.

### Discriminators, not unions

Per repo convention (`.github/copilot-instructions.md`): **never** introduce a TSP type-union (`A | B | C`) — the generator will produce `BinaryData` properties. Always model variants with `@discriminator("type")` + concrete `extends`. The Shell environment / network-policy / skill / outcome trees all follow this rule.

## Logistical / codegen issues you'll hit

- **`Invoke-CodeGen.ps1` MSB3030 with deeply nested `bin\Debug\netX.Y\bin\Debug\…` paths** — re-run with `-Clean`. This happens any time you run codegen twice in quick succession after the C# stub set changes.
- **Recordings should not be polluted with live API keys.** When writing live e2e tests, use a plain `[Test]` class that does NOT inherit `OpenAIRecordedTestBase`, mark it `[Explicit]` + `[Category("Live")]`, and instantiate `ResponsesClient` directly with `new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))`. Do not commit recordings for these tests.
- **`StringAssert.Contains` is not available** in the NUnit version this repo uses (NUnit 4+). Use `Assert.That(text, Does.Contain("..."))` instead.
- **Hosted shell responses echo `container_auto` back as `container_reference`** after the server provisions the container. Assertions on the responded environment type should accept both.
- **Local-mode shell never returns `shell_call_output` items from the server.** The test for the local resolution path must (a) verify there are zero `ShellCallOutputItem` instances in the first response and (b) feed the simulated outputs back via `CreateResponseOptions.InputItems` with `PreviousResponseId` set to continue the turn.

## File locations cheat-sheet

| Concern | Path |
|---|---|
| Tool model | `specification/base/typespec/responses/models.tsp` |
| Item models | `specification/base/typespec/responses/models_items.tsp` |
| Client overlays + `@@dynamicModel` | `specification/client/responses.client.tsp` |
| Internal stubs (renames + namespace shift to internal) | `OpenAI.Responses/src/Custom/Internal/GeneratorStubs.cs` |
| Public stub anchor files | `OpenAI.Responses/src/Custom/Tools/<ToolName>/` and `OpenAI.Responses/src/Custom/Items/<ToolName>/` |
| Tool factories | `OpenAI.Responses/src/Custom/Tools/ResponseTool.cs` |
| Item factories | `OpenAI.Responses/src/Custom/Items/ResponseItem.cs` |
| Kind hide list (tools) | `OpenAI.Responses/src/Custom/Tools/ResponseToolKind.cs` |
| Kind hide list (items) | `OpenAI.Responses/src/Custom/Items/ResponseItemKind.cs` |
| Prohibited-namespace visitor | `codegen/generator/src/Visitors/ProhibitedNamespaceVisitor.cs` |
| Responses dir router | `codegen/generator/src/Visitors/ResponsesDirectoryVisitor.cs` |
| API export | `api/OpenAI.{net8.0,net10.0,netstandard2.0}.cs` (regenerate with `scripts/Export-Api.ps1`) |

## Worked example: the Shell tool

The shell tool family is the canonical, freshly-added example of this pattern:

- Tool models live in `models.tsp` (`ShellTool`, `ShellToolEnvironment` + 3 variants, `ShellToolContainerNetworkPolicy` + 2 variants, `ShellToolSkill` + 2 variants, `ShellToolDomainSecret`, `ShellToolLocalSkill`, etc.)
- Item models live in `models_items.tsp` (`ShellAction`, `ShellCallStatus`, `ShellCallOutputContent`, `ShellCallOutcome` + 2 variants, `ShellToolCallItem(Param|Resource)`, `ShellToolCallOutputItem(Param|Resource)`)
- Items split `Param` (internal stub in `GeneratorStubs.cs`) / `Resource` (public, renamed to `ShellCallItem`/`ShellCallOutputItem`)
- Output items carry `IList<ShellCallOutputContent>` with stdout/stderr/discriminated outcome
- `ResponseItem.CreateShellCallOutputItem(callId, output)` is the input-side factory callers use to resolve local-mode shell tool calls
- `ResponseTool.CreateShellTool(environment)` is the tool factory
- Both kind values (`ResponseToolKind.Shell`, `ResponseItemKind.ShellCall`, `ResponseItemKind.ShellCallOutput`) are exposed by removing the `[CodeGenVisibility(..., Internal)]` lines
