namespace OpenAI.Chat;

[CodeGenType("ChatCompletionMessageListDatum")]
internal partial class InternalChatCompletionMessageListDatum
{
    // CUSTOM: Ensure enumerated value is used.
    [CodeGenMember("Role")]
    internal ChatMessageRole Role { get; set; } = ChatMessageRole.Assistant;
}