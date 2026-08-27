using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM:
// - Renamed.
// - Suppressed the constructor that only takes the server label as a parameter. This is because,
//   even though the server URI and the connector ID are specified as optional, in practice, one of
//   the two must be provided.
[CodeGenType("MCPToolGA")]
[CodeGenSuppress(nameof(RealtimeMcpTool), typeof(string))]
public partial class RealtimeMcpTool
{
    // CUSTOM: Added a constructor that takes the server URI in addition to the server label.
    public RealtimeMcpTool(string serverLabel, Uri serverUri) : base(InternalRealtimeToolBaseTypeGA.Mcp)
    {
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));
        Argument.AssertNotNull(serverUri, nameof(serverUri));

        ServerLabel = serverLabel;
        ServerUri = serverUri;
        Headers = new ChangeTrackingDictionary<string, string>();
    }

    // CUSTOM: Added a constructor that takes the connector ID in addition to the server label.
    public RealtimeMcpTool(string serverLabel, RealtimeMcpToolConnectorId connectorId) : base(InternalRealtimeToolBaseTypeGA.Mcp)
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
    public RealtimeMcpToolCallApprovalPolicy ToolCallApprovalPolicy { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("ServerUrl")]
    public Uri ServerUri { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Authorization")]
    public string AuthorizationToken { get; set; }
}