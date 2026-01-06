using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Assistants;

[CodeGenType("DeleteMessageResponse")]
public partial class MessageDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string MessageId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `thread.message.deleted`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "thread.message.deleted";
}
