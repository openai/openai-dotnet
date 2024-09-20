using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("DeleteMessageResponse")]
public partial class MessageDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string MessageId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.message.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteMessageResponseObject Object { get; } = InternalDeleteMessageResponseObject.ThreadMessageDeleted;
}
