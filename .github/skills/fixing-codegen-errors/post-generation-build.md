# Category 5 — Post-generation build errors (`dotnet build`)

## Symptom

`Invoke-CodeGen.ps1` succeeds but `dotnet build OpenAI.slnx` fails.

## Cause

Generated C# code references renamed/removed custom types, or numeric type conversions need
updating.

## Fix

1. Check for renamed types → update `[CodeGenType]` attributes in `GeneratorStubs.cs`.
2. Check for numeric type issues → update exclusion lists in
   `codegen/generator/src/Visitors/NumericTypesVisitor.cs`.
3. Check custom code for broken references:
   - Main OpenAI package (area-scoped): `OpenAI/src/Custom/{Area}/`
   - Responses package: `OpenAI.Responses/src/Custom/`
