using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Graders;

// CUSTOM: Renamed.
[CodeGenType("Grader")]
[CodeGenVisibility(nameof(Grader), CodeGenVisibility.Internal, typeof(GraderType))]
public partial class Grader
{
}
