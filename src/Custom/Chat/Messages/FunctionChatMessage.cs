using System;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>function</c> role as provided to a chat completion request. A function message
/// resolves a prior <c>function_call</c> received from the model and correlates to both a supplied
/// <see cref="ChatFunction"/> instance as well as a <see cref="ChatFunctionCall"/> made by the model on an
/// <c>assistant</c> response message.
/// </summary>
[Obsolete($"This class is obsolete. Please use {nameof(ToolChatMessage)} instead.")]
[CodeGenType("ChatCompletionRequestFunctionMessage")]
[CodeGenSuppress("FunctionChatMessage", typeof(string))]
[CodeGenSuppress("FunctionChatMessage", typeof(ChatMessageContent), typeof(string))]
public partial class FunctionChatMessage : ChatMessage
{
    /// <summary>
    /// Creates a new instance of <see cref="FunctionChatMessage"/>.
    /// </summary>
    /// <param name="functionName">
    ///     The name of the called function that this message provides information from.
    /// </param>
    /// <param name="content">
    ///     The textual content that represents the output or result from the called function. There is no format
    ///     restriction (e.g. JSON) imposed on this content.
    /// </param>
    public FunctionChatMessage(string functionName, string content)
        : this(content is null ? null : new ChatMessageContent([content]), ChatMessageRole.Function, null, functionName)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        // FunctionChatMessage treats content as *required* but explicitly *nullable*. If null content was provided,
        // enforce manifestation of the (nullable) content collection via .Clear().
        if (!Content.IsInnerCollectionDefined())
        {
            Content.Clear();
        }
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// The <c>name</c> of the called function that this message provides information from.
    /// </summary>
    [CodeGenMember("Name")]
    public string FunctionName { get; }
}
