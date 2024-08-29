namespace OpenAI.Chat;

using System;
using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// Represents an incremental item of new data in a streaming response to a chat completion request.
/// </summary>
[CodeGenModel("CreateChatCompletionStreamResponse")]
public partial class StreamingChatCompletionUpdate
{
    private IReadOnlyList<ChatMessageContentPart> _contentUpdate;
    private IReadOnlyList<StreamingChatToolCallUpdate> _toolCallUpdates;
    private IReadOnlyList<ChatTokenLogProbabilityInfo> _contentTokenLogProbabilities;
    private IReadOnlyList<ChatTokenLogProbabilityInfo> _refusalTokenLogProbabilities;

    // CUSTOM:
    // - Made private. This property does not add value in the context of a strongly-typed class.
    // - Changed type from string.
    /// <summary> The object type, which is always `chat.completion.chunk`. </summary>
    [CodeGenMember("Object")]
    internal InternalCreateChatCompletionStreamResponseObject Object { get; } = InternalCreateChatCompletionStreamResponseObject.ChatCompletionChunk;

    // CUSTOM: Made internal.We only get back a single choice, and instead we flatten the structure for usability.
    /// <summary>
    /// A list of chat completion choices. Can contain more than one elements if `n` is greater than 1. Can also be empty for the
    /// last chunk if you set `stream_options: {"include_usage": true}`.
    /// </summary>
    [CodeGenMember("Choices")]
    internal IReadOnlyList<InternalCreateChatCompletionStreamResponseChoice> Choices { get; }

    // CUSTOM: Renamed.
    /// <summary> The Unix timestamp (in seconds) of when the chat completion was created. Each chunk has the same timestamp. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }

    // CUSTOM: Changed type from InternalCreateChatCompletionStreamResponseUsage.
    /// <summary>
    /// An optional field that will only be present when you set `stream_options: {"include_usage": true}` in your request.
    /// When present, it contains a null value except for the last chunk which contains the token usage statistics for the entire request.
    /// </summary>
    [CodeGenMember("Usage")]
    public ChatTokenUsage Usage { get; }

    // CUSTOM: Made internal.
    [CodeGenMember("ServiceTier")]
    internal InternalCreateChatCompletionStreamResponseServiceTier? ServiceTier { get; }

    // CUSTOM: Flattened choice property.
    /// <summary>
    /// Gets the <see cref="ChatFinishReason"/> associated with this update.
    /// </summary>
    public ChatFinishReason? FinishReason => (Choices.Count > 0)
        ? Choices[0].FinishReason
        : null;

    // CUSTOM: Flattened choice logprobs property.
    /// <summary>
    /// Log probability information.
    /// </summary>
    public IReadOnlyList<ChatTokenLogProbabilityInfo> ContentTokenLogProbabilities => (Choices.Count > 0 && Choices[0].Logprobs != null)
        ? Choices[0].Logprobs.Content
        : _contentTokenLogProbabilities ??= new ChangeTrackingList<ChatTokenLogProbabilityInfo>();

    // CUSTOM: Flattened refusal logprobs property.
    public IReadOnlyList<ChatTokenLogProbabilityInfo> RefusalTokenLogProbabilities => (Choices.Count > 0 && Choices[0].Logprobs != null)
        ? Choices[0].Logprobs.Refusal
        : _refusalTokenLogProbabilities ??= new ChangeTrackingList<ChatTokenLogProbabilityInfo>();

    // CUSTOM: Flattened choice delta property.
    /// <summary>
    /// Gets the content fragment associated with this update.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Corresponds to e.g. <c>$.choices[0].delta.content</c> in the underlying REST schema.
    /// </para>
    /// Each update contains only a small number of tokens. When presenting or reconstituting a full, streamed
    /// response, all <see cref="ContentUpdate"/> values for the same chat completions should be combined.
    /// </remarks>
    public IReadOnlyList<ChatMessageContentPart> ContentUpdate => (Choices.Count > 0)
        ? Choices[0].Delta.Content
        : _contentUpdate ??= new ChangeTrackingList<ChatMessageContentPart>();

    // CUSTOM: Flattened choice delta property.
    /// <summary>
    /// Gets the <see cref="ChatMessageRole"/> associated with this update.
    /// </summary>
    /// <remarks>
    /// <see cref="ChatMessageRole"/> assignment typically occurs in a single update across a streamed Chat Completions
    /// and the value should be considered to be persist for all subsequent updates.
    /// </remarks>
    public ChatMessageRole? Role => (Choices.Count > 0)
        ? Choices[0].Delta.Role
        : null;

    // CUSTOM: Flattened choice delta property.
    /// <summary> Gets the tool calls. </summary>
    public IReadOnlyList<StreamingChatToolCallUpdate> ToolCallUpdates => (Choices.Count > 0)
        ? Choices[0].Delta.ToolCalls
        : _toolCallUpdates ??= new ChangeTrackingList<StreamingChatToolCallUpdate>();

    // CUSTOM: Flattened choice delta property.
    /// <summary>
    /// Deprecated and replaced by <see cref="ToolCallUpdates"/>. The name and arguments of a function that
    /// should be called, as generated by the model.
    /// </summary>
    public StreamingChatFunctionCallUpdate FunctionCallUpdate => (Choices.Count > 0)
        ? Choices[0].Delta.FunctionCall
        : null;

    // CUSTOM: Flattened choice delta property.
    public string RefusalUpdate => (Choices.Count > 0)
        ? Choices[0].Delta?.Refusal
        : null;

    internal static List<StreamingChatCompletionUpdate> DeserializeStreamingChatCompletionUpdates(JsonElement element)
    {
        return [StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate(element)];
    }
}
