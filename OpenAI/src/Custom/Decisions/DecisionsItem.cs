using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

[CodeGenVisibility(nameof(DecisionsItem), CodeGenVisibility.ProtectedInternal, typeof(DecisionsItemKind))]
[CodeGenVisibility("Kind", CodeGenVisibility.Public)]
public partial class DecisionsItem
{
}
