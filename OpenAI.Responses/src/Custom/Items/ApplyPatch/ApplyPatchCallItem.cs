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
}
