using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
///     The manner in which the model chooses which tool (if any) to call.
///     <list>
///         <item>
///             Call <see cref="CreateAutoChoice()"/> to create a <see cref="ChatToolChoice"/> indicating that the
///             model can freely pick between generating a message or calling one or more tools.
///         </item>
///         <item>
///             Call <see cref="CreateNoneChoice()"/> to create a <see cref="ChatToolChoice"/> indicating that the
///             model must not call any tools and that instead it must generate a message.
///         </item>
///         <item>
///             Call <see cref="CreateRequiredChoice()"/> to create a <see cref="ChatToolChoice"/> indicating that the
///             model must call one or more tools.
///         </item>
///         <item>
///             Call <see cref="CreateFunctionChoice(string)"/> to create a <see cref="ChatToolChoice"/> indicating
///             that the model must call the specified function.
///         </item>
///     </list>
/// </summary>
[CodeGenType("ChatCompletionToolChoice")]
public partial class ChatToolChoice
{
    private const string FunctionType = "function";
    private readonly bool _predefined;
    private readonly string _predefinedValue;
    private readonly string _type;
    private readonly InternalChatCompletionNamedToolChoiceFunction _function;

    private const string AutoValue = "auto";
    private const string NoneValue = "none";
    private const string RequiredValue = "required";

    // CUSTOM: Made internal.
    internal ChatToolChoice()
    {
    }

    // CUSTOM: Added to support deserialization.
    internal ChatToolChoice(bool predefined, string predefinedValue, string type, InternalChatCompletionNamedToolChoiceFunction function, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        _predefined = predefined;
        _predefinedValue = predefinedValue;
        _type = type;
        _function = function;
        _additionalBinaryDataProperties = serializedAdditionalRawData;
    }

    /// <summary>
    ///     Creates a new <see cref="ChatToolChoice"/> indicating that the model can freely pick between generating a
    ///     message or calling one or more tools.
    /// </summary>
    public static ChatToolChoice CreateAutoChoice()
    {
        return new ChatToolChoice(
            predefined: true,
            predefinedValue: AutoValue,
            type: null,
            function: null,
            serializedAdditionalRawData: null);
    }

    /// <summary>
    ///     Creates a new <see cref="ChatToolChoice"/> indicating that the model must not call any tools and that
    ///     instead it must generate a message.
    /// </summary>
    public static ChatToolChoice CreateNoneChoice()
    {
        return new ChatToolChoice(
            predefined: true,
            predefinedValue: NoneValue,
            type: null,
            function: null,
            serializedAdditionalRawData: null);
    }

    /// <summary>
    ///     Creates a new <see cref="ChatToolChoice"/> indicating that the model must call one or more tools.
    /// </summary>
    public static ChatToolChoice CreateRequiredChoice()
    {
        return new ChatToolChoice(
            predefined: true,
            predefinedValue: RequiredValue,
            type: null,
            function: null,
            serializedAdditionalRawData: null);
    }

    /// <summary>
    ///     Creates a new <see cref="ChatToolChoice"/> indicating that the model must call the specified function.
    /// </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="functionName"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="functionName"/> is an empty string, and was expected to be non-empty. </exception>
    public static ChatToolChoice CreateFunctionChoice(string functionName)
    {
        Argument.AssertNotNullOrEmpty(functionName, nameof(functionName));

        return new ChatToolChoice(
            predefined: false,
            predefinedValue: null,
            type: FunctionType,
            function: new InternalChatCompletionNamedToolChoiceFunction(functionName),
            serializedAdditionalRawData: null);
    }
}
