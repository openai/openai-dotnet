namespace OpenAI.Chat;

using System;
using System.Text.Json;

/// <summary>
/// A base representation of an incremental update to a streaming tool call that is part of a streaming chat completion
/// request.
/// </summary>
/// <remarks>
/// <para>
/// This type encapsulates the payload located in e.g. <c>$.choices[0].delta.tool_calls[]</c> in the REST API schema.
/// </para>
/// <para>
/// To differentiate between parallel streaming tool calls within a single streaming choice, use the value of the
/// <see cref="Index"/> property.
/// </para>
/// <para>
/// <see cref="StreamingChatToolCallUpdate"/> is the streaming, base class counterpart to <see cref="ChatToolCall"/>.
/// </para>
/// </remarks>
[CodeGenModel("ChatCompletionMessageToolCallChunk")]
[CodeGenSuppress("StreamingChatToolCallUpdate", typeof(int))]
public partial class StreamingChatToolCallUpdate
{
    [CodeGenMember("Function")]
    internal InternalChatCompletionMessageToolCallChunkFunction Function { get; }

    internal StreamingChatToolCallUpdate(int index, string id, InternalChatCompletionMessageToolCallChunkFunction function)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(function, nameof(function));

        Kind = ChatToolCallKind.Function;

        Index = index;
        Id = id;
        Function = function;
    }

    // CUSTOM:
    // - Renamed.
    // - Changed type from string.
    /// <summary> The kind of tool.Currently, only<see cref="ChatToolCallKind.Function"/> is supported. </summary>
    [CodeGenMember("Type")]
    public ChatToolCallKind Kind { get; } = ChatToolCallKind.Function;

    /// <summary>
    /// The name of the function requested by the tool call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Corresponds to e.g. <c>$.choices[0].delta.tool_calls[0].function.name</c> in the REST API schema.
    /// </para>
    /// <para>
    /// For a streaming function tool call, this name will appear in a single streaming update payload, typically the
    /// first. Use the <see cref="Index"/> property to differentiate between multiple,
    /// parallel tool calls when streaming.
    /// </para>
    /// </remarks>
    public string FunctionName => Function?.Name;

    /// <summary>
    /// The next new segment of the function arguments for the function tool called by a streaming tool call.
    /// These must be accumulated for the complete contents of the function arguments.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Corresponds to e.g. <c>$.choices[0].delta.tool_calls[0].function.arguments</c> in the REST API schema.
    /// </para>
    /// Note that the model does not always generate valid JSON and may hallucinate parameters
    /// not defined by your function schema. Validate the arguments in your code before calling
    /// your function.
    /// </remarks>
    public string FunctionArgumentsUpdate => Function?.Arguments;
}
