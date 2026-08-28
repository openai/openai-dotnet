using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: correct namespace.
[CodeGenType("CodeInterpreterTool")]
[CodeGenSuppress("CodeInterpreterTool")]
public partial class CodeInterpreterTool
{
    public CodeInterpreterTool() : base(ResponseToolKind.CodeInterpreter)
    {
    }
}