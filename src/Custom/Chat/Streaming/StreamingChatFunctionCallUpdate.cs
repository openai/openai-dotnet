using System;

namespace OpenAI.Chat;

[Obsolete($"This class is obsolete. Please use {nameof(StreamingChatToolCallUpdate)} instead.")]
[CodeGenModel("ChatCompletionStreamResponseDeltaFunctionCall")]
[CodeGenSerialization(nameof(FunctionArgumentsUpdate), SerializationValueHook = nameof(SerializeFunctionArgumentsUpdateValue), DeserializationValueHook = nameof(DeserializeFunctionArgumentsUpdateValue))]
public partial class StreamingChatFunctionCallUpdate
{
    // CUSTOM: Renamed.
    /// <summary> The name of the function to call. </summary>
    [CodeGenMember("Name")]
    public string FunctionName { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from string to BinaryData.
    /// <summary>
    /// Gets a function arguments fragment associated with this update.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Each update contains only a small number of tokens. When presenting or reconstituting a full, streamed
    ///         arguments body, all <see cref="FunctionArgumentsUpdate"/> values should be combined.
    ///     </para>
    ///     <para>
    ///         As is the case for non-streaming <see cref="ChatFunctionCall.FunctionArguments"/>, the content provided
    ///         for function arguments is not guaranteed to be well-formed JSON or to contain expected data. Callers
    ///         should validate function arguments before using them.
    ///     </para>
    /// </remarks>
    [CodeGenMember("Arguments")]
    public BinaryData FunctionArgumentsUpdate { get; }
}
