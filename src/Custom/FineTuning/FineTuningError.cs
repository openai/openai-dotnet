using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobError1")]
public partial class FineTuningError
{
    [CodeGenMember("Param")]
    public string InvalidParameter { get; }
}
