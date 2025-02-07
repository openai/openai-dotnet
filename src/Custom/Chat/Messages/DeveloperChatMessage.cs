using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>developer</c> role as supplied to a chat completion request. A developer message is
/// generally supplied as the first message to a chat completion request and guides the model's behavior across future
/// <c>assistant</c> role response messages. These messages may help control behavior, style, tone, and
/// restrictions for a model-based assistant. Developer messages replace system messages for o1 models and newer.
/// </summary>
[CodeGenModel("ChatCompletionRequestDeveloperMessage")]
[CodeGenSuppress("DeveloperChatMessage", typeof(ChatMessageContent))]
public partial class DeveloperChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="DeveloperChatMessage"/> using a collection of content items.
    /// For <c>developer</c> messages, these can only be of type <c>text</c>.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public DeveloperChatMessage(IEnumerable<ChatMessageContentPart> contentParts)
        : base(ChatMessageRole.Developer, contentParts)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="DeveloperChatMessage"/> using a collection of content items.
    /// For <c>developer</c> messages, these can only be of type <c>text</c>.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public DeveloperChatMessage(params ChatMessageContentPart[] contentParts)
        : base(ChatMessageRole.Developer, contentParts)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="DeveloperChatMessage"/> with a single item of text content.
    /// </summary>
    /// <param name="content"> The text content of the message. </param>
    public DeveloperChatMessage(string content)
        : base(ChatMessageRole.Developer, content)
    {
        Argument.AssertNotNull(content, nameof(content));
    }

    // CUSTOM: Hide the default constructor.
    internal DeveloperChatMessage()
    {
    }

    /// <summary>
    /// An optional <c>name</c> for the participant.
    /// </summary>
    [CodeGenMember("Name")]
    public string ParticipantName { get; set; }
}