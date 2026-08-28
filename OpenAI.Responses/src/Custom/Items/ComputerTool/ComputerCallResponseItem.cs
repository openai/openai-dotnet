using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ComputerToolCallItemResource")]
[CodeGenSuppress("ComputerCallResponseItem")]
public partial class ComputerCallResponseItem
{
    public ComputerCallResponseItem() : this(ResponseItemKind.ComputerCall, null, default, default, null, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ComputerCallStatus? Status { get; set; }
}
