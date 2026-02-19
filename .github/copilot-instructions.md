# Copilot Instructions

## TypeSpec Code Generation

When making changes to TypeSpec files (`.tsp` files), you must regenerate the code by running the following script:

```powershell
./scripts/Invoke-CodeGen.ps1
```

This ensures that any modifications to the TypeSpec definitions are properly reflected in the generated code.

When making changes to TypeSpec files (`.tsp` files), do not use type unions and instead use discriminators to ensure that we don't use binary data types when the generation occurs.

# Shared Guidelines for TypeSpec Union Refactoring Agents

## Objective

These guidelines govern all sub-agents that evaluate and refactor OpenAI TypeSpec specifications
to ensure the generated .NET SDK provides strongly-typed, idiomatic C# models rather than
loosely-typed `BinaryData` properties.

## Union Classification

Every union encountered in a TypeSpec file MUST be classified into exactly one of three categories:

### Category 1: Discriminated Union (should use `@discriminator`)

A union where all components are **model types** that share (or should share) a common string
property whose value uniquely identifies the variant.

**Indicators:**
- All components are `model` types (not scalars, not enums).
- A `type` (or similar) property exists on each component with distinct literal values.
- The union represents mutually-exclusive, structurally-distinct alternatives.

**Action:** Convert to a `@discriminator`-based model hierarchy.

### Category 2: Non-Discriminated Union (composition wrapper)

A union where components are **heterogeneous** — typically mixing a model with an enum,
or mixing structurally-incompatible models where no shared discriminator field exists.

**Indicators:**
- Components include a mix of model types and string-literal enums.
- No common discriminator field is feasible.
- The property is used as both input AND output (echoed back by the server).

**Action:** Create a composition wrapper class with:
- One nullable property per component.
- One constructor per component (assert non-null).
- Implicit conversion operators from each component to the wrapper.
- Only one property is non-null at a time; consumers check for `null` to discover the active type.

### Category 3: Shorthand Notation Union (normalize to longhand)

