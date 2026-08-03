using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// The available detail settings to use when processing an image.
/// These settings balance token consumption and the resolution of evaluation performed.
/// </summary>
[Experimental("OPENAI001")]
public readonly partial struct MessageImageDetail : IEquatable<MessageImageDetail>
{
    private readonly string _value;

    private const string AutoValue = "auto";
    private const string LowValue = "low";
    private const string HighValue = "high";

    /// <summary> Creates a new <see cref="MessageImageDetail"/> instance. </summary>
    public MessageImageDetail(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    /// <summary> Default. Allows the model to automatically select detail. </summary>
    public static MessageImageDetail Auto { get; } = new MessageImageDetail(AutoValue);

    /// <summary> Reduced detail that uses fewer tokens than <see cref="High"/>. </summary>
    public static MessageImageDetail Low { get; } = new MessageImageDetail(LowValue);

    /// <summary> Increased detail that uses more tokens than <see cref="Low"/>. </summary>
    public static MessageImageDetail High { get; } = new MessageImageDetail(HighValue);

    /// <inheritdoc/>
    public static bool operator ==(MessageImageDetail left, MessageImageDetail right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(MessageImageDetail left, MessageImageDetail right) => !left.Equals(right);

    /// <inheritdoc/>
    public static implicit operator MessageImageDetail(string value) => new MessageImageDetail(value);
    /// <inheritdoc/>
    public static implicit operator MessageImageDetail?(string value) => value == null ? null : new MessageImageDetail(value);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is MessageImageDetail other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(MessageImageDetail other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    /// <inheritdoc/>
    public override string ToString() => _value;
}
