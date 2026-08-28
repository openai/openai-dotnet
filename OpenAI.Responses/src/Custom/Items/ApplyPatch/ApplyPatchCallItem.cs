using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchToolCallItemResource")]
[CodeGenSuppress("ApplyPatchCallItem")]
public partial class ApplyPatchCallItem
{
    public ApplyPatchCallItem() : this(ResponseItemKind.ApplyPatchCall, null, default, null, default, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ApplyPatchCallStatus? Status { get; set; }
}
