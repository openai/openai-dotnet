using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Chat;

[CodeGenType("ChatOutputPredictionContent")]
internal partial class InternalChatOutputPredictionContent
{
    // CUSTOM: Assign type to a collection of content parts
    [CodeGenMember("Content")]
    public ChatMessageContent Content { get; set; } = new();
}