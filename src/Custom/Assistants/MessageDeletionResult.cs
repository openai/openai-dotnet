namespace OpenAI.Assistants;

[CodeGenModel("DeleteMessageResponse")]
public partial class MessageDeletionResult
{
    [CodeGenMember("Id")]
    public string MessageId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.message.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteMessageResponseObject Object { get; } = InternalDeleteMessageResponseObject.ThreadMessageDeleted;
}
