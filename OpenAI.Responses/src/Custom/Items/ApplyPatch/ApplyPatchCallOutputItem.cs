using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchToolCallOutputItemResource")]
[CodeGenSuppress("ApplyPatchCallOutputItem")]
public partial class ApplyPatchCallOutputItem
{
    public ApplyPatchCallOutputItem() : this(ResponseItemKind.ApplyPatchCallOutput, null, default, null, default, null, null)
    {
    }
}
