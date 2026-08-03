using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[Experimental("OPENAI001")]
public readonly partial struct ResponseToolChoiceKind : IEquatable<ResponseToolChoiceKind>
{
    private readonly string _value;

    private const string AutoValue = "auto";
    private const string NoneValue = "none";
    private const string RequiredValue = "required";
    private const string FunctionValue = "function";
    private const string FileSearchValue = "file_search";
    private const string WebSearchValue = "web_search_preview";
    private const string ComputerValue = "computer_use_preview";

    public ResponseToolChoiceKind(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    public static ResponseToolChoiceKind Auto { get; } = new ResponseToolChoiceKind(AutoValue);
    public static ResponseToolChoiceKind None { get; } = new ResponseToolChoiceKind(NoneValue);
    public static ResponseToolChoiceKind Required { get; } = new ResponseToolChoiceKind(RequiredValue);
    public static ResponseToolChoiceKind Function { get; } = new ResponseToolChoiceKind(FunctionValue);
    public static ResponseToolChoiceKind FileSearch { get; } = new ResponseToolChoiceKind(FileSearchValue);
    public static ResponseToolChoiceKind WebSearch { get; } = new ResponseToolChoiceKind(WebSearchValue);
    public static ResponseToolChoiceKind Computer { get; } = new ResponseToolChoiceKind(ComputerValue);

    public static bool operator ==(ResponseToolChoiceKind left, ResponseToolChoiceKind right) => left.Equals(right);
    public static bool operator !=(ResponseToolChoiceKind left, ResponseToolChoiceKind right) => !left.Equals(right);
    public static implicit operator ResponseToolChoiceKind(string value) => new ResponseToolChoiceKind(value);
    public static implicit operator ResponseToolChoiceKind?(string value) => value == null ? null : new ResponseToolChoiceKind(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ResponseToolChoiceKind other && Equals(other);
    public bool Equals(ResponseToolChoiceKind other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;
    public override string ToString() => _value;
}
