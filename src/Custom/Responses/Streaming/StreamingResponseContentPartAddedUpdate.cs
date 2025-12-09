namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseContentPartAddedEvent")]
public partial class StreamingResponseContentPartAddedUpdate
{
    // CUSTOM: Apply generalized content type.
    [CodeGenMember("Part")]
    public ResponseContentPart Part { get; set; }
}