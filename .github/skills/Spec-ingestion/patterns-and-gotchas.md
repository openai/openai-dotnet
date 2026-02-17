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

TypeSpec's `integer` type maps to `long` in C# by default. The `NumericTypesVisitor` (in `codegen/generator/`) converts:
- `long` → `int`
- `double` → `float`

for specific properties unless explicitly excluded.

**When adding new numeric properties:** Check if they need to be excluded from or included in the `NumericTypesVisitor`. If a property genuinely requires `long` (e.g., byte counts), add it to the exclusion list.

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

## 10. Test Fixes After Ingestion

Expect test updates after spec ingestion:
- **Session records** may need regeneration if API shapes changed
- **Assertion changes** for renamed/retyped properties
- **New test coverage** for new features (can be deferred)

---

## 12. API Export After Ingestion

Always run `./scripts/Export-Api.ps1` after successful code generation to update the API surface files (`api/OpenAI.net8.0.cs`, etc.). These files are used for API compatibility checks and should be committed as part of the PR.
