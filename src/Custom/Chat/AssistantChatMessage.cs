using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>assistant</c> role as supplied to a chat completion request. As assistant
/// messages are originated by the model on responses, <see cref="AssistantChatMessage"/> instances typically
/// represent chat history or example interactions to guide model behavior.
/// </summary>
[CodeGenModel("ChatCompletionRequestAssistantMessage")]
public partial class AssistantChatMessage : ChatMessage
{
    // CUSTOM: Made internal.
    /// <summary> Initializes a new instance of <see cref="AssistantChatMessage"/>. </summary>
    internal AssistantChatMessage()
    {
    }

    // Assistant messages may present ONE OF:
    //	- Ordinary text content without tools or a function, in which case the content is required.
    //	- A list of tool calls, together with optional text content
    //	- A function call, together with optional text content

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents ordinary text content and
    /// does not feature tool or function calls.
    /// </summary>
    /// <param name="content"> The text content of the message. </param>
    public AssistantChatMessage(string content)
    {
        Argument.AssertNotNull(content, nameof(content));

        Role = "assistant";
        Content = [ChatMessageContentPart.CreateTextMessageContentPart(content)];
        ToolCalls = new ChangeTrackingList<ChatToolCall>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents <c>tool_calls</c> that
    /// were provided by the model.
    /// </summary>
    /// <param name="toolCalls"> The <c>tool_calls</c> made by the model. </param>
    /// <param name="content"> Optional text content associated with the message. </param>
    public AssistantChatMessage(IEnumerable<ChatToolCall> toolCalls, string content = null)
    {
        Argument.AssertNotNull(toolCalls, nameof(toolCalls));

        Role = "assistant";
        Content = (content == null)
            ? new ChangeTrackingList<ChatMessageContentPart>()
            : [ChatMessageContentPart.CreateTextMessageContentPart(content)];
        ToolCalls = new List<ChatToolCall>(toolCalls);
    }

    /// <summary>
    /// Creates a new instance of <see cref="AssistantChatMessage"/> that represents a <c>function_call</c>
    /// (deprecated in favor of <c>tool_calls</c>) that was made by the model.
    /// </summary>
    /// <param name="functionCall"> The <c>function_call</c> made by the model. </param>
    /// <param name="content"> Optional text content associated with the message. </param>
    public AssistantChatMessage(ChatFunctionCall functionCall, string content = null)
    {
        Argument.AssertNotNull(functionCall, nameof(functionCall));

        Role = "assistant";
        Content = (content == null)
            ? new ChangeTrackingList<ChatMessageContentPart>()
            : [ChatMessageContentPart.CreateTextMessageContentPart(content)];
        ToolCalls = new ChangeTrackingList<ChatToolCall>();
        FunctionCall = functionCall;
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
    {
        Argument.AssertNotNull(chatCompletion, nameof(chatCompletion));

        if (chatCompletion.Role != ChatMessageRole.Assistant)
        {
            throw new NotSupportedException($"Cannot instantiate an {nameof(AssistantChatMessage)} from a {nameof(ChatCompletion)} with role: {chatCompletion.Role}.");
        }

        Role = "assistant";
        Content = (IList<ChatMessageContentPart>)chatCompletion.Content;
        ToolCalls = (IList<ChatToolCall>)chatCompletion.ToolCalls;
        FunctionCall = chatCompletion.FunctionCall;
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// An optional <c>name</c> associated with the assistant message. This is typically defined with a <c>system</c>
    /// message and is used to differentiate between multiple participants of the same role.
    /// </summary>
    [CodeGenMember("Name")]
    public string ParticipantName { get; init; }
}