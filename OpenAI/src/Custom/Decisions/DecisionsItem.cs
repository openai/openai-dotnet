using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

// CUSTOM: Expose a read-only discriminator and a protected-internal constructor for external subclasses.
[CodeGenVisibility(nameof(DecisionsItem), CodeGenVisibility.ProtectedInternal, typeof(DecisionsItemKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsItem
{
}
