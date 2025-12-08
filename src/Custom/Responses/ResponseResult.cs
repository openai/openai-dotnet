using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
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

    // CUSTOM: Using convenience type.
    [CodeGenMember("ToolChoice")]
    public ResponseToolChoice ToolChoice { get; set; }

    // CUSTOM: Use a plain string.
    [CodeGenMember("Model")]
    public string Model { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("TopLogprobs")]
    public int? TopLogProbabilityCount { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "response";

    [CodeGenMember("Temperature")]
    public float? Temperature { get; set; }

    [CodeGenMember("TopP")]
    public float? TopP { get; set; }

    [CodeGenMember("SafetyIdentifier")]
    public string SafetyIdentifier { get; set; }

    [CodeGenMember("ServiceTier")]
    public ResponseServiceTier? ServiceTier { get; set; }

    [CodeGenMember("PreviousResponseId")]
    public string PreviousResponseId { get; set; }

    [CodeGenMember("Instructions")]
    public string Instructions { get; set; }

    [CodeGenMember("Id")]
    public string Id { get; set; }

    [CodeGenMember("Status")]
    public ResponseStatus? Status { get; set; }

    [CodeGenMember("CreatedAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [CodeGenMember("Error")]
    public ResponseError Error { get; set; }

    [CodeGenMember("Usage")]
    public ResponseTokenUsage Usage { get; set; }

    [CodeGenMember("Conversation")]
    public ResponseConversationOptions ConversationOptions { get; set; }

    public string GetOutputText()
    {
        IEnumerable<string> outputTextSegments = OutputItems.Where(item => item is MessageResponseItem)
            .Select(item => item as MessageResponseItem)
            .SelectMany(message => message.Content.Where(contentPart => contentPart.Kind == ResponseContentPartKind.OutputText)
                .Select(outputTextPart => outputTextPart.Text));
        return outputTextSegments.Any() ? string.Concat(outputTextSegments) : null;
    }

    public static explicit operator ResponseResult(BinaryData data)
    {
        using JsonDocument document = JsonDocument.Parse(data);
        return DeserializeResponseResult(document.RootElement, data, ModelSerializationExtensions.WireOptions);
    }
}
