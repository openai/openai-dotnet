using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallItemResource")]
[CodeGenSuppress("FunctionCallResponseItem")]
public partial class FunctionCallResponseItem
{
    public FunctionCallResponseItem() : this(ResponseItemKind.FunctionCall, null, default, default, null, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public FunctionCallStatus? Status { get; set; }
}