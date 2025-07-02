using System;

namespace OpenAI.Responses;

[CodeGenType("FunctionTool")]
internal partial class InternalFunctionTool
{
    // CUSTOM: Use plain BinaryData.
    [CodeGenMember("Parameters")]
    internal BinaryData Parameters { get; set; }
}
