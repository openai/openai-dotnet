using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionToolCallItemResource")]
[CodeGenSuppress("FunctionCallResponseItem")]
public partial class FunctionCallResponseItem
{
}