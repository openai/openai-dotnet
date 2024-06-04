using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenAI.Assistants;

/// <summary>
/// Specifies the format that the model must output. Compatible with GPT-4o, GPT-4 Turbo, and all GPT-3.5 Turbo models since gpt-3.5-turbo-1106.
/// </summary>
/// <remarks>
/// <para>
/// Setting to { "type": "json_object" } enables JSON mode, which guarantees the message the model generates is valid JSON.
/// </para>
/// <para>
/// Important: when using JSON mode, you must also instruct the model to produce JSON yourself via a system or user message.Without this, the model may generate an unending stream of whitespace until the generation reaches the token limit, resulting in a long-running and seemingly "stuck" request.Also note that the message content may be partially cut off if finish_reason= "length", which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
/// </para>
/// </remarks>
[CodeGenModel("AssistantResponseFormat")]
public partial class AssistantResponseFormat
{
    private readonly string _plainTextValue;
    private readonly string _objectType;
    private readonly IDictionary<string, BinaryData> _serializedAdditionalRawData;

    private const string AutoValue = "auto";
    private const string TextValue = "text";
    private const string JsonObjectValue = "json_object";

    /// <summary>
    /// Default option. Specifies that the model should automatically determine whether it should respond with
    /// plain text or another format.
    /// </summary>
    public static AssistantResponseFormat Auto { get; }
        = new(plainTextValue: AutoValue, objectType: null, serializedAdditionalRawData: null);

    /// <summary>
    /// Specifies that the model should respond with plain text.
    /// </summary>
    public static AssistantResponseFormat Text { get; }
        = new(plainTextValue: null, objectType: TextValue, serializedAdditionalRawData: null);

    /// <summary>
    /// Specifies that the model should reply with a structured JSON object, enabling JSON mode.
    /// </summary>
    /// <remarks>
    /// **Important:** when using JSON mode, you **must** also instruct the model to produce JSON yourself via a
    /// system or user message. Without this, the model may generate an unending stream of whitespace until the
    /// generation reaches the token limit, resulting in a long-running and seemingly "stuck" request. Also note that
    /// the message content may be partially cut off if `finish_reason="length"`, which indicates the generation
    /// exceeded `max_tokens` or the conversation exceeded the max context length.
    /// </remarks>
    public static AssistantResponseFormat JsonObject { get; }
        = new(plainTextValue: null, objectType: JsonObjectValue, serializedAdditionalRawData: null);

    /// <summary>
    /// Creates a new instance of <see cref="AssistantResponseFormat"/> for mocking.
    /// </summary>
    protected AssistantResponseFormat()
    { }

    internal AssistantResponseFormat(string plainTextValue, string objectType, IDictionary<string, BinaryData> serializedAdditionalRawData)
    {
        _plainTextValue = plainTextValue;
        _objectType = objectType;
        _serializedAdditionalRawData = serializedAdditionalRawData ?? new ChangeTrackingDictionary<string, BinaryData>();
    }

    /// <inheritdoc />
    public static bool operator ==(AssistantResponseFormat left, AssistantResponseFormat right) => left.Equals(right);
    /// <inheritdoc />
    public static bool operator !=(AssistantResponseFormat left, AssistantResponseFormat right) => !left.Equals(right);

    /// <inheritdoc />
    public static implicit operator AssistantResponseFormat(string value)
    {
        if (string.Equals(value, AutoValue, StringComparison.InvariantCultureIgnoreCase))
        {
            return Auto;
        }
        if (string.Equals(value, TextValue, StringComparison.InvariantCultureIgnoreCase))
        {
            return Text;
        }
        if (string.Equals(value, JsonObjectValue, StringComparison.InvariantCultureIgnoreCase))
        {
            return JsonObject;
        }
        else
        {
            return new(plainTextValue: null, objectType: value, serializedAdditionalRawData: null);
        }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is AssistantResponseFormat other && Equals(other);
    /// <inheritdoc />
    public bool Equals(AssistantResponseFormat other)
        => string.Equals(_plainTextValue, other?._plainTextValue, StringComparison.InvariantCultureIgnoreCase)
            && string.Equals(_objectType, other?._objectType, StringComparison.InvariantCultureIgnoreCase);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => $"{_plainTextValue}/{_objectType}".GetHashCode();
    /// <inheritdoc />
    public override string ToString() => _plainTextValue ?? _objectType;
}
