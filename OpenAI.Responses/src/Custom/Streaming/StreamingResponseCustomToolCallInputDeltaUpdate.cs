using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseCustomToolCallInputDeltaEvent")]
[CodeGenVisibility(nameof(Agent), CodeGenVisibility.Internal)] // feat: multi-agent
public partial class StreamingResponseCustomToolCallInputDeltaUpdate
{
}