using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchToolCallOutputItemResource")]
public partial class ApplyPatchCallOutputItem
{
    // NOTE: Contrary to other tools, the `Status` property of `ApplyPatchCallOutputItem` is set by the client instead
    // of being set by the server, and it is also required. The `Status` property is used to indicate if the client
    // successfuly applied the patch or not.
}
