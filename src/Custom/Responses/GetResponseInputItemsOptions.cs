namespace OpenAI.Responses;

public struct GetResponseInputItemsOptions
{
    public GetResponseInputItemsOptions(string responseId)
    {
        ResponseId = responseId;
    }

    public string ResponseId { get; set; }

    public string AfterId { get; set; }

    public string BeforeId { get; set; }

    public int? PageSizeLimit { get; set; }

    public string Order { get; set; }

}