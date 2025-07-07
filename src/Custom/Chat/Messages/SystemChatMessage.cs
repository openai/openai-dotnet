using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>system</c> role as supplied to a chat completion request. A system message is
/// generally supplied as the first message to a chat completion request and guides the model's behavior across future
/// <c>assistant</c> role response messages. These messages may help control behavior, style, tone, and
/// restrictions for a model-based assistant. Developer messages replace system messages for o1 models and newer.
/// </summary>
[CodeGenType("ChatCompletionRequestSystemMessage")]
[CodeGenVisibility(nameof(SystemChatMessage), CodeGenVisibility.Internal)]
[CodeGenSuppress("SystemChatMessage", typeof(ChatMessageContent))]
public partial class SystemChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="SystemChatMessage"/> using a collection of content items.
    /// For <c>system</c> messages, these can only be of type <c>text</c>.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public SystemChatMessage(IEnumerable<ChatMessageContentPart> contentParts)
        : this(new(contentParts), ChatMessageRole.System, null, null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="SystemChatMessage"/> using a collection of content items.
    /// For <c>system</c> messages, these can only be of type <c>text</c>.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public SystemChatMessage(params ChatMessageContentPart[] contentParts)
        : this(new(contentParts), ChatMessageRole.System, null, null)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="SystemChatMessage"/> with a single item of text content.
    /// </summary>
    /// <param name="content"> The text content of the message. </param>
    public SystemChatMessage(string content)
        : this(new([content]), ChatMessageRole.System, null, null)
    {
        Argument.AssertNotNull(content, nameof(content));
    }

    /// <summary>
    /// An optional <c>name</c> for the participant.
    /// </summary>
    [CodeGenMember("Name")]
    public string ParticipantName { get; set; }
}