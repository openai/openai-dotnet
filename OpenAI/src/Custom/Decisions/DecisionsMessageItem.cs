using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

[CodeGenVisibility("Role", CodeGenVisibility.Public)]
public abstract partial class DecisionsMessageItem
{
    protected internal DecisionsMessageItem(DecisionsMessageRole role) : base(DecisionsItemKind.Message)
    {
        Role = role;
    }
}
