using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("CreateMessageRequestRole")]
public enum MessageRole
{
    /// <summary>
    /// Indicates the message is sent by an actual user.
    /// </summary>
    [CodeGenMember("User")]
    User,

    /// <summary>
    /// Indicates the message was generated by the assistant.
    /// </summary>
    [CodeGenMember("Assistant")]
    Assistant,
}
