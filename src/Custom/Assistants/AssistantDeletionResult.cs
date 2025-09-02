using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("DeleteAssistantResponse")]
public partial class AssistantDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string AssistantId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `assistant.deleted`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "assistant.deleted";
}
