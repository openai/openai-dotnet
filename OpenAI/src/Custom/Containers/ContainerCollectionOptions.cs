using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerCollectionOptions")]
public partial class ContainerCollectionOptions
{
    [CodeGenMember("Limit")]
    public int? PageSizeLimit { get; set; }


    [CodeGenMember("After")]
    public string AfterId { get; set; }
}
