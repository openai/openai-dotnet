namespace OpenAI.Responses;

[CodeGenType("DeleteResponseResponse")]
public partial class ResponseDeletionResult
{
    // CUSTOM: Made internal.
    [CodeGenMember("Object")]
    internal InternalDeleteResponseResponseObject Object { get; } = "response.deleted";

}