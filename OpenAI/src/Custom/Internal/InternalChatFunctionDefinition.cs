using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI;

[CodeGenType("ChatFunctionObject")]
internal partial class InternalChatFunctionDefinition
{
    /// <summary>
    /// The parameters to the function, formatting as a JSON Schema object.
    /// </summary>
    [CodeGenMember("Parameters")]
    internal BinaryData Parameters { get; set; }
}
