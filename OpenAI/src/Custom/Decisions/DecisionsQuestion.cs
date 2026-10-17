using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

// CUSTOM: Expose a read-only discriminator and a protected-internal constructor for external subclasses.
[CodeGenVisibility(nameof(DecisionsQuestion), CodeGenVisibility.ProtectedInternal, typeof(DecisionsQuestionKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsQuestion
{
}
