using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: This type represents a non-discriminated union of the following components:
// * A GlobalPolicy defined as an extensible enum.
// * A CustomPolicy defined as an object.
[CodeGenType("DotNetRealtimeToolCallApprovalPolicyGA")]
[CodeGenVisibility(nameof(GARealtimeMcpToolCallApprovalPolicy), CodeGenVisibility.Internal)]
public partial class GARealtimeMcpToolCallApprovalPolicy
{
    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeMcpToolCallApprovalPolicy(GARealtimeDefaultMcpToolCallApprovalPolicy defaultPolicy)
    {
        DefaultPolicy = defaultPolicy;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public GARealtimeMcpToolCallApprovalPolicy(GARealtimeCustomMcpToolCallApprovalPolicy customPolicy)
    {
        Argument.AssertNotNull(customPolicy, nameof(customPolicy));

        CustomPolicy = customPolicy;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultPolicy")]
    public GARealtimeDefaultMcpToolCallApprovalPolicy? DefaultPolicy { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomPolicy")]
    public GARealtimeCustomMcpToolCallApprovalPolicy CustomPolicy { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeMcpToolCallApprovalPolicy(GARealtimeDefaultMcpToolCallApprovalPolicy defaultPolicy) => new(defaultPolicy);

    // CUSTOM: Added for convenience.
    public static implicit operator GARealtimeMcpToolCallApprovalPolicy(GARealtimeCustomMcpToolCallApprovalPolicy customPolicy) => customPolicy is null ? null : new(customPolicy);
}
