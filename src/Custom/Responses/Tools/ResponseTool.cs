using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("Tool")]
public partial class ResponseTool
{
    // CUSTOM: Added factory method as a convenience.
    public static FunctionTool CreateFunctionTool(string functionName, BinaryData functionParameters, bool? strictModeEnabled, string functionDescription = null)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        return new FunctionTool(
            kind: InternalToolType.Function,
            additionalBinaryDataProperties: null,
            functionName: functionName,
            functionDescription: functionDescription,
            functionParameters: functionParameters,
            strictModeEnabled: strictModeEnabled);
    }

    // CUSTOM: Added factory method as a convenience.
    [Experimental("OPENAICUA001")]
    public static ComputerTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth,int displayHeight)
    {
        return new ComputerTool(
            kind: InternalToolType.ComputerUsePreview,
            additionalBinaryDataProperties: null,
            environment: environment,
            displayWidth: displayWidth,
            displayHeight:displayHeight);
    }

    // CUSTOM: Added factory method as a convenience.
    public static FileSearchTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null)
    {
        Argument.AssertNotNull(vectorStoreIds, nameof(vectorStoreIds));

        return new FileSearchTool(
            kind: InternalToolType.FileSearch,
            additionalBinaryDataProperties: null,
            vectorStoreIds: vectorStoreIds.ToList(),
            maxResultCount: maxResultCount,
            rankingOptions: rankingOptions,
            filters: filters);
    }

    // CUSTOM: Added factory method as a convenience.
    public static WebSearchTool CreateWebSearchTool(WebSearchToolLocation userLocation = null, WebSearchToolContextSize? searchContextSize = null)
    {
        return new WebSearchTool(
            kind: InternalToolType.WebSearchPreview,
            additionalBinaryDataProperties: null,
            userLocation: userLocation,
            searchContextSize: searchContextSize);
    }

    // CUSTOM: Added factory method as a convenience.
    public static McpTool CreateMcpTool(string serverLabel, Uri serverUri, string authorizationToken = null, string serverDescription = null, IDictionary<string, string> headers = null, McpToolFilter allowedTools = null, McpToolCallApprovalPolicy toolCallApprovalPolicy = null)
    {
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));
        Argument.AssertNotNull(serverUri, nameof(serverUri));

        return new McpTool(
            kind: InternalToolType.Mcp,
            additionalBinaryDataProperties: null,
            serverLabel: serverLabel,
            serverUri: serverUri,
            connectorId: null,
            authorizationToken: authorizationToken,
            serverDescription: serverDescription,
            headers: headers,
            allowedTools: allowedTools,
            toolCallApprovalPolicy: toolCallApprovalPolicy);
    }

    // CUSTOM: Added factory method as a convenience.
    public static McpTool CreateMcpTool(string serverLabel, McpToolConnectorId connectorId, string authorizationToken = null, string serverDescription = null, IDictionary<string, string> headers = null, McpToolFilter allowedTools = null, McpToolCallApprovalPolicy toolCallApprovalPolicy = null)
    {
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));

        return new McpTool(
            kind: InternalToolType.Mcp,
            additionalBinaryDataProperties: null,
            serverLabel: serverLabel,
            serverUri: null,
            connectorId: connectorId,
            authorizationToken: authorizationToken,
            serverDescription: serverDescription,
            headers: headers,
            allowedTools: allowedTools,
            toolCallApprovalPolicy: toolCallApprovalPolicy);
    }
}
