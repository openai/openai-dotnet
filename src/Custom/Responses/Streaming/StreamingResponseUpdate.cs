namespace OpenAI.Responses;

[CodeGenType("ResponsesResponseStreamEvent")]
public partial class StreamingResponseUpdate
{
    // CUSTOM: Made public and renamed to "Kind."
    [CodeGenMember("Type")]
    public StreamingResponseUpdateKind Kind { get; }
}