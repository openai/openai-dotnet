using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallOutputItemResource")]
[CodeGenSuppress("FunctionCallOutputResponseItem")]
public partial class FunctionCallOutputResponseItem
{
    public FunctionCallOutputResponseItem() : this(ResponseItemKind.FunctionCallOutput, null, default, default, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public FunctionCallOutputStatus? Status { get; set; }
}
