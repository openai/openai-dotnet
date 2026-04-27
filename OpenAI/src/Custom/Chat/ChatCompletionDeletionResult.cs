using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
[CodeGenType("ChatCompletionDeleted")]
public partial class ChatCompletionDeletionResult
{
    // CUSTOM: Made internal.
    [CodeGenMember("Object")]
    internal string Object { get; } = "chat.completion.deleted";

    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ChatCompletionId { get; }
}
