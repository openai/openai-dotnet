using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningError")]
public partial class FineTuningError
{
    [CodeGenMember("Param")]
    public string InvalidParameter { get; }
}
