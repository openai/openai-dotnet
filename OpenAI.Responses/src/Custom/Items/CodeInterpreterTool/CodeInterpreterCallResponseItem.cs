using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed and made public.
[CodeGenType("CodeInterpreterToolCallItemResource")]
[CodeGenSuppress("CodeInterpreterCallResponseItem")]
public partial class CodeInterpreterCallResponseItem
{
}