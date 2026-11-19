# Category 4 — Post-generation build errors (`dotnet build`)

## Symptom

`Invoke-CodeGen.ps1` succeeds but `dotnet build OpenAI.slnx` fails.

## Cause

The new generator version changed the generated C# code, such as generated names, signatures,
or serialization helpers, so custom code no longer compiles. Numeric type conversions may also
need updating.

## Fix

1. Check for generated type names changed by the generator → update `[CodeGenType]` attributes
   in the relevant generator stub file for the affected area.
2. Check for numeric type issues → follow `eng/patterns/numeric-types.md`. Generic `integer`
   and `numeric` keep their default `long` and `double` mappings unless the API's range or
   precision is known; only then add an `@@alternateType` override in the area's
   `specification/client/{area}.client.tsp`. Otherwise, update the custom code to use the
   generated type. Do not narrow a type just to fix a compile error.
3. Check custom code for broken references under `OpenAI/src/Custom/` or
   `OpenAI.Responses/src/Custom/`.
