using System.ComponentModel;
using System;

namespace OpenAI.Chat;

/// <summary>
/// Represents the possibles of underlying data for a chat message's <c>content</c> property.
/// </summary>
public readonly partial struct ChatMessageContentPartKind : IEquatable<ChatMessageContentPartKind>
{
    private readonly string _value;

    /// <summary> Initializes a new instance of <see cref="ChatMessageContentPartKind"/>. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is null. </exception>
    public ChatMessageContentPartKind(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private const string TextValue = "text";
    private const string RefusalValue = "refusal";
    private const string ImageValue = "image_url";

    /// <summary> Text. </summary>
    public static ChatMessageContentPartKind Text { get; } = new ChatMessageContentPartKind(TextValue);
    /// <summary> Refusal. </summary>
    public static ChatMessageContentPartKind Refusal { get; } = new(RefusalValue);
    /// <summary> Image. </summary>
    public static ChatMessageContentPartKind Image { get; } = new ChatMessageContentPartKind(ImageValue);

    /// <summary> Determines if two <see cref="ChatMessageContentPartKind"/> values are the same. </summary>
    public static bool operator ==(ChatMessageContentPartKind left, ChatMessageContentPartKind right) => left.Equals(right);
    /// <summary> Determines if two <see cref="ChatMessageContentPartKind"/> values are not the same. </summary>
    public static bool operator !=(ChatMessageContentPartKind left, ChatMessageContentPartKind right) => !left.Equals(right);
    /// <summary> Converts a string to a <see cref="ChatMessageContentPartKind"/>. </summary>
    public static implicit operator ChatMessageContentPartKind(string value) => new ChatMessageContentPartKind(value);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ChatMessageContentPartKind other && Equals(other);
    /// <inheritdoc />
    public bool Equals(ChatMessageContentPartKind other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    /// <inheritdoc />
    public override string ToString() => _value;
}