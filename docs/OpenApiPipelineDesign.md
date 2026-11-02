# OpenAPI Pipeline Design

This reference describes implementation decisions and supported schema shapes
for maintainers of the OpenAPI transformation pipeline. For commands, map
formats, and manual policy maintenance, see the user-facing
[Component Maps guide](ComponentMaps.md).

## Pipeline boundaries

The three top-level phases share a feature-scoped artifact directory and a
mutable `<feature-area>.openapi.json` alias. Each successful phase atomically
publishes its retained result to that alias.

| Phase | Retained artifacts | Responsibility |
| --- | --- | --- |
| Generate | 01-15 | Extract the operation, normalize its schemas and discriminator compositions, sort the inline document, and componentize definitions. Publishes artifact 15. |
| Inherit | 16-17 | Convert componentized discriminator groups into inheritance, then sort. Publishes artifact 17. |
| Finalize | 18, plus the default component map | Validate manual policy, regenerate the default map, apply grouping and naming decisions, and publish artifact 18. |

The default and policy maps are unnumbered control files, not transformation
snapshots. The alias is also unnumbered. Stage numbering runs continuously from
01 through 18. Type-array normalization is stage 04, immediately after reference
inlining and before allOf flattening.

The discriminator-related handoffs are:

| Stage | Responsibility |
| --- | --- |
| 04 | Normalize schema type arrays into scalar types or constrained `anyOf` alternatives. |
| 08 | Require an exact `properties.type` field on eligible explicit object schemas. |
| 09 | Split eligible required multi-value `type` enums into singleton variants. |
| 10 | Reconcile existing `oneOf` discriminators, infer eligible `type` discriminators, and declare common outer discriminator constraints on nested groups. |
| 11 | Promote eligible discriminated `anyOf` groups to `oneOf`, relocate affected pointers, and remove orphaned discriminator metadata. |
| 12 | Flatten compatible groups using the same discriminator property without flattening groups using a different property. |
| 13 | Wrap or group eligible singleton `type` objects that are not already direct members of a valid discriminated group. |
| 14 | Sort groups using the discriminator property appropriate to each level. |
| 15 | Name and componentize schemas, relocate references, and materialize exhaustive discriminator mappings. |
| 16-17 | Build the supported inheritance hierarchy and sort the result. |
| 18 | Apply component maps while preserving references and discriminator mappings. |

## Type-array normalization

`Normalize-OpenApiTypeArrays.ps1` handles exact array-valued Schema Object
`type` keywords, including non-nullable mixed types and singleton arrays. Schema
maps, items, compositions, definitions, and conditionals are traversed using the
shared schema contexts. Property names, examples, defaults, extension data, and
other literal values are not interpreted as schemas. Scalar types and
typeless/boolean schemas are left intact except for eligible descendants.

Each declared JSON type becomes a constrained scalar branch. Non-null types
retain encounter order and null is placed last. Multiple branches use `anyOf`,
not `oneOf`, because `number` and `integer` overlap. One surviving branch becomes
a scalar-type schema without a redundant composition. Type arrays must be
nonempty and contain unique, exact JSON Schema type names; malformed arrays fail
with the source breadcrumb.

### Finite constraints and nullability

Enum values are partitioned by JSON kind without coercing numbers, booleans, or
numeric strings. Mathematically integral numbers also satisfy `integer`.
`const` and `enum` constraints are intersected with each declared type, and
branches proved impossible by those finite constraints are removed. A fully
impossible intersection becomes boolean `false`, never an unrestricted schema
or an empty composition. Malformed or duplicate enum entries fail clearly.

For example:

```json
{
	"type": ["string", "null"],
	"enum": ["explicit", "implicit", null]
}
```

becomes:

```json
{
	"anyOf": [
		{ "type": "string", "enum": ["explicit", "implicit"] },
		{ "type": "null" }
	]
}
```

If the enum excludes null, the result is a non-null string enum. Null is admitted
only when both the type declaration and finite constraints permit it. An
accepted null branch uses `type: "null"` without redundant null enum/const
constraints. Required-property membership is not changed by this stage.

### Constraint applicability and references

Object constraints such as `properties`, `required`, dictionaries, and property
counts stay on object branches. Item/tuple/contains constraints stay on array
branches; length/pattern/content constraints stay on strings; bounds and
`multipleOf` stay on numeric branches. Formats are preserved, including numeric
OpenAPI formats. Annotations and extensions stay on the outer schema when an
`anyOf` is introduced, or on the single schema when the type array collapses.

