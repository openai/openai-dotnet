using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.FineTuning;

[CodeGenType("CreateFineTuningJobRequestWandbIntegrationWandb")]
internal partial class InternalCreateFineTuningJobRequestWandbIntegrationWandb
{
    [CodeGenMember("Project")]
    public string Project { get; set; }
}
