using System.ComponentModel;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("DeleteResponseResponse")]
public partial class ResponseDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ResponseId { get; set; }

    // CUSTOM: Applied EditorBrowsableState.Never.
    [CodeGenMember("Object")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "response.deleted";
}