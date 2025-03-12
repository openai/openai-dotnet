using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesTool")]
public partial class ResponseTool
{
    // CUSTOM: Made internal.
    [CodeGenMember("Type")]
    internal InternalResponsesToolType Type { get; }

    //// CUSTOM: Exposed function tool properties.
    //public string FunctionName => (this as InternalResponsesFunctionTool)?.Name;
    //public string FunctionDescription => (this as InternalResponsesFunctionTool)?.Description;
    //public BinaryData FunctionParameters => (this as InternalResponsesFunctionTool)?.Parameters;
    //public bool? FunctionSchemaIsStrict => (this as InternalResponsesFunctionTool)?.Strict;

    //// CUSTOM: Exposed computer tool properties.
    //public int? ComputerDisplayWidth => (this as InternalResponsesComputerTool)?.DisplayWidth;
    //public int? ComputerDisplayHeight => (this as InternalResponsesComputerTool)?.DisplayHeight;
    //public ComputerToolEnvironment? ComputerEnvironment => (this as InternalResponsesComputerTool)?.Environment;

    //// CUSTOM: Exposed file search tool properties.
    //public IList<string> FileSearchVectorStoreIds => (this as InternalResponsesFileSearchTool)?.VectorStoreIds;
    //public int? FileSearchMaxResultCount => (this as InternalResponsesFileSearchTool)?.MaxNumResults;

    //// CUSTOM: Exposed web search tool properties.
    //public IList<string> WebSearchDomains => (this as InternalResponsesWebSearchTool)?.Domains;
    //public WebSearchToolUserLocation WebSearchUserLocation => (this as InternalResponsesWebSearchTool)?.UserLocation;

    //// CUSTOM: Exposed code interpreter tool properties.
    //// TODO

    public static ResponseTool CreateFunctionTool(string functionName, string functionDescription, BinaryData functionParameters, bool functionSchemaIsStrict = false)
    {
        return new InternalResponsesFunctionTool(
            type: InternalResponsesToolType.Function,
            additionalBinaryDataProperties: null,
            functionName,
            functionDescription,
            functionParameters,
            functionSchemaIsStrict);
    }

    [Experimental("OPENAICUA001")]
    public static ResponseTool CreateComputerTool(int displayWidth,int displayHeight, ComputerToolEnvironment environment)
    {
        return new InternalResponsesComputerTool(
            type: InternalResponsesToolType.Computer,
            additionalBinaryDataProperties: null,
            displayWidth,
            displayHeight,
            environment);
    }

    public static ResponseTool CreateFileSearchTool(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, FileSearchToolRankingOptions rankingOptions = null, BinaryData filters = null)
    {
        return new InternalResponsesFileSearchTool(
            type: InternalResponsesToolType.FileSearch,
            additionalBinaryDataProperties: null,
            vectorStoreIds.ToList(),
            maxResultCount,
            rankingOptions,
            filters);
    }

    public static ResponseTool CreateWebSearchTool(WebSearchToolLocation webSearchToolUserLocation = null, WebSearchToolContextSize? webSearchToolContextSize = null)
    {
        return new InternalResponsesWebSearchTool(
            type: InternalResponsesToolType.WebSearch,
            additionalBinaryDataProperties: null,
            webSearchToolUserLocation,
            webSearchToolContextSize);
    }
}
