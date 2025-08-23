namespace OpenAI.Responses;

// CUSTOM: This type represents a non-discriminated union of the following components:
// * A GlobalPolicy defined as an extensible enum.
// * A CustomPolicy defined as an object.
[CodeGenType("DotNetToolCallApprovalPolicy")]
public partial class MCPToolCallApprovalPolicy
{
    // CUSTOM: Made internal.
    internal MCPToolCallApprovalPolicy()
    {
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public MCPToolCallApprovalPolicy(GlobalMCPToolCallApprovalPolicy globalPolicy)
    {
        GlobalPolicy = globalPolicy;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public MCPToolCallApprovalPolicy(CustomMCPToolCallApprovalPolicy customPolicy)
    {
        CustomPolicy = customPolicy;
    }

    // CUSTOM: Removed setter.
    public GlobalMCPToolCallApprovalPolicy? GlobalPolicy { get; }

    // CUSTOM: Removed setter.
    public CustomMCPToolCallApprovalPolicy CustomPolicy { get; }
}
