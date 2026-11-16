# Category 2 — Missing type / unresolved reference errors

## Symptom

`error type-not-found: Type "Foo" is not defined` or similar.

## Cause

The base spec references a type that doesn't yet exist in the local copy. Types are distributed
across the individual area `.tsp` files in the base spec directory.

The local base spec is under `specification/base/typespec/`; client customizations are under
`specification/client/`.

## Fix

1. Search the entire local base spec for the type definition:
   ```powershell
   Get-ChildItem -Path "specification/base/typespec" -Filter "*.tsp" -Recurse |
     Select-String -Pattern "model Foo|union Foo|enum Foo|alias Foo|scalar Foo"
   ```
2. If found locally, the type may not be imported in the right place — check the import chain
   in the main TSP entry point or the relevant area `.tsp` files.
3. If not found locally, check the upstream repository to confirm whether the type exists there.
4. Do **not** add the missing definition directly in the base spec directory.
   It is treated as an upstream copy in this workflow and should only be
   updated via the repository's base-spec sync/update process.
5. If the missing type is required for a local fix, implement the workaround in
   the client customization layer or custom C# code, or report that the local base copy is
   incomplete and needs to be refreshed from upstream.
