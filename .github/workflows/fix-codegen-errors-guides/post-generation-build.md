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
2. Check for numeric type issues → update exclusion lists in the numeric types visitor
   under `codegen/generator/src/Visitors/`.
3. Check custom code for broken references under `OpenAI/src/Custom/` or
   `OpenAI.Responses/src/Custom/`.
