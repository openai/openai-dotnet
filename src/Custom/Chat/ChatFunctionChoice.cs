using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a desired manner in which the model should use the functions defined in a chat completion request.
/// </summary>
[CodeGenModel("ChatCompletionFunctionChoice")]
[CodeGenSuppress("ChatFunctionChoice", typeof(IDictionary<string, BinaryData>))]
public partial class ChatFunctionChoice
{
    private readonly bool _isPlainString;
    private readonly string _string;
    private readonly InternalChatCompletionFunctionCallOption _function;

    private const string AutoValue = "auto";
    private const string NoneValue = "none";

    // CUSTOM: Made internal.
    internal ChatFunctionChoice()
    {
    }

    // CUSTOM: Added custom internal constructor to handle the plain string representation (e.g. "auto", "none", etc.).
    internal ChatFunctionChoice(string predefinedFunctionChoice)
    {
        Argument.AssertNotNull(predefinedFunctionChoice, nameof(predefinedFunctionChoice));

        _string = predefinedFunctionChoice;
        _isPlainString = true;
    }

    // CUSTOM: Added the function name parameter to the constructor that takes additional data to handle the object representation.
    /// <summary> Initializes a new instance of <see cref="ChatFunctionChoice"/>. </summary>
    /// <param name="functionName"> The function name. </param>
    /// <param name="serializedAdditionalRawData"> Keeps track of any properties unknown to the library. </param>
    internal ChatFunctionChoice(string functionName, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        _function = new(functionName);
        _isPlainString = false;

        _serializedAdditionalRawData = serializedAdditionalRawData;
    }

    // CUSTOM: Added custom public constructor to handle the object representation.
    /// <summary>
    /// Creates a new instance of <see cref="ChatFunctionChoice"/>.
    /// </summary>
    public ChatFunctionChoice(ChatFunction chatFunction)
    {
        Argument.AssertNotNull(chatFunction, nameof(chatFunction));

        _function = new(chatFunction.FunctionName);
        _isPlainString = false;
    }

    /// <summary>
    /// Specifies that the model must freely pick between generating a message or calling one or more tools.
    /// </summary>
    public static ChatFunctionChoice Auto { get; } = new ChatFunctionChoice(AutoValue);
    /// <summary>
    /// Specifies that the model must not invoke any tools, and instead it must generate an ordinary message. Note
    /// that the tools that were provided may still influence the model's behavior even if they are not called.
    /// </summary>
    public static ChatFunctionChoice None { get; } = new ChatFunctionChoice(NoneValue);
}