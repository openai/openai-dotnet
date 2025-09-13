using System;

namespace OpenAI.Responses;

// CUSTOM: correct namespace.
[CodeGenType("CodeInterpreterTool")]
public partial class CodeInterpreterTool
{
    internal static BinaryData AutoContainer { get; } = BinaryData.FromString("""{"type": "auto"}""");
}