using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: This type represents a non-discriminated union of the following components:
// * A GlobalPolicy defined as an extensible enum.
// * A CustomPolicy defined as an object.
[CodeGenType("DotNetRealtimeToolCallApprovalPolicyGA")]
[CodeGenVisibility(nameof(RealtimeMcpToolCallApprovalPolicy), CodeGenVisibility.Internal)]
public partial class RealtimeMcpToolCallApprovalPolicy
{
    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy defaultPolicy)
    {
        DefaultPolicy = defaultPolicy;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public RealtimeMcpToolCallApprovalPolicy(RealtimeCustomMcpToolCallApprovalPolicy customPolicy)
    {
        Argument.AssertNotNull(customPolicy, nameof(customPolicy));

        CustomPolicy = customPolicy;
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
