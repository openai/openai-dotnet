# Category 5 — Post-generation build errors (`dotnet build`)

## Symptom

`Invoke-CodeGen.ps1` succeeds but `dotnet build OpenAI.slnx` fails.

## Cause

Generated C# code references renamed/removed custom types, or numeric type conversions need
updating.

## Fix

1. Check for renamed types → update `[CodeGenType]` attributes in the relevant generator stub
   file for the affected area.
2. Check for numeric type issues → update exclusion lists in the numeric types visitor
   under `codegen/generator/src/Visitors/`.
3. Check custom code for broken references under `OpenAI/src/Custom/` or
   `OpenAI.Responses/src/Custom/`.
