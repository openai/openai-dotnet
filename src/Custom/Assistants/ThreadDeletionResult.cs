using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("DeleteThreadResponse")]
public partial class ThreadDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ThreadId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.deleted`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread.deleted";
}
