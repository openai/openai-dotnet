using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ShellToolCallOutputItemResource")]
public partial class ShellCallOutputItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ShellCallStatus? Status { get; set; }
}
