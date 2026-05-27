using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Made public.
[CodeGenType("ToolType")]
[CodeGenVisibility("Computer", CodeGenVisibility.Internal)]
[CodeGenVisibility("LocalShell", CodeGenVisibility.Internal)]
[CodeGenVisibility("Shell", CodeGenVisibility.Internal)]
[CodeGenVisibility("Custom", CodeGenVisibility.Internal)]
[CodeGenVisibility("Namespace", CodeGenVisibility.Internal)]
[CodeGenVisibility("ToolSearch", CodeGenVisibility.Internal)]
public readonly partial struct ResponseToolKind
{
}