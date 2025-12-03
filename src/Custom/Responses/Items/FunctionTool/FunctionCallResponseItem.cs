namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallItemResource")]
public partial class FunctionCallResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public FunctionCallStatus? Status { get; set; }
}