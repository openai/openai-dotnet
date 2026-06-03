# Category 1 — `prohibited-namespace` errors

## Symptom

Compiler error naming a type with `prohibited-namespace`.

## Cause

A TypeSpec type landed in the root `OpenAI` namespace but has no `[CodeGenType]` stub to place
it in the correct area namespace (e.g., `OpenAI.Audio`).

## Fix

1. Identify the type name from the error message.
2. Determine visibility: types prefixed with `Internal` are internal; others are public.
3. Add a stub in the correct Custom generator stub file. Consult [file-locations.md](../ingesting-spec/file-locations.md)
   for the canonical mapping of areas to custom C# code directories and generator stub paths.

   If the target `GeneratorStubs.cs` file does not yet exist for an area, create it with the
   appropriate namespace. Match the namespace and file header of an existing `GeneratorStubs.cs`
   in a sibling area.

```csharp
// Internal example
[CodeGenType("SomeNewInternalType")] internal partial class InternalSomeNewInternalType { }

// Public example
[CodeGenType("SomeNewPublicType")] public partial class SomeNewPublicType { }
```
