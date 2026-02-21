# Copilot Instructions

## TypeSpec Code Generation

When making changes to TypeSpec files (`.tsp` files), you must regenerate the code by running the following script:

```powershell
./scripts/Invoke-CodeGen.ps1
```

This ensures that any modifications to the TypeSpec definitions are properly reflected in the generated code.


When making Changes to TypeSpec files (`.tsp` files), Do not use type unions and instead use discriminators to ensure that we don't use binary data types when the generation occurs.
