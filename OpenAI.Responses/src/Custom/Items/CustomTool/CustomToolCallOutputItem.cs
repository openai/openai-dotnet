using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CustomToolCallOutputItem")]
[CodeGenVisibility(nameof(Agent), CodeGenVisibility.Internal)] // feat: multi-agent
[CodeGenVisibility(nameof(Caller), CodeGenVisibility.Internal)] // feat: programmatic tool calling
[CodeGenVisibility(nameof(CreatedBy), CodeGenVisibility.Internal)] // feat: ???
public partial class CustomToolCallOutputItem
{
}
