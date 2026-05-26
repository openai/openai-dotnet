using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;
[CodeGenType("ContainerResourceExpiresAfter")]

public partial class ContainerResourceExpiresAfter
{
    // CUSTOM: Convert to a string for back compatibility.
    public string Anchor { get; }
}
