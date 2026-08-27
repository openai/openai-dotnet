using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerCollection")]
public partial class ContainerCollectionPage
{
    // CUSTOM: Applied EditorBrowsableState.Never.
    [CodeGenMember("Object")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "list";
}