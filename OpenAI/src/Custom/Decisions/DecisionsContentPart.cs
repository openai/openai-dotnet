using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

// CUSTOM: Expose a read-only discriminator and a protected-internal constructor for external subclasses.
[CodeGenVisibility(nameof(DecisionsContentPart), CodeGenVisibility.ProtectedInternal, typeof(DecisionsContentPartKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsContentPart
{
}
