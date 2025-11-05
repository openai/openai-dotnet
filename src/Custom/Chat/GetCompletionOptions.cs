namespace OpenAI.Chat;

public struct GetCompletionOptions
{
    public GetCompletionOptions(string completionId)
    {
        CompletionId = completionId;
    }

    public string CompletionId { get; set; }
}