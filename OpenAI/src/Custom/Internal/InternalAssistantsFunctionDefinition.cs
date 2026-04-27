using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI;

[CodeGenType("AssistantsFunctionObject")]
internal partial class InternalAssistantsFunctionDefinition
{
    /// <summary>
    /// The parameters to the function, formatting as a JSON Schema object.
    /// </summary>
    [CodeGenMember("Parameters")]
    internal BinaryData Parameters { get; set; }
}
