using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Represents a realtime server event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventGA")]
[CodeGenVisibility(nameof(RealtimeServerUpdate), CodeGenVisibility.ProtectedInternal, typeof(RealtimeServerUpdateKind))]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class RealtimeServerUpdate
{
}
