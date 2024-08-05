using System;
using System.Text.Json;
using System.Text;

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

    /// <summary>
    /// Parses a specific argument from function call arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public T GetFunctionArgument<T>(string parameterName, T defaultValue)
    {
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

        if (TryReadArgumentValue<T>(parameterName, out var value)) {
            return value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Parses a specific argument from function call arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public T GetFunctionArgument<T>(string parameterName)
    {
        if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

        if (TryReadArgumentValue<T>(parameterName, out var value)) {
            return value;
        }
        throw new ArgumentOutOfRangeException(nameof(parameterName));
    }

    private bool TryReadArgumentValue<T>(string propertyName, out T value)
    {
        // TODO: FunctionArguments should return BinaryData. Once it does, eliminate the transcoding call. 
        Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(FunctionArguments));

        if (!reader.Read()) throw new InvalidOperationException("Malformed JSON");
        if (reader.TokenType != JsonTokenType.StartObject) throw new InvalidOperationException("Malformed JSON");

        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(propertyName)) {
                if (!reader.Read()) throw new InvalidOperationException("Malformed JSON");
                return TryReadValue<T>(ref reader, out value);
            }
        }
        value = default;
        return false;
    }
    private static bool TryReadValue<T>(ref Utf8JsonReader reader, out T value)
    {
        object readValue = null;
        if (typeof(T).IsEnum && reader.TokenType == JsonTokenType.String) {
            var enumValueName = reader.GetString();
            readValue = Enum.Parse(typeof(T), enumValueName, ignoreCase: true);
        }
        else if (typeof(T) == typeof(string) && reader.TokenType == JsonTokenType.String) {
            readValue = (object)reader.GetString();
        }
        else if (typeof(T) == typeof(bool) && (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)) {
            readValue = (object)reader.GetBoolean();
        }
        else if (typeof(T) == typeof(double) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetDouble();
        }
        else if (typeof(T) == typeof(float) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetSingle();
        }
        else if (typeof(T) == typeof(int) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetInt32();
        }
        else if (typeof(T) == typeof(long) && reader.TokenType == JsonTokenType.Number) {
            readValue = (T)(object)reader.GetInt64();
        }
        else if (typeof(T) == typeof(short) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetInt16();
        }
        else if (typeof(T) == typeof(sbyte) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetSByte();
        }
        else if (typeof(T) == typeof(ulong) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetUInt64();
        }
        else if (typeof(T) == typeof(uint) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetUInt32();
        }
        else if (typeof(T) == typeof(ushort) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetUInt16();
        }
        else if (typeof(T) == typeof(byte) && reader.TokenType == JsonTokenType.Number) {
            readValue = (object)reader.GetByte();
        }
        else {
            value = default(T);
            return false;
        }

        value = (T)readValue;
        return true;
    }

}