# Non-discriminated unions

This document defines how the OpenAI .NET library represents non-discriminated unions, including the special case where one union component is shorthand for another.

## Definition

A non-discriminated union is a union whose components do not share a discriminator. For example, the `require_approval` property of an MCP tool in the Responses API accepts either an object or one of two string values:

```typespec
require_approval?:
  | {
      always?: MCPToolFilter;
      `never`?: MCPToolFilter;
    }
  | "always"
  | "never"
  | null;
```

The object contains granular approval policies based on an MCP tool filter:

```typespec
model MCPToolFilter {
  tool_names?: string[];
  read_only?: boolean;
}
```

Apply these interpretations before modeling the union in .NET:

- When a union contains `null`, model the containing property as nullable. Do not treat `null` as a component represented by the union type. In other words, only consider this to be a proper union if it includes two or more non-`null` components.
- When a union contains one or more string literals, such as `"always"` and `"never"`, model them as an extensible enum.

After applying these rules, `require_approval` translates to a non-discriminated union between an object and an extensible enum.

## Pattern 1: Represent distinct components through composition

When the components represent distinct concepts, define a public type for each component. For `require_approval`, the object component represents a custom policy:

```csharp
public partial class CustomMcpToolCallApprovalPolicy
{
    public CustomMcpToolCallApprovalPolicy();

    public McpToolFilter ToolsAlwaysRequiringApproval { get; set; }
    public McpToolFilter ToolsNeverRequiringApproval { get; set; }
}
```

The string-literal component is represented by an extensible enum:

```csharp
public readonly partial struct GlobalMcpToolCallApprovalPolicy : IEquatable<GlobalMcpToolCallApprovalPolicy>
{
    public GlobalMcpToolCallApprovalPolicy(string value);

    public static GlobalMcpToolCallApprovalPolicy AlwaysRequireApproval { get; }
    public static GlobalMcpToolCallApprovalPolicy NeverRequireApproval { get; }

    public static implicit operator GlobalMcpToolCallApprovalPolicy(string value);
    public override string ToString();
}
```

Then, define a class that composes the component types and represents the union:

```csharp
public partial class McpToolCallApprovalPolicy
{
  public McpToolCallApprovalPolicy(CustomMcpToolCallApprovalPolicy customPolicy)
  {
    Argument.AssertNotNull(customPolicy, nameof(customPolicy));

    CustomPolicy = customPolicy;
  }

  public McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy globalPolicy)
  {
    GlobalPolicy = globalPolicy;
  }

    public CustomMcpToolCallApprovalPolicy CustomPolicy { get; }
    public GlobalMcpToolCallApprovalPolicy? GlobalPolicy { get; }

    public static implicit operator McpToolCallApprovalPolicy(CustomMcpToolCallApprovalPolicy customPolicy) => customPolicy is null ? null : new(customPolicy);
    public static implicit operator McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy globalPolicy) => new(globalPolicy);
}
```

The union class must follow these rules:

1. Expose one property for each component of the union.
2. Provide one constructor for each component. Each constructor accepts only its corresponding component and rejects `null` for reference-type arguments.
3. Populate exactly one component property for every instance. All other component properties must be `null`.
4. Keep component properties read-only so an instance cannot be mutated from one component into another.
5. Make every component property nullable. In projects without nullable reference type annotations, reference-type properties are nullable by behavior; value-type properties use `?` explicitly.
6. Keep collection component properties nullable as an exception to the library's usual collection-property convention. A `null` collection is necessary to indicate that another component is active.
7. Provide implicit conversions from each component type to the union type. When a component parameter can be `null`, the conversion must return `null` rather than call the constructor. This keeps implicit conversions from throwing when constructors reject `null`. Components that cannot be `null`, such as non-nullable value types, do not need this check.

See the implementation of `McpToolCallApprovalPolicy` and the associated tests for a reference implementation of this pattern.

### Usage

Callers determine the active component by checking the properties for `null`:

```csharp
if (policy.CustomPolicy is not null)
{
    // ...
}
else if (policy.GlobalPolicy is not null)
{
    // ...
}
```

Implicit conversions allow callers to assign a component without explicitly constructing the wrapper:

```csharp
McpToolCallApprovalPolicy policy = GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval;
```

### Serialization requirements

The union class is a .NET representation of multiple possible JSON values; it is not an additional JSON object. Its custom serialization must directly write the active component:

- Write an extensible enum component as its JSON string value.
- Write an object component as a JSON object.
- Do not emit the union's .NET property names into the payload.
- Dispatch deserialization according to the JSON value shape and populate only the matching component property.
- Handle JSON `null` as nullability of the containing property rather than as an initialized union instance.
- Preserve unknown properties associated with an object component according to the library's normal model round-trip behavior.

#### `JsonPatch` implementation

A union wrapper is a JSON value, not a JSON object with properties corresponding to its .NET component properties. Callers therefore patch the property on the containing model, not the wrapper directly. The wrapper still needs an internal `JsonPatch` to receive those delegated paths, preserve a replacement for the complete union value, and forward nested paths to an active object component.

