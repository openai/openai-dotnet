namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallItemResource")]
public partial class FunctionCallResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public FunctionCallStatus? Status { get; }
}