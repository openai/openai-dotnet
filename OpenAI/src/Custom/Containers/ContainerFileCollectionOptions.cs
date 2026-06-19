using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerFileCollectionOptions")]
public partial class ContainerFileCollectionOptions
{
    [CodeGenMember("Limit")]
    public int? PageSizeLimit { get; set; }


    [CodeGenMember("After")]
    public string AfterId { get; set; }
}
