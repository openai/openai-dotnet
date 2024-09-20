using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("DeleteThreadResponse")]
public partial class ThreadDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ThreadId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteThreadResponseObject Object { get; } = InternalDeleteThreadResponseObject.ThreadDeleted;
}
