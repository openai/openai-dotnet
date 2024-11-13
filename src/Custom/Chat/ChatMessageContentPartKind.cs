namespace OpenAI.Chat;

/// <summary>
/// Represents the possibles of underlying data for a chat message's <c>content</c> property.
/// </summary>
public enum ChatMessageContentPartKind
{
    Text,

    Refusal,

    Image
}