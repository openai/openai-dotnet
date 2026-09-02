using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Represents a realtime client event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventGA")]
[CodeGenVisibility(nameof(RealtimeClientCommand), CodeGenVisibility.ProtectedInternal, typeof(RealtimeClientCommandKind))]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class RealtimeClientCommand
{
}