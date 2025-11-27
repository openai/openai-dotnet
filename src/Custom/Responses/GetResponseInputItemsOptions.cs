namespace OpenAI.Responses;

public struct GetResponseInputItemsOptions
{
    public GetResponseInputItemsOptions(string responseId)
    {
        ResponseId = responseId;
    }

    public string ResponseId { get; set; }

    public string After { get; set; }

    public string Before { get; set; }

    public int? Limit { get; set; }

    public string Order { get; set; }

}