using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>assistant</c> role as supplied to a chat completion request. As assistant
/// messages are originated by the model on responses, <see cref="AssistantChatMessage"/> instances typically
/// represent chat history or example interactions to guide model behavior.
/// </summary>
[CodeGenType("ChatCompletionRequestAssistantMessage")]
[CodeGenVisibility(nameof(AssistantChatMessage), CodeGenVisibility.Internal)]
public partial class AssistantChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> using a collection of content items.
    /// For <c>assistant</c> messages, this can be one or more of type <c>text</c> or exactly one of type <c>refusal</c>.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of content items associated with the message.
    /// </param>
    public AssistantChatMessage(IEnumerable<ChatMessageContentPart> contentParts)
        : this(new ChatMessageContent(contentParts), ChatMessageRole.Assistant, null, null, null, null, null, null)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> using a collection of content items.
    /// For <c>assistant</c> messages, this can be one or more of type <c>text</c> or exactly one of type <c>refusal</c>.
    /// </summary>
    /// <param name="contentParts">
    ///     The collection of text and image content items associated with the message.
    /// </param>
    public AssistantChatMessage(params ChatMessageContentPart[] contentParts)
        : this(new ChatMessageContent(contentParts), ChatMessageRole.Assistant, null, null, null, null, null, null)
    {
        Argument.AssertNotNullOrEmpty(contentParts, nameof(contentParts));
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents ordinary text content and
    /// does not feature tool or function calls.
    /// </summary>
    /// <param name="content"> The text content of the message. </param>
    public AssistantChatMessage(string content)
        : this(new ChatMessageContent([content]), ChatMessageRole.Assistant, null, null, null, null, null, null)
    {
        Argument.AssertNotNull(content, nameof(content));
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents <c>tool_calls</c> that
    /// were provided by the model.
    /// </summary>
    /// <param name="toolCalls"> The <c>tool_calls</c> made by the model. </param>
    public AssistantChatMessage(IEnumerable<ChatToolCall> toolCalls)
        : this(null, ChatMessageRole.Assistant, null, null, null, null, null, null)
    {
        Argument.AssertNotNullOrEmpty(toolCalls, nameof(toolCalls));

        foreach (ChatToolCall toolCall in toolCalls)
        {
            ToolCalls.Add(toolCall);
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents a <c>function_call</c>
    /// (deprecated in favor of <c>tool_calls</c>) that was made by the model.
    /// </summary>
    /// <param name="functionCall"> The <c>function_call</c> made by the model. </param>
    [Obsolete($"This constructor is obsolete. Please use the constructor that takes an IEnumerable<ChatToolCall> parameter instead.")]
    public AssistantChatMessage(ChatFunctionCall functionCall)
        : this(null, ChatMessageRole.Assistant, null, null, null, null, functionCall, null)
    {
        Argument.AssertNotNull(functionCall, nameof(functionCall));
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents a prior response from the model
    /// that included audio with a correlation ID.
    /// </summary>
    /// <param name="outputAudioReference"> The <c>audio</c> reference with an <c>id</c>, produced by the model. </param>
    [Experimental("OPENAI001")]
    public AssistantChatMessage(ChatOutputAudioReference outputAudioReference)
        : this(null, ChatMessageRole.Assistant, null, null, null, null, null, outputAudioReference)
    {
        Argument.AssertNotNull(outputAudioReference, nameof(outputAudioReference));
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> from a <see cref="ChatCompletion"/> with
    /// an <c>assistant</c> role response.
    /// </summary>
    /// <remarks>
    ///     This constructor will copy the <c>content</c>, <c>tool_calls</c>, and <c>function_call</c> from a chat
    ///     completion response into a new <c>assistant</c> role request message.
    /// </remarks>
    /// <param name="chatCompletion">
    ///     The <see cref="ChatCompletion"/> from which the conversation history request message should be created.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The <c>role</c> of the provided chat completion response was not <see cref="ChatMessageRole.Assistant"/>.
    /// </exception>
    public AssistantChatMessage(ChatCompletion chatCompletion)
        : this(
              chatCompletion?.Content,
              ChatMessageRole.Assistant,
              null,
              chatCompletion?.Refusal,
              null,
              null,
              chatCompletion?.FunctionCall,
              outputAudioReference: chatCompletion?.OutputAudio is not null
                ? new(chatCompletion.OutputAudio.Id)
                : null)
    {
        Argument.AssertNotNull(chatCompletion, nameof(chatCompletion));

        if (chatCompletion.Role != ChatMessageRole.Assistant)
        {
            throw new NotSupportedException($"Cannot instantiate an {nameof(AssistantChatMessage)} from a {nameof(ChatCompletion)} with role: {chatCompletion.Role}.");
        }

        foreach (ChatToolCall toolCall in chatCompletion.ToolCalls ?? [])
        {
            ToolCalls.Add(toolCall);
        }
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// An optional <c>name</c> associated with the assistant message. This is typically defined with a <c>system</c>
    /// message and is used to differentiate between multiple participants of the same role.
    /// </summary>
    [CodeGenMember("Name")]
    public string ParticipantName { get; set; }

    // CUSTOM: Common initialization for input model collection property.
    [CodeGenMember("ToolCalls")]
    public IList<ChatToolCall> ToolCalls { get; }

    [Obsolete($"This property is obsolete. Please use {nameof(ToolCalls)} instead.")]
    public ChatFunctionCall FunctionCall { get; set; }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("Audio")]
    public ChatOutputAudioReference OutputAudioReference { get; set; }
}