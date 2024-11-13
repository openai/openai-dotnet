using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a desired manner in which the model should use the functions defined in a chat completion request.
/// </summary>
[Obsolete($"This class is obsolete. Please use {nameof(ChatToolChoice)} instead.")]
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

        SerializedAdditionalRawData = serializedAdditionalRawData;
    }

    /// <summary>
    /// Creates an instance of <see cref="ChatFunctionChoice"/> that specifies the model must call a specific function,
    /// referred to via its name.
    /// </summary>
    /// <param name="functionName"> The name of the function the model must call. </param>
    /// <returns> A new instance of <see cref="ChatFunctionChoice"/>. </returns>
    public static ChatFunctionChoice CreateNamedChoice(string functionName)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        return new(functionName, serializedAdditionalRawData: null);
    }

    /// <summary>
    /// Creates an instance of <see cref="ChatFunctionChoice"/> that specifies the model may freely pick between
    /// generating a message or calling a function.
    /// </summary>
    /// <returns> A new instance of <see cref="ChatFunctionChoice"/>. </returns>
    public static ChatFunctionChoice CreateAutoChoice() => new ChatFunctionChoice(AutoValue);

    /// <summary>
    /// Creates an instance of <see cref="ChatFunctionChoice"/> that specifies the model should not call any function
    /// and instead only generate a message.
    /// </summary>
    /// <returns> A new instance of <see cref="ChatFunctionChoice"/>. </returns>
    public static ChatFunctionChoice CreateNoneChoice() => new ChatFunctionChoice(NoneValue);
}