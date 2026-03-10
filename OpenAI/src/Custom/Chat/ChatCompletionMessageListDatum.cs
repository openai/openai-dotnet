using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionMessageListDatum")]
public partial class ChatCompletionMessageListDatum
{
    // CUSTOM: Ensure enumerated value is used.
    [CodeGenMember("Role")]
    internal ChatMessageRole Role { get; set; } = ChatMessageRole.Assistant;

    // CUSTOM: Rename
    [CodeGenMember("Audio")]
    public ChatOutputAudio OutputAudio { get; }
}