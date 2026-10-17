using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

[CodeGenVisibility(nameof(DecisionsContentPart), CodeGenVisibility.ProtectedInternal, typeof(DecisionsContentPartKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsContentPart
{
}
