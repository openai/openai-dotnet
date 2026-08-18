# Numeric types

This document defines how the OpenAI .NET library maps TypeSpec's generic `integer` and `numeric` scalar types to C# numeric types.

## Default mappings

The generator uses these mappings unless the client TypeSpec overrides them:

| TypeSpec type | C# type |
|---------------|---------|
| `integer` | `long` |
| `numeric` | `double` |

The generic TypeSpec types do not communicate the range or precision required by the API. Choose an explicit type for the .NET API whenever that requirement is known:

| TypeSpec alternate type | C# type | Use when |
|-------------------------|---------|----------|
| `int32` | `int` | Values fit in a 32-bit signed integer, such as counts, limits, indexes, and token totals |
| `int64` | `long` | Values may exceed the 32-bit range, such as byte counts or large identifiers |
| `float32` | `float` | Single precision is sufficient, such as scores, probabilities, ratios, and thresholds |
| `float64` | `double` | The value requires greater range or precision |

Do not narrow a type based only on its current examples or observed values. Preserve `int64` or `float64` when the API contract requires the larger range or precision.

## Apply the mapping in client TypeSpec

The files under `specification/base/` are copies of the upstream specification and must not be modified. Add `@@alternateType` customizations to the area's `specification/client/{area}.client.tsp` file instead.

Override a model property directly:

```typespec
@@alternateType(TokenUsage.total_tokens, int32);
@@alternateType(RankingOptions.score_threshold, float32);
```

Reference operation parameters through the operation's `parameters` meta-property:

```typespec
@@alternateType(ListItems::parameters.limit, int32);
```

Preserve optionality when replacing a nullable type:

```typespec
@@alternateType(Resource.completed_at, int32 | null);
```

Preserve wrappers required by the wire format. For example, multipart properties retain `HttpPart`:

```typespec
@@alternateType(CreateRequest.count, HttpPart<int32 | null>);
```

For collections, specify the complete alternate collection type, such as `int32[]` or `float64[]`, rather than only its element type.
