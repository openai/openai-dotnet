using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Graders;

// CUSTOM: Renamed.
[CodeGenType("Grader")]
[CodeGenSuppress("Grader", typeof(GraderType))]
public partial class Grader
{
    private protected Grader(GraderType kind)
    {
        Kind = kind;
    }
}
