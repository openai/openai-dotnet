﻿namespace OpenAI.Responses;

// CUSTOM: This type represents a non-discriminated union of the following components:
// * A GlobalPolicy defined as an extensible enum.
// * A CustomPolicy defined as an object.
[CodeGenType("DotNetToolCallApprovalPolicy")]
public partial class McpToolCallApprovalPolicy
{
    // CUSTOM: Made internal.
    internal McpToolCallApprovalPolicy()
    {
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy globalPolicy)
    {
        GlobalPolicy = globalPolicy;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public McpToolCallApprovalPolicy(CustomMcpToolCallApprovalPolicy customPolicy)
    {
        CustomPolicy = customPolicy;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("GlobalPolicy")]
    public GlobalMcpToolCallApprovalPolicy? GlobalPolicy { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomPolicy")]
    public CustomMcpToolCallApprovalPolicy CustomPolicy { get; }
}
