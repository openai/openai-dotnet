using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CustomTool")]
[CodeGenVisibility(nameof(DeferLoading), CodeGenVisibility.Internal)] // feat: tool search
[CodeGenVisibility(nameof(AllowedCallers), CodeGenVisibility.Internal)] // feat: programmatic tool calling
public partial class CustomTool
{
}
