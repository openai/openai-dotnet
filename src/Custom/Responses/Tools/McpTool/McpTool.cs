using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed the constructor that only takes the server label as a parameter. This is because,
//   even though the server URI and the connector ID are specified as optional, in practice, one of
//   the two must be provided.
[CodeGenType("MCPTool")]
[CodeGenSuppress("McpTool", typeof(string))]
public partial class McpTool
{
    // CUSTOM: Added a constructor that takes the server URI in addition to the server label.
    public McpTool(string serverLabel, Uri serverUri) : base(InternalToolType.Mcp)
    {
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));
        Argument.AssertNotNull(serverUri, nameof(serverUri));

        ServerLabel = serverLabel;
        ServerUri = serverUri;
        Headers = new ChangeTrackingDictionary<string, string>();
    }

    // CUSTOM: Added a constructor that takes the connector ID in addition to the server label.
    public McpTool(string serverLabel, McpToolConnectorId connectorId) : base(InternalToolType.Mcp)
    {
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));

        ServerLabel = serverLabel;
        ConnectorId = connectorId;
        Headers = new ChangeTrackingDictionary<string, string>();
    }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("RequireApproval")]
    public McpToolCallApprovalPolicy ToolCallApprovalPolicy { get; set; }
}