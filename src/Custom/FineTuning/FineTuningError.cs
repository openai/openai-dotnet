using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobError1")]
public partial class FineTuningError
{
    [CodeGenMember("Param")]
    public string InvalidParameter { get; }
}
