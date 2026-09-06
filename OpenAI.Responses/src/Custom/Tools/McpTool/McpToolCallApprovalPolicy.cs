using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: This type represents a non-discriminated union of the following components:
// * A GlobalPolicy defined as an extensible enum.
// * A CustomPolicy defined as an object.
[CodeGenType("DotNetToolCallApprovalPolicy")]
[CodeGenVisibility(nameof(McpToolCallApprovalPolicy), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class McpToolCallApprovalPolicy
{
    // CUSTOM: Added to support the corresponding component of the union.
    public McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy globalPolicy)
    {
        GlobalPolicy = globalPolicy;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public McpToolCallApprovalPolicy(CustomMcpToolCallApprovalPolicy customPolicy)
    {
        Argument.AssertNotNull(customPolicy, nameof(customPolicy));

        CustomPolicy = customPolicy;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("GlobalPolicy")]
    public GlobalMcpToolCallApprovalPolicy? GlobalPolicy { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomPolicy")]
    public CustomMcpToolCallApprovalPolicy CustomPolicy { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy globalPolicy) => new(globalPolicy);

    // CUSTOM: Added for convenience.
    public static implicit operator McpToolCallApprovalPolicy(CustomMcpToolCallApprovalPolicy customPolicy) => customPolicy is null ? null : new(customPolicy);
}
