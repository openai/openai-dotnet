using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Internal;

[CodeGenType("ResponseFormatJsonSchemaJsonSchema")]
internal partial class InternalResponseFormatJsonSchemaJsonSchema
{
    [CodeGenMember("Schema")]
    public BinaryData Schema { get; set; }
}