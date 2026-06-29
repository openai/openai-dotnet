using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerFileCollection")]
public partial class ContainerFileCollectionPage
{
    // CUSTOM: Applied EditorBrowsableState.Never.
    [CodeGenMember("Object")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "list";
}