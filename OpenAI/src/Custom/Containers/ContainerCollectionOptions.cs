using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerCollectionOptions")]
public partial class ContainerCollectionOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Limit")]
    public int? PageSizeLimit { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("After")]
    public string AfterId { get; set; }
}
