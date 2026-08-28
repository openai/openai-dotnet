using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchToolCallOutputItemResource")]
public partial class ApplyPatchCallOutputItem
{
    public ApplyPatchCallOutputItem(string callId, ApplyPatchCallOutputStatus status) : this(callId)
    {
        Status = status;
    }
}
