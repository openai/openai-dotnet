using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallOutputItemResource")]
[CodeGenSuppress("ComputerCallOutputResponseItem")]
public partial class ComputerCallOutputResponseItem
{
    public ComputerCallOutputResponseItem() : this(ResponseItemKind.ComputerCallOutput, null, default, default, null, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ComputerCallOutputStatus? Status { get; set; }
}
