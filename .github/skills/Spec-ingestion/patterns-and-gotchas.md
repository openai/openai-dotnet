# Common Patterns and Gotchas

Lessons learned from previous spec ingestion PRs. Review these before starting a new ingestion.

---

## 1. Operations Without Interfaces Need `@@clientLocation`

The latest upstream spec removes `interface` blocks. Every operation **MUST** have a `@@clientLocation` decorator in the client TSP or it won't be assigned to the correct client class.

```typespec
// OLD pattern (upstream used to have this):
interface Audio {
  createSpeech(...): ...;
  createTranscription(...): ...;
}

// NEW pattern (operations are standalone, so you need):
@@clientLocation(createSpeech, "Audio");
@@clientLocation(createTranscription, "Audio");
```

---

## 2. `GeneratorStubs.cs` is the Rename Registry

When types are renamed in the spec, the `[CodeGenType("NewGeneratedName")]` attribute in `src/Custom/{Area}/Internal/GeneratorStubs.cs` maps the new generated name to the existing custom partial class. This is how backward compatibility is maintained without renaming public types.

```csharp
// Maps generated type "CreateSpeechRequestModel" to internal class
[CodeGenType("CreateSpeechRequestModel")] internal readonly partial struct InternalCreateSpeechRequestModel { }
```

If a TypeSpec model is renamed from `FooBar` to `BazQux`, update:
```csharp
// Before:
[CodeGenType("FooBar")] internal partial class InternalFooBar { }
// After:
[CodeGenType("BazQux")] internal partial class InternalFooBar { }
```

---

## 3. Numeric Type Conversions

TypeSpec's `integer` type maps to `long` in C# by default. The `NumericTypesVisitor` (at `codegen/generator/src/Visitors/NumericTypesVisitor.cs`) converts:
- `long` → `int`
- `double` → `float`

for all generated properties unless explicitly excluded.

**After code generation, you MUST review the generated numeric properties.** If a property genuinely requires `long` (e.g., byte counts, large IDs) or `double` (high-precision values), add it to the exclusion list in the NumericTypesVisitor:

```csharp
private static readonly HashSet<string> _excludedLongProperties = new(StringComparer.OrdinalIgnoreCase)
{
    "OpenAI.{Area}.{TypeName}.{PropertyName}",
};
```

See [PR #935 (VectorStore)](https://github.com/openai/openai-dotnet/pull/935) for an example where this visitor was enhanced to handle fields and methods in addition to properties.

---

## 4. Streaming Responses and Discriminated Unions

Some areas (audio, chat, responses) have streaming variants. The client models TSP typically needs **discriminated union wrappers** for streaming event types:

```typespec
@usage(Usage.output | Usage.json)
@discriminator("type")
model DotNetCreateTranscriptionStreamingResponse {
  type: DotNetCreateTranscriptionStreamingResponseType;
}

union DotNetCreateTranscriptionStreamingResponseType {
  `transcript.text.segment`: "transcript.text.segment",
  `transcript.text.delta`: "transcript.text.delta",
  `transcript.text.done`: "transcript.text.done",
  string
}

model DotNetTranscriptTextSegmentEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextSegmentEvent;
}
```

---

## 5. `prohibited-namespace` Errors Require `[CodeGenType]` Stubs

A `prohibited-namespace` compile error means the generator found a type that doesn't have a corresponding `[CodeGenType]` stub in the custom C# code. This can be triggered by any type — inline unions, new models, new enums, etc. — but **not every new type causes it**. Only fix the specific types named in the error.

**Fix:** Add a `[CodeGenType]` stub for each type named in the error, placing it in the correct location:

- **Internal types** → `src/Custom/{Area}/Internal/GeneratorStubs.cs`
- **Public types** → `src/Custom/{Area}/GeneratorStubs.cs`

Look at existing stubs in the area to determine the right pattern (class vs. struct, readonly, etc.).

**Example — internal stubs** (`src/Custom/{Area}/Internal/GeneratorStubs.cs`):
```csharp
[CodeGenType("ContainerResourceMemoryLimit")] internal readonly partial struct InternalContainerResourceMemoryLimit { }
[CodeGenType("ContainerListResource")] internal partial class InternalContainerListResource { }
```

**Example — public stubs** (`src/Custom/{Area}/GeneratorStubs.cs`):
```csharp
[CodeGenType("ContainerResource")] public partial class ContainerResource { }
[CodeGenType("ContainerCollectionOrder")] public readonly partial struct ContainerCollectionOrder { }
```

**How to identify these:** The compiler error message will name the type exactly. Only add stubs for the types that appear in `prohibited-namespace` errors — do not preemptively stub every new type.

---

## 6. NEVER Modify the Base Spec

> **CRITICAL:** The base spec at `specification/base/typespec/` must be an **exact copy** of the upstream spec from `microsoft/openai-openapi-pr`. Do NOT modify it for any reason — not for type unions, not for import paths, not for namespaces, not for suppression directives.

If there are issues with the base spec:
- **Type unions** that would generate binary data types → handle in `specification/client/models/{area}.models.tsp` using discriminator patterns
- **Any other issues** → resolve in the client TSP layer if possible, or suggest them as upstream issues to be filed (do NOT file issues yourself)

---

## 7. Follow-up PRs for Complex Features

Not everything needs to be done during the spec ingestion. New features that require significant custom C# implementation should be listed as suggested follow-up items for the user to review.

**Examples from past ingestions:**
- Speech streaming events → [#914](https://github.com/openai/openai-dotnet/issues/914) (from Audio #913)
- Diarized transcription → [#916](https://github.com/openai/openai-dotnet/issues/916) (from Audio #913)
- Pagination for `GetFiles` → [#895](https://github.com/openai/openai-dotnet/issues/895) (from Files #894)
- `ExpiresAfter` parameter → [#896](https://github.com/openai/openai-dotnet/issues/896) (from Files #894)

---

## 8. `[Experimental]` Attribute for New Features

New public types and properties that are not yet stable should be marked with `[Experimental]` in the custom C# code. This was done during the Moderations ingestion (#888).

---

## 9. Test Fixes After Ingestion

Expect test updates after spec ingestion:
- **Session records** may need regeneration if API shapes changed
- **Assertion changes** for renamed/retyped properties
- **New test coverage** for new features (can be deferred)

---

## 10. API Export After Ingestion

Always run `./scripts/Export-Api.ps1` after successful code generation to update the API surface files (`api/OpenAI.net8.0.cs`, etc.). These files are used for API compatibility checks and should be committed as part of the PR.
