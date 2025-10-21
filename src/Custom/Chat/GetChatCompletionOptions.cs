namespace OpenAI.Chat;

public class GetChatCompletionOptions
{
    public GetChatCompletionOptions(string completionId)
    {
        CompletionId = completionId;
    }

    public string CompletionId { get; set; }
}