namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseStreamEvent")]
[CodeGenSuppress("StreamingResponseUpdate", typeof(System.ClientModel.ClientResult))]
public partial class StreamingResponseUpdate
{
    // CUSTOM: Added setter because this is a property of protocol model.
    [CodeGenMember("SequenceNumber")]
    public int SequenceNumber { get; set; }
}