using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[Experimental("OPENAI001")]
public readonly partial struct ResponseTextFormatKind : IEquatable<ResponseTextFormatKind>
{
    private readonly string _value;

    private const string TextValue = "text";
    private const string JsonObjectValue = "json_object";
    private const string JsonSchemaValue = "json_schema";

    public ResponseTextFormatKind(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    public static ResponseTextFormatKind Text { get; } = new ResponseTextFormatKind(TextValue);
    public static ResponseTextFormatKind JsonObject { get; } = new ResponseTextFormatKind(JsonObjectValue);
    public static ResponseTextFormatKind JsonSchema { get; } = new ResponseTextFormatKind(JsonSchemaValue);

    public static bool operator ==(ResponseTextFormatKind left, ResponseTextFormatKind right) => left.Equals(right);
    public static bool operator !=(ResponseTextFormatKind left, ResponseTextFormatKind right) => !left.Equals(right);
    public static implicit operator ResponseTextFormatKind(string value) => new ResponseTextFormatKind(value);
    public static implicit operator ResponseTextFormatKind?(string value) => value == null ? null : new ResponseTextFormatKind(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ResponseTextFormatKind other && Equals(other);
    public bool Equals(ResponseTextFormatKind other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;
    public override string ToString() => _value;
}