namespace OpenAI.Chat;

public struct GetChatCompletionMessageOptions
{
    public GetChatCompletionMessageOptions(string completionId)
    {
        CompletionId = completionId;
    }

    public string CompletionId { get; set; }

    public string After { get; set; }

    public int? Limit { get; set; }

    public string Order { get; set; }

}