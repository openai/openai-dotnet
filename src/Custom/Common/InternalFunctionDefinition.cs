using System;

namespace OpenAI;

[CodeGenModel("FunctionObject")]
internal partial class InternalFunctionDefinition
{
    /// <summary>
    /// The parameters to the function, formatting as a JSON Schema object.
    /// </summary>
    [CodeGenMember("Parameters")]
    internal BinaryData Parameters;
}
