namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallOutputItemResource")]
public partial class FunctionCallOutputResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public FunctionCallOutputStatus? Status { get; }
}
