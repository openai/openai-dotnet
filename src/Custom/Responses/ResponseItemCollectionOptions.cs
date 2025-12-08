namespace OpenAI.Responses;

// CUSTOM: Make public and use the correct namespace. 
[CodeGenType("ResponseItemCollectionOptions")]
public partial class ResponseItemCollectionOptions
{
    public ResponseItemCollectionOptions(string responseId)
    {
        ResponseId = responseId;
    }

    public string ResponseId { get; set; }
}