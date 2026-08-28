using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallItemResource")]
[CodeGenSuppress("FunctionCallResponseItem")]
public partial class FunctionCallResponseItem
{
    public FunctionCallResponseItem() : this(ResponseItemKind.FunctionCall, null, default, default, null, null, null)
    {
    }
}