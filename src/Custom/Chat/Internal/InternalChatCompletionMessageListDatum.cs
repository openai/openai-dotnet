namespace OpenAI.Chat;

[CodeGenType("ChatCompletionMessageListDatum")]
public partial class ChatCompletionMessageListDatum
{
    // CUSTOM: Ensure enumerated value is used.
    [CodeGenMember("Role")]
    public ChatMessageRole Role { get; set; } = ChatMessageRole.Assistant;
}