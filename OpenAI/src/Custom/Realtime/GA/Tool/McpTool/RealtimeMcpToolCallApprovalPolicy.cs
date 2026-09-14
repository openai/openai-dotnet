using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetRealtimeToolCallApprovalPolicyGA")]
[CodeGenVisibility(nameof(RealtimeMcpToolCallApprovalPolicy), CodeGenVisibility.Internal)]
[CodeGenVisibility("Patch", CodeGenVisibility.Internal)]
public partial class RealtimeMcpToolCallApprovalPolicy
{
    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy defaultPolicy)
    {
        DefaultPolicy = defaultPolicy;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeMcpToolCallApprovalPolicy(RealtimeCustomMcpToolCallApprovalPolicy customPolicy)
    {
        Argument.AssertNotNull(customPolicy, nameof(customPolicy));

        CustomPolicy = customPolicy;
    #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        _patch.SetPropagators(PropagateSet, PropagateGet);
    #pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultPolicy")]
    public RealtimeDefaultMcpToolCallApprovalPolicy? DefaultPolicy { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomPolicy")]
    public RealtimeCustomMcpToolCallApprovalPolicy CustomPolicy { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy defaultPolicy) => new(defaultPolicy);

    // CUSTOM: Added for convenience.
    public static implicit operator RealtimeMcpToolCallApprovalPolicy(RealtimeCustomMcpToolCallApprovalPolicy customPolicy) => customPolicy is null ? null : new(customPolicy);
}
