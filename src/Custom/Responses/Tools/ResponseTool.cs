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
            patch: default,
            functionName: functionName,
            functionDescription: functionDescription,
            functionParameters: functionParameters,
            strictModeEnabled: strictModeEnabled);
    }

    // CUSTOM: Added factory method as a convenience.
    [Experimental("OPENAICUA001")]
    public static ComputerTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth, int displayHeight)
    {
        return new ComputerTool(
            kind: InternalToolType.ComputerUsePreview,
            patch: default,
            environment: environment,
            displayWidth: displayWidth,
            displayHeight: displayHeight);
    }

    // CUSTOM: Added factory method as a convenience.
    public static FileSearchTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null)
    {
        Argument.AssertNotNull(vectorStoreIds, nameof(vectorStoreIds));

        return new FileSearchTool(
            kind: InternalToolType.FileSearch,
            patch: default,
            vectorStoreIds: vectorStoreIds.ToList(),
            maxResultCount: maxResultCount,
            rankingOptions: rankingOptions,
            filters: filters);
    }

    // CUSTOM: Added factory method as a convenience.
    public static WebSearchTool CreateWebSearchTool(WebSearchToolLocation userLocation = null, WebSearchToolContextSize? searchContextSize = null, WebSearchToolFilters filters = null)
    {
        return new WebSearchTool(
            kind: InternalToolType.WebSearch,
            patch: default,
            userLocation: userLocation,
            searchContextSize: searchContextSize,
            filters: filters);
    }

    // CUSTOM: Added factory method as a convenience.
    public static WebSearchPreviewTool CreateWebSearchPreviewTool(WebSearchToolLocation userLocation = null, WebSearchToolContextSize? searchContextSize = null)
    {
        return new WebSearchPreviewTool(
            kind: InternalToolType.WebSearchPreview,
            patch: default,
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
            patch: default,
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
            patch: default,
            serverLabel: serverLabel,
            serverUri: null,
            connectorId: connectorId,
            authorizationToken: authorizationToken,
            serverDescription: serverDescription,
            headers: headers,
            allowedTools: allowedTools,
            toolCallApprovalPolicy: toolCallApprovalPolicy);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CodeInterpreterTool"/> class.
    /// </summary>
    /// <param name="container">The container for the code interpreter.</param>
    /// <returns>A new instance of the <see cref="CodeInterpreterTool"/> class.</returns>
    public static CodeInterpreterTool CreateCodeInterpreterTool(CodeInterpreterToolContainer container)
    {
        Argument.AssertNotNull(container, nameof(container));

        return new CodeInterpreterTool(
            kind: InternalToolType.CodeInterpreter,
            patch: default,
            container: container);
    }

    // CUSTOM: Added factory method for a convenience.
    /// <summary>
    /// Creates a new instance of the <see cref="ImageGenerationTool"/> class.
    /// </summary>
    public static ImageGenerationTool CreateImageGenerationTool(string model, ImageGenerationToolQuality? quality = null, ImageGenerationToolSize? size = null, ImageGenerationToolOutputFileFormat? outputFileFormat = null, int? outputCompressionFactor = null, ImageGenerationToolModerationLevel? moderationLevel = null, ImageGenerationToolBackground? background = null, ImageGenerationToolInputFidelity? inputFidelity = null, ImageGenerationToolInputImageMask inputImageMask = null, int? partialImageCount = null)
    {
        return new ImageGenerationTool(
            kind: InternalToolType.ImageGeneration,
            patch: default,
            model: model,
            quality: quality,
            size: size,
            outputFileFormat: outputFileFormat,
            outputCompressionFactor: outputCompressionFactor,
            moderationLevel: moderationLevel,
            background: background,
            inputFidelity: inputFidelity,
            inputImageMask: inputImageMask,
            partialImageCount: partialImageCount);
    }
}
