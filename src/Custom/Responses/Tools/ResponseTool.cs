using OpenAI.Assistants;
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
    public static ComputerTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth,int displayHeight)
    {
        return new ComputerTool(
            kind: InternalToolType.ComputerUsePreview,
            additionalBinaryDataProperties: null,
            environment: environment,
            displayWidth: displayWidth,
            displayHeight:displayHeight);
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

    public static ResponseTool CreateCodeInterpreterTool(string type = "auto", IDictionary<string, BinaryData> additionalBinaryDataProperties = null)
    {
        return new InternalCodeInterpreterTool(
            kind: InternalToolType.CodeInterpreter,
            additionalBinaryDataProperties: additionalBinaryDataProperties,
            container: new CodeInterpreterToolDefinition(kind: type, additionalBinaryDataProperties:null)
            );
    }

	public static ResponseTool CreateMCPServerTool(string serverLabel, string serverUrl, Dictionary<string, string> headers, List<string> allowedTools, string requireApproval, IDictionary<string, BinaryData> additionalBinaryDataProperties = null)
	{
        return new InternalMCPTool(
            kind: InternalToolType.Mcp,
            additionalBinaryDataProperties: additionalBinaryDataProperties,
            serverLabel: serverLabel,
            serverUrl: serverUrl,
            headers: headers,
            allowedTools: BinaryData.FromString("[" + string.Join(",", allowedTools.Select(t => "\"" + t + "\"").ToList()) + "]"),
            requireApproval: BinaryData.FromString("\"" + requireApproval + "\"")
        );
	}

	/* </GP> CUSTOM: Added code interpreter tool.                               *
     ****************************************************************************/
}
