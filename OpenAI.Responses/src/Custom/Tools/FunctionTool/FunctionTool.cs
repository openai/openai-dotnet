using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("FunctionTool")]
[CodeGenSuppress("FunctionTool")]
public partial class FunctionTool
{
    public FunctionTool() : this(ResponseToolKind.Function, default, null, null, null, default)
    {
    }
}
