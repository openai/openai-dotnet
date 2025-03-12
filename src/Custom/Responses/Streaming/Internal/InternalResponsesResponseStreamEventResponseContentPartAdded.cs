namespace OpenAI.Responses;

[CodeGenType("ResponsesResponseStreamEventResponseContentPartAdded")]
internal partial class InternalResponsesResponseStreamEventResponseContentPartAdded
{
    // CUSTOM: Apply generalized content type.
    [CodeGenMember("Part")]
    public ResponseContentPart Part { get; }
}