using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed client-only OutputText property in favor of a method.
[CodeGenType("Response")]
[CodeGenSuppress("OutputText")]
public partial class ResponseResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Background")]
    public bool? BackgroundModeEnabled { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Reasoning")]
    public ResponseReasoningOptions ReasoningOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxOutputTokens")]
    public int? MaxOutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxToolCalls")]
    public int? MaxToolCallCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Text")]
    public ResponseTextOptions TextOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Truncation")]
    public ResponseTruncationMode? TruncationMode { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("IncompleteDetails")]
    public ResponseIncompleteStatusDetails IncompleteStatusDetails { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public IList<ResponseItem> OutputItems { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("ParallelToolCalls")]
    public bool ParallelToolCallsEnabled { get; set; }

    // CUSTOM: Changed type.
    [CodeGenMember("ToolChoice")]
    public ResponseToolChoice ToolChoice { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("TopLogprobs")]
    public int? TopLogProbabilityCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Conversation")]
    public ResponseConversationOptions ConversationOptions { get; set; }

    // CUSTOM: Applied EditorBrowsableState.Never.
    [CodeGenMember("Object")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "response";

    public string GetOutputText()
    {
        IEnumerable<string> outputTextSegments = OutputItems.Where(item => item is MessageResponseItem)
            .Select(item => item as MessageResponseItem)
            .SelectMany(message => message.Content.Where(contentPart => contentPart.Kind == ResponseContentPartKind.OutputText)
                .Select(outputTextPart => outputTextPart.Text));
        return outputTextSegments.Any() ? string.Concat(outputTextSegments) : null;
    }
}
