using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
public readonly partial struct RunCollectionOrder: IEquatable<RunCollectionOrder>
{
    public static RunCollectionOrder Ascending { get; } = new RunCollectionOrder("asc");

    public static RunCollectionOrder Descending { get; } = new RunCollectionOrder("desc");

    private readonly string _value;
    private const string AscValue = "asc";
    private const string DescValue = "desc";

    public RunCollectionOrder(string value)
    {
        Argument.AssertNotNull(value, nameof(value));

        _value = value;
    }

    public static bool operator ==(RunCollectionOrder left, RunCollectionOrder right) => left.Equals(right);

    public static bool operator !=(RunCollectionOrder left, RunCollectionOrder right) => !left.Equals(right);

    public static implicit operator RunCollectionOrder(string value) => new RunCollectionOrder(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is RunCollectionOrder other && Equals(other);

    public bool Equals(RunCollectionOrder other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    public override string ToString() => _value;
}

