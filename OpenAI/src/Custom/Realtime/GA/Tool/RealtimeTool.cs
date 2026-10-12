using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeToolBaseGA")]
[CodeGenVisibility(nameof(RealtimeTool), CodeGenVisibility.ProtectedInternal, typeof(RealtimeToolKind))]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class RealtimeTool
{
}
