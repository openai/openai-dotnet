# Category 4 — npm / build errors (plugin compilation)

## Symptom

Errors during `npm ci` or `npm run build` in the codegen plugin.

## Cause

Version incompatibilities after a generator update, or breaking API changes in the TypeSpec SDK.

## Fix

1. Check `codegen/package.json` for version mismatches; the generator plugin source is under
   `codegen/generator/src/`.
2. Delete `node_modules` only, then reinstall from the existing lockfile:
   ```powershell
   npm ci
   npm run build -w codegen
   ```
3. Do not delete or regenerate `package-lock.json` as part of this workflow fix path.
4. If the codegen plugin source has TypeScript compile errors, fix the TypeScript visitor code.
