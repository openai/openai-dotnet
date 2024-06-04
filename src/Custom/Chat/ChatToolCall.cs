using System;

namespace OpenAI.Chat;

/// <summary>
/// A base representation of an item in an <c>assistant</c> role response's <c>tool_calls</c> that specifies
/// parameterized resolution against a previously defined tool that is needed for the model to continue the logical
/// conversation.
/// </summary>
[CodeGenModel("ChatCompletionMessageToolCall")]
public partial class ChatToolCall
{
    /// <summary> The function that the model called. </summary>
    [CodeGenMember("Function")]
    internal InternalChatCompletionMessageToolCallFunction Function { get; }

    // CUSTOM: Made internal.
    /// <summary> Initializes a new instance of <see cref="ChatToolCall"/>. </summary>
    /// <param name="id"> The ID of the tool call. </param>
    /// <param name="function"> The function that the model called. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="id"/> or <paramref name="function"/> is null. </exception>
    internal ChatToolCall(string id, InternalChatCompletionMessageToolCallFunction function)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(function, nameof(function));

        Kind = ChatToolCallKind.Function;

        Id = id;
        Function = function;
    }

    // CUSTOM: Renamed.
    /// <summary> The kind of tool. Currently, only <see cref="ChatToolCallKind.Function"/> is supported. </summary>
    [CodeGenMember("Type")]
    public ChatToolCallKind Kind { get; } = ChatToolCallKind.Function;

    // CUSTOM: Flattened.
    /// <summary>
    /// Gets the <c>name</c> of the function.
    /// </summary>
    public string FunctionName => Function?.Name;

    // CUSTOM: Flattened.
    /// <summary>
    /// Gets the <c>arguments</c> to the function.
    /// </summary>
    public string FunctionArguments => Function?.Arguments;

    /// <summary>
    /// Creates a new instance of <see cref="ChatToolCall"/>.
    /// </summary>
    /// <param name="toolCallId">
    ///     The ID of the tool call, used when resolving the tool call with a future
    ///     <see cref="ToolChatMessage"/>.
    /// </param>
    /// <param name="functionName"> The <c>name</c> of the function. </param>
    /// <param name="functionArguments"> The <c>arguments</c> to the function. </param>
    public static ChatToolCall CreateFunctionToolCall(string toolCallId, string functionName, string functionArguments)
    {
        Argument.AssertNotNull(toolCallId, nameof(toolCallId));
        Argument.AssertNotNull(functionName, nameof(functionName));
        Argument.AssertNotNull(functionArguments, nameof(functionArguments));

        InternalChatCompletionMessageToolCallFunction function = new(functionName, functionArguments);

        return new(toolCallId, function);
    }
}