using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents <c>tool_choice</c>, the desired manner in which the model should use the <c>tools</c> defined in a
/// chat completion request.
/// </summary>
[CodeGenModel("ChatCompletionToolChoice")]
[CodeGenSuppress("ChatToolChoice", typeof(IDictionary<string, BinaryData>))]
public partial class ChatToolChoice
{
    private readonly bool _isPlainString;
    private readonly string _string;
    private readonly InternalChatCompletionNamedToolChoiceType _type;
    private readonly InternalChatCompletionNamedToolChoiceFunction _function;

    private const string AutoValue = "auto";
    private const string NoneValue = "none";
    private const string RequiredValue = "required";

    // CUSTOM: Added custom internal constructor to handle the plain string representation (e.g. "auto", "none", etc.).
    internal ChatToolChoice(string predefinedToolChoice)
    {
        Argument.AssertNotNull(predefinedToolChoice, nameof(predefinedToolChoice));

        _string = predefinedToolChoice;
        _isPlainString = true;
    }

    // CUSTOM: Added custom public constructor to handle the object representation.
    /// <summary>
    /// Creates a new instance of <see cref="ChatToolChoice"/> which requests that the model restricts its behavior
    /// to calling the specified tool.
    /// </summary>
    /// <param name="tool"> The definition of the tool that the model should call. </param>
    public ChatToolChoice(ChatTool tool)
    {
        Argument.AssertNotNull(tool, nameof(tool));

        _function = new(tool.FunctionName);
        _type = InternalChatCompletionNamedToolChoiceType.Function;
        _isPlainString = false;
    }

    // CUSTOM: Added the function name parameter to the constructor that takes additional data to handle the object representation.
    /// <summary> Initializes a new instance of <see cref="ChatToolChoice"/>. </summary>
    /// <param name="functionName"> The function name. </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal ChatToolChoice(string functionName, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        _function = new(functionName);
        _type = InternalChatCompletionNamedToolChoiceType.Function;
        _isPlainString = false;

        _serializedAdditionalRawData = serializedAdditionalRawData;
    }

    /// <summary>
    /// Specifies that the model must freely pick between generating a message or calling one or more tools.
    /// </summary>
    public static ChatToolChoice Auto { get; } = new ChatToolChoice(AutoValue);
    /// <summary>
    /// Specifies that the model must not invoke any tools, and instead it must generate an ordinary message. Note
    /// that the tools that were provided may still influence the model's behavior even if they are not called.
    /// </summary>
    public static ChatToolChoice None { get; } = new ChatToolChoice(NoneValue);
    /// <summary>
    /// Specifies that the model must call one or more tools.
    /// </summary>
    public static ChatToolChoice Required { get; } = new ChatToolChoice(RequiredValue);
}
