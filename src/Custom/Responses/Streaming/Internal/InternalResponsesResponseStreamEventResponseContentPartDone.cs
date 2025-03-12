namespace OpenAI.Responses;

[CodeGenType("ResponsesResponseStreamEventResponseContentPartDone")]
internal partial class InternalResponsesResponseStreamEventResponseContentPartDone
{
    // CUSTOM: Apply generalized content type.
    [CodeGenMember("Part")]
    public ResponseContentPart Part { get; }
}