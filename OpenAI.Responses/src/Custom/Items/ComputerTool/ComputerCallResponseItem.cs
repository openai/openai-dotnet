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
}
