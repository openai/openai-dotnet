using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobEvent")]
public partial class FineTuningEvent
{
    [CodeGenMember("FineTuningJobEventLevel")]
    public string Level;

    [CodeGenMember("Object")]
    private string _object;
}