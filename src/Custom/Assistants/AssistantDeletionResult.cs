using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("DeleteAssistantResponse")]
public partial class AssistantDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string AssistantId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `assistant.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteAssistantResponseObject Object { get; } = InternalDeleteAssistantResponseObject.AssistantDeleted;
}