The union wrapper's patch support must follow these rules:

1. Make the generated `Patch` property internal by applying `[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]` to the customization class. Do not expose synthetic component paths such as `$.global_policy` or `$.custom_policy` to callers.
2. Implement the patch propagators `PropagateSet` and `PropagateGet` to reflect the union's wire representation rather than the synthetic TypeSpec model.
3. Initialize the propagators along all valid construction paths. Call `_patch.SetPropagators(PropagateSet, PropagateGet)` in each component constructor because those constructors initialize their components directly.
4. Keep a patch for `$` on the wrapper by returning `false` from both propagators for that path. Before normal serialization, write the raw root patch when `Patch.Contains("$"u8)` is true. This supports replacing the complete union value through a containing model, such as patching `$.require_approval` from an object to a string.
5. For any non-root path, forward the path unchanged to the active object component's `Patch`. The object component owns its properties, nested model propagation, and additional-property round-tripping. For example, forward `$.always.tool_names` to the custom policy as `$.always.tool_names`, not through `$.custom_policy`.
6. Return `false` for non-root paths when the active component is a scalar. A scalar has no nested properties to patch; callers can replace the complete union value through the containing model's property path instead.
7. During normal serialization, select and write the active component without consulting patch paths for the wrapper's synthetic .NET component properties. Additional properties for an object component are emitted by that component's serializer.
8. During deserialization, create the wrapper's `JsonPatch` from the source `data` and pass it to the generated constructor together with the active component. Deserialize an object component with its own source data so its `Patch` independently preserves unknown object properties.

### Test requirements

Tests for a conventional non-discriminated union must cover:

- Constructing and serializing every component.
- Deserializing every supported JSON shape and inspecting its component property.
- Round-tripping every supported JSON shape.
- Rejecting unsupported JSON shapes.
- Preserving additional properties when the object model supports them.
- Confirming that only one component property is populated.
- Confirming that implicitly converting a nullable component with a `null` value produces a `null` union.
- Patching a nested path through a containing model and confirming that it reaches the active object component.
- Patching the complete union-valued property on a containing model and confirming that the raw replacement value is serialized.

## Pattern 2: Normalize shorthand to longhand

Some non-discriminated unions use one component only as shorthand for another. For example, the `allowed_tools` property of the MCP tool of the Responses API accepts either a list of tool names or a complete filter:

```typespec
allowed_tools?: string[] | MCPToolFilter | null;
```

The `string[]` component is shorthand for setting the `tool_names` property of `MCPToolFilter`:

```typespec
model MCPToolFilter {
  tool_names?: string[];
  read_only?: boolean;
}
```

The shorthand is a logical subset of the longhand representation. These components do not represent distinct concepts, so exposing both through a union wrapper as described in the pattern above would add another type and two properties to represent the same single concept. It would also make callers decide between forms that are functionally equivalent.

When one component is strictly shorthand for another, expose only the longhand type in the public .NET API:

```csharp
public partial class McpTool
{
    public McpToolFilter AllowedTools { get; set; }
}
```

### Serialization requirements

Customize deserialization for the property so that it accepts both wire representations:

- Deserialize the longhand object normally.
- Convert the shorthand value into the equivalent longhand object.
- Store only the longhand object in the public property.
- Continue to reject JSON shapes that are not members of the service-defined union.

For `allowed_tools` in the example above, an incoming array is normalized into an `McpToolFilter` whose `ToolNames` collection contains the array values. Serialization then uses the longhand object form:

```json
{
  "allowed_tools": {
    "tool_names": ["search"]
  }
}
```

### Test requirements

Tests for shorthand normalization must cover:

- Deserializing shorthand and longhand inputs.
- Producing equivalent public model state from both forms.
- Serializing normalized shorthand as longhand.
- Preserving longhand-only fields when the longhand form is received.
- Handling `null` and rejecting unsupported JSON shapes.

### Limitations of shorthand normalization

Normalization deliberately does not preserve the original wire representation:

1. If the service sends shorthand and the caller sends the model back, the library emits longhand. A service operation that requires the exact shorthand representation might not accept that result.
2. If a caller must send shorthand specifically, they must use `JsonPatch` to control the wire payload.

These limitations are preferable to adding a public wrapper when shorthand and longhand represent the same concept. If preserving the original representation becomes a functional requirement, reevaluate whether the alternatives should instead use the composition pattern.

## Choosing the pattern

Use the following decision process for each non-discriminated union:

1. Remove `null` from consideration as a union component and model it as property nullability.
2. Group string literals into a single extensible enum component.
3. Decide whether the remaining components represent distinct concepts.
4. If they are distinct, use a compositional union class with one nullable, read-only property and one constructor per component.
5. If one component is only shorthand for another, expose the longhand type and normalize shorthand during deserialization.
6. Add serialization tests for every accepted JSON shape and for the expected round-trip behavior.

When TypeSpec files are changed while implementing either pattern, regenerate the code by running:

```powershell
./scripts/Invoke-CodeGen.ps1
```