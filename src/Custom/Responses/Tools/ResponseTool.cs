using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("Tool")]
public partial class ResponseTool
{
    // CUSTOM: Added factory method a a convenience.
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

    // CUSTOM: Added factory method a a convenience.
    [Experimental("OPENAICUA001")]
    public static ComputerTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth, int displayHeight)
    {
        return new ComputerTool(
            kind: InternalToolType.ComputerUsePreview,
            additionalBinaryDataProperties: null,
            environment: environment,
            displayWidth: displayWidth,
            displayHeight: displayHeight);
    }

    // CUSTOM: Added factory method a a convenience.
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

    // CUSTOM: Added factory method a a convenience.
    public static WebSearchTool CreateWebSearchTool(WebSearchToolLocation userLocation = null, WebSearchToolContextSize? searchContextSize = null)
    {
        return new WebSearchTool(
            kind: InternalToolType.WebSearchPreview,
            additionalBinaryDataProperties: null,
            userLocation: userLocation,
            searchContextSize: searchContextSize);
    }

    // CUSTOM: Added factory method a a convenience.
    public static McpTool CreateMcpTool(string serverLabel, Uri serverUri, IDictionary<string, string> headers = null, McpToolFilter allowedTools = null, McpToolCallApprovalPolicy toolCallApprovalPolicy = null)
    {
        Argument.AssertNotNull(serverLabel, nameof(serverLabel));
        Argument.AssertNotNull(serverUri, nameof(serverUri));

        return new McpTool(
            kind: InternalToolType.Mcp,
            additionalBinaryDataProperties: null,
            serverLabel: serverLabel,
            serverUri: serverUri,
            headers: headers,
            allowedTools: allowedTools,
            toolCallApprovalPolicy: toolCallApprovalPolicy);
    }

    // CUSTOM: Added factory method a a convenience.
    /// <summary>
    /// Creates a new instance of the <see cref="CodeInterpreterTool"/> class with an auto-generated container.
    /// </summary>
    /// <param name="fileIds">The file IDs to include in the container.</param>
    /// <returns></returns>
    public static CodeInterpreterTool CreateCodeInterpreterTool(IEnumerable<string> fileIds = null)
    {
        string containerJson = fileIds?.Any() == true ?
            $"{{\"type\": \"auto\", \"file_ids\": [{string.Join(", ", fileIds.Select(id => $"\"{id}\""))}]}}" :
            "{\"type\": \"auto\"}";

        return new CodeInterpreterTool(
            kind: InternalToolType.CodeInterpreter,
            additionalBinaryDataProperties: null,
            container: new BinaryData(containerJson));
    }

    // CUSTOM: Added factory method a a convenience.
    /// <summary>
    /// Creates a new instance of the <see cref="CodeInterpreterTool"/> class with a specified container ID.
    /// </summary>
    /// <param name="containerId">The Id of a previously created container</param>
    /// <returns></returns>
    public static CodeInterpreterTool CreateCodeInterpreterTool(string containerId)
    {
        Argument.AssertNotNull(containerId, nameof(containerId));

        // Since the container field can be a string or an object, when it's a string (container ID),
        // we need to serialize it as a JSON string value
        string containerJson = $"\"{containerId}\"";
        
        return new CodeInterpreterTool(
            kind: InternalToolType.CodeInterpreter,
            additionalBinaryDataProperties: null,
            container: new BinaryData(containerJson));
    }
}
