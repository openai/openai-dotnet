using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerFileDeletionResult")]
public partial class ContainerFileDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ContainerFileId { get; set; }

    // CUSTOM: Added EditorBrowsableState.Never.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CodeGenMember("Object")]
    public string Object { get; set; }
}
