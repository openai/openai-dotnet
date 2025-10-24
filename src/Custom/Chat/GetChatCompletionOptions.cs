namespace OpenAI.Chat;

public struct GetChatCompletionOptions
{
    public GetChatCompletionOptions(string completionId)
    {
        CompletionId = completionId;
    }

    public string CompletionId { get; set; }
}