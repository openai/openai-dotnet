using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: correct namespace.
[CodeGenType("CodeInterpreterTool")]
public partial class CodeInterpreterTool
{
    // CUSTOM: Delegate to internal hydration constructor.
    public CodeInterpreterTool() : this(ResponseToolKind.CodeInterpreter, default, null)
    {
    }
}