Existing `anyOf`, `oneOf`, `allOf`, `not`, and conditionals are copied as
conjunctive constraints on the typed alternatives, including null when they can
exclude it. The stage does not overwrite existing compositions or solve arbitrary
schema satisfiability. Candidate subtrees with scope-sensitive IDs/anchors, or
candidates inheriting an explicit URI scope, fail rather than risk changing
reference resolution.

References to a schema root target the complete normalized schema.
Moved descendants are tracked from original source pointers to final destination
pointers. The stage opts into single-pass longest-prefix relocation: a move from
`/anyOf` to `/anyOf/0/anyOf` must not match and apply itself repeatedly. Iterative
relocation is the default for callers that follow chained moves. Reference
rewriting is schema-aware, so literal examples containing reference-like keys
remain data.

Local ordinary/dynamic/recursive JSON pointers and discriminator mappings follow
their canonical retained targets; escaped tokens are preserved. Equivalent
copied subtrees use the first retained copy. References to removed values or
partitioned constraint containers without an identical retained copy fail
explicitly instead of silently pointing to different data. External references
are preserved without fetching them.

### Componentization prerequisite

Componentization requires normalized input and rejects remaining schema type
arrays with an instruction to run `Normalize-OpenApiTypeArrays.ps1`. Its object
predicate accepts only scalar `type: "object"`.

The non-null object branch becomes an ordinary object component, while the
property retains `anyOf` containing that component reference and null. Nullable
string enums similarly reference non-null enum components. This yields a named
TypeSpec model with nullability at its use site. Standalone callers must
normalize explicitly; componentization does not perform hidden normalization.

## Discriminator analysis plans

`Get-OpenApiInlineDiscriminatorUnionPlan` returns a `PSCustomObject` describing
its schema analysis. A plan is not a pipeline stage or a generated specification.
Analysis does not mutate the source; conversion functions copy schemas before
declaring constraints or moving definitions.

The union plan records its source `Schema`, discriminator `PropertyName`,
`CompositionName`, original `Branches`, per-alternative `BranchPlans`, and the
accepted discriminator `Values` at that level. Each branch plan records its
`Kind`, source `Branch`, JSON Pointer `Breadcrumb`, outer `Values`, and a
`NestedPlan` when that branch owns another discriminated group.

The shared analysis records each alternative as one of three branch kinds.
These names describe the relationship to the immediate parent, not outer versus
inner depth:

- `Leaf`: a concrete object alternative with required, finite discriminator values.
- `FlattenableGroup`: a nested group using the same discriminator property as its parent, with compatible metadata and matching, scope-safe structural constraints. Its alternatives can be promoted into the parent.
- `NestedDiscriminatorGroup`: a nested group using a different discriminator property. It remains a single outer alternative and uses its own discriminator to distinguish its members.

For a `type` group containing a role-discriminated Message and a FunctionCall,
the outer plan's values are `message` and `function_call`. Message's branch plan
has kind `NestedDiscriminatorGroup` and outer value `message`; its nested plan
has property `role` and values such as `user` and `assistant`. The two role
members do not contribute two duplicate `message` values to the outer plan.

The comprehensive comment before the shared analysis function documents the
object fields with an abbreviated example in
[the document helpers](../scripts/specification/pipeline/_OpenApiDocument.Helpers.ps1).

## Nested discriminator hierarchies

An outer discriminator can select a subgroup that has its own discriminator. For
example, `type: "message"` selects a message base whose children are selected by
`role`. The pipeline preserves that subgroup instead of flattening its children
into duplicate outer discriminator values.

Before adding an outer discriminator property to a `NestedDiscriminatorGroup`,
the pipeline proves that every concrete member already requires the same single
string discriminator value and that the group's existing constraints accept
that value. It then declares that property constraint directly on the subgroup.
The concrete members already enforce this discriminator constraint; the
pipeline does not guess an outer value or require an optional property on them.

Both discriminator levels are preserved through component naming, exhaustive
mappings, and inheritance. Names use the outer identity, such as
`typeOfMessage`, followed by inner identities such as `roleOfUser`. The outer
mapping targets the message subgroup, and the subgroup's mapping targets its
role-specific members. Inheritance retains the intermediate base's fixed outer
discriminator value and inner mapping, with one `allOf` reference to its
immediate parent. This repeats recursively for supported deeper groups.

Automatic discriminator inference is specialized for `type`. Nested discriminator
property names come from the schema; `role` is not hard-coded.
Analysis and normalization support more than two levels, subject to depth guards
and the required, finite, unambiguous discriminator checks at each level.

