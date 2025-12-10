using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CreateResponse")]
public partial class CreateResponseOptions
{
    // CUSTOM: Added as a convenience.
    public CreateResponseOptions(string model, IEnumerable<ResponseItem> inputItems) : this()
    {
        Argument.AssertNotNull(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        Model = model;
        InputItems = inputItems.ToList();
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets whether to run the response in background mode. This corresponds to the "background" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Background")]
    public bool? BackgroundModeEnabled { get; set; }

    // CUSTOM: Changed type.
    /// <summary>
    /// Gets or sets how tool calls should be selected during response generation. This corresponds to the "tool_choice" property in the JSON representation.
    /// </summary>
    [CodeGenMember("ToolChoice")]
    public ResponseToolChoice ToolChoice { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the input items to be processed for the response. This corresponds to the "input" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Input")]
    public IList<ResponseItem> InputItems { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the list of fields to include in the response. This corresponds to the "include" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Include")]
    public IList<IncludedResponseProperty> IncludedProperties { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets whether multiple tool calls can be made in parallel. This corresponds to the "parallel_tool_calls" property in the JSON representation.
    /// </summary>
    [CodeGenMember("ParallelToolCalls")]
    public bool? ParallelToolCallsEnabled { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets whether the response should be stored for later retrieval. This corresponds to the "store" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Store")]
    public bool? StoredOutputEnabled { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets whether the response should be streamed. This corresponds to the "stream" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Stream")]
    public bool? StreamingEnabled { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the maximum number of output tokens to generate. This corresponds to the "max_output_tokens" property in the JSON representation.
    /// </summary>
    [CodeGenMember("MaxOutputTokens")]
    public int? MaxOutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the maximum number of tool calls allowed during response generation. This corresponds to the "max_tool_calls" property in the JSON representation.
    /// </summary>
    [CodeGenMember("MaxToolCalls")]
    public int? MaxToolCallCount { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the reasoning options for the response. This corresponds to the "reasoning" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Reasoning")]
    public ResponseReasoningOptions ReasoningOptions { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the text format options for the response. This corresponds to the "text" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Text")]
    public ResponseTextOptions TextOptions { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the conversation options for the response. This corresponds to the "conversation" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Conversation")]
    public ResponseConversationOptions ConversationOptions { get; set; }

    /// <summary>
    /// An integer between 0 and 20 specifying the number of most likely tokens to return at each token position, each with an associated log probability.
    /// This corresponds to the "top_logprobs" property in the JSON representation.
    /// </summary>
    [CodeGenMember("TopLogprobs")]
    public int? TopLogProbabilityCount { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the truncation mode for the response. This corresponds to the "truncation" property in the JSON representation.
    /// </summary>
    [CodeGenMember("Truncation")]
    public ResponseTruncationMode? TruncationMode { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the end user identifier. This corresponds to the "user" property in the JSON representation.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    internal CreateResponseOptions GetClone()
    {
        CreateResponseOptions copiedOptions = (CreateResponseOptions)this.MemberwiseClone();
        copiedOptions.Patch = _patch;
        return copiedOptions;
    }
}
