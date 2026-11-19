# Category 3 — npm / build errors (plugin compilation)

## Symptom

Errors during `npm ci` or `npm run build` in the codegen plugin.

## Cause

Version incompatibilities after a generator update, or breaking API changes in the TypeSpec SDK.

## Fix

1. Check `codegen/package.json` for version mismatches.
2. Delete `node_modules` only, then reinstall from the existing lockfile:
   ```powershell
   npm ci
   npm run build -w codegen
   ```
3. Do not delete or regenerate `package-lock.json` as part of this workflow fix path.
4. Identify which plugin component failed to build:
   - `build:emitter` failures are TypeScript compile errors in the emitter under
     `codegen/emitter/src/`. Update the emitter code for the changed TypeSpec or emitter API.
   - `build:generator` failures are C# compile errors in the generator project under
     `codegen/generator/`, including the visitors in `codegen/generator/src/Visitors/`.
     Update the C# code for the changed generator API.
