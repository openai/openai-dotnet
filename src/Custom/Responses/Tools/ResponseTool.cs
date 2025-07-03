using OpenAI.Assistants;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("Tool")]
public partial class ResponseTool
{
    public static ResponseTool CreateFunctionTool(string functionName, string functionDescription, BinaryData functionParameters, bool functionSchemaIsStrict = false)
    {
        return new InternalFunctionTool(functionName, functionSchemaIsStrict, functionParameters)
        {
            Description = functionDescription
        };
    }

    [Experimental("OPENAICUA001")]
    public static ResponseTool CreateComputerTool(ComputerToolEnvironment environment, int displayWidth,int displayHeight)
    {
        return new InternalComputerUsePreviewTool(environment, displayWidth, displayHeight);
    }

    public static ResponseTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null)
    {
        return new InternalFileSearchTool(vectorStoreIds)
        {
            MaxNumResults = maxResultCount,
            RankingOptions = rankingOptions,
            Filters = filters,
        };
    }

    public static ResponseTool CreateWebSearchTool(WebSearchUserLocation userLocation = null, WebSearchContextSize? searchContextSize = null)
    {
        return new InternalWebSearchTool()
        {
            UserLocation = userLocation,
            SearchContextSize = searchContextSize,
        };
    }

	public static ResponseTool CreateCodeInterpreterTool(string type = "auto", IDictionary<string, BinaryData> additionalBinaryDataProperties = null)
	{
        return new InternalCodeInterpreterTool(
            kind: InternalToolType.CodeInterpreter,
            additionalBinaryDataProperties: additionalBinaryDataProperties,
            container: new CodeInterpreterToolDefinition(kind: type, additionalBinaryDataProperties:null)
            );
	}

	/* </GP> CUSTOM: Added code interpreter tool.                               *
	 ****************************************************************************/
}
