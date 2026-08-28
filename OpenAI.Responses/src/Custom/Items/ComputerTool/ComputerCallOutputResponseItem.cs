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
}
