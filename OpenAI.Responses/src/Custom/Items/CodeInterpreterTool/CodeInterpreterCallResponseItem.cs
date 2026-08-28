using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed and made public.
[CodeGenType("CodeInterpreterToolCallItemResource")]
[CodeGenSuppress("CodeInterpreterCallResponseItem")]
public partial class CodeInterpreterCallResponseItem
{
    public CodeInterpreterCallResponseItem() : this(ResponseItemKind.CodeInterpreterCall, null, default, default, null, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public CodeInterpreterCallStatus? Status { get; set; }
}