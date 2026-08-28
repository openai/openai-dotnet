using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerFileCollectionOptions")]
public partial class ContainerFileCollectionOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Limit")]
    public int? PageSizeLimit { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("After")]
    public string AfterId { get; set; }
}
