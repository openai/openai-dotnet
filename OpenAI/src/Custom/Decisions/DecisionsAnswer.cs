using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

// CUSTOM: Expose a read-only discriminator and a protected-internal constructor for external subclasses.
[CodeGenVisibility(nameof(DecisionsAnswer), CodeGenVisibility.ProtectedInternal, typeof(DecisionsAnswerKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsAnswer
{
}
