# Category 3 — Client TSP decorator errors

## Symptom

Errors referencing `@@clientLocation`, `@@clientName`, `@@visibility`, `@@alternateType`, or
`@@usage` decorators.

## Cause

A type or operation was renamed/removed in the new base spec, but the client TSP still
references the old name.

## Fix

1. Open `specification/client/{area}.client.tsp`.
2. Find the stale reference named in the error.
3. Update it to match the new name from the base spec, or remove the `@@clientLocation` line if
   the operation was removed.
