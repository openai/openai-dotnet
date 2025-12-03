namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallOutputItemResource")]
public partial class FunctionCallOutputResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public FunctionCallOutputStatus? Status { get; set; }
}