Nested bases must have one inheritance parent. Shared concrete leaves are cloned
per parent; shared nested bases, inheritance cycles, and unsupported intermediate
payload constraints fail explicitly. Both discriminator levels remain part of
structural component identities during finalization.

## Discriminator validation

An "invalid existing discriminator relationship" means one the pipeline cannot
justify under its supported, conservative rules. Some unsupported shapes can
still be valid JSON Schema; the pipeline does not infer a generated inheritance
model from them merely because discriminator metadata is present.

### Inline analysis and reconciliation

Stage 10 validates existing `oneOf` discriminators. Analysis rejects the
relationship when any of these conditions applies:

- `discriminator` is not an object, or `propertyName` is missing, empty, whitespace-only, or not a string.
- The composition is not a nonempty array of supported Schema Objects. Concrete alternatives must explicitly have scalar `type: "object"`; referenced alternatives are expected to have been inlined before this stage. Nested groups may omit that scalar type when their validated members are objects.
- A concrete alternative does not directly declare and require the discriminator property, or that property does not expose a nonempty finite set of string values through `const`, `enum`, or their intersection.
- Discriminator property constraints exclude strings; an enum is malformed, empty, contains non-string or duplicate entries; a const is not a string; or a const is excluded by its enum.
- Different immediate logical alternatives accept an overlapping discriminator value. Repeated `"message"` values inside one valid role-discriminated subgroup are not overlap at the outer level: the subgroup contributes `"message"` only once.
- A `FlattenableGroup` has incompatible discriminator metadata, unequal sibling constraints, or scope-sensitive identifiers/anchors/extensions that prevent safe flattening. Mapping differences alone do not prevent flattening because canonical mappings are rebuilt later.
- A `NestedDiscriminatorGroup` has an invalid inner discriminator, its concrete members do not all require the same single outer value, or an intermediate group's existing discriminator property constraints conflict with that common value. Optional or differing outer values are not normalized into a guessed value.

Stage 10 removes unsupported `oneOf` discriminator metadata without removing or
flattening the alternatives. It can replace that metadata with a valid inferred
`type` discriminator when the branches qualify. It does not infer arbitrary
property names such as `role`.

Stage 11 promotes supported discriminated `anyOf` compositions to `oneOf`.
Afterward it removes discriminator metadata not paired with a direct `oneOf`;
unsupported compositions themselves remain unchanged. Consequently, passing
stage 10 does not yet prove that a source-authored mapping is correct.

### Component mappings and inheritance

Stage 15 builds exhaustive canonical mappings for componentized `oneOf`/`anyOf`
groups. A missing or partial source mapping can be completed. A supplied mapping
must be an object whose entries have string targets resolving to supported local
component schema roots. Unknown discriminator values, targets that do not accept
their mapped value, targets pointing to the wrong alternative, dangling targets,
and ambiguous values across alternatives cause failure. `allOf` or mapping-only
discriminators require a nonempty explicit mapping. Incorrect mappings are not
silently treated as valid or repaired by deleting their incorrect entries.

Stage 16 additionally requires one singleton string-enum discriminator value per
mapped child and exactly one immediate inheritance base per generated child.
It rejects pre-existing unsupported `allOf`, true inheritance cycles, shared
nested bases with multiple parents, and intermediate payload constraints that
the inheritance transformation does not support. Shared concrete leaves are
cloned once per parent. These are inheritance limitations, not evidence that the
source is necessarily invalid OpenAPI. Later mapping or inheritance failures do
not publish a new final alias for the failing phase.

## Structural identities and finalization

Component maps compare schemas using deterministic structural fingerprints,
not component names. Object properties are ordered canonically; array order,
JSON value types, reference keyword kinds, and descendant pointer suffixes remain
part of the identity. Ordinary component references contribute structural
dependencies, and actual cycles or components depending on them are rejected.

Discriminator mapping targets are not structural dependency edges: an inherited
child references its base, and the base maps discriminator values back to that
child. Treating both directions as structural edges would create a false cycle
for every inheritance family. Instead, mapped targets are fingerprinted relative
to the owning base. References back to that base use a relative identity rather
than recursively fingerprinting the entire base again.

Nested discriminator groups require the full active ancestor context to remain
available. A role-specific child may reference the outer type base as well as
its immediate role base. Relative identity traversal and its cache keys retain
those ancestor anchors, keeping equivalent renamed families comparable without
discarding either level's mapping or payload constraints. Different mapped child
shapes still produce different fingerprints. This does not permit actual
structural inheritance cycles.