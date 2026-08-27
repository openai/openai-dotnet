using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobEventLevel")]
internal enum FineTuningJobEventLevel
{
    Info,
    Warn,
    Error
}
