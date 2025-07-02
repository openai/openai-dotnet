using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>user</c> role as supplied to a chat completion request. A user message contains
/// information originating from the caller and serves as a prompt for the model to complete. User messages may result
/// in either direct <c>assistant</c> message responses or in calls to supplied <c>tools</c> or <c>functions</c>.
/// </summary>
[CodeGenType("ChatCompletionRequestUserMessage")]
[CodeGenVisibility(nameof(UserChatMessage), CodeGenVisibility.Internal)]
[CodeGenSuppress("UserChatMessage", typeof(ChatMessageContent))]
public partial class UserChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="UserChatMessage"/> using a collection of content items that can
    /// include text and image information. This content format is currently only applicable to the
    /// <c>gpt-4o</c> and later models and will not be accepted by older models.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of text and image content items associated with the message.
    /// </param>
    public UserChatMessage(IEnumerable<ChatMessageContentPart> contentParts)
        : this(new(contentParts), ChatMessageRole.User, null, null)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="UserChatMessage"/> using a collection of content items that can
    /// include text and image information. This content format is currently only applicable to the
    /// <c>gpt-4o</c> and later models and will not be accepted by older models.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of text and image content items associated with the message.
    /// </param>
    public UserChatMessage(params ChatMessageContentPart[] contentParts)
        : this(new(contentParts), ChatMessageRole.User, null, null)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="UserChatMessage"/> with ordinary text <c>content</c>.
    /// </summary>
    /// <param name="content"> The textual content associated with the message. </param>
    public UserChatMessage(string content)
        : this(new([content]), ChatMessageRole.User, null, null)
    {
        Argument.AssertNotNull(content, nameof(content));
    }

    // CUSTOM: Rename.
    /// <summary>
    /// An optional <c>name</c> for the participant.
    /// </summary>
    [CodeGenMember("Name")]
    public string ParticipantName { get; set; }
}
