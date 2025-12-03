namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallOutputItemResource")]
public partial class ComputerCallOutputResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ComputerCallOutputStatus? Status { get; set; }
}
