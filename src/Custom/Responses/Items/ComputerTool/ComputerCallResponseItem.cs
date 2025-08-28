namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallItemResource")]
public partial class ComputerCallResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public ComputerCallStatus? Status { get; }
}
