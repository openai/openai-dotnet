using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

[CodeGenVisibility(nameof(DecisionsAnswer), CodeGenVisibility.ProtectedInternal, typeof(DecisionsAnswerKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsAnswer
{
}
