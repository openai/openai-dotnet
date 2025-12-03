namespace OpenAI.Responses;

// CUSTOM: Renamed and made public.
[CodeGenType("CodeInterpreterToolCallItemResource")]
public partial class CodeInterpreterCallResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public CodeInterpreterCallStatus? Status { get; set; }
}