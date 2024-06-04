using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// A common, base representation of a message provided as input into a chat completion request.
/// </summary>
/// <remarks>
/// <list type="table">
/// <listheader>
///     <type>Type -</type>
///     <role>Role -</role>
///     <note>Description</note>
/// </listheader>
/// <item>
///     <type><see cref="SystemChatMessage"/> -</type>
///     <role><c>system</c> -</role>
///     <note>Instructions to the model that guide the behavior of future <c>assistant</c> messages.</note>
/// </item>
/// <item>
///     <type><see cref="UserChatMessage"/> -</type>
///     <role><c>user</c> -</role>
///     <note>Input messages from the caller, typically paired with <c>assistant</c> messages in a conversation.</note>
/// </item>
/// <item>
///     <type><see cref="AssistantChatMessage"/> -</type>
///     <role><c>assistant</c> -</role>
///     <note>
///         Output messages from the model with responses to the <c>user</c> or calls to tools or functions that are
///         needed to continue the logical conversation.
///     </note>
/// </item>
/// <item>
///     <type><see cref="ToolChatMessage"/> -</type>
///     <role><c>tool</c> -</role>
///     <note>
///         Resolution information for a <see cref="ChatToolCall"/> in an earlier
///         <see cref="AssistantChatMessage"/> that was made against a supplied
///         <see cref="ChatTool"/>.
///     </note>
/// </item>
/// <item>
///     <type><see cref="FunctionChatMessage"/> -</type>
///     <role><c>function</c> -</role>
///     <note>
///         Resolution information for a <see cref="ChatFunctionCall"/> in an earlier
///         <see cref="AssistantChatMessage"/> that was made against a supplied
///         <see cref="ChatFunction"/>. Note that <c>functions</c> are deprecated in favor of
///         <c>tool_calls</c>.
///     </note>
/// </item>
/// </list>
/// </remarks>
[CodeGenModel("ChatCompletionRequestMessage")]
[CodeGenSerialization(nameof(Content), SerializationValueHook = nameof(SerializeContentValue), DeserializationValueHook = nameof(DeserializeContentValue))]
public abstract partial class ChatMessage
{
    /// <summary>
    /// The content associated with the message. The interpretation of this content will vary depending on the message type.
    /// </summary>
    public IList<ChatMessageContentPart> Content { get; protected init; }

    /// <inheritdoc cref="SystemChatMessage(string)"/>
    public static SystemChatMessage CreateSystemMessage(string content) => new SystemChatMessage(content);

    /// <inheritdoc cref="UserChatMessage(string)"/>
    public static UserChatMessage CreateUserMessage(string content) => new UserChatMessage(content);

    /// <inheritdoc cref="UserChatMessage(IEnumerable{ChatMessageContentPart})"/>
    public static UserChatMessage CreateUserMessage(IEnumerable<ChatMessageContentPart> contentParts) => new UserChatMessage(contentParts);

    /// <inheritdoc cref="UserChatMessage(ChatMessageContentPart[])"/>
    public static UserChatMessage CreateUserMessage(params ChatMessageContentPart[] contentParts) => new UserChatMessage(contentParts);

    /// <inheritdoc cref="AssistantChatMessage(string)"/>
    public static AssistantChatMessage CreateAssistantMessage(string content) => new AssistantChatMessage(content);

    /// <inheritdoc cref="AssistantChatMessage(IEnumerable{ChatToolCall}, string)"/>
    public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatToolCall> toolCalls, string content = null) => new AssistantChatMessage(toolCalls, content);

    /// <inheritdoc cref="AssistantChatMessage(ChatFunctionCall, string)"/>
    public static AssistantChatMessage CreateAssistantMessage(ChatFunctionCall functionCall, string content = null) => new AssistantChatMessage(functionCall, content);

    /// <inheritdoc cref="AssistantChatMessage(ChatCompletion)"/>
    public static AssistantChatMessage CreateAssistantMessage(ChatCompletion chatCompletion) => new AssistantChatMessage(chatCompletion);

    /// <inheritdoc cref="ToolChatMessage(string, string)"/>
    public static ToolChatMessage CreateToolChatMessage(string toolCallId, string content) => new ToolChatMessage(toolCallId, content);

    /// <inheritdoc cref="FunctionChatMessage(string, string)"/>
    public static FunctionChatMessage CreateFunctionMessage(string functionName, string content) => new FunctionChatMessage(functionName, content);
}
