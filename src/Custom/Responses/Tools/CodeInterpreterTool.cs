using System;

namespace OpenAI.Responses;

// CUSTOM: correct namespace / customize Container property.
[CodeGenType("CodeInterpreterTool")]
[CodeGenVisibility(nameof(CodeInterpreterTool), CodeGenVisibility.Internal, typeof(BinaryData))]
public partial class CodeInterpreterTool
{
    [CodeGenMember("Container")]
    internal BinaryData InternalContainer { get; }

    public CodeInterpreterContainer Container { get; }
}