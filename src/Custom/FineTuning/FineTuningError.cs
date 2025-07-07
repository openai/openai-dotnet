using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
[CodeGenType("FineTuningJobError1")]
public partial class FineTuningError
{
    [CodeGenMember("Param")]
    public string InvalidParameter { get; }
}
