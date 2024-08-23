using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a chat message of the <c>function</c> role as provided to a chat completion request. A function message
/// resolves a prior <c>function_call</c> received from the model and correlates to both a supplied
/// <see cref="ChatFunction"/> instance as well as a <see cref="ChatFunctionCall"/> made by the model on an
/// <c>assistant</c> response message.
/// </summary>
[CodeGenModel("ChatCompletionRequestFunctionMessage")]
[CodeGenSuppress("FunctionChatMessage", typeof(IEnumerable<ChatMessageContentPart>), typeof(string))]
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
    public FunctionChatMessage(string functionName, string content = null)
        : base(ChatMessageRole.Function, content)
    {
        Argument.AssertNotNull(functionName, nameof(functionName));

        FunctionName = functionName;
    }

    // CUSTOM: Renamed.
    /// <summary>
    /// The <c>name</c> of the called function that this message provides information from.
    /// </summary>
    [CodeGenMember("Name")]
    public string FunctionName { get; }
}
