using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[Experimental("OPENAI001")]
public readonly partial struct ResponseContentPartKind : IEquatable<ResponseContentPartKind>
{
    private readonly string _value;

    private const string InputTextValue = "input_text";
    private const string InputImageValue = "input_image";
    private const string InputFileValue = "input_file";
    private const string OutputTextValue = "output_text";
    private const string RefusalValue = "refusal";

    public ResponseContentPartKind(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    public static ResponseContentPartKind InputText { get; } = new ResponseContentPartKind(InputTextValue);
    public static ResponseContentPartKind InputImage { get; } = new ResponseContentPartKind(InputImageValue);
    public static ResponseContentPartKind InputFile { get; } = new ResponseContentPartKind(InputFileValue);
    public static ResponseContentPartKind OutputText { get; } = new ResponseContentPartKind(OutputTextValue);
    public static ResponseContentPartKind Refusal { get; } = new ResponseContentPartKind(RefusalValue);

    public static bool operator ==(ResponseContentPartKind left, ResponseContentPartKind right) => left.Equals(right);
    public static bool operator !=(ResponseContentPartKind left, ResponseContentPartKind right) => !left.Equals(right);
    public static implicit operator ResponseContentPartKind(string value) => new ResponseContentPartKind(value);
    public static implicit operator ResponseContentPartKind?(string value) => value == null ? null : new ResponseContentPartKind(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ResponseContentPartKind other && Equals(other);
    public bool Equals(ResponseContentPartKind other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;
    public override string ToString() => _value;
}