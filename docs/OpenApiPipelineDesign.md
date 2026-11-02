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
| Generate | 01-14 | Extract the operation, normalize its schemas and discriminator compositions, sort the inline document, and componentize definitions. Publishes artifact 14. |
| Inherit | 15-16 | Convert componentized discriminator groups into inheritance, then sort. Publishes artifact 16. |
| Finalize | 17, plus the default component map | Validate manual policy, regenerate the default map, apply grouping and naming decisions, and publish artifact 17. |

The default and policy maps are unnumbered control files, not transformation
snapshots. The alias is also unnumbered. Stage numbering remains continuous from
01 through 17; nested discriminator support does not introduce another stage.

The discriminator-related handoffs are:

| Stage | Responsibility |
| --- | --- |
| 07 | Require an exact `properties.type` field on eligible explicit object schemas. |
| 08 | Split eligible required multi-value `type` enums into singleton variants. |
| 09 | Reconcile existing `oneOf` discriminators, infer eligible `type` discriminators, and declare common outer discriminator constraints on nested groups. |
| 10 | Promote eligible discriminated `anyOf` groups to `oneOf`, relocate affected pointers, and remove orphaned discriminator metadata. |
| 11 | Flatten compatible groups using the same discriminator property without flattening groups using a different property. |
| 12 | Wrap or group eligible singleton `type` objects that are not already direct members of a valid discriminated group. |
| 13 | Sort groups using the discriminator property appropriate to each level. |
| 14 | Name and componentize schemas, relocate references, and materialize exhaustive discriminator mappings. |
| 15-16 | Build the supported inheritance hierarchy and sort the result. |
| 17 | Apply component maps while preserving references and discriminator mappings. |

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

Only new `type` discriminator inference is special-cased. Existing nested
discriminator property names come from the schema; `role` is not hard-coded.
Analysis and normalization support more than two levels, subject to depth guards
and the required, finite, unambiguous discriminator checks at each level.

Nested bases must have one inheritance parent. Shared concrete leaves retain
per-parent cloning support; shared nested bases, inheritance cycles, and
unsupported intermediate payload constraints fail explicitly. Both discriminator
levels remain part of structural component identities during finalization.

## Discriminator validation

An "invalid existing discriminator relationship" means one the pipeline cannot
justify under its supported, conservative rules. Some unsupported shapes can
still be valid JSON Schema; the pipeline does not infer a generated inheritance
model from them merely because discriminator metadata is present.

### Inline analysis and reconciliation

Stage 09 validates existing `oneOf` discriminators. Analysis rejects the
relationship when any of these conditions applies:

- `discriminator` is not an object, or `propertyName` is missing, empty, whitespace-only, or not a string.
- The composition is not a nonempty array of supported Schema Objects. Concrete alternatives must explicitly have scalar `type: "object"`; referenced alternatives are expected to have been inlined before this stage. Nested groups may omit that scalar type when their validated members are objects.
- A concrete alternative does not directly declare and require the discriminator property, or that property does not expose a nonempty finite set of string values through `const`, `enum`, or their intersection.
- Discriminator property constraints exclude strings; an enum is malformed, empty, contains non-string or duplicate entries; a const is not a string; or a const is excluded by its enum.
- Different immediate logical alternatives accept an overlapping discriminator value. Repeated `"message"` values inside one valid role-discriminated subgroup are not overlap at the outer level: the subgroup contributes `"message"` only once.
- A `FlattenableGroup` has incompatible discriminator metadata, unequal sibling constraints, or scope-sensitive identifiers/anchors/extensions that prevent safe flattening. Mapping differences alone do not prevent flattening because canonical mappings are rebuilt later.
- A `NestedDiscriminatorGroup` has an invalid inner discriminator, its concrete members do not all require the same single outer value, or an intermediate group's existing discriminator property constraints conflict with that common value. Optional or differing outer values are not normalized into a guessed value.

Stage 09 removes unsupported `oneOf` discriminator metadata without removing or
flattening the alternatives. It can replace that metadata with a valid inferred
`type` discriminator when the branches qualify. It does not infer arbitrary
property names such as `role`.

Stage 10 promotes supported discriminated `anyOf` compositions to `oneOf`.
Afterward it removes discriminator metadata not paired with a direct `oneOf`;
unsupported compositions themselves remain unchanged. Consequently, passing
stage 09 does not yet prove that a source-authored mapping is correct.

### Component mappings and inheritance

Stage 14 builds exhaustive canonical mappings for componentized `oneOf`/`anyOf`
groups. A missing or partial source mapping can be completed. A supplied mapping
must be an object whose entries have string targets resolving to supported local
component schema roots. Unknown discriminator values, targets that do not accept
their mapped value, targets pointing to the wrong alternative, dangling targets,
and ambiguous values across alternatives cause failure. `allOf` or mapping-only
discriminators require a nonempty explicit mapping. Incorrect mappings are not
silently treated as valid or repaired by deleting their incorrect entries.

Stage 15 additionally requires one singleton string-enum discriminator value per
mapped child and exactly one immediate inheritance base per generated child.
It rejects pre-existing unsupported `allOf`, true inheritance cycles, shared
nested bases with multiple parents, and intermediate payload constraints that
the inheritance transformation does not support. Shared concrete leaves retain
the existing per-parent cloning behavior. These are inheritance limitations,
not evidence that the source is necessarily invalid OpenAPI. Later mapping or
inheritance failures do not publish a new final alias for the failing phase.

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