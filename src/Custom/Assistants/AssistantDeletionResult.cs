namespace OpenAI.Assistants;

[CodeGenModel("DeleteAssistantResponse")]
public partial class AssistantDeletionResult
{
    [CodeGenMember("Id")]
    public string AssistantId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `assistant.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteAssistantResponseObject Object { get; } = InternalDeleteAssistantResponseObject.AssistantDeleted;
}
