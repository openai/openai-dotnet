using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CustomToolCallItem")]
[CodeGenVisibility(nameof(Agent), CodeGenVisibility.Internal)] // feat: multi-agent
[CodeGenVisibility(nameof(Caller), CodeGenVisibility.Internal)] // feat: programmatic tool calling
[CodeGenVisibility(nameof(CreatedBy), CodeGenVisibility.Internal)] // feat: ???
[CodeGenVisibility(nameof(Namespace), CodeGenVisibility.Internal)] // feat: tool search
public partial class CustomToolCallItem
{
}