A union where one component is a **convenience shorthand** for another
(e.g., `string[]` is shorthand for a model's `tool_names` property).

**Indicators:**
- One component is a scalar or scalar-array that maps directly to a single property of
  the other component.
- The shorthand exists purely for input convenience.
- The longhand component is a strict superset of the shorthand.

**Action:**
- Surface only the longhand type in the SDK.
- Customize deserialization to accept shorthand and normalize it to longhand.
- Accept the minor limitations: round-tripping may convert shorthand→longhand, and
  sending shorthand explicitly requires `JsonPatch`.

## General Rules

1. **Preserve existing discriminators.** If a type already uses `@discriminator`, do not change it.
2. **Prefer compile-time safety over runtime flexibility.** Strongly-typed hierarchies are always
   preferable to `BinaryData` or `object`.
3. **Nullable semantics.** When a union includes `null`, treat it as the *property* being nullable,
   not as a union component.
4. **String-literal sets are enums.** A union of string literals (e.g., `"always" | "never"`)
   should be represented as an extensible enum (`readonly partial struct` in C#).
5. **Collection properties in union wrappers must be nullable** — unlike normal collection
   properties which are never null in the library.
6. **Implicit conversions.** Union wrapper classes MUST have implicit conversion operators to
   reduce ceremony for callers.
7. **Immutability of variant.** Once a union wrapper is constructed with a given component,
   it cannot be mutated to a different component (no setters on component properties).
8. **Output model compatibility.** All designs must work for properties that are both input
   and output (echoed back by the server).

## Three-Tier Fix Strategy (CRITICAL)

All agents MUST follow this escalation path when addressing union issues. The base TypeSpec
spec is treated as an **immutable upstream dependency** — never modify it directly.

### Tier 1: Use the Exact Base TypeSpec (No Changes)

The base TypeSpec lives in the upstream repo (`microsoft/openai-openapi-pr`) under
`packages/openai-typespec/src/<area>/`. This is imported into the SDK repo at
`base/typespec/<area>/main.tsp`.

**Rule:** Always start with an exact, unmodified copy of the latest base TypeSpec.
Do NOT fork, patch, or hand-edit these files. They are synced from upstream.

### Tier 2: Fix via Client/Model Overlays

If the base spec produces incorrect or suboptimal codegen, fix it using the SDK's overlay
files. These use TypeSpec augment decorators (`@@`) to reshape the generated output
without touching the base spec.

**Overlay locations (in the `openai-dotnet` SDK repo):**

| Overlay Type | Path | Purpose |
|---|---|---|
| **Client overlay** | `specification/client/<area>.client.tsp` | Operation-level customizations: rename methods (`@@clientName`), move operations (`@@clientLocation`), control access (`@@access`, `@@usage`), suppress convenience methods (`@@convenientAPI`), swap types (`@@alternateType`), enable JsonPatch (`@@dynamicModel`) |
| **Model overlay** | `specification/client/models/<area>.models.tsp` | Model-level customizations: define new SDK-only types, add collection options, define extensible enums (`union { string, ... }`), create alias types for query parameters |

**What overlays CAN do:**
- Change generated names, access levels, and visibility
- Substitute alternate types for properties (`@@alternateType`)
- Mark models as dynamic for JsonPatch support (`@@dynamicModel`)
- Add new SDK-only models/enums/aliases that don't exist in the base spec
- Define collection options and query parameter shapes

**What overlays CANNOT do:**
- Add `@discriminator` to an existing base model (requires base spec change)
- Change the structural hierarchy (add `extends` relationships) of base models
- Remove or rename properties defined in the base spec
- Change a union to a non-union in the base spec

### Tier 3: Open an Upstream Issue

If the problem **cannot** be fixed through overlays (e.g., a union needs to become a
`@discriminator` hierarchy, which requires structural changes to the base models), then:

1. **Do NOT modify the base spec locally.**
2. **Open an issue** on the upstream repo (`microsoft/openai-openapi-pr`) with:
   - Title: `[TypeSpec] <Area>: <Brief description of the union problem>`
   - Body containing:
     - The file path and line number in the base spec
     - The current union expression
     - The expected pattern (e.g., `@discriminator` hierarchy)
     - Which union category (1, 2, or 3) this falls under
     - The impact on SDK codegen (e.g., "produces `BinaryData` instead of typed model")
     - A proposed TypeSpec diff showing the desired base spec change
3. **Document the issue** in the changeset output so it can be tracked.

### Decision Flowchart

```
Found a problematic union
         │
         ▼
Can it be fixed with @@alternateType,
@@clientName, new SDK-only models, or
custom deserialization in overlays?
         │
    ┌────┴────┐
    Yes       No
    │         │
    ▼         ▼
  Tier 2:   Does it require structural
  Write     changes to base models?
  overlay   (e.g., @discriminator, extends)
  files           │
             ┌────┴────┐
             Yes       No → re-evaluate
             │
             ▼
           Tier 3:
           Open upstream
           issue
```

## TypeSpec Refactoring Conventions

- Use `@discriminator("type")` (or the appropriate field name) on base models — but only
  when proposing upstream changes (Tier 3). Never add this to the local base copy.
- Child models should use `extends` to derive from the base — upstream only.
- In overlays, use `@@alternateType` to swap a union property to a custom SDK type.
- Keep union syntax in the base when the union is genuinely non-discriminated and will be
  handled by a composition wrapper in the SDK (Tier 2 model overlay).
- Add `@doc` annotations to new base models when proposing upstream changes.

## File Organization

- **Base spec files** (`base/typespec/<area>/`): NEVER modify. Synced from upstream.
- **Client overlays** (`specification/client/<area>.client.tsp`): Operation customizations.
- **Model overlays** (`specification/client/models/<area>.models.tsp`): Type customizations.
- Each agent outputs its findings and proposed changes in a structured format.
- Changes must be minimal and surgical.
- Every proposed change must reference:
  - Which category (1, 2, or 3) it falls under
  - Which tier (1, 2, or 3) the fix uses
  - If Tier 3: the upstream issue details
