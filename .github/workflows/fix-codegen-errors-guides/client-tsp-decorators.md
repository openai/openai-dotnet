# Category 2 — Client TSP decorator errors

## Symptom

Errors referencing `@@clientLocation`, `@@clientName`, `@@visibility`, `@@alternateType`, or
`@@usage` decorators.

## Cause

The new generator version changed how it validates or applies client decorators, so an existing
decorator no longer compiles or no longer targets the expected type, operation, or property.

## Fix

1. Open the client customization TSP file for the affected area under `specification/client/`.
2. Find the decorator named in the error.
3. Check the generator release notes and related `microsoft/typespec` PRs for decorator changes,
   then update the decorator to the new requirements or move the customization to where it
   belongs.
