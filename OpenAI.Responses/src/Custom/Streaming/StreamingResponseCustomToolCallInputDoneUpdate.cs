using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseCustomToolCallInputDoneEvent")]
[CodeGenVisibility(nameof(Agent), CodeGenVisibility.Internal)] // feat: multi-agent
public partial class StreamingResponseCustomToolCallInputDoneUpdate
{
}