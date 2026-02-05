using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: correct namespace / customize Container property.
[CodeGenType("CodeInterpreterTool")]
public partial class CodeInterpreterTool
{
    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("Container")]
    public CodeInterpreterToolContainer Container { get; }
}