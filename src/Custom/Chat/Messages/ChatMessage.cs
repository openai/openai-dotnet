using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
[CodeGenType("ChatCompletionRequestMessage")]
[CodeGenSuppress("_content")]
[CodeGenVisibility(nameof(ChatMessage), CodeGenVisibility.Internal)]
public partial class ChatMessage
{
    /// <summary>
    /// The content associated with the message. The interpretation of this content will vary depending on the message type.
    /// </summary>
    public ChatMessageContent Content { get; }

    // CUSTOM: Changed type from string to ChatMessageRole.
    [CodeGenMember("Role")]
    internal ChatMessageRole Role { get; set; }

    // CUSTOM: Made internal.
    internal ChatMessage(ChatMessageRole role)
        : this(null, role, null)
    { }
 
    #region SystemChatMessage
    /// <inheritdoc cref="SystemChatMessage(string)"/>
    public static SystemChatMessage CreateSystemMessage(string content) => new(content);

    /// <inheritdoc cref="SystemChatMessage(IEnumerable{ChatMessageContentPart})"/>
    public static SystemChatMessage CreateSystemMessage(IEnumerable<ChatMessageContentPart> contentParts) => new(contentParts);

    /// <inheritdoc cref="SystemChatMessage(ChatMessageContentPart[])"/>
    public static SystemChatMessage CreateSystemMessage(params ChatMessageContentPart[] contentParts) => new(contentParts);
    #endregion

    #region DeveloperChatMessage
    /// <inheritdoc cref="DeveloperChatMessage(string)"/>
    [Experimental("OPENAI001")]
    public static DeveloperChatMessage CreateDeveloperMessage(string content) => new(content);

    /// <inheritdoc cref="DeveloperChatMessage(IEnumerable{ChatMessageContentPart})"/>
    [Experimental("OPENAI001")]
    public static DeveloperChatMessage CreateDeveloperMessage(IEnumerable<ChatMessageContentPart> contentParts) => new(contentParts);

    /// <inheritdoc cref="DeveloperChatMessage(ChatMessageContentPart[])"/>
    [Experimental("OPENAI001")]
    public static DeveloperChatMessage CreateDeveloperMessage(params ChatMessageContentPart[] contentParts) => new(contentParts);
    #endregion

    #region UserChatMessage
    /// <inheritdoc cref="UserChatMessage(string)"/>
    public static UserChatMessage CreateUserMessage(string content) => new(content);

    /// <inheritdoc cref="UserChatMessage(IEnumerable{ChatMessageContentPart})"/>
    public static UserChatMessage CreateUserMessage(IEnumerable<ChatMessageContentPart> contentParts) => new(contentParts);

    /// <inheritdoc cref="UserChatMessage(ChatMessageContentPart[])"/>
    public static UserChatMessage CreateUserMessage(params ChatMessageContentPart[] contentParts) => new(contentParts);
    #endregion

    #region AssistantChatMessage
    /// <inheritdoc cref="AssistantChatMessage(string)"/>
    public static AssistantChatMessage CreateAssistantMessage(string content) => new(content);

    /// <inheritdoc cref="AssistantChatMessage(IEnumerable{ChatMessageContentPart})"/>
    public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatMessageContentPart> contentParts) => new(contentParts);

    /// <inheritdoc cref="AssistantChatMessage(ChatMessageContentPart[])"/>
    public static AssistantChatMessage CreateAssistantMessage(params ChatMessageContentPart[] contentParts) => new(contentParts);

    /// <inheritdoc cref="AssistantChatMessage(IEnumerable{ChatToolCall})"/>
    public static AssistantChatMessage CreateAssistantMessage(IEnumerable<ChatToolCall> toolCalls) => new(toolCalls);

    /// <inheritdoc cref="AssistantChatMessage(ChatFunctionCall)"/>
    public static AssistantChatMessage CreateAssistantMessage(ChatFunctionCall functionCall) => new(functionCall);

    /// <inheritdoc cref="AssistantChatMessage(ChatCompletion)"/>
    public static AssistantChatMessage CreateAssistantMessage(ChatCompletion chatCompletion) => new(chatCompletion);

    /// <inheritdoc cref="AssistantChatMessage(ChatOutputAudioReference)"/>
    [Experimental("OPENAI001")]
    public static AssistantChatMessage CreateAssistantMessage(ChatOutputAudioReference outputAudioReference) => new(outputAudioReference);

    #endregion

    #region ToolChatMessage
    /// <inheritdoc cref="ToolChatMessage(string, string)"/>
    public static ToolChatMessage CreateToolMessage(string toolCallId, string content) => new(toolCallId, content);

    /// <inheritdoc cref="ToolChatMessage(string, IEnumerable{ChatMessageContentPart})"/>
    public static ToolChatMessage CreateToolMessage(string toolCallId, IEnumerable<ChatMessageContentPart> contentParts) => new(toolCallId, contentParts);

    /// <inheritdoc cref="ToolChatMessage(string, ChatMessageContentPart[])"/>
    public static ToolChatMessage CreateToolMessage(string toolCallId, params ChatMessageContentPart[] contentParts) => new(toolCallId, contentParts);
    #endregion

    #region FunctionChatMessage
    /// <inheritdoc cref="FunctionChatMessage(string, string)"/>
    [Obsolete($"This method is obsolete. Please use {nameof(CreateToolMessage)} instead.")]
    public static FunctionChatMessage CreateFunctionMessage(string functionName, string content) => new(functionName, content);
    #endregion

    /// <summary> Creates a new instance of <see cref="UserChatMessage"/>. </summary>
    /// <param name="content"> The text content of the <see cref="UserChatMessage"/>. </param>
    public static implicit operator ChatMessage(string content) => new UserChatMessage(content);
}
