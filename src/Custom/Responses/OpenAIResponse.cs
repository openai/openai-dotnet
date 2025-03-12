using System.Collections.Generic;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesResponse")]
public partial class OpenAIResponse
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    internal InternalCreateResponsesResponseObject Object { get; } = "response";

    // CUSTOM: Renamed.
    [CodeGenMember("User")]
    public string EndUserId { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Reasoning")]
    public ResponseReasoningOptions ReasoningOptions { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxOutputTokens")]
    public int? MaxOutputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Text")]
    public ResponseTextOptions TextOptions { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Truncation")]
    public ResponseTruncationMode? TruncationMode { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("IncompleteDetails")]
    public ResponseIncompleteStatusDetails IncompleteStatusDetails { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public IList<ResponseItem> OutputItems { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ParallelToolCalls")]
    public bool AllowParallelToolCalls { get; }
}
