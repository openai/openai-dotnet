using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>user</c> role as supplied to a chat completion request. A user message contains
/// information originating from the caller and serves as a prompt for the model to complete. User messages may result
/// in either direct <c>assistant</c> message responses or in calls to supplied <c>tools</c> or <c>functions</c>.
/// </summary>
[CodeGenModel("ChatCompletionRequestUserMessage")]
[CodeGenSuppress("UserChatMessage", typeof(ReadOnlyMemory<ChatMessageContentPart>))]
public partial class UserChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="UserChatMessage"/> using a collection of content items that can
    /// include text and image information. This content format is currently only applicable to the
    /// <c>gpt-4o</c> and later models and will not be accepted by older models.
    /// </summary>
    /// <param name="content">
    ///     The collection of text and image content items associated with the message.
    /// </param>
    public UserChatMessage(IEnumerable<ChatMessageContentPart> content)
        : base(ChatMessageRole.User, content)
    {
        Argument.AssertNotNullOrEmpty(content, nameof(content));
    }

    /// <summary>
    /// Creates a new instance of <see cref="UserChatMessage"/> using a collection of content items that can
    /// include text and image information. This content format is currently only applicable to the
    /// <c>gpt-4o</c> and later models and will not be accepted by older models.
    /// </summary>
    /// <param name="content">
    ///     The collection of text and image content items associated with the message.
    /// </param>
    public UserChatMessage(params ChatMessageContentPart[] content)
        : base(ChatMessageRole.User, content)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="UserChatMessage"/> with ordinary text <c>content</c>.
    /// </summary>
    /// <param name="content"> The textual content associated with the message. </param>
    public UserChatMessage(string content)
        : base(ChatMessageRole.User, content)
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
