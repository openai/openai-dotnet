using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>tool</c> role as supplied to a chat completion request. A tool message
/// encapsulates a resolution of a <see cref="ChatToolCall"/> made by the model. The typical interaction flow featuring
/// tool messages is:
/// <list type="number">
/// <item> A <see cref="ToolChatMessage"/> provides a <see cref="ToolChatMessage"/> on a request; </item>
/// <item>
///     Based on the <c>name</c> and <c>description</c> information of provided tools, the model responds with one or
///     more <see cref="ChatToolCall"/> instances that need to be resolved to continue the logical conversation;
/// </item>
/// <item>
///     For each <see cref="ChatToolCall"/>, the matching tool is invoked and its output is supplied back to the model
///     via a <see cref="ToolChatMessage"/> to resolve the tool call and allow the logical conversation to
///     continue.
/// </item>
/// </list>
/// </summary>
[CodeGenType("ChatCompletionRequestToolMessage")]
[CodeGenSuppress("ToolChatMessage", typeof(ChatMessageContent), typeof(string))]
[CodeGenSuppress("ToolChatMessage", typeof(string))]
public partial class ToolChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolChatMessage"/> using a collection of content items.
    /// For <c>tool</c> messages, these can only be of type <c>text</c>.
    /// </summary>
    /// <param name="toolCallId">
    ///     The ID of the tool call that this message responds to.
    /// </param>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public ToolChatMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts)
        : this(new(contentParts), ChatMessageRole.Tool, null, toolCallId)
    {
        Argument.AssertNotNull(toolCallId, nameof(toolCallId));
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="ToolChatMessage"/> using a collection of content items.
    /// For <c>tool</c> messages, these can only be of type <c>text</c>.
    /// </summary>
    /// <param name="toolCallId">
    ///     The ID of the tool call that this message responds to.
    /// </param>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public ToolChatMessage(string toolCallId, params ChatMessageContentPart[] contentParts)
        : this(new(contentParts), ChatMessageRole.Tool, null, toolCallId)
    {
        Argument.AssertNotNull(toolCallId, nameof(toolCallId));
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="ToolChatMessage"/> with a single item of text content.
    /// </summary>
    /// <param name="toolCallId">
    ///     The ID of the tool call that this message responds to.
    /// </param>
    /// <param name="content"> The text content of the message. </param>
    public ToolChatMessage(string toolCallId, string content)
        : this(new([content]), ChatMessageRole.Tool, null, toolCallId)
    {
        Argument.AssertNotNull(toolCallId, nameof(toolCallId));
        Argument.AssertNotNull(content, nameof(content));
    }
}
