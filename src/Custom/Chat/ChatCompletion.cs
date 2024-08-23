using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;

namespace OpenAI.Chat;

[CodeGenModel("CreateChatCompletionResponse")]
public partial class ChatCompletion
{
    private IReadOnlyList<ChatTokenLogProbabilityInfo> _contentTokenLogProbabilities;
    private IReadOnlyList<ChatTokenLogProbabilityInfo> _refusalTokenLogProbabilities;

    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always `chat.completion`. </summary>
    [CodeGenMember("Object")]
    private InternalCreateChatCompletionResponseObject Object { get; } = InternalCreateChatCompletionResponseObject.ChatCompletion;

    // CUSTOM: Made internal. We only get back a single choice, and instead we flatten the structure for usability.
    /// <summary> A list of chat completion choices. Can be more than one if `n` is greater than 1. </summary>
    [CodeGenMember("Choices")]
    internal IReadOnlyList<InternalCreateChatCompletionResponseChoice> Choices { get; }

    // CUSTOM: Renamed.
    /// <summary> The Unix timestamp (in seconds) of when the chat completion was created. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }

    // CUSTOM: Flattened choice property.
    /// <summary>
    /// The reason the model stopped generating tokens. This will be `stop` if the model hit a natural stop point or a provided stop sequence,
    /// `length` if the maximum number of tokens specified in the request was reached,
    /// `content_filter` if content was omitted due to a flag from our content filters,
    /// `tool_calls` if the model called a tool, or `function_call` (deprecated) if the model called a function.
    /// </summary>
    public ChatFinishReason FinishReason => Choices[0].FinishReason;

    // CUSTOM: Flattened choice logprobs property.
    /// <summary>
    /// Log probability information.
    /// </summary>
    public IReadOnlyList<ChatTokenLogProbabilityInfo> ContentTokenLogProbabilities => (Choices[0].Logprobs != null)
        ? Choices[0].Logprobs.Content
        : _contentTokenLogProbabilities ??= new ChangeTrackingList<ChatTokenLogProbabilityInfo>();

    // CUSTOM: Flattened refusal logprobs property.
    public IReadOnlyList<ChatTokenLogProbabilityInfo> RefusalTokenLogProbabilities => (Choices[0]?.Logprobs != null)
        ? Choices[0].Logprobs.Refusal
        : _refusalTokenLogProbabilities ??= new ChangeTrackingList<ChatTokenLogProbabilityInfo>();

    // CUSTOM: Flattened choice message property.
    /// <summary>
    /// The role of the author of this message.
    /// </summary>
    public ChatMessageRole Role => Choices[0].Message.Role;

    // CUSTOM: Flattened choice message property.
    /// <summary>
    /// The contents of the message.
    /// </summary>
    public IReadOnlyList<ChatMessageContentPart> Content => Choices[0].Message.Content;

    // CUSTOM: Flattened choice message property.
    /// <summary>
    /// The tool calls.
    /// </summary>
    public IReadOnlyList<ChatToolCall> ToolCalls => Choices[0].Message.ToolCalls;

    // CUSTOM: Flattened choice message property.
    public ChatFunctionCall FunctionCall => Choices[0].Message.FunctionCall;

    // CUSTOM: Flattened choice message property.
    public string Refusal => Choices[0].Message.Refusal;

    /// <summary>
    /// Returns text representation of the first part of the first choice.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Content.Count > 0 ? Content[0].Text
        : ToolCalls.Count > 0 ? ModelReaderWriter.Write(ToolCalls[0]).ToString()
        : null;

    // CUSTOM: Made internal.
    [CodeGenMember("ServiceTier")]
    internal InternalCreateChatCompletionResponseServiceTier? _serviceTier;
}
