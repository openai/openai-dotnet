using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("CodeInterpreterToolImageOutput")]
[CodeGenSuppress("CodeInterpreterCallImageOutput")]
public partial class CodeInterpreterCallImageOutput
{
    public CodeInterpreterCallImageOutput() : this(InternalCodeInterpreterToolOutputType.Image, default, null)
    {
    }
}