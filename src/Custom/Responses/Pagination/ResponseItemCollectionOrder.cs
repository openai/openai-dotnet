using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

[Experimental("OPENAI001")]
public readonly partial struct ResponseItemCollectionOrder: IEquatable<ResponseItemCollectionOrder>
{
    public static ResponseItemCollectionOrder Ascending { get; } = new("asc");

    public static ResponseItemCollectionOrder Descending { get; } = new("desc");

    private readonly string _value;

    public ResponseItemCollectionOrder(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    public static bool operator ==(ResponseItemCollectionOrder left, ResponseItemCollectionOrder right) => left.Equals(right);

    public static bool operator !=(ResponseItemCollectionOrder left, ResponseItemCollectionOrder right) => !left.Equals(right);

    public static implicit operator ResponseItemCollectionOrder(string value) => new(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ResponseItemCollectionOrder other && Equals(other);

    public bool Equals(ResponseItemCollectionOrder other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    public override string ToString() => _value;
}
