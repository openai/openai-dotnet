# Category 1 — `prohibited-namespace` errors

## Symptom

Compiler error naming a type with `prohibited-namespace`.

## Cause

A TypeSpec type landed in the root `OpenAI` namespace but has no `[CodeGenType]` stub to place
it in the correct area namespace (e.g., `OpenAI.Audio`).

## Fix

1. Identify the type name from the error message.
2. Determine visibility: types prefixed with `Internal` are internal; others are public.
3. Add a stub in the correct file. The repo has two source roots:

   **For areas in the main OpenAI package** (e.g., `OpenAI.Audio`, `OpenAI.Chat`):
   - Internal → `OpenAI/src/Custom/{Area}/Internal/GeneratorStubs.cs`
   - Public → `OpenAI/src/Custom/{Area}/GeneratorStubs.cs`

   **For the Responses package** (`OpenAI.Responses`):
   - Internal → `OpenAI.Responses/src/Custom/Internal/GeneratorStubs.cs`
   - Public → `OpenAI.Responses/src/Custom/GeneratorStubs.cs`

   *(Note: the Responses package has no `{Area}` subfolder — stubs live directly under `src/Custom/`.)*

   **Area-specific path exceptions:**
   - **Realtime**: stubs are in `OpenAI/src/Custom/Realtime/GA/Internal/GeneratorStubs.cs` (note the `GA/` subfolder).
   - **Assistants**: the internal stubs file is named `GeneratorStubs.Internal.cs`, not `GeneratorStubs.cs`.

   If the target `GeneratorStubs.cs` file does not yet exist for an area, create it with the
   appropriate namespace. Match the namespace and file header of an existing `GeneratorStubs.cs`
   in a sibling area.

```csharp
// Internal example
[CodeGenType("SomeNewInternalType")] internal partial class InternalSomeNewInternalType { }

// Public example
[CodeGenType("SomeNewPublicType")] public partial class SomeNewPublicType { }
```
