using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallOutputItemResource")]
[CodeGenSuppress("FunctionCallOutputResponseItem")]
public partial class FunctionCallOutputResponseItem
{
    public FunctionCallOutputResponseItem() : this(ResponseItemKind.FunctionCallOutput, null, default, default, null, null)
    {
    }
}
