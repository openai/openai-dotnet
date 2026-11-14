using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Decisions;

// CUSTOM: Make messages an extensible role-discriminated base and expose the generated role discriminator publicly.
[CodeGenVisibility("Role", CodeGenVisibility.Public)]
public abstract partial class DecisionsMessageItem
{
    // CUSTOM: Let derived message types choose a role while retaining the fixed message item kind.
    protected internal DecisionsMessageItem(DecisionsMessageRole role) : base(DecisionsItemKind.Message)
    {
        Role = role;
    }
}
