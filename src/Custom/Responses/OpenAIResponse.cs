using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public bool ParallelToolCallsEnabled { get; }

    // CUSTOM: Using convenience type.
    [CodeGenMember("ToolChoice")]
    public ResponseToolChoice ToolChoice { get; }

    public string GetOutputText()
    {
        IEnumerable<string> outputTextSegments = OutputItems.Where(item => item is MessageResponseItem)
            .Select(item => item as MessageResponseItem)
            .SelectMany(message => message.Content.Where(contentPart => contentPart.Kind == ResponseContentPartKind.OutputText)
                .Select(outputTextPart => outputTextPart.Text));
        return outputTextSegments.Any() ? string.Join(string.Empty, outputTextSegments) : null;
    }
}
