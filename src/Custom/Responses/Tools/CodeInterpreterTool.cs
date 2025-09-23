using System;

namespace OpenAI.Responses;

// CUSTOM: correct namespace / customize Container property.
[CodeGenType("CodeInterpreterTool")]
public partial class CodeInterpreterTool
{
    [CodeGenMember("Container")]
    public CodeInterpreterContainer Container { get; }
}