using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerDeletionResult")]
public partial class ContainerDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ContainerId { get; set; }

    // CUSTOM: Added EditorBrowsableState.Never.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CodeGenMember("Object")]
    public string Object { get; set; }
}
