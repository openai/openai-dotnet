# Numeric types

This document defines how the OpenAI .NET library maps TypeSpec's generic `integer` and `numeric` scalar types to C# numeric types.

## Default mappings

The generator uses these mappings unless the client TypeSpec overrides them:

| TypeSpec type | C# type |
|---------------|---------|
| `integer` | `long` |
| `numeric` | `double` |

The generic TypeSpec types do not communicate the range or precision required by the API. Choose an explicit type for the .NET API whenever that requirement is known:

| TypeSpec alternate type | C# type |
|-------------------------|---------|
| `int32` | `int` |
| `int64` | `long` |
| `float32` | `float` |
| `float64` | `double` |

Do not narrow a type based only on its current examples or observed values. Preserve `int64` or `float64` when the API contract requires the larger range or precision.

## Apply the mapping in client TypeSpec

To override generic TypeSpec types, add `@@alternateType` customizations to the area's `specification/client/{area}.client.tsp` file.

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

```typespec
@@alternateType(CreateTranscriptionResponseJsonLogprobs.bytes, float64[]);
```
