# Component maps

Responses OpenAPI finalization uses two component maps:

- `responses.component-default-map.json` is generated from scratch on every run.
- `responses.component-policy-map.json` is optional, maintained manually, and never modified by tooling.

Run the three top-level phases in order:

```powershell
./scripts/specification/Generate-ResponsesOpenApi.ps1
./scripts/specification/Inherit-ResponsesOpenApi.ps1
./scripts/specification/Finalize-ResponsesOpenApi.ps1
```

Each successful phase atomically publishes its result to
`specification/openapi/responses.openapi.json`. Generation publishes the
componentized document. Inheritance reads that alias by default, retains the
discriminator-inherited and inherited-sorted artifacts, and republishes the
inherited-sorted document. Finalization also reads the alias by default,
validates policy, replaces the default map, applies both maps, retains the
deduplicated document, and republishes it as the final alias.

`Inherit-ResponsesOpenApi.ps1` and `Finalize-ResponsesOpenApi.ps1` accept
`-InputPath` when a different input is needed. Their retained outputs and the
published alias are always written beneath `-OutputDirectory`.

## Default map

Every component not claimed by policy appears exactly once in the default map. This includes components without duplicates, which appear as one-member groups.

```json
{
  "version": 1,
  "groups": [
    {
      "id": "LeafA",
      "type": "duplicate",
      "representative": "LeafA",
      "fingerprint": "sha256:...",
      "members": ["LeafA", "LeafB"]
    }
  ]
}
```

Manual changes to this file are discarded by the next finalization. To preserve a decision, copy the relevant group into the policy map.

## Policy map

The policy map has the same root shape but supports duplicate and override groups. All IDs, including IDs nested under overrides, are globally unique. Every group has at least one member, and its representative must be one of those members.

### Duplicate groups

A duplicate group preserves a reviewed structural grouping and output name:

```json
{
  "id": "ReviewedLeaf",
  "type": "duplicate",
  "representative": "LeafA",
  "fingerprint": "sha256:...",
  "members": ["LeafA", "LeafB"]
}
```

Every member must match the recorded fingerprint. If another component later matches exactly one policy duplicate group, the generated default map contains an `is_partial: true` companion with the same ID and only the new members. If it matches multiple policy targets, generation fails and asks for manual promotion.

### Override groups

An override intentionally replaces structurally different component groups with the parent representative schema:

```json
{
  "id": "CountOverride",
  "type": "override",
  "representative": "IntegerCount",
  "fingerprint": "sha256:...",
  "members": ["IntegerCount", "OtherIntegerCount"],
  "overrides": [
    {
      "id": "NumberCountOverrides",
      "type": "duplicate",
      "representative": "NumberCount",
      "fingerprint": "sha256:...",
      "members": ["NumberCount", "OtherNumberCount"]
    }
  ]
}
```

The parent members match the parent fingerprint. Each nested group is a complete duplicate group whose fingerprint differs from the parent fingerprint. All parent and nested members collapse to `CountOverride`, using the `IntegerCount` schema.

New components may produce partial companions for either the override's primary fingerprint or a nested group fingerprint. Both ultimately collapse to the parent override output. Nested representatives validate their structural partitions; they are not emitted as separate output components.

## Validation

Finalization fails without modifying policy when:

- a group has an unknown type, empty members, duplicate members, or a representative outside its members;
- an ID or component member is claimed more than once in policy;
- a policy component is missing from the componentized document;
- members do not match their recorded fingerprint;
- an override has no nested groups or a nested fingerprint equals its parent fingerprint;
- a new component matches multiple policy targets;
- the source graph, or policy normalization, contains cycle-dependent components;
- the generated default map is stale or manually edited when passed directly to the deduplicator.

Policy fingerprints are strict. Schema evolution requires manually updating the affected policy groups from a newly generated default-map view.
