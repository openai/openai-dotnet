using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Response")]
[CodeGenSuppress("OutputText")]
public partial class OpenAIResponse
{
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

    // CUSTOM: Use a plain string.
    [CodeGenMember("Model")]
    public string Model { get; }

    // CUSTOM: Made internal
    internal string Object { get; } = "response";

    public string GetOutputText()
    {
        IEnumerable<string> outputTextSegments = OutputItems.Where(item => item is MessageResponseItem)
            .Select(item => item as MessageResponseItem)
            .SelectMany(message => message.Content.Where(contentPart => contentPart.Kind == ResponseContentPartKind.OutputText)
                .Select(outputTextPart => outputTextPart.Text));
        return outputTextSegments.Any() ? string.Join(string.Empty, outputTextSegments) : null;
    }

    internal static OpenAIResponse FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeOpenAIResponse(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
