namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallItemResource")]
public partial class ComputerCallResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ComputerCallStatus? Status { get; set; }
}
