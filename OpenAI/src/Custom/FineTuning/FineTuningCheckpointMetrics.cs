using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobCheckpointMetrics")]
public partial class FineTuningCheckpointMetrics
{
    [CodeGenMember("Step")]
    public int StepNumber { get; }

}
