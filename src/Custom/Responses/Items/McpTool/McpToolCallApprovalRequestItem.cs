using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Made internal the constructor that does not take the item ID as a parameter. This is because MCP
//   approval requests are correlated to MCP approval responses using this ID. Therefore, it
//   implies that the ID of MCP approval requests is required. Note that this is contrary to other
//   similar cases, such as function call items, which are instead correlated to function call
//   outputs using a dedicated "call ID".
[CodeGenType("MCPApprovalRequestItemResource")]
[CodeGenVisibility(nameof(McpToolCallApprovalRequestItem), CodeGenVisibility.Internal, typeof(string), typeof(string), typeof(BinaryData))]
public partial class McpToolCallApprovalRequestItem
{
    // CUSTOM: Added a constructor that takes the item ID.
    public McpToolCallApprovalRequestItem(string id, string serverLabel, string toolName, BinaryData toolArguments) : base(InternalItemType.McpApprovalRequest)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));
        Argument.AssertNotNull(toolName, nameof(toolName));
        Argument.AssertNotNull(toolArguments, nameof(toolArguments));

        Id = id;
        ServerLabel = serverLabel;
        ToolName = toolName;
        ToolArguments = toolArguments;
    }
}
