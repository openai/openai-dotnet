using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

[CodeGenVisibility(nameof(DecisionsQuestion), CodeGenVisibility.ProtectedInternal, typeof(DecisionsQuestionKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsQuestion
{
}
