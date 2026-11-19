# Category 1 — `prohibited-namespace` errors

## Symptom

Compiler error naming a type with `prohibited-namespace`.

## Cause

A TypeSpec type landed in the root `OpenAI` namespace but has no `[CodeGenType]` stub to place
it in the correct area namespace (e.g., `OpenAI.Audio`).

## Fix

1. Identify the type name and the suggested stub from the error message.
2. Preserve internal visibility by default. The diagnostic suggests an `internal` stub with an
   `Internal` prefix, and the TypeSpec name does not determine SDK visibility. Make the type
   public only when the intended SDK contract requires it, such as when it is used by an
   existing public API or matches public patterns in the area's existing customizations.
3. Add an internal stub to the area's existing `Internal/GeneratorStubs.cs`:
   `OpenAI/src/Custom/{Area}/Internal/GeneratorStubs.cs`, or
   `OpenAI.Responses/src/Custom/Internal/GeneratorStubs.cs` for Responses. Public types instead
   use a dedicated custom file named after the type in the area's custom directory, such as
   `OpenAI/src/Custom/Batch/BatchCollectionOptions.cs`.

   If the area has no `Internal/GeneratorStubs.cs`, create one. Match the namespace and file
   header of the same file in a sibling area.

```csharp
// Default, as suggested by the diagnostic
[CodeGenType("SomeNewType")] internal partial class InternalSomeNewType { }

// Only when the type is intended to be public SDK API
[CodeGenType("SomeNewPublicType")] public partial class SomeNewPublicType { }
```
