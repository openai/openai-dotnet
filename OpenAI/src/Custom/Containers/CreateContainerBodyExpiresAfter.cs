using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;
[CodeGenType("CreateContainerBodyExpiresAfter")]

public partial class CreateContainerBodyExpiresAfter
{
    // CUSTOM: Make public property for back compatibility.
    public string Anchor { get; } = "last_active_at";
}
