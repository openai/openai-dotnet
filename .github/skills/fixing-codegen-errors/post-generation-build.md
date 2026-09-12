# Category 5 — Post-generation build errors (`dotnet build`)

## Symptom

`Invoke-CodeGen.ps1` succeeds but `dotnet build OpenAI.slnx` fails.

## Cause

Generated C# code references renamed/removed custom types, or numeric type conversions need
updating.

## Fix

Consult [file-locations.md](../ingesting-spec/file-locations.md) for the canonical paths referenced below.

1. Check for renamed types → update `[CodeGenType]` attributes in the relevant generator stub
   file for the affected area.
2. Check for numeric type issues → update exclusion lists in the numeric types visitor
   (under the codegen plugin source).
3. Check custom code for broken references in the custom C# code directories for the
   affected area.